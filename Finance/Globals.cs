// Global usings.
global using Finance.Resources.Languages;
global using System.Globalization;

namespace Finance;

// Global variables and methods.
static class Globals
{
    // Global variables.
    public static string cTheme;
    public static bool bDateFormatSystem;
    public static string cSysDateFormat;
    public static string cDateFormat;
    public static string cNumDecimalSeparator;
    public static string cNumDecimalDigits;
    public static string cPercDecimalDigits;
    public static string cRoundNumber;
    public static bool bColorNumber;
    public static string cColorNegNumber;
    public static string cColorPosNumber;
    public static string cISOCurrencyCode;
    public static string cKeyboard;
    public static string cLanguage;
    public static bool bLanguageChanged = false;
    public static string cPageFormat;
    public static bool bLicense;

    // Local variables.
    private static readonly string cColorNegNumberLight = "#FF0000";
    private static readonly string cColorPosNumberLight = "#000000";
    private static readonly string cColorNegNumberDark = "#FFB0B0";
    private static readonly string cColorPosNumberDark = "#FFFFFF";

    // Global methods.
    // Set the theme and the number text color.
    public static void SetThemeAndNumberColor()
    {
        switch (cTheme)
        {
            case "Light":
                Application.Current.UserAppTheme = AppTheme.Light;

                cColorNegNumber = bColorNumber ? cColorNegNumberLight : cColorPosNumberLight;
                cColorPosNumber = cColorPosNumberLight;
                break;

            case "Dark":
                Application.Current.UserAppTheme = AppTheme.Dark;

                cColorNegNumber = bColorNumber ? cColorNegNumberDark : cColorPosNumberDark;
                cColorPosNumber = cColorPosNumberDark;
                break;

            default:
                Application.Current.UserAppTheme = AppTheme.Unspecified;

                // Get the current device theme and set the number color.
                AppTheme currentTheme = Application.Current.RequestedTheme;
                if (currentTheme == AppTheme.Dark)
                {
                    cColorNegNumber = bColorNumber ? cColorNegNumberDark : cColorPosNumberDark;
                    cColorPosNumber = cColorPosNumberDark;
                }
                else
                {
                    cColorNegNumber = bColorNumber ? cColorNegNumberLight : cColorPosNumberLight;
                    cColorPosNumber = cColorPosNumberLight;
                }
                break;
        }
    }

    // Set the current UI culture of the selected language.
    public static void SetCultureSelectedLanguage()
    {
        try
        {
            CultureInfo switchToCulture = new(cLanguage);
            LocalizationResourceManager.Instance.SetCulture(switchToCulture);
        }
        catch
        {
            // Do nothing.
        }
    }

    // Replace decimal point with decimal comma.
    public static string ReplaceDecimalPointComma(string cNumber)
    {
        if (Globals.cNumDecimalSeparator == "," && cNumber.Contains('.'))
        {
            cNumber = cNumber.Replace(".", ",");
        }
        else if (Globals.cNumDecimalSeparator == "." && cNumber.Contains(','))
        {
            cNumber = cNumber.Replace(",", ".");
        }

        return cNumber;
    }

    // Rounding numbers.
    // Round away from zero: MidpointRounding.AwayFromZero = 1-4 down ; 5-9 up
    // Round half to even or banker's rounding: MidpointRounding.ToEven
    //       Value      Default    ToEven     AwayFromZero    ToZero
    //       12.0       12         12         12              12
    //       12.1       12         12         12              12
    //       12.2       12         12         12              12
    //       12.3       12         12         12              12
    //       12.4       12         12         12              12
    //       12.5       12         12         13              12
    //       12.6       13         13         13              12
    //       12.7       13         13         13              12
    //       12.8       13         13         13              12
    //       12.9       13         13         13              12
    //       13.0       13         13         13              13

    // Format specifier: "F" = 1234.56 or 1234,56 ; "N" = 1,234.56 or 1.234,56.

    // Rounding and formatting double number to # decimals returning number as value and as string.
    public static string RoundDoubleToNumDecimals(ref double nNumber, int nNumDec, string cFormatSpecifier)
    {
        if (Globals.cRoundNumber == "AwayFromZero")
        {
            nNumber = Math.Round(nNumber, nNumDec, MidpointRounding.AwayFromZero);
        }
        else if (Globals.cRoundNumber == "ToEven")
        {
            nNumber = Math.Round(nNumber, nNumDec, MidpointRounding.ToEven);
        }

        string cNumFormat = cFormatSpecifier + nNumDec.ToString();
        return nNumber.ToString(cNumFormat);
    }

    // Rounding and formatting decimal number to # decimals returning number as value and as string.
    public static string RoundDecimalToNumDecimals(ref decimal nNumber, int nNumDec, string cFormatSpecifier)
    {
        if (Globals.cRoundNumber == "AwayFromZero")
        {
            nNumber = Math.Round(nNumber, nNumDec, MidpointRounding.AwayFromZero);
        }
        else if (Globals.cRoundNumber == "ToEven")
        {
            nNumber = Math.Round(nNumber, nNumDec, MidpointRounding.ToEven);
        }

        string cNumFormat = cFormatSpecifier + nNumDec.ToString();
        return nNumber.ToString(cNumFormat);
    }

    // Set the label text color to a different color for a negative and a positive number.
    public static void SetLabelTextColorForNumber(Label label)
    {
        if (decimal.TryParse(label.Text, out decimal nValue))
        {
            label.TextColor = nValue < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);
        }
    }

    // Close the keyboard.
    //public static void CloseKeyboard(Entry entry, EventArgs e)
    //{
    //    try
    //    {
    //        entry.IsEnabled = false;
    //        entry.IsEnabled = true;
    //        //await entry.HideSoftInputAsync(default);
    //    }
    //    catch (Exception)
    //    {
    //        return;
    //    }
    //}
}