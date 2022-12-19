using Finance.Resources.Languages;
using System.Diagnostics;

namespace Finance;

public partial class PageSettings : ContentPage
{
    // Local variables.
    private readonly Stopwatch stopWatch = new();

    public PageSettings()
    {        
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            DisplayAlert("InitializeComponent PageSettings", ex.Message, MainPage.cButtonCloseText);
            return;
        }

        // Set the current UI culture of the selected language.
        MainPage.SetCultureSelectedLanguage();

        // Put text in the chosen language in the controls.
        lblTitle.Text = FinLang.Settings_Text;

        lblExplanation.Text = FinLang.SettingsSaved_Text;
        lblLanguage.Text = FinLang.Language_Text;
        lblTheme.Text = FinLang.Theme_Text;
        lblDateFormat.Text = FinLang.DateFormat_Text;
        rbnDateFormatSystem.Content = FinLang.System_Text;
        rbnDateFormatISO8601.Content = FinLang.DateISO8601_Text;
        lblPageFormat.Text = FinLang.PageFormat_Text;
        rbnPageFormatA4.Content = FinLang.PageA4_Text;
        rbnPageFormatLetter.Content = FinLang.PageLetter_Text;
        lblRoundNumber.Text = FinLang.RoundNumber_Text;
        rbnRoundNumberAwayFromZero.Content = FinLang.RoundNumberAwayFromZero_Text;
        rbnRoundNumberToEven.Content = FinLang.RoundNumberToEven_Text;
        lblKeyboard.Text = FinLang.Keyboard_Text;
        rbnKeyboardDefault.Content = FinLang.Default_Text;
        rbnKeyboardNumeric.Content = FinLang.Numeric_Text;
        rbnKeyboardText.Content = FinLang.Text_Text;
        btnSettingsSave.Text = FinLang.Save_Text;
        btnSettingsReset.Text = FinLang.Reset_Text;

        var ThemeList = new List<string>
        {
            FinLang.System_Text,
            FinLang.Light_Text,
            FinLang.Dark_Text
        };
        pckTheme.ItemsSource = ThemeList;

        // Set the current language in the picker.
        pckLanguage.SelectedIndex = MainPage.cLanguage switch
        {
            // German (Deutsch).
            "de" => 0,

            // Spanish (Español).
            "es" => 2,

            // French (Français).
            "fr" => 3,

            // Italian (Italiano).
            "it" => 4,

            // Dutch (Nederlands).
            "nl" => 5,

            // Portuguese (Português).
            "pt" => 6,

            // English.
            _ => 1,
        };

        // Set the current theme in the picker.
        pckTheme.SelectedIndex = MainPage.cTheme switch
        {
            // Light.
            "Light" => 1,

            // Dark.
            "Dark" => 2,

            // System.
            _ => 0,
        };

        // Set radiobutton to the date format.
        if (MainPage.bDateFormatSystem == true)
        {
            rbnDateFormatSystem.IsChecked = true;
        }
        else
        {
            rbnDateFormatISO8601.IsChecked = true;
        }

        // Set radiobutton to the page format.
        if (MainPage.cPageFormat == "A4")
        {
            rbnPageFormatA4.IsChecked = true;
        }
        else
        {
            rbnPageFormatLetter.IsChecked = true;
        }

        // Set radiobutton to the rounding numbers method.
        if (MainPage.cRoundNumber == "AwayFromZero")
        {
            rbnRoundNumberAwayFromZero.IsChecked = true;
        }
        else
        {
            rbnRoundNumberToEven.IsChecked = true;
        }

        // Set radiobutton to the used keyboard.
        if (MainPage.cKeyboard == "Default")
        {
            rbnKeyboardDefault.IsChecked = true;
        }
        else if (MainPage.cKeyboard == "Numeric")
        {
            rbnKeyboardNumeric.IsChecked = true;
        }
        else
        {
            rbnKeyboardText.IsChecked = true;
        }

