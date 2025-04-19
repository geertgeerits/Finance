namespace Finance
{
    internal static class ClassEntryMethods
    {
        // Global variables
        public static string cNumDecimalSeparator = "";
        public static string cNumDecimalDigits = "";
        public static string cPercDecimalDigits = "";
        public static string cRoundNumber = "";
        public static bool bColorNumber;
        public static string cColorNegNumber = "";
        public static string cColorPosNumber = "";

        // Local variables
        private const string cNumericCharacters = ",-.0123456789";
        private static readonly string cColorNegNumberLight = "#FF0000";
        private static readonly string cColorPosNumberLight = "#000000";
        private static readonly string cColorNegNumberDark = "#FFB0B0";
        private static readonly string cColorPosNumberDark = "#FFFFFF";

        /// <summary>
        /// Set the Placeholder and MaxLength for a numeric entry field
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="cWholeNumFrom"></param>
        /// <param name="cDecDigetFrom"></param>
        /// <param name="cWholeNumTo"></param>
        /// <param name="cDecDigetTo"></param>
        /// <param name="cNumberOfDecimals"></param>
        /// <param name="cMaxNumberOfDecimals"></param>
        public static void SetNumberEntryProperties(Entry entry, string cWholeNumFrom, string cDecDigetFrom, string cWholeNumTo, string cDecDigetTo, string cNumberOfDecimals, string cMaxNumberOfDecimals)
        {
            if (!decimal.TryParse(cWholeNumFrom, out _) || !int.TryParse(cDecDigetFrom, out _) || !decimal.TryParse(cWholeNumTo, out _) || !int.TryParse(cDecDigetTo, out _) || !int.TryParse(cNumberOfDecimals, out int nNumberOfDecimals) || !int.TryParse(cMaxNumberOfDecimals, out int nMaxNumberOfDecimals))
            {
                return;
            }

            if (nNumberOfDecimals > nMaxNumberOfDecimals)
            {
                nNumberOfDecimals = nMaxNumberOfDecimals;
            }

            string cDecimalSeparator = nNumberOfDecimals switch
            {
                0 => "",
                _ => cNumDecimalSeparator,
            };

            string cValueFrom = cDecDigetFrom == "0" ? cWholeNumFrom : $"{cWholeNumFrom}{cDecimalSeparator}{string.Concat(Enumerable.Repeat(cDecDigetFrom, nNumberOfDecimals))}";
            string cValueTo = $"{cWholeNumTo}{cDecimalSeparator}{string.Concat(Enumerable.Repeat(cDecDigetTo, nNumberOfDecimals))}";

            entry.Placeholder = $"{cValueFrom} - {cValueTo}";
            entry.MaxLength = cValueTo.Length > cValueFrom.Length ? cValueTo.Length : cValueFrom.Length;

            Debug.WriteLine($"entry.MaxLength: {entry.MaxLength}");
        }

        /// <summary>
        /// Test if the text is a numeric value
        /// </summary>
        /// <param name="cText"></param>
        /// <returns></returns>
        public static bool IsNumeric(Entry entry, string cText)
        {
            foreach (char c in cText)
            {
                if (!cNumericCharacters.Contains(c))
                {
                    return false;
                }

                // Check if the character is a negative sign - the negative sign must be at the first position (index 0)
                if (c == '-' && !cText.StartsWith(c))
                {
                    return false;
                }

                // Check if the character is a decimal separator
                if (c == cNumDecimalSeparator[0])
                {
                    // Check if the character is already in the string
                    if (cText.IndexOf(c) != cText.LastIndexOf(c))
                    {
                        return false;
                    }
                }
            }

            // Check the number of decimals after the decimal separator
            int nDecimals = entry.AutomationId switch
            {
                "Percentage" => int.Parse(cPercDecimalDigits),
                _ => int.Parse(cNumDecimalDigits),
            };

            // Search for the decimal separator in the string
            if (cText.Contains(cNumDecimalSeparator[0]))
            {
                // Check if the number of decimals is greater than the allowed number of decimals
                if (cText.Length - cText.IndexOf(cNumDecimalSeparator[0]) > nDecimals + 1)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Entry focused event: format the text value for a numeric entry without the number separator and select the entire text value
        /// </summary>
        /// <param name="entry"></param>
        public async static void FormatNumberEntryFocused(Entry entry)
        {
            // Show the keyboard if it is not already shown
            if (!entry.IsSoftInputShowing())
            {
                _ = await entry.ShowSoftInputAsync(System.Threading.CancellationToken.None);
            }

            if (string.IsNullOrEmpty(entry.Text))
            {
                return;
            }

            if (decimal.TryParse(entry.Text, out decimal nValue))
            {
                // Ensure AutomationId is set before accessing it (Entry property: AutomationId="Percentage")
                entry.Text = entry.AutomationId switch
                {
                    "Percentage" => RoundToNumDecimals(ref nValue, int.Parse(cPercDecimalDigits), "F"),
                    _ => RoundToNumDecimals(ref nValue, int.Parse(cNumDecimalDigits), "F"),
                };

                entry.CursorPosition = 0;
                entry.SelectionLength = entry.Text.Length;
            }
        }

        /// <summary>
        /// Entry unfocused event: format the text value for a numeric entry field with the number separator
        /// </summary>
        /// <param name="entry"></param>
        public static void FormatNumberEntryUnfocused(Entry entry)
        {
            if (string.IsNullOrEmpty(entry.Text))
            {
                return;
            }

            // Replace decimal point with decimal comma or point
            entry.Text = ReplaceDecimalPointComma(entry.Text);

            if (decimal.TryParse(entry.Text, out decimal nValue))
            {
                // Ensure AutomationId is set before accessing it
                entry.Text = entry.AutomationId switch
                {
                    "Percentage" => RoundToNumDecimals(ref nValue, int.Parse(cPercDecimalDigits), "N"),
                    _ => RoundToNumDecimals(ref nValue, int.Parse(cNumDecimalDigits), "N"),
                };
            }
            else
            {
                entry.Text = "";
                entry.Focus();
            }
        }

        /// <summary>
        /// Replace decimal point with decimal comma or point - The number may NOT be formatted with the 'N' specifier because there can be more than 1 separator
        /// </summary>
        /// <param name="cNumber"></param>
        /// <returns></returns>
        private static string ReplaceDecimalPointComma(string cNumber)
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

            return nNumber.ToString(format: cFormatSpecifier + nNumDec.ToString());
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

            return nNumber.ToString(format: cFormatSpecifier + nNumDec.ToString());
        }

        /// Global methods
        /// <summary>
        /// Set the theme and the number text color 
        /// </summary>
        public static void SetNumberColor()
        {
            if (Application.Current == null)
            {
#if DEBUG
                Debug.WriteLine("Application.Current is null. Ensure the application is properly initialized.");
#endif
                return;
            }

            // Get the current device theme
            AppTheme currentTheme = Application.Current.RequestedTheme;

            //  Set the number text color
            switch (currentTheme)
            {
                case AppTheme.Light:
                    cColorNegNumber = bColorNumber ? cColorNegNumberLight : cColorPosNumberLight;
                    cColorPosNumber = cColorPosNumberLight;
                    break;

                case AppTheme.Dark:
                    cColorNegNumber = bColorNumber ? cColorNegNumberDark : cColorPosNumberDark;
                    cColorPosNumber = cColorPosNumberDark;
                    break;
                
                default:
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

        /// <summary>
        /// Hide the keyboard
        /// </summary>
        /// <param name="entry"></param>
        public async static void HideKeyboard(Entry entry)
        {
            try
            {
                if (entry.IsSoftInputShowing())
                {
                    // Android !!!BUG!!!: entry.Unfocus() must be called before HideSoftInputAsync() otherwise entry.Unfocus() is not called
                    entry.Unfocus();
                    _ = await entry.HideSoftInputAsync(System.Threading.CancellationToken.None);
                }
            }
            catch (Exception)
            {
                entry.IsEnabled = false;
                entry.IsEnabled = true;

                return;
            }
        }
    }
}
