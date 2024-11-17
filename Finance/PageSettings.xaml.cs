﻿using System.Diagnostics;

namespace Finance
{
    public sealed partial class PageSettings : ContentPage
    {
        //// Local variables.
        private readonly Stopwatch stopWatch = new();

        public PageSettings()
        {        
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                DisplayAlert("InitializeComponent PageSettings", ex.Message, "OK");
                return;
            }
#if WINDOWS
            // Set the margin of the title for windows
            lblTitlePage.Margin = new Thickness(80, 15, 0, 0);
#endif
            //// Put text in the chosen language in the controls and variables
            SetLanguage();
        
            //// Set the current language in the picker
            pckLanguage.SelectedIndex = Globals.cLanguage switch
            {
                "cs" => 0,      // Čeština - Czech
                "da" => 1,      // Dansk - Danish
                "de" => 2,      // Deutsch - German
                "es" => 4,      // Español - Spanish
                "fr" => 5,      // Français - French
                "it" => 6,      // Italiano - Italian
                "hu" => 7,      // Magyar - Hungarian
                "nl" => 8,      // Nederlands - Dutch
                "nb" => 9,      // Norsk Bokmål - Norwegian Bokmål
                "pl" => 10,     // Polski - Polish
                "pt" => 11,     // Português - Portuguese
                "ro" => 12,     // Română - Romanian
                "fi" => 13,     // Suomi - Finnish
                "sv" => 14,     // Svenska - Swedish
                _ => 3,         // English
            };

            //// Set the current theme in the picker
            pckTheme.SelectedIndex = Globals.cTheme switch
            {
                "Light" => 1,   // Light
                "Dark" => 2,    // Dark
                _ => 0,         // System
            };

            //// Set the number of decimal digits after the decimal point
            entNumDec.Text = Globals.cNumDecimalDigits;
            entPercDec.Text = Globals.cPercDecimalDigits;

            //// Set radiobutton to the date format
            if (Globals.bDateFormatSystem == true)
            {
                rbnDateFormatSystem.IsChecked = true;
            }
            else
            {
                rbnDateFormatISO8601.IsChecked = true;
            }

            //// Set radiobutton to the page format
            if (Globals.cPageFormat == "A4")
            {
                rbnPageFormatA4.IsChecked = true;
            }
            else
            {
                rbnPageFormatLetter.IsChecked = true;
            }

            //// Set radiobutton to the rounding numbers method
            if (Globals.cRoundNumber == "AwayFromZero")
            {
                rbnRoundNumberAwayFromZero.IsChecked = true;
            }
            else
            {
                rbnRoundNumberToEven.IsChecked = true;
            }

            //// Set radiobutton to the used keyboard
            if (Globals.cKeyboard == "Default")
            {
                rbnKeyboardDefault.IsChecked = true;
            }
            else if (Globals.cKeyboard == "Numeric")
            {
                rbnKeyboardNumeric.IsChecked = true;
            }
            else
            {
                rbnKeyboardText.IsChecked = true;
            }

            //// Set the color of number to false or true
            swtColorNumber.IsToggled = Globals.bColorNumber;

            //// Start the stopWatch for resetting all the settings
            stopWatch.Start();
        }

        /// <summary>
        /// Picker language clicked event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerLanguageChanged(object sender, EventArgs e)
        {
            string cLanguageOld = Globals.cLanguage;

            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                Globals.cLanguage = selectedIndex switch
                {
                    0 => "cs",      // Čeština - Czech
                    1 => "da",      // Dansk - Danish
                    2 => "de",      // Deutsch - German
                    4 => "es",      // Español - Spanish
                    5 => "fr",      // Français - French
                    6 => "it",      // Italiano - Italian
                    7 => "hu",      // Magyar - Hungarian
                    8 => "nl",      // Nederlands - Dutch
                    9 => "nb",      // Norsk Bokmål - Norwegian Bokmål
                    10 => "pl",     // Polski - Polish
                    11 => "pt",     // Português - Portuguese
                    12 => "ro",     // Română - Romanian
                    13 => "fi",     // Suomi - Finnish
                    14 => "sv",     // Svenska - Swedish
                    _ => "en",      // English
                };
            }

