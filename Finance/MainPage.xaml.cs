/* Program .....: Finance.sln
 * Author ......: Geert Geerits - E-mail: geertgeerits@gmail.com
 * Copyright ...: (C) 1992-2025
 * Version .....: 3.0.69
 * Date ........: 2024-12-09 (YYYY-MM-DD)
 * Language ....: Microsoft Visual Studio 2022: .NET 9.0 MAUI C# 13.0
 * Description .: Financial calculations
 * Thanks to ...: Gerald Versluis for his video's on YouTube about .NET MAUI */

using System.Diagnostics;
#if IOS
using Foundation;
#endif

namespace Finance
{
    public sealed partial class MainPage : ContentPage
    {
        //// Local variables
        private string cCopyright = "";
        private string cLicenseText = "";

        public MainPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                DisplayAlert("InitializeComponent", ex.Message, "OK");
                return;
            }
            //#if WINDOWS
            //            //// Set the margins for the controls in the title bar for Windows if using the Shell
            //            imgbtnAbout.Margin = new Thickness(20, 0, 0, 0);
            //            lblTitle.Margin = new Thickness(20, 8, 0, 0);
            //#endif
#if WINDOWS
            // !!!BUG!!! in Windows - Set the ColumnDefinitions for the TitleView because XAML 140* does not work in Windows if using the NavigationPage
            grdTitleView.ColumnDefinitions.Clear();
            grdTitleView.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
            grdTitleView.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(726) });
            grdTitleView.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });

            imgbtnAbout.HorizontalOptions = LayoutOptions.Center;
            lblTitle.Margin = new Thickness(16, 8, 0, 0);
#endif
            //// Select all the text in the entry field - works for all pages in the app
            Globals.ModifyEntrySelectAllText();

            //// Get the saved settings
            Globals.cTheme = Preferences.Default.Get("SettingTheme", "System");
            Globals.bDateFormatSystem = Preferences.Default.Get("SettingDateFormatSystem", true);
            Globals.cPageFormat = Preferences.Default.Get("SettingPageFormat", "");
            Globals.cNumDecimalDigits = Preferences.Default.Get("SettingNumDecimalDigits", "");
            Globals.cPercDecimalDigits = Preferences.Default.Get("SettingPercDecimalDigits", "2");
            Globals.cRoundNumber = Preferences.Default.Get("SettingRoundNumber", "AwayFromZero");
            Globals.bColorNumber = Preferences.Default.Get("SettingColorNumber", false);
            Globals.cKeyboard = Preferences.Default.Get("SettingKeyboard", "Numeric");
            Globals.cLanguage = Preferences.Default.Get("SettingLanguage", "");
            Globals.bLicense = Preferences.Default.Get("SettingLicense", false);

            //// The height of the title bar is lower when an iPhone is in horizontal position
            if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Phone)
            {
                imgbtnAbout.VerticalOptions = LayoutOptions.Start;
                lblTitle.VerticalOptions = LayoutOptions.Start;
                lblTitle.VerticalTextAlignment = TextAlignment.Start;
                imgbtnSettings.VerticalOptions = LayoutOptions.Start;
            }

            //// Set the theme and the number color
            Globals.SetThemeAndNumberColor();

            //// Get the system date format and set the date format
            Globals.cSysDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        
            if (Globals.bDateFormatSystem == true)
            {
                Globals.cDateFormat = Globals.cSysDateFormat;
            }
            else
            {
                Globals.cDateFormat = "yyyy-MM-dd";
            }
            //App.Current.MainPage.DisplayAlert("Globals.cDateFormat", Globals.cDateFormat, "OK");  // For testing

            //// Get the number decimal separator
            Globals.cNumDecimalSeparator = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Debug.WriteLine($"Number Decimal Separator: {Globals.cNumDecimalSeparator}");

            //// Get the number of decimal digits after the decimal point
            if (string.IsNullOrEmpty(Globals.cNumDecimalDigits))
            {
                Globals.cNumDecimalDigits = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits);
            }
            Debug.WriteLine($"Number Decimal Digits: {Globals.cNumDecimalDigits}");

            //// Get the system culture and country code
            string cCountry2LetterISO;

            try
            {
#if IOS
                // iOS                
                // !!!BUG!!! in iOS: The result of the CurrentCulture is wrong (shows only the language: en)
                // since .NET 9, so we use the CurrentLocale
                string cLocale = NSLocale.CurrentLocale.LocaleIdentifier;   // "en_US@rg=bezzzz" for United States @ region Belgium
                Debug.WriteLine($"cLocale: {cLocale}");
                cCountry2LetterISO = cLocale.Substring(3, 2);               // "US" for United States
#else
                //Android and Windows
                CultureInfo cCulture = CultureInfo.CurrentCulture;
                Debug.WriteLine($"cCulture.Name: {cCulture.Name}");         // "en_US" for United States
                cCountry2LetterISO = cCulture.Name.Split('-')[1];           // "US" for United States
#endif
                Debug.WriteLine($"cCountry2LetterISO: {cCountry2LetterISO}");
            }
            catch (Exception Ex)
            {
                Debug.WriteLine("CurrentCulture or CurrentLocale failed.  " + Ex.Message);
                cCountry2LetterISO = "US";
            }

            //// Get the system ISO currency code
            RegionInfo myRegInfo = new(cCountry2LetterISO);
            Globals.cISOCurrencyCode = myRegInfo.ISOCurrencySymbol;
            Debug.WriteLine($"ISO Currency Code: {Globals.cISOCurrencyCode}");

            //// Set the page format
            if (string.IsNullOrEmpty(Globals.cPageFormat))
            {
                Globals.cPageFormat = "CA;CL;CO;CR;DO;GT;MX;PA;PH;US".Contains(cCountry2LetterISO) ? "Letter" : "A4";
            }

            //// Set the rounding system of numbers
            if (string.IsNullOrEmpty(Globals.cRoundNumber))
            {
                Globals.cRoundNumber = "AwayFromZero";
            }

            //// Get and set the system OS user language
            try
            {
                if (string.IsNullOrEmpty(Globals.cLanguage))
                {
                    Globals.cLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                }
                Debug.WriteLine($"Language: {Globals.cLanguage}");
            }
            catch (Exception)
            {
                Globals.cLanguage = "en";
            }

            SetTextLanguage();
        }

        //// Buttons clicked events
        private async void OnPageInterestEffectiveClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageInterestEffective());
        }

        private async void OnPageInterestEffectiveBEClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageInterestEffectiveBE());
        }

        private async void OnPageInterestAnnualClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageInterestAnnual());
        }

        private async void OnPageInterestMonthDayClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageInterestMonthDay());
        }

        private async void OnPageInterestPayDiscountClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageInterestPayDiscount());
        }

        private async void OnPageLoanDetailClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageLoanDetail());
        }

        private async void OnPageVATCalculationClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageVATCalculation());
        }

        private async void OnPageAmountGrossOfNetClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageAmountGrossOfNet());
        }

        private async void OnPageInvestmentReturnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageInvestmentReturn());
        }

        private async void OnPageDifferenceNumbersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageDifferenceNumbers());
        }

        private async void OnPageDifferenceDatesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageDifferenceDates());
        }

        private async void OnPageSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageSettings());
        }

        private async void OnPageAboutClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PageAbout());
        }

        /// <summary>
        /// Put text in the chosen language in the controls 
        /// </summary>
        private void SetTextLanguage()
        {
            // Set the CurrentUICulture
            //Globals.cLanguage = "es";  // For testing
            //App.Current.MainPage.DisplayAlert("Globals.cLanguage", Globals.cLanguage, "OK");  // For testing

            Globals.SetCultureSelectedLanguage();

            cCopyright = $"{FinLang.Copyright_Text} © 1992-2025 Geert Geerits";
            cLicenseText = $"{FinLang.License_Text}\n\n{FinLang.LicenseMit2_Text}";

            //App.Current.MainPage.DisplayAlert(FinLang.ErrorTitle_Text, Globals.cLanguage, cButtonCloseText);  // For testing
        }

        /// <summary>
        /// Show license using the Loaded event of the MainPage.xaml 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnPageLoad(object sender, EventArgs e)
        {
            if (Globals.bLicense == false)
            {
                Globals.bLicense = await Application.Current!.Windows[0].Page!.DisplayAlert(FinLang.LicenseTitle_Text, $"Finance\n{cCopyright}\n\n{cLicenseText}", FinLang.Agree_Text, FinLang.Disagree_Text);

                if (Globals.bLicense)
                {
                    Preferences.Default.Set("SettingLicense", true);
                }
                else
                {
#if IOS
                    //Thread.CurrentThread.Abort();  // Not allowed in iOS
                    imgbtnAbout.IsEnabled = false;
                    imgbtnSettings.IsEnabled= false;
                    btnInterestEffective.IsEnabled = false;
                    btnInterestEffectiveBE.IsEnabled = false;
                    btnInterestAnnual.IsEnabled = false;
                    btnInterestMonthDay.IsEnabled = false;
                    btnInterestPayDiscount.IsEnabled = false;
                    btnLoanDetail.IsEnabled = false;
                    btnVATCalculation.IsEnabled = false;
                    btnAmountGrossOfNet.IsEnabled = false;
                    btnInvestmentReturn.IsEnabled = false;
                    btnDifferenceNumbers.IsEnabled = false;
                    btnDifferenceDates.IsEnabled = false;

                    await DisplayAlert(FinLang.LicenseTitle_Text, FinLang.CloseApplication_Text, FinLang.ButtonClose_Text);
#else
                    Application.Current.Quit();
#endif
                }
            }
        }

        /// <summary>
        /// Set language using the Appearing event of the MainPage.xaml 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageAppearing(object sender, EventArgs e)
        {
            if (Globals.bLanguageChanged)
            {
                SetTextLanguage();
                Globals.bLanguageChanged = false;

                //DisplayAlert("Globals.bLanguageChanged", "true", "OK");  // For testing
            }
        }
    }
}
/* CultureInfo.CurrentCulture Property
   https://learn.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.currentculture?view=net-8.0
   CultureInfo.CurrentCulture.Name); Display the name of the current culture - returns en-US
   CultureInfo.CurrentUICulture.Name); Display the name of the current UI culture - returns en-US

Android:
[0:] Number Decimal Separator: .
[0:] Number Decimal Digits: 2
[0:] Culture: en-US
[0:] Country: US
[0:] ISO Currency Code: USD
[0:] Language: en

iOS
[0:] Number Decimal Separator: .
[0:] Number Decimal Digits: 2
[0:] Culture: en
[0:] Locale: en_BE
[0:] Country: BE
[0:] ISO Currency Code: EUR
[0:] Language: en

Windows
Number Decimal Separator: ,
Number Decimal Digits: 2
Culture: en-BE
Country: BE
ISO Currency Code: EUR
Language: en */