        // Start the stopWatch for resetting all the settings.
        stopWatch.Start();
    }

    // Picker language clicked event.
    private void OnPickerLanguageChanged(object sender, EventArgs e)
    {
        string cLanguageOld = MainPage.cLanguage;

        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            MainPage.cLanguage = selectedIndex switch
            {
                // German (Deutsch).
                0 => "de",

                // Spanish (Español).
                2 => "es",

                // French (Français).
                3 => "fr",

                // Italian (Italiano).
                4 => "it",

                // Dutch (Nederlands).
                5 => "nl",

                // Portuguese (Português).
                6 => "pt",

                // English.
                _ => "en",
            };
        }

        if (cLanguageOld != MainPage.cLanguage)
        {
            MainPage.bLanguageChanged = true;
        }
    }

    // Picker theme clicked event.
    private void OnPickerThemeChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            MainPage.cTheme = selectedIndex switch
            {
                // Light.
                1 => "Light",

                // Dark.
                2 => "Dark",

                // System.
                _ => "System",
            };
        }
    }
    
    // Radio button date format clicked event.
    private void OnDateFormatRadioButtonCheckedChanged(object sender, EventArgs e)
    {
        if (rbnDateFormatSystem.IsChecked)
        {
            MainPage.bDateFormatSystem = true;
            MainPage.cDateFormat = MainPage.cSysDateFormat;
        }
        else if (rbnDateFormatISO8601.IsChecked)
        {
            MainPage.bDateFormatSystem = false;
            MainPage.cDateFormat = "yyyy-MM-dd";
        }
    }

    // Radio button page format clicked event.
    private void OnPageFormatRadioButtonCheckedChanged(object sender, EventArgs e)
    {
        if (rbnPageFormatA4.IsChecked)
        {
            MainPage.cPageFormat = "A4";
        }
        else if (rbnPageFormatLetter.IsChecked)
        {
            MainPage.cPageFormat = "Letter";
        }
    }

    // Radio button roundig numbers method clicked event.
    private void OnRoundNumberRadioButtonCheckedChanged(object sender, EventArgs e)
    {
        if (rbnRoundNumberAwayFromZero.IsChecked)
        {
            MainPage.cRoundNumber = "AwayFromZero";
        }
        else if (rbnRoundNumberToEven.IsChecked)
        {
            MainPage.cRoundNumber = "ToEven";
        }
    }

    // Radio button keyboard clicked event.
    private void OnKeyboardRadioButtonCheckedChanged(object sender, EventArgs e)
    {
        if (rbnKeyboardDefault.IsChecked)
        {
            MainPage.cKeyboard = "Default";
        }
        else if (rbnKeyboardNumeric.IsChecked)
        {
            MainPage.cKeyboard = "Numeric";
        }
        else if (rbnKeyboardText.IsChecked)
        {
            MainPage.cKeyboard = "Text";
        }
    }

    // Button save settings clicked event.
    private static void OnSettingsSaveClicked(object sender, EventArgs e)
    {
        Preferences.Default.Set("SettingTheme", MainPage.cTheme);
        Preferences.Default.Set("SettingDateFormatSystem", MainPage.bDateFormatSystem);
        Preferences.Default.Set("SettingPageFormat", MainPage.cPageFormat);
        Preferences.Default.Set("SettingRoundNumber", MainPage.cRoundNumber);
        Preferences.Default.Set("SettingKeyboard", MainPage.cKeyboard);
        Preferences.Default.Set("SettingLanguage", MainPage.cLanguage);

        // Wait 500 milliseconds otherwise the settings are not saved in Android.
        Task.Delay(500).Wait();

        // Restart the application.
        //Application.Current.MainPage = new AppShell();
        Application.Current.MainPage = new NavigationPage(new MainPage());
        //await Navigation.PushAsync(new MainPage());
        //await Navigation.PopAsync();
    }

    // Button reset settings clicked event.
    private void OnSettingsResetClicked(object sender, EventArgs e)
    {
        // Get the elapsed time in milli seconds.
        stopWatch.Stop();

        if (stopWatch.ElapsedMilliseconds < 2001)
        {
            // Clear all settings after the first clicked event within the first 2 seconds after opening the setting page.
            Preferences.Default.Clear();
        }
        else
        {
            // Reset some settings.
            Preferences.Default.Remove("SettingTheme");
            Preferences.Default.Remove("SettingDateFormatSystem");
            Preferences.Default.Remove("SettingPageFormat");
            Preferences.Default.Remove("SettingRoundNumber");
            Preferences.Default.Remove("SettingKeyboard");
            Preferences.Default.Remove("SettingLanguage");
        }

        // Wait 500 milliseconds otherwise the settings are not saved in Android.
        Task.Delay(500).Wait();

        // Restart the application.
        //Application.Current.MainPage = new AppShell();
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }
}