            if (cLanguageOld != Globals.cLanguage)
            {
                Globals.bLanguageChanged = true;

                // Set the current UI culture of the selected language
                Globals.SetCultureSelectedLanguage();

                // Put text in the chosen language in the controls and variables
                SetLanguage();
            }
        }

        /// <summary>
        /// Picker theme clicked event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerThemeChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                Globals.cTheme = selectedIndex switch
                {
                    1 => "Light",   // Light
                    2 => "Dark",    // Dark
                    _ => "System",  // System
                };

                Globals.SetThemeAndNumberColor();
            }
        }

        /// <summary>
        /// Verify the number of decimal digits for numbers after the decimal point 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerifyNumberDecimals(object sender, EventArgs e)
        {
            // Validate input values
            bool bIsNumber = int.TryParse(entNumDec.Text, out int nNumDec);
            if (bIsNumber == false || nNumDec < 0 || nNumDec > 4 || entNumDec.Text == "")
            {
                entNumDec.Text = "";
                _ = entNumDec.Focus();
                return;
            }

            Globals.cNumDecimalDigits = Convert.ToString(nNumDec);
        }

        /// <summary>
        /// Verify the number of decimal digits for percentages after the decimal point 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerifyPercentageDecimals(object sender, EventArgs e)
        {
            bool bIsNumber = int.TryParse(entPercDec.Text, out int nPercDec);
            if (bIsNumber == false || nPercDec < 0 || nPercDec > 8 || entPercDec.Text == "")
            {
                entPercDec.Text = "";
                _ = entPercDec.Focus();
                return;
            }

            Globals.cPercDecimalDigits = Convert.ToString(nPercDec);

            // Close the keyboard
            entPercDec.IsEnabled = false;
            entPercDec.IsEnabled = true;
        }

        /// <summary>
        /// Put text in the chosen language in the controls and variables 
        /// </summary>
        private void SetLanguage()
        {
            var ThemeList = new List<string>
            {
                FinLang.System_Text,
                FinLang.Light_Text,
                FinLang.Dark_Text
            };
            pckTheme.ItemsSource = ThemeList;

            // Set the current theme in the picker
            pckTheme.SelectedIndex = Globals.cTheme switch
            {
                "Light" => 1,   // Light
                "Dark" => 2,    // Dark
                _ => 0,         // System
            };
        }

        /// <summary>
        /// Radio button date format clicked event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDateFormatRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            if (rbnDateFormatSystem.IsChecked)
            {
                Globals.bDateFormatSystem = true;
                Globals.cDateFormat = Globals.cSysDateFormat;
            }
            else if (rbnDateFormatISO8601.IsChecked)
            {
                Globals.bDateFormatSystem = false;
                Globals.cDateFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Radio button page format clicked event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageFormatRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            if (rbnPageFormatA4.IsChecked)
            {
                Globals.cPageFormat = "A4";
            }
            else if (rbnPageFormatLetter.IsChecked)
            {
                Globals.cPageFormat = "Letter";
            }
        }

        /// <summary>
        /// Radio button roundig numbers method clicked event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRoundNumberRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            if (rbnRoundNumberAwayFromZero.IsChecked)
            {
                Globals.cRoundNumber = "AwayFromZero";
            }
            else if (rbnRoundNumberToEven.IsChecked)
            {
                Globals.cRoundNumber = "ToEven";
            }
        }

        /// <summary>
        /// Radio button keyboard clicked event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyboardRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            if (rbnKeyboardDefault.IsChecked)
            {
                Globals.cKeyboard = "Default";
            }
            else if (rbnKeyboardNumeric.IsChecked)
            {
                Globals.cKeyboard = "Numeric";
            }
            else if (rbnKeyboardText.IsChecked)
            {
                Globals.cKeyboard = "Text";
            }
        }

        /// <summary>
        /// Switch color number toggled 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSwtColorNumberToggled(object sender, ToggledEventArgs e)
        {
            Globals.bColorNumber = swtColorNumber.IsToggled;
            Globals.SetThemeAndNumberColor();
        }

        /// <summary>
        /// Button save settings clicked event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnSettingsSaveClicked(object sender, EventArgs e)
        {
            Preferences.Default.Set("SettingTheme", Globals.cTheme);
            Preferences.Default.Set("SettingDateFormatSystem", Globals.bDateFormatSystem);
            Preferences.Default.Set("SettingPageFormat", Globals.cPageFormat);
            Preferences.Default.Set("SettingNumDecimalDigits", Globals.cNumDecimalDigits);
            Preferences.Default.Set("SettingPercDecimalDigits", Globals.cPercDecimalDigits);
            Preferences.Default.Set("SettingRoundNumber", Globals.cRoundNumber);
            Preferences.Default.Set("SettingColorNumber", Globals.bColorNumber);
            Preferences.Default.Set("SettingKeyboard", Globals.cKeyboard);
            Preferences.Default.Set("SettingLanguage", Globals.cLanguage);

            // Give it some time to save the settings
            Task.Delay(400).Wait();

            // Restart the application
            Application.Current!.Windows[0].Page = new AppShell();
        }

        /// <summary>
        /// Button reset settings clicked event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsResetClicked(object sender, EventArgs e)
        {
            // Get the elapsed time in milli seconds
            stopWatch.Stop();

            if (stopWatch.ElapsedMilliseconds < 2001)
            {
                // Clear all settings after the first clicked event within the first 2 seconds after opening the setting page.
                Preferences.Default.Clear();
            }
            else
            {
                // Reset some settings
                Preferences.Default.Remove("SettingTheme");
                Preferences.Default.Remove("SettingDateFormatSystem");
                Preferences.Default.Remove("SettingPageFormat");
                Preferences.Default.Remove("SettingNumDecimalDigits");
                Preferences.Default.Remove("SettingPercDecimalDigits");
                Preferences.Default.Remove("SettingRoundNumber");
                Preferences.Default.Remove("SettingColorNumber");
                Preferences.Default.Remove("SettingKeyboard");
                Preferences.Default.Remove("SettingLanguage");
            }

            // Give it some time to remove the settings
            Task.Delay(400).Wait();

            // Restart the application
            Application.Current!.Windows[0].Page = new AppShell();
        }
    }
}