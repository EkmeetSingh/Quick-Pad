using Microsoft.AppCenter.Analytics;
using Microsoft.Services.Store.Engagement;
using Newtonsoft.Json.Linq;
using QuickPad;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Services.Store;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Quick_Pad_Free_Edition
{
  public sealed partial class MainPage : Page, INotifyPropertyChanged
  {
    #region Notification overhead, no need to write it thousands times on set { }
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Set property and also alert the UI if the value is changed
    /// </summary>
    /// <param name="value">New value</param>
    public void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
      if (!Equals(storage, value))
      {
        storage = value;
        NotifyPropertyChanged(propertyName);
      }
    }

    /// <summary>
    /// Alert the UI there is a change in this property and need update
    /// </summary>
    /// <param name="name"></param>
    public void NotifyPropertyChanged(string name)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    #endregion

    public ResourceLoader textResource { get; } = ResourceLoader.GetForCurrentView(); //Use to get a text resource from Strings/en-US

    public Setting QSetting { get; } = new Setting(); //Store all app setting here..

    public MainPage()
    {
      InitializeComponent();

      //extent app in to the title bar
      CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
      ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
      titleBar.ButtonBackgroundColor = Colors.Transparent;
      titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

      //Subscribe to events
      QSetting.afterThemeChanged += UpdateUIAccordingToNewTheme;
      UpdateUIAccordingToNewTheme(QSetting.Theme);
      QSetting.afterFontSizeChanged += UpdateText1FontSize;
      QSetting.afterAutoSaveChanged += UpdateAutoSave;
      //
      CreateItems();
      LoadSettings();
      LoadFonts();

      VersionNumber.Text = string.Format(textResource.GetString("VersionFormat"), Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision);

      //check if focus is on app or off the app
      Window.Current.CoreWindow.Activated += (sender, args) =>
      {
        if (args.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
        {
          CommandBar2.Focus(FocusState.Programmatic); // Set focus off the main content
        }
      };

      //ask user if they want to save before closing the app
      Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += async (sender, e) =>
      {
        var deferral = e.GetDeferral();

        if (!Changed)
        {
          //No change made, either new document or file saved
          deferral.Complete();
        }

        //close dialogs so the app does not hang
        SaveDialog.Hide();
        Settings.Hide();

        await SaveDialog.ShowAsync();

        if (SaveDialogValue != DialogResult.Cancel)
        {
          deferral.Complete();
        }

        if (SaveDialogValue == DialogResult.Cancel)
        {
          e.Handled = true;
          deferral.Complete();
        }

        SaveDialogValue = DialogResult.None; //reset save dialog    
      };

      CheckPushNotifications(); //check for push notifications

      this.Loaded += MainPage_Loaded;
      this.LayoutUpdated += MainPage_LayoutUpdated;
    }

    private void UpdateAutoSave(bool to)
    {
      if (to)
      {
        timer.Enabled = true;
        timer.Start();
      }
      else
      {
        timer.Enabled = false;
        timer.Stop();
      }
    }

    #region Startup and function handling (Main_Loaded, Uodate UI, Launch sub function, Navigation hangler
    private void UpdateUIAccordingToNewTheme(ElementTheme to)
    {
      //Is it dark theme or light theme? Just in case if it default, get a theme info from application
      bool isDarkTheme = to == ElementTheme.Dark;
      if (to == ElementTheme.Default)
      {
        isDarkTheme = App.Current.RequestedTheme == ApplicationTheme.Dark;
      }
      //Tell analytics what theme is selected
      if (to == ElementTheme.Default)
      {
        Analytics.TrackEvent($"Loaded app in {QSetting.Theme.ToString().ToLower()} theme from a system, which is {(isDarkTheme ? "dark theme" : "light theme")}");
      }
      else
      {
        Analytics.TrackEvent($"Loaded app in {QSetting.Theme.ToString().ToLower()} theme");
      }
      //Make the minimize, maxamize and close button visible
      ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
      if (isDarkTheme)
      {
        titleBar.ButtonForegroundColor = Colors.White;
      }
      else
      {
        titleBar.ButtonForegroundColor = Colors.Black;
      }

      FontBoxFrame.Background = Fonts.Background; //Make the frame over the font box the same color as the font box

      //Update combobox items font color collection

      if (QSetting.DefaultFontColor == "Default")
      {
        Text1.Document.Selection.CharacterFormat.ForegroundColor = isDarkTheme ? Colors.White : Colors.Black;
      }
    }

    private void UpdateText1FontSize(int to)
    {
      Text1.Document.Selection.CharacterFormat.Size = to; //set the font size
    }

    public void LaunchCheck()
    {
      //check what mode to launch the app in
      switch ((AvailableModes)QSetting.LaunchMode)
      {
        case AvailableModes.Focus:
          SwitchToFocusMode();
          break;
        case AvailableModes.OnTop:
          //TODO:Launch compact overlay mode
          SwitchCompactOverlayMode(true);
          break;
      }
    }

    private void CreateItems()
    {
      FontColorCollections = new ObservableCollection<FontColorItem>
            {
                new FontColorItem(),
                new FontColorItem("Black"),
                new FontColorItem("White"),
                new FontColorItem("Blue", "SkyBlue"),
                new FontColorItem("Green", "LightGreen"),
                new FontColorItem("Pink", "LightPink"),
                new FontColorItem("Yellow", "LightYellow"),
                new FontColorItem("Orange", "LightSalmon")
            };
    }

    private void LoadSettings()
    {
      //check if auto save is on or off
      //start auto save timer
      timer.Elapsed += new System.Timers.ElapsedEventHandler(send);
      timer.AutoReset = true;
      if (QSetting.AutoSave)
      {
        timer.Enabled = true;
        timer.Start();
      }
      else
      {
        timer.Enabled = false;
      }

      QSetting.NewUser++;
      if (QSetting.NewUser == 2)
      {
        NewUserFeedbackAsync(); //call method that asks user if they want to review the app
      }
    }

    private void LoadFonts()
    {
      //Load all fonts
      List<string> fonts = Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies().ToList();
      //Sort it in alphabet order
      fonts.Sort((fontA, fontB) => fontA.CompareTo(fontB));
      //Put it on an observable list
      AllFonts = new ObservableCollection<string>(fonts);
    }

    // Stuff for putting the focus on the content
    private void MainPage_LayoutUpdated(object sender, object e)
    {
      if (_isPageLoaded == true)
      {
        Text1.Focus(FocusState.Programmatic); // Set focus on the main content so the user can start typing right away

        //set default font to UIs that still not depend on binding
        Fonts.PlaceholderText = QSetting.DefaultFont;
        Fonts.SelectedItem = QSetting.DefaultFont;
        FontSelected.Text = Convert.ToString(Fonts.SelectedItem);
        Text1.Document.Selection.CharacterFormat.Name = QSetting.DefaultFont;

        //check what default font color is

        if (QSetting.DefaultFontColor == "Default")
        {
          SelectedDefaultFontColor = 0;
        }
        else
        {
          SelectedDefaultFontColor = FontColorCollections.IndexOf(FontColorCollections.First(i => i.TechnicalName == QSetting.DefaultFontColor));
        }

        Text1.Document.Selection.CharacterFormat.Size = QSetting.DefaultFontSize;

        LaunchCheck(); //call method to check what mode the app should launch in

        _isPageLoaded = false;
      }
    }

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
      _isPageLoaded = true;
    }

    public void send(object source, System.Timers.ElapsedEventArgs e)
    {
      //timer for auto save
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
      CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
      {
        if (CurrentWorkingFile != null)
        {
          try
          {
            var result = CurrentWorkingFile.FileType; //find out the file extension
            if ((result.ToLower() != ".rtf"))
            {
              //tries to update file if it exsits and is not read only
              Text1.Document.GetText(TextGetOptions.None, out var value);
              await PathIO.WriteTextAsync(CurrentWorkingFile.Path, value);
              Changed = false;
            }
            if (result.ToLower() == ".rtf")
            {
              //tries to update file if it exsits and is not read only
              Text1.Document.GetText(TextGetOptions.FormatRtf, out var value);
              await PathIO.WriteTextAsync(CurrentWorkingFile.Path, value);
              Changed = false;
            }
          }
          catch (Exception) { }
        }
      });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }
    #endregion

    #region Properties
    bool? _compact = null;
    public bool CompactOverlaySwitch
    {
      get
      {
        if (_compact is null)
        {
          _compact = QSetting.LaunchMode == (int)AvailableModes.OnTop;
        }
        return _compact.Value;
      }
      set
      {
        Set(ref _compact, value);
        SwitchCompactOverlayMode(value);
      }
    }

    ObservableCollection<string> _fonts;
    public ObservableCollection<string> AllFonts
    {
      get => _fonts;
      set => Set(ref _fonts, value);
    }

    //Colors
    ObservableCollection<FontColorItem> _fci;
    public ObservableCollection<FontColorItem> FontColorCollections
    {
      get => _fci;
      set => Set(ref _fci, value);
    }

    public int _fc_selection = -1;
    public int SelectedDefaultFontColor
    {
      get => _fc_selection;
      set
      {
        if (!Equals(_fc_selection, value))
        {
          Set(ref _fc_selection, value);
          //Update setting
          QSetting.DefaultFontColor = FontColorCollections[value].TechnicalName;
          Text1.Document.Selection.CharacterFormat.ForegroundColor = FontColorCollections[value].ActualColor;
        }
      }
    }

    string _file_name = null;
    public string CurrentFilename
    {
      get
      {
        if (_file_name is null)
        {
          _file_name = textResource.GetString("NewDocument");
        }
        if (Changed)
        {
          return $"*{_file_name}";
        }
        return _file_name;
      }

      set
      {
        if (value is null)
        {
          value = textResource.GetString("NewDocument");
          Changed = false;
        }
        if (!Equals(_file_name, value))
        {
          Set(ref _file_name, value);
        }
        //Set Title bar
        var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
        appView.Title = Changed ? $"*{value}" : value;
      }
    }

    bool _changed;
    public bool Changed
    {
      get => _changed;
      set
      {
        if (!Equals(_changed, value))
        {
          Set(ref _changed, value);
          NotifyPropertyChanged(nameof(CurrentFilename));
        }
      }
    }
    public StorageFile CurrentWorkingFile = null;
    private string key; //future access list

    private bool _isPageLoaded = false;
    private Int64 LastFontSize; //this value is the last selected characters font size
    /// <summary>
    /// this is to know if the user clicks cancel when asked if they want to save
    /// 
    /// </summary>
    private DialogResult SaveDialogValue = DialogResult.None;
    public System.Timers.Timer timer = new System.Timers.Timer(10000); //this is the auto save timer interval

    #endregion

    #region Store service
    public async void CheckPushNotifications()
    {
      //regisiter for push notifications
      StoreServicesEngagementManager engagementManager = StoreServicesEngagementManager.GetDefault();
      await engagementManager.RegisterNotificationChannelAsync();
    }

    public async void NewUserFeedbackAsync()
    {
      ContentDialog deleteFileDialog = new ContentDialog //brings up a content dialog
      {
        Title = textResource.GetString("NewUserFeedbackTitle"),//"Do you enjoy using Quick Pad?",
        Content = textResource.GetString("NewUserFeedbackContent"),//"Please consider leaving a review for Quick Pad in the store.",
        PrimaryButtonText = textResource.GetString("NewUserFeedbackYes"),//"Yes",
        CloseButtonText = textResource.GetString("NewUserFeedbackNo"),//"No"
      };

      ContentDialogResult result = await deleteFileDialog.ShowAsync(); //get the results if the user clicked to review or not

      if (result == ContentDialogResult.Primary)
      {
        await ShowRatingReviewDialog(); //show the review dialog.

        //log even in app center
        Analytics.TrackEvent("Pressed review from popup in app");
      }
    }

    public async Task<bool> ShowRatingReviewDialog()
    {
      StoreSendRequestResult result = await StoreRequestHelper.SendRequestAsync(StoreContext.GetDefault(), 16, String.Empty);

      if (result.ExtendedError == null)
      {
        JObject jsonObject = JObject.Parse(result.Response);
        if (jsonObject.SelectToken("status").ToString() == "success")
        {
          // The customer rated or reviewed the app.
          return true;
        }
      }

      // There was an error with the request, or the customer chose not to rate or review the app.
      return false;
    }

    #endregion

    #region Handling navigation
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      var args = e.Parameter as Windows.ApplicationModel.Activation.IActivatedEventArgs;
      if (args != null)
      {
        if (args.Kind == Windows.ApplicationModel.Activation.ActivationKind.File)
        {
          var fileArgs = args as Windows.ApplicationModel.Activation.FileActivatedEventArgs;
          string strFilePath = fileArgs.Files[0].Path;
          var file = (StorageFile)fileArgs.Files[0];
          await LoadFasFile(file); //call method to open the file the app was launched from
        }
      }
    }

    #endregion

    #region Load/Save file
    public async Task LoadFileIntoTextBox()
    {
      Text1.TextChanging -= Text1_TextChanging;
      if (CurrentWorkingFile.FileType.ToLower() == ".rtf")
      {
        Windows.Storage.Streams.IRandomAccessStream randAccStream = await CurrentWorkingFile.OpenAsync(FileAccessMode.Read);

        key = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(CurrentWorkingFile); //let file be accessed later

        // Load the file into the Document property of the RichEditBox.
        Text1.Document.LoadFromStream(TextSetOptions.FormatRtf, randAccStream);
      }
      else
      {
        key = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(CurrentWorkingFile); //let file be accessed later

        Text1.Document.SetText(TextSetOptions.None, await FileIO.ReadTextAsync(CurrentWorkingFile));
      }

      Text1.TextChanging += Text1_TextChanging;
    }

    private async Task LoadFasFile(StorageFile inputFile)
    {
      try
      {
        CurrentWorkingFile = inputFile;
        await LoadFileIntoTextBox();
        CurrentFilename = CurrentWorkingFile.DisplayName;
      }
      catch (Exception) { }
    }

    public async Task SaveWork()
    {
      try
      {
        if (CurrentWorkingFile != null)
        {
          Text1.Document.GetText(
            CurrentWorkingFile.FileType.ToLower() == ".rtf" ? TextGetOptions.FormatRtf : TextGetOptions.None,
            out var value);
          await PathIO.WriteTextAsync(CurrentWorkingFile.Path, value);
          Changed = false;
        }
      }
      catch (Exception)
      {
      }

      Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker
      {
        SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
      };

      // Dropdown of file types the user can save the file as
      //check if default file type is .txt
      if (QSetting.DefaultFileType == ".txt")
      {
        savePicker.FileTypeChoices.Add("Text File", new List<string>() { ".txt" });
        savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });
      }
      else if (QSetting.DefaultFileType == ".rtf")
      {
        savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });
        savePicker.FileTypeChoices.Add("Text File", new List<string>() { ".txt" });
      }
      savePicker.FileTypeChoices.Add("All Files", new List<string>() { "." });

      // Default file name if the user does not type one in or select a file to replace
      savePicker.SuggestedFileName = $"{_file_name}{QSetting.NewFileAutoNumber}";

      StorageFile file = await savePicker.PickSaveFileAsync();
      if (file != null)
      {
        //Set the current working file
        CurrentWorkingFile = file;
        //update title bar
        CurrentFilename = file.DisplayName;

        key = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file); //let file be accessed later

        //save as plain text for text file
        if ((file.FileType.ToLower() != ".rtf"))
        {
          Text1.Document.GetText(TextGetOptions.None, out var value); //get the text to save
          await FileIO.WriteTextAsync(file, value); //write the text to the file
        }
        //save as rich text for rich text file
        if (file.FileType.ToLower() == ".rtf")
        {
          Text1.Document.GetText(TextGetOptions.FormatRtf, out var value); //get the text to save
          await FileIO.WriteTextAsync(file, value); //write the text to the file
        }

        // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
        Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
        if (status != Windows.Storage.Provider.FileUpdateStatus.Complete)
        {
          //let user know if there was an error saving the file
          Windows.UI.Popups.MessageDialog errorBox = new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
          await errorBox.ShowAsync();
        }

        //Increase new auto number, so next file will not get the same name
        QSetting.NewFileAutoNumber++;
      }
    }
    #endregion

    #region Command bar click
    public void SetTheme(object sender, RoutedEventArgs e)
    {
      QSetting.Theme = (ElementTheme)Enum.Parse(typeof(ElementTheme), (sender as RadioButton).Tag as string);
    }

    private void SetFormatColor(object sender, RoutedEventArgs e)
    {
      string tag = (sender as FrameworkElement).Tag.ToString();
      Text1.Document.Selection.CharacterFormat.ForegroundColor = (Color)XamlBindingHelper.ConvertValue(typeof(Color), tag);
    }

    private async void CmdSettings_Click(object sender, RoutedEventArgs e)
    {
      ContentDialogResult result = await Settings.ShowAsync();
    }

    private void Justify_Click(object sender, RoutedEventArgs e)
    {
      Text1.Document.Selection.ParagraphFormat.Alignment = Windows.UI.Text.ParagraphAlignment.Justify;
    }

    private void Right_Click(object sender, RoutedEventArgs e)
    {
      Text1.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Right;
    }

    private void Center_Click(object sender, RoutedEventArgs e)
    {
      Text1.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Center;
    }

    private void Left_Click(object sender, RoutedEventArgs e)
    {
      Text1.Document.Selection.ParagraphFormat.Alignment = ParagraphAlignment.Left;
    }

    private async void CmdNew_Click(object sender, RoutedEventArgs e)
    {
      if (CurrentWorkingFile is null && Changed)
      {
        //File has not been save yet ask use if they want to save
        await SaveDialog.ShowAsync();

        if (SaveDialogValue != DialogResult.Cancel)
        {
          Text1.Document.SetText(TextSetOptions.None, string.Empty);

          //reset the value of the friendly file name
          CurrentWorkingFile = null;
          //update the title bar to reflect it is a new document
          CurrentFilename = null;
        }
      }
      else
      {
        //File have been saved! And no change has been made. Reset right away
        Text1.Document.SetText(TextSetOptions.None, string.Empty);

        //reset the value of the friendly file name
        CurrentWorkingFile = null;
        //update the title bar to reflect it is a new document
        CurrentFilename = null;
      }
    }

    private async void CmdOpen_Click(object sender, RoutedEventArgs e)
    {
      Windows.Storage.Pickers.FileOpenPicker open = new Windows.Storage.Pickers.FileOpenPicker();
      open.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
      open.FileTypeFilter.Add(".rtf"); //add file type to the file picker
      open.FileTypeFilter.Add(".txt"); //add file type to the file picker
      open.FileTypeFilter.Add("*"); //add wild card so more file types can be opened

      StorageFile file = await open.PickSingleFileAsync();

      if (file != null)
      {
        try
        {
          Windows.Storage.Streams.IRandomAccessStream randAccStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
          key = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file); //let file be accessed later

          if ((file.FileType.ToLower() != ".rtf"))
          {
            Text1.Document.SetText(Windows.UI.Text.TextSetOptions.None, await FileIO.ReadTextAsync(file));
          }
          if (file.FileType.ToLower() == ".rtf")
          {
            Text1.Document.LoadFromStream(Windows.UI.Text.TextSetOptions.FormatRtf, randAccStream);
          }

          //Set current file
          CurrentWorkingFile = file;
          CurrentFilename = CurrentWorkingFile.DisplayName;
        }
        catch (Exception)
        {
          ContentDialog errorDialog = new ContentDialog()
          {
            Title = "File open error",
            Content = "Sorry, Quick Pad couldn't open the file.",
            PrimaryButtonText = "Ok"
          };
          await errorDialog.ShowAsync();
        }
      }
    }

    public async void CmdSave_Click(object sender, RoutedEventArgs e)
    {
      await SaveWork(); //call the function to save
    }

    private void CmdUndo_Click(object sender, RoutedEventArgs e)
    {
      Text1.Document.Undo(); //undo changes the user did to the text
    }

    private void CmdRedo_Click(object sender, RoutedEventArgs e)
    {
      Text1.Document.Redo(); //redo changes the user did to the text
    }

    private void Bold_Click(object sender, RoutedEventArgs e)
    {
      //set the selected text to be bold if not already
      //if the text is already bold it will make it regular
      ITextSelection selectedText = Text1.Document.Selection;
      if (selectedText != null)
      {
        ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
        charFormatting.Bold = Windows.UI.Text.FormatEffect.Toggle;
        selectedText.CharacterFormat = charFormatting;
      }
    }

    private void Italic_Click(object sender, RoutedEventArgs e)
    {
      //set the selected text to be in italics if not already
      //if the text is already in italics it will make it regular
      ITextSelection selectedText = Text1.Document.Selection;
      if (selectedText != null)
      {
        ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
        charFormatting.Italic = Windows.UI.Text.FormatEffect.Toggle;
        selectedText.CharacterFormat = charFormatting;
      }
    }

    private void Underline_Click(object sender, RoutedEventArgs e)
    {
      //set the selected text to be underlined if not already
      //if the text is already underlined it will make it regular
      ITextSelection selectedText = Text1.Document.Selection;
      if (selectedText != null)
      {
        ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
        if (charFormatting.Underline == Windows.UI.Text.UnderlineType.None)
        {
          charFormatting.Underline = Windows.UI.Text.UnderlineType.Single;
        }
        else
        {
          charFormatting.Underline = Windows.UI.Text.UnderlineType.None;
        }
        selectedText.CharacterFormat = charFormatting;
      }
    }

    private async void Paste_Click(object sender, RoutedEventArgs e)
    {
      DataPackageView dataPackageView = Clipboard.GetContent();
      if (dataPackageView.Contains(StandardDataFormats.Text))
      {
        string text = await dataPackageView.GetTextAsync();
        //if there is nothing to paste then dont paste anything since it will crash
        if (text != "")
        {
          Text1.Document.Selection.TypeText(text); //paste the text from the clipboard
        }
      }
    }

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
      //send the selected text to the clipboard
      DataPackage dataPackage = new DataPackage();
      dataPackage.RequestedOperation = DataPackageOperation.Copy;
      dataPackage.SetText(Text1.Document.Selection.Text);
      Clipboard.SetContent(dataPackage);
    }

    private void Cut_Click(object sender, RoutedEventArgs e)
    {
      //deletes the selected text but sends it to the clipboard to be pasted somewhere else
      DataPackage dataPackage = new DataPackage();
      dataPackage.SetText(Text1.Document.Selection.Text);
      Text1.Document.Selection.Text = "";
      Clipboard.SetContent(dataPackage);
    }

    private void SizeUp_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        //makes the selected text font size bigger
        Text1.Document.Selection.CharacterFormat.Size = Text1.Document.Selection.CharacterFormat.Size + 2;
      }
      catch (Exception)
      {
        Text1.Document.Selection.CharacterFormat.Size = LastFontSize;
      }
    }

    private void SizeDown_Click(object sender, RoutedEventArgs e)
    {
      //checks if the font size is too small
      if (Text1.Document.Selection.CharacterFormat.Size > 4)
      {
        //make the selected text font size smaller
        Text1.Document.Selection.CharacterFormat.Size = Text1.Document.Selection.CharacterFormat.Size - 2;
      }
    }

    private void Emoji_Checked(object sender, RoutedEventArgs e)
    {
      Emoji2.Visibility = Windows.UI.Xaml.Visibility.Visible;
      E1.Focus(FocusState.Programmatic);
    }

    private void Emoji_Unchecked(object sender, RoutedEventArgs e)
    {
      Emoji2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
      EmojiPivot.SelectedIndex = 0; //Set focus to first item in pivot control in the emoji panel
    }

    private void EmojiPanel_LostFocus(object sender, RoutedEventArgs e) { }

    public void EmojiSub(object sender, RoutedEventArgs e)
    {
      string objname = ((Button)sender).Content.ToString(); //get emoji from button that was pressed
      Text1.Document.Selection.TypeText(objname); //insert emoji in the text box
    }

    private void CmdShare_Click(object sender, RoutedEventArgs e)
    {
      Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
      Windows.ApplicationModel.DataTransfer.DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
    }

    private void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
      Text1.Document.GetText(TextGetOptions.UseCrlf, out var value);

      if (!string.IsNullOrEmpty(value))
      {
        args.Request.Data.SetText(value);
        args.Request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;
      }
      else
      {
        //"Nothing to share, type something in order to share it."
        args.Request.FailWithDisplayText(textResource.GetString("NothingToShare"));
      }
    }

    private async void CmdReview_Click(object sender, RoutedEventArgs e)
    {
      await ShowRatingReviewDialog(); //show the review dialog.

      Analytics.TrackEvent("User clicked on review"); //log even in app center
    }

    private void Strikethrough_Click(object sender, RoutedEventArgs e)
    {
      ITextSelection selectedText = Text1.Document.Selection;
      if (selectedText != null)
      {
        ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
        charFormatting.Strikethrough = Windows.UI.Text.FormatEffect.Toggle;
        selectedText.CharacterFormat = charFormatting;
      }
    }

    private void BulletList_Click(object sender, RoutedEventArgs e)
    {
      if (Text1.Document.Selection.ParagraphFormat.ListType == MarkerType.Bullet)
      {
        Text1.Document.Selection.ParagraphFormat.ListType = MarkerType.None;
      }
      else
      {
        Text1.Document.Selection.ParagraphFormat.ListType = MarkerType.Bullet;
      }
    }

    private void CmdBack_Click(object sender, RoutedEventArgs e)
    {
      Settings.Hide();
      SettingsPivot.SelectedIndex = 0; //Set focus to first item in pivot control in the settings panel
    }

    private void Settings_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
      SettingsPivot.SelectedItem = SettingsTab1; //Set focus to first item in pivot control in the settings panel
    }

    private void Fonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var selectedFont = e.AddedItems[0].ToString();
      Text1.Document.Selection.CharacterFormat.Name = selectedFont;
    }

    private void Frame_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
      Fonts.IsDropDownOpen = true; //open the font combo box
    }

    private void FontSelected_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
      //Change color of the font combo box when hovering over it
      if (App.Current.RequestedTheme == ApplicationTheme.Dark)
      {
        FontBoxFrame.Background = new SolidColorBrush(Colors.Black);
      }
      else if (App.Current.RequestedTheme == ApplicationTheme.Light)
      {
        FontBoxFrame.Background = new SolidColorBrush(Colors.White);
      }

      if (this.RequestedTheme == ElementTheme.Dark)
      {
        FontBoxFrame.Background = new SolidColorBrush(Colors.Black);
      }
      else if (this.RequestedTheme == ElementTheme.Light)
      {
        FontBoxFrame.Background = new SolidColorBrush(Colors.White);
      }
    }

    private void FontSelected_PointerExited(object sender, PointerRoutedEventArgs e)
    {
      FontBoxFrame.Background = Fonts.Background; //Make the frame over the font box the same color as the font box
    }

    private async void SaveDialogYes_Click(object sender, RoutedEventArgs e)
    {
      await SaveWork();
      SaveDialogValue = DialogResult.Yes;
      SaveDialog.Hide();
    }

    private void SaveDialogNo_Click(object sender, RoutedEventArgs e)
    {
      SaveDialogValue = DialogResult.No;
      SaveDialog.Hide();
    }

    private void SaveDialogCancel_Click(object sender, RoutedEventArgs e)
    {
      SaveDialogValue = DialogResult.Cancel;
      SaveDialog.Hide();
    }

    #endregion

    #region UI Mode change
    public async void SwitchCompactOverlayMode(bool switching)
    {
      if (switching)
      {
        ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
        bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, compactOptions);

        Grid.SetRow(CommandBar2, 2);
        Shadow1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        CommandBar1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        Title.Visibility = Visibility.Collapsed;
        CommandBar2.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
        FrameTop.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        Text1.Margin = new Thickness(0, 0, 0, 0);
        CmdSettings.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        CmdFocusMode.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        CmdFocusMode.IsEnabled = false;
        CommandBar3.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        CommandBar2.Margin = new Thickness(0, 0, 0, 0);

        //make text smaller size if user did not do so on their own and if they did not type anything yet.
        Text1.Document.GetText(TextGetOptions.UseCrlf, out var value);
        if (string.IsNullOrEmpty(value) && Text1.FontSize == 18)
        {
          Text1.FontSize = 16;
        }

        //log even in app center
        Analytics.TrackEvent("Compact Overlay");
      }
      else
      {
        bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
        Grid.SetRow(CommandBar2, 0);
        Title.Visibility = Visibility.Visible;
        Shadow1.Visibility = Windows.UI.Xaml.Visibility.Visible;
        CommandBar1.Visibility = Windows.UI.Xaml.Visibility.Visible;
        CommandBar2.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
        FrameTop.Visibility = Windows.UI.Xaml.Visibility.Visible;
        CmdSettings.Visibility = Windows.UI.Xaml.Visibility.Visible;
        CmdFocusMode.Visibility = Windows.UI.Xaml.Visibility.Visible;
        CommandBar3.Visibility = Windows.UI.Xaml.Visibility.Visible;
        CmdFocusMode.IsEnabled = true;
        Text1.Margin = new Thickness(0, 74, 0, 40);
        CommandBar2.Margin = new Thickness(0, 33, 0, 0);
      }
    }

    private void SwitchToFocusMode()
    {
      Text1.SetValue(Canvas.ZIndexProperty, 90);
      Text1.Margin = new Thickness(0, 33, 0, 0);
      CommandBar2.Visibility = Visibility.Collapsed;
      Shadow2.Visibility = Visibility.Collapsed;
      Shadow1.Visibility = Visibility.Collapsed;
      CloseFocusMode.Visibility = Visibility.Visible;
    }

    private void CmdFocusMode_Click(object sender, RoutedEventArgs e)
    {
      SwitchToFocusMode();
    }

    private void CloseFocusMode_Click(object sender, RoutedEventArgs e)
    {
      Text1.SetValue(Canvas.ZIndexProperty, 0);
      CommandBar2.Visibility = Visibility.Visible;
      Shadow2.Visibility = Visibility.Visible;
      Shadow1.Visibility = Visibility.Visible;
      CloseFocusMode.Visibility = Visibility.Collapsed;
      Text1.Margin = new Thickness(0, 74, 0, 40);
    }
    #endregion

    #region Textbox function
    private void Text1_GotFocus(object sender, RoutedEventArgs e)
    {
      Emoji.IsChecked = false; //hide emoji panel if open 
      LastFontSize = Convert.ToInt64(Text1.Document.Selection.CharacterFormat.Size); //get font size of last selected character
    }

    private void Text1_TextChanged(object sender, RoutedEventArgs e)
    {
      if (Text1.Document.CanUndo() == true)
      {
        CmdUndo.IsEnabled = true;
      }
      else
      {
        CmdUndo.IsEnabled = false;
        Changed = false;
      }
      /////
      if (Text1.Document.CanRedo() == true)
      {
        CmdRedo.IsEnabled = true;
      }
      else
      {
        CmdRedo.IsEnabled = false;
      }
    }

    private void Text1_TextChanging(RichEditBox sender, RichEditBoxTextChangingEventArgs args)
    {
      Changed = true; //Mark document as changed
    }

    private void Text1_KeyDown(object sender, KeyRoutedEventArgs e)
    {
      if (e.Key == VirtualKey.Tab)
      {
        RichEditBox richEditBox = sender as RichEditBox;
        if (richEditBox != null)
        {
          richEditBox.Document.Selection.TypeText("\t");
          e.Handled = true;
        }
      }
    }

    private void Text1_DragOver(object sender, DragEventArgs e)
    {
      e.AcceptedOperation = DataPackageOperation.Copy;
    }

    private async void Text1_Drop(object sender, DragEventArgs e)
    {
      //Check if file is open and ask user if they want to save it when dragging a file in to Quick Pad.
      if (CurrentWorkingFile == null)
      {
        await SaveDialog.ShowAsync();
        if (SaveDialogValue == DialogResult.Cancel)
        {
          SaveDialogValue = DialogResult.None; //reset save dialog value
          return;
        }
      }
      if (CurrentWorkingFile != null && Changed)
      {
        //Save all the change before loading new file
        await SaveWork();
      }

      //load rich text files dropped in from file explorer
      try
      {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
          var items = await e.DataView.GetStorageItemsAsync();
          if (items.Count > 0)
          {
            CurrentWorkingFile = items[0] as StorageFile;
            await LoadFileIntoTextBox();
            CurrentFilename = CurrentWorkingFile.DisplayName;

            //log event in app center
            Analytics.TrackEvent("Droped file in to Quick Pad");
          }
        }
      }
      catch (Exception) { }
    }

    private void Text1_SelectionChanged(object sender, RoutedEventArgs e)
    {
      FontSelected.Text = Text1.Document.Selection.CharacterFormat.Name; //updates font box to show the selected characters font
    }

    #endregion
  }

  public class FontColorItem : INotifyPropertyChanged
  {
    #region Notification overhead, no need to write it thousands times on set { }
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Set property and also alert the UI if the value is changed
    /// </summary>
    /// <param name="value">New value</param>
    public void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
      if (!Equals(storage, value))
      {
        storage = value;
        NotifyPropertyChanged(propertyName);
      }
    }

    /// <summary>
    /// Alert the UI there is a change in this property and need update
    /// </summary>
    /// <param name="name"></param>
    public void NotifyPropertyChanged(string name)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    #endregion

    string _name;
    public string ColorName
    {
      get => _name;
      set => Set(ref _name, value);
    }

    string _tname;
    public string TechnicalName
    {
      get => _tname;
      set => Set(ref _tname, value);
    }

    Color _ac;
    public Color ActualColor
    {
      get => _ac;
      set => Set(ref _ac, value);
    }

    public FontColorItem()
    {
      ColorName = "Default";
      TechnicalName = "Default";
      if (App.Current.RequestedTheme == ApplicationTheme.Dark)
      {
        ActualColor = Colors.White;
      }
      else if (App.Current.RequestedTheme == ApplicationTheme.Light)
      {
        ActualColor = Colors.Black;
      }
    }

    public FontColorItem(string name)
    {
      ColorName = ResourceLoader.GetForCurrentView().GetString($"FontColor{name}");
      TechnicalName = name;
      ActualColor = (Color)XamlBindingHelper.ConvertValue(typeof(Color), name);
    }
    public FontColorItem(string name, string technical)
    {
      ColorName = ResourceLoader.GetForCurrentView().GetString($"FontColor{name}");
      TechnicalName = technical;
      ActualColor = (Color)XamlBindingHelper.ConvertValue(typeof(Color), technical);
    }

    public static FontColorItem Default => new FontColorItem();
  }

  public enum DialogResult
  {
    None,
    Yes,
    No,
    Cancel
  }
}
