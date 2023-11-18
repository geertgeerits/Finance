// Program .....: Finance.sln
// Author ......: Geert Geerits - E-mail: geertgeerits@gmail.com
// Copyright ...: (C) 1992-2023
// Version .....: 3.0.64
// Date ........: 2023-11-18 (YYYY-MM-DD)
// Language ....: Microsoft Visual Studio 2022: .NET 8.0 MAUI C# 12.0
// Description .: Financial calculations
// Thanks to ...: Gerald Versluis

namespace Finance;

public partial class MainPage : ContentPage
{
    // Local variables.
    private string cCopyright;
    private string cLicenseText;

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

        // Get the saved settings.License
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

        // The height of the title bar is lower when an iPhone is in horizontal position.
        if (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.Idiom == DeviceIdiom.Phone)
        {
            imgbtnAbout.VerticalOptions = LayoutOptions.Start;
            lblTitle.VerticalOptions = LayoutOptions.Start;
            lblTitle.VerticalTextAlignment = TextAlignment.Start;
            imgbtnSettings.VerticalOptions = LayoutOptions.Start;
        }

        // Set the theme and the number color.
        Globals.SetThemeAndNumberColor();

        // Get the system date format and set the date format.
        Globals.cSysDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        
        if (Globals.bDateFormatSystem == true)
        {
            Globals.cDateFormat = Globals.cSysDateFormat;
        }
        else
        {
            Globals.cDateFormat = "yyyy-MM-dd";
        }
        //App.Current.MainPage.DisplayAlert("Globals.cDateFormat", Globals.cDateFormat, "OK");  // For testing.

        // Get the number decimal separator.
        Globals.cNumDecimalSeparator = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

        // Get the number of decimal digits after the decimal point.
        if (string.IsNullOrEmpty(Globals.cNumDecimalDigits))
        {
            Globals.cNumDecimalDigits = Convert.ToString(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits);
        }

        // Get the system ISO currency code.
        string cCountry = Thread.CurrentThread.CurrentCulture.Name.Substring(3, 2);       
        
        RegionInfo myRegInfo = new(cCountry);
        Globals.cISOCurrencyCode = myRegInfo.ISOCurrencySymbol;

        // Set the page format.
        if (string.IsNullOrEmpty(Globals.cPageFormat))
        {
            Globals.cPageFormat = "CA;CL;CO;CR;DO;GT;MX;PA;PH;US".Contains(cCountry) ? "Letter" : "A4";
        }

        // Set the rounding system of numbers.
        if (string.IsNullOrEmpty(Globals.cRoundNumber))
        {
            Globals.cRoundNumber = "AwayFromZero";
        }

        // Get and set the system OS user language.
        try
        {
            if (string.IsNullOrEmpty(Globals.cLanguage))
            {
                Globals.cLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            }
        }
        catch (Exception)
        {
            Globals.cLanguage = "en";
        }

        SetTextLanguage();
    }

    // Buttons clicked events.
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
    
    // Put text in the chosen language in the controls.
    private void SetTextLanguage()
    {
        // Set the CurrentUICulture.
        //Globals.cLanguage = "es";  // For testing.
        //App.Current.MainPage.DisplayAlert("Globals.cLanguage", Globals.cLanguage, "OK");  // For testing.

        Globals.SetCultureSelectedLanguage();

        cCopyright = $"{FinLang.Copyright_Text} © 1992-2023 Geert Geerits";
        cLicenseText = $"{FinLang.License_Text}\n\n{FinLang.LicenseMit2_Text}";

        //App.Current.MainPage.DisplayAlert(FinLang.ErrorTitle_Text, Globals.cLanguage, cButtonCloseText);  // For testing.
    }

    // Show license using the Loaded event of the MainPage.xaml.
    private async void OnPageLoad(object sender, EventArgs e)
    {
        if (Globals.bLicense == false)
        {
            Globals.bLicense = await Application.Current.MainPage.DisplayAlert(FinLang.LicenseTitle_Text, $"Finance\n{cCopyright}\n\n{cLicenseText}", FinLang.Agree_Text, FinLang.Disagree_Text);

            if (Globals.bLicense)
            {
                Preferences.Default.Set("SettingLicense", true);
            }
            else
            {
#if IOS
                //Thread.CurrentThread.Abort();  // Not allowed in iOS.
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

    // Set language using the Appearing event of the MainPage.xaml.
    private void OnPageAppearing(object sender, EventArgs e)
    {
        if (Globals.bLanguageChanged)
        {
            SetTextLanguage();
            Globals.bLanguageChanged = false;

            //DisplayAlert("Globals.bLanguageChanged", "true", "OK");  // For testing.
        }
    }
}
