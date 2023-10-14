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
    public static string cISOCurrencyCode;
    public static string cKeyboard;
    public static string cLanguage;
    public static bool bLanguageChanged = false;
    public static string cPageFormat;
    public static bool bLicense;

    // Global methods.
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

    // Close the keyboard.
    //public static void CloseKeyboard(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        var entry = (Entry)sender;
    //        entry.IsEnabled = false;
    //        entry.IsEnabled = true;
    //    }
    //    catch (Exception)
    //    {
    //        return;
    //    }
    //}
}