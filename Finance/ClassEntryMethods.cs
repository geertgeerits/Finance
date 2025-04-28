namespace Finance
{
    internal static class ClassEntryMethods
    {
        // Global variables
        public static string cNumDecimalDigits = "";
        public static string cPercDecimalDigits = "";
        public static string cRoundNumber = "";
        public static bool bColorNumber;
        public static bool bShowFormattedNumber;

        // Local variables
        private static string cNumGroupSeparator = "";
        private static string cNumDecimalSeparator = "";
        private static string cNumNegativeSign = "";
        private static string cNumNativeDigits = "";
        private static string cNumericCharacters = "";
        private static string cColorNegNumber = "";
        private static string cColorPosNumber = "";

        /// <summary>
        /// Initialize the number format settings based on the current culture
        /// </summary>
        public static void InitializeNumberFormat()
        {
            // Get the current culture's number format
            // For initializing number formatting settings, use CurrentCulture
            // It ensures that the number format aligns with the user's regional settings, which is the intended behavior in the InitializeNumberFormat method
            NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;

            // Set the number properties based on the current culture
            cNumGroupSeparator = numberFormatInfo.NumberGroupSeparator;                 // Use numberFormatInfo.Number...
            cNumDecimalSeparator = numberFormatInfo.NumberDecimalSeparator;             // Use numberFormatInfo.Number...
            cNumNegativeSign = numberFormatInfo.NegativeSign;
            cNumNativeDigits = string.Join("", numberFormatInfo.NativeDigits);          // Get the native digits as a string from the array

            if (string.IsNullOrEmpty(cNumDecimalDigits))
            {
                cNumDecimalDigits = numberFormatInfo.CurrencyDecimalDigits.ToString();  // Use numberFormatInfo.Currency...
                Preferences.Default.Set("SettingNumDecimalDigits", cNumDecimalDigits);
            }

            if (string.IsNullOrEmpty(cPercDecimalDigits))
            {
                cPercDecimalDigits = numberFormatInfo.PercentDecimalDigits.ToString();  // Use numberFormatInfo.Percent...
                Preferences.Default.Set("SettingPercDecimalDigits", cPercDecimalDigits);
            }

            // Set the rounding system of numbers
            if (string.IsNullOrEmpty(cRoundNumber))
            {
                cRoundNumber = "AwayFromZero";
                Preferences.Default.Set("SettingRoundNumber", cRoundNumber);
            }

            Debug.WriteLine($"cNumGroupSeparator: {cNumGroupSeparator}");
            Debug.WriteLine($"cNumDecimalSeparator: {cNumDecimalSeparator}");
            Debug.WriteLine($"cNumNegativeSign: {cNumNegativeSign}");
            Debug.WriteLine($"cNumNativeDigits: {cNumNativeDigits}");
            Debug.WriteLine($"cNumDecimalDigits: {cNumDecimalDigits}");
            Debug.WriteLine($"cPercDecimalDigits: {cPercDecimalDigits}");
            Debug.WriteLine($"cRoundNumber: {cRoundNumber}");

            // Check the number settings and set default values if they are empty
            if (string.IsNullOrEmpty(cNumGroupSeparator))
            {
                cNumGroupSeparator = ",";
            }

            if (string.IsNullOrEmpty(cNumDecimalSeparator))
            {
                cNumDecimalSeparator = cNumGroupSeparator == "," ? "." : ",";
            }

            if (string.IsNullOrEmpty(cNumNegativeSign))
            {
                cNumNegativeSign = "-";
            }

            if (string.IsNullOrEmpty(cNumNativeDigits))
            {
                cNumNativeDigits = "0123456789";
            }

            if (string.IsNullOrEmpty(cNumDecimalDigits))
            {
                cNumDecimalDigits = "2";
            }

            if (string.IsNullOrEmpty(cPercDecimalDigits))
            {
                cPercDecimalDigits = "2";
            }

            // Set the allowed characters for numeric input
            cNumericCharacters = $"{cNumDecimalSeparator}{cNumNegativeSign}{cNumNativeDigits}";
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
        /// <param name="bIncreaseMaxLength"></param>
        public static void SetNumberEntryProperties(Entry entry, string cWholeNumFrom, string cDecDigetFrom, string cWholeNumTo, string cDecDigetTo, string cNumberOfDecimals, bool bIncreaseMaxLength = false)
        {
            if (!decimal.TryParse(cWholeNumFrom, out _) || !int.TryParse(cDecDigetFrom, out _) || !decimal.TryParse(cWholeNumTo, out _) || !int.TryParse(cDecDigetTo, out _) || !int.TryParse(cNumberOfDecimals, out int nNumberOfDecimals))
            {
                return;
            }

            string cDecimalSeparator = nNumberOfDecimals switch
            {
                0 => "",
                _ => cNumDecimalSeparator,
            };

            string cValueFrom = cDecDigetFrom == "0" ? cWholeNumFrom : $"{cWholeNumFrom}{cDecimalSeparator}{string.Concat(Enumerable.Repeat(cDecDigetFrom, nNumberOfDecimals))}";
            string cValueTo = $"{cWholeNumTo}{cDecimalSeparator}{string.Concat(Enumerable.Repeat(cDecDigetTo, nNumberOfDecimals))}";

            // Set the Placeholder text for the entry field
            entry.Placeholder = $"{cValueFrom} - {cValueTo}";

            // Calculate and set the MaxLength of the entry with the group separators
            decimal nValueFrom = decimal.Parse(cValueFrom);
            decimal nValueTo = decimal.Parse(cValueTo);
            string cRoundedNumber = string.Empty;

            if (cValueFrom.Length > cValueTo.Length)
            {
                cRoundedNumber = nValueFrom.ToString(format: "N" + nNumberOfDecimals);
            }
            else
            {
                cRoundedNumber = nValueTo.ToString(format: "N" + nNumberOfDecimals);
            }

            Debug.WriteLine($"cRoundedNumber: {cRoundedNumber}");

            // Increase the MaxLength by 3 to allow result entry fields
            // and add also '2' extra just to be sure that the MaxLength is enough when using unusual group sizes
            /*
                       999999999999.9999 = 17 characters
                      9999999999999.9999 = 18 characters
                    999,999,999,999.9999 = 20 characters
                  9,999,999,999,999.9999 = 22 characters
                  9,99,99,99,99,999.9999 = 22 characters    India
                 99,99,99,99,99,999.9999 = 23 characters    India 10,00,00,00,00,000.0000

                      -999999999999.9999 = 18 characters
                     -9999999999999.9999 = 19 characters
                   -999,999,999,999.9999 = 21 characters
                 -9,999,999,999,999.9999 = 23 characters
                 -9,99,99,99,99,999.9999 = 23 characters    India
                -99,99,99,99,99,999.9999 = 24 characters    India -10,00,00,00,00,000.0000
            */
            int nIncreaseMaxLength = 0;

            if (bIncreaseMaxLength)
            {
                nIncreaseMaxLength += 3;
            }
            else if (!bIncreaseMaxLength)
            {
                nIncreaseMaxLength += 2;
            }

            //entry.MaxLength = cRoundedNumber.Length + nIncreaseMaxLength;

#if WINDOWS
            // Windows !!!BUG!!!: MaxLength problem for Entry - when reaching the MaxLength and moving to the next field the app hangs
            //entry.MaxLength *= 2;
#endif
            Debug.WriteLine($"entry.MaxLength: {entry.MaxLength}");
        }

        /// <summary>
        /// Check if the text is a numeric value
        /// </summary>
        /// <param name="cText"></param>
        /// <returns></returns>
        public static bool IsNumeric(Entry entry, string cText)
        {
            // Do not execute this method because this is only to show the formatted number just like in a label
            if (bShowFormattedNumber)
            {
                return true;
            }

            if (string.IsNullOrEmpty(cText))
            {
                return true;
            }

            // Check the text for invalid characters
            foreach (char c in cText)
            {
                // Check if the character is allowed
                if (!cNumericCharacters.Contains(c))
                {
                    return false;
                }

                // Check if the character is a negative sign and at the first position (index 0), or there is no more than one negative sign
                if ((c == cNumNegativeSign[0] && !cText.StartsWith(c)) || cText.Count(static ch => ch == cNumNegativeSign[0]) > 1)
                {
                    return false;
                }

                // Check if the decimal separator is already in the string
                if (c == cNumDecimalSeparator[0])
                {
                    if (cText.IndexOf(c) != cText.LastIndexOf(c))
                    {
                        return false;
                    }
                }
            }

            // Get and check the number of decimals allowed after the decimal separator
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
            bShowFormattedNumber = false;

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

                // Select all the text in the entry field
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
            bShowFormattedNumber = true;

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

        /* Rounding numbers
           Round away from zero: MidpointRounding.AwayFromZero = 1-4 down ; 5-9 up
           Round half to even or banker's rounding: MidpointRounding.ToEven
           Round towards zero: 1-9 down

           Value      Default    ToEven     AwayFromZero    ToZero
            12.0       12         12         12              12
            12.1       12         12         12              12
            12.2       12         12         12              12
            12.3       12         12         12              12
            12.4       12         12         12              12
            12.5       12         12         13              12
            12.6       13         13         13              12
            12.7       13         13         13              12
            12.8       13         13         13              12
            12.9       13         13         13              12
            13.0       13         13         13              13
         
          Format specifier: "F" = 1234.56 or 1234,56 ; "N" = 1,234.56 or 1.234,56 */

        /// <summary>
        /// Rounding and formatting double number to # decimals returning number as value and as string 
        /// </summary>
        /// <param name="nNumber"></param>
        /// <param name="nNumDec"></param>
        /// <param name="cFormatSpecifier"></param>
        /// <returns></returns>
        public static string RoundToNumDecimals(ref double nNumber, int nNumDec, string cFormatSpecifier)
        {
            nNumber = cRoundNumber switch
            {
                "AwayFromZero" => Math.Round(nNumber, nNumDec, MidpointRounding.AwayFromZero),
                "ToEven" => Math.Round(nNumber, nNumDec, MidpointRounding.ToEven),
                _ => Math.Round(nNumber, nNumDec, MidpointRounding.ToZero),
            };
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
            nNumber = cRoundNumber switch
            {
                "AwayFromZero" => Math.Round(nNumber, nNumDec, MidpointRounding.AwayFromZero),
                "ToEven" => Math.Round(nNumber, nNumDec, MidpointRounding.ToEven),
                _ => Math.Round(nNumber, nNumDec, MidpointRounding.ToZero),
            };
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

            // Set the number text color
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
        ///// Test the rounding of numbers
        ///// </summary>
        //public static void TestRoundingNumbers()
        //{
        //    List<double> numbers = new List<double> { 12.0, 12.1, 12.2, 12.3, 12.4, 12.5, 12.6, 12.7, 12.8, 12.9, 13.0 };

        //    foreach (double number in numbers)
        //    {
        //        double nNumber = number;
        //        string cFormatSpecifier = "F";
        //        int nNumDec = 0;
        //        cRoundNumber = "AwayFromZero";
        //        //cRoundNumber = "ToEven";
        //        //cRoundNumber = "ToZero";
                
        //        string cRoundedNumber = RoundToNumDecimals(ref nNumber, nNumDec, cFormatSpecifier);
                
        //        Debug.WriteLine($"Original: {number} - Rounded: {cRoundedNumber}");
        //    }
        //}
    }
}
