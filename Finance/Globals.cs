//// Global usings
global using Finance.Resources.Languages;
global using System.Globalization;

using System.Diagnostics;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace Finance
{
    //// Global variables and methods
    internal static class Globals
    {
        // Global variables
        public static string cTheme = "";
        public static bool bDateFormatSystem;
        public static string cSysDateFormat = "";
        public static string cDateFormat = "";
        public static string cNumDecimalSeparator = "";
        public static string cNumDecimalDigits = "";
        public static string cPercDecimalDigits = "";
        public static string cRoundNumber = "";
        public static bool bColorNumber;
        public static string cColorNegNumber = "";
        public static string cColorPosNumber = "";
        public static string cISOCurrencyCode = "";
        public static string cKeyboard = "";
        public static string cLanguage = "";
        public static bool bLanguageChanged;
        public static string cPageFormat = "";
        public static bool bLicense;

        // Local variables
        private static readonly string cColorNegNumberLight = "#FF0000";
        private static readonly string cColorPosNumberLight = "#000000";
        private static readonly string cColorNegNumberDark = "#FFB0B0";
        private static readonly string cColorPosNumberDark = "#FFFFFF";

        /// Global methods
        /// <summary>
        /// Set the theme and the number text color 
        /// </summary>
        public static void SetThemeAndNumberColor()
        {
            switch (cTheme)
            {
                case "Light":
                    Application.Current!.UserAppTheme = AppTheme.Light;

                    cColorNegNumber = bColorNumber ? cColorNegNumberLight : cColorPosNumberLight;
                    cColorPosNumber = cColorPosNumberLight;
                    break;

                case "Dark":
                    Application.Current!.UserAppTheme = AppTheme.Dark;

                    cColorNegNumber = bColorNumber ? cColorNegNumberDark : cColorPosNumberDark;
                    cColorPosNumber = cColorPosNumberDark;
                    break;

                default:
                    Application.Current!.UserAppTheme = AppTheme.Unspecified;

                    // Get the current device theme and set the number color
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

        /// <summary>
        /// Set the current UI culture of the selected language 
        /// </summary>
        public static void SetCultureSelectedLanguage()
        {
            try
            {
                CultureInfo switchToCulture = new(cLanguage);
                LocalizationResourceManager.Instance.SetCulture(switchToCulture);
            }
            catch
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Replace decimal point with decimal comma - The number may not be formatted with the 'N' specifier
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        public static string ReplaceDecimalPointComma(string cNumber)
        {
            // Check if the string cNumber is a number
            if (string.IsNullOrEmpty(cNumber) || !double.TryParse(cNumber, out _))
            {
                return cNumber;
            }

            if (cNumDecimalSeparator == "," && cNumber.Contains('.'))
            {
                cNumber = cNumber.Replace(".", ",");
            }
            else if (cNumDecimalSeparator == "." && cNumber.Contains(','))
            {
                cNumber = cNumber.Replace(",", ".");
            }

            return cNumber;
        }

        /// <summary>
        /// Set the Placeholder and MaxLength for the numeric entry field
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="cWholeNumFrom"></param>
        /// <param name="cDecDigetFrom"></param>
        /// <param name="cWholeNumTo"></param>
        /// <param name="cDecDigetTo"></param>
        /// <param name="cNumberOfDecimals"></param>
        /// <param name="cMaxNumberOfDecimals"></param>
        public static void SetEntryProperties(Entry entry, string cWholeNumFrom, string cDecDigetFrom, string cWholeNumTo, string cDecDigetTo, string cNumberOfDecimals, string cMaxNumberOfDecimals)
        {
            if (!int.TryParse(cNumberOfDecimals, out int nNumberOfDecimals) || !int.TryParse(cMaxNumberOfDecimals, out int nMaxNumberOfDecimals))
            {
                return;
            }

            if (nNumberOfDecimals > nMaxNumberOfDecimals)
            {
                nNumberOfDecimals = nMaxNumberOfDecimals;
            }

            string cValueFrom = cDecDigetFrom == "0" ? cWholeNumFrom : $"{cWholeNumFrom}{cNumDecimalSeparator}{string.Concat(Enumerable.Repeat(cDecDigetFrom, nNumberOfDecimals))}";
            string cValueTo = $"{cWholeNumTo}{cNumDecimalSeparator}{string.Concat(Enumerable.Repeat(cDecDigetTo, nNumberOfDecimals))}";
            
            entry.Placeholder = $"{cValueFrom} - {cValueTo}";
            entry.MaxLength = cValueTo.Length > cValueFrom.Length ? cValueTo.Length : cValueFrom.Length;

            Debug.WriteLine($"entry.MaxLength: {entry.MaxLength}");
        }

        /* Rounding numbers
         * Round away from zero: MidpointRounding.AwayFromZero = 1-4 down ; 5-9 up
         * Round half to even or banker's rounding: MidpointRounding.ToEven
         *
         *  Value      Default    ToEven     AwayFromZero    ToZero
         *   12.0       12         12         12              12
         *   12.1       12         12         12              12
         *   12.2       12         12         12              12
         *   12.3       12         12         12              12
         *   12.4       12         12         12              12
         *   12.5       12         12         13              12
         *   12.6       13         13         13              12
         *   12.7       13         13         13              12
         *   12.8       13         13         13              12
         *   12.9       13         13         13              12
         *   13.0       13         13         13              13
         *
         * Format specifier: "F" = 1234.56 or 1234,56 ; "N" = 1,234.56 or 1.234,56 */

        /// <summary>
        /// Rounding and formatting double number to # decimals returning number as value and as string 
        /// </summary>
        /// <param name="nNumber"></param>
        /// <param name="nNumDec"></param>
        /// <param name="cFormatSpecifier"></param>
        /// <returns></returns>
        public static string RoundToNumDecimals(ref double nNumber, int nNumDec, string cFormatSpecifier)
        {
            if (cRoundNumber == "AwayFromZero")
            {
                nNumber = Math.Round(nNumber, nNumDec, MidpointRounding.AwayFromZero);
            }
            else if (cRoundNumber == "ToEven")
            {
                nNumber = Math.Round(nNumber, nNumDec, MidpointRounding.ToEven);
            }

            string cNumFormat = cFormatSpecifier + nNumDec.ToString();
            return nNumber.ToString(cNumFormat);
        }

        /// <summary>
        /// Rounding and formatting decimal number to # decimals returning number as value and as string 
        /// </summary>
        /// <param name="nNumber"></param>
        /// <param name="nNumDec"></param>
        /// <param name="cFormatSpecifier"></param>
        /// <returns></returns>
        public static string RoundToNumDecimals(ref decimal nNumber, int nNumDec, string cFormatSpecifier)
        {
            if (cRoundNumber == "AwayFromZero")
            {
                nNumber = Math.Round(nNumber, nNumDec, MidpointRounding.AwayFromZero);
            }
            else if (cRoundNumber == "ToEven")
            {
                nNumber = Math.Round(nNumber, nNumDec, MidpointRounding.ToEven);
            }

            string cNumFormat = cFormatSpecifier + nNumDec.ToString();
            return nNumber.ToString(cNumFormat);
        }

        /// <summary>
        /// Set the label text color to a different color for a negative and a positive number 
        /// </summary>
        /// <param name="label"></param>
        public static void SetLabelTextColorForNumber(Label label)
        {
            if (decimal.TryParse(label.Text, out decimal nValue))
            {
                label.TextColor = nValue < 0 ? Color.FromArgb(cColorNegNumber) : Color.FromArgb(cColorPosNumber);
            }
        }

        /// <summary>
        /// Select all the text in the entry field
        /// </summary>
        public static void ModifyEntrySelectAllText()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("MyCustomization", (handler, view) =>
            {
#if ANDROID
            handler.PlatformView.SetSelectAllOnFocus(true);
#elif IOS || MACCATALYST
                handler.PlatformView.EditingDidBegin += (s, e) =>
                {
                    handler.PlatformView.PerformSelector(new ObjCRuntime.Selector("selectAll"), null, 0.0f);
                };
#elif WINDOWS
            handler.PlatformView.GotFocus += (s, e) =>
            {
                handler.PlatformView.SelectAll();
            };
#endif
            });
        }

        ///// <summary>
        ///// Close the keyboard 
        ///// </summary>
        ///// <param name="entry"></param>
        ///// <param name="e"></param>
        //public async static void CloseKeyboard(Entry entry, EventArgs e)
        //{
        //    try
        //    {
        //        entry.IsEnabled = false;
        //        entry.IsEnabled = true;
        //        // or
        //        await entry.HideSoftInputAsync(default);
        //        await entry.ShowSoftInputAsync(default);
        //    }
        //    catch (Exception)
        //    {
        //        return;
        //    }
        //}
    }
}