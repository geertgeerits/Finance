namespace Finance
{
    internal static class ClassEntryMethods
    {
        // Global variables
        public static string cNumDecimalDigits = "";
        public static string cPercDecimalDigits = "";
        public static string cRoundNumber = "";
        public static bool bColorNumber;
        public static bool bExecuteMethodIsNumeric;

        // Local variables
        private static string cNumGroupSeparator = "";
        private static string cNumDecimalSeparator = "";
        private static string cNumericCharacters = "";
        private static string cColorNegNumber = "";
        private static string cColorPosNumber = "";

        /// <summary>
        /// Initialize the number format settings based on the current culture
        /// </summary>
        public static void InitializeNumberFormat()
        {
            // Get the current culture's number format
            NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;

            // Set the number properties based on the current culture
            cNumGroupSeparator = numberFormatInfo.NumberGroupSeparator;
            cNumDecimalSeparator = numberFormatInfo.NumberDecimalSeparator;

            if (string.IsNullOrEmpty(cNumDecimalDigits))
            {
                cNumDecimalDigits = numberFormatInfo.CurrencyDecimalDigits.ToString();
                Preferences.Default.Set("SettingNumDecimalDigits", cNumDecimalDigits);
            }

            if (string.IsNullOrEmpty(cPercDecimalDigits))
            {
                cPercDecimalDigits = numberFormatInfo.PercentDecimalDigits.ToString();
                Preferences.Default.Set("SettingPercDecimalDigits", cPercDecimalDigits);
            }

            //// Set the rounding system of numbers
            if (string.IsNullOrEmpty(cRoundNumber))
            {
                cRoundNumber = "AwayFromZero";
                Preferences.Default.Set("SettingRoundNumber", cRoundNumber);
            }

            Debug.WriteLine($"cNumGroupSeparator: {cNumGroupSeparator}");
            Debug.WriteLine($"cNumDecimalSeparator: {cNumDecimalSeparator}");
            Debug.WriteLine($"cNumDecimalDigits: {cNumDecimalDigits}");
            Debug.WriteLine($"cPercDecimalDigits: {cPercDecimalDigits}");
            Debug.WriteLine($"cRoundNumber: {cRoundNumber}");

            // Set the allowed characters for numeric input
            cNumericCharacters = cNumDecimalSeparator switch
            {
                "," => ",-0123456789",
                "." => "-.0123456789",
                _ => ",-.0123456789",
            };
            
            Debug.WriteLine($"cNumericCharacters: {cNumericCharacters}");
        }

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
            if (!bExecuteMethodIsNumeric)
            {
                return true;
            }

            foreach (char c in cText)
            {
                // Check if the character is a digit or a decimal separator
                if (!cNumericCharacters.Contains(c))
                {
                    return false;
                }

                // Check if the character is a negative sign - the negative sign must be at the first position (index 0)
                if (c == '-' && !cText.StartsWith(c))
                {
                    return false;
                }

                // Check if the character is already in the string
                if (c == cNumDecimalSeparator[0])
                {
                    if (cText.IndexOf(c) != cText.LastIndexOf(c))
                    {
                        return false;
                    }
                }
            }

            // Get the number of decimals allowed after the decimal separator
            int nDecimals = entry.AutomationId switch
            {
                "Percentage" => int.Parse(cPercDecimalDigits),
                _ => int.Parse(cNumDecimalDigits),
            };

            if (cText.Contains(cNumDecimalSeparator))
            {
                if (cText.Length - cText.IndexOf(cNumDecimalSeparator[0]) > nDecimals + 1)
                {
                    return false;
                }
            }

            // Validate the number and set the text color
            if (decimal.TryParse(entry.Text, out decimal nValue))
            {
                entry.TextColor = nValue < 0 ? Color.FromArgb(cColorNegNumber) : Color.FromArgb(cColorPosNumber);
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
            
            // Allow the IsNumeric method to execute
            bExecuteMethodIsNumeric = true;
            
            if (string.IsNullOrEmpty(entry.Text))
            {
                return;
            }

            if (decimal.TryParse(entry.Text, out decimal nValue))
            {
                // Ensure AutomationId is set before accessing it (Entry property: AutomationId="Percentage")
                entry.Text = entry.AutomationId switch
                {
                    "Percentage" => nValue.ToString(format: "F" + cPercDecimalDigits),
                    _ => nValue.ToString(format: "F" + cNumDecimalDigits),
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

            // Do not allow the IsNumeric method to execute
            bExecuteMethodIsNumeric = false;

            if (decimal.TryParse(entry.Text, out decimal nValue))
            {
                // Ensure AutomationId is set before accessing it
                entry.Text = entry.AutomationId switch
                {
                    "Percentage" => nValue.ToString(format: "N" + cPercDecimalDigits),
                    _ => nValue.ToString(format: "N" + cNumDecimalDigits),
                };
            }
            else
            {
                entry.Text = "";
                entry.Focus();
            }
        }

        ///// <summary>
        ///// Replace decimal point with decimal comma or point - The number may NOT be formatted with the 'N' specifier because there can be more than 1 separator
        ///// </summary>
        ///// <param name="cNumber"></param>
        ///// <returns></returns>
        //private static string ReplaceDecimalPointComma(string cNumber)
        //{
        //    // Check if the string cNumber is a number
        //    if (string.IsNullOrEmpty(cNumber) || !decimal.TryParse(cNumber, out _))
        //    {
        //        return cNumber;
        //    }

        //    if (cNumDecimalSeparator == "," && cNumber.Contains('.'))
        //    {
        //        cNumber = cNumber.Replace(".", ",");
        //    }
        //    else if (cNumDecimalSeparator == "." && cNumber.Contains(','))
        //    {
        //        cNumber = cNumber.Replace(",", ".");
        //    }

        //    return cNumber;
        //}

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
        /// Set the entry text color to a different color for a negative and a positive number
        /// </summary>
        public static void SetNumberColor()
        {
            if (Microsoft.Maui.Controls.Application.Current == null)
            {
                Debug.WriteLine("Application.Current is null. Ensure the application is properly initialized.");
                return;
            }

            // Set the color for negative and positive numbers
            const string cColorNegNumberLight = "#FF0000";
            const string cColorPosNumberLight = "#000000";
            const string cColorNegNumberDark = "#FFB0B0";
            const string cColorPosNumberDark = "#FFFFFF";

            // Get the current device theme
            AppTheme currentTheme = Microsoft.Maui.Controls.Application.Current.RequestedTheme;

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

                case AppTheme.Unspecified:
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
