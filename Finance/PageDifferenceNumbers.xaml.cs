namespace Finance
{
    public sealed partial class PageDifferenceNumbers : ContentPage
    {
        //// The currently focused Entry field
        private Entry? _focusedEntry;

        public PageDifferenceNumbers()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                DisplayAlertAsync("InitializeComponent", ex.Message, "OK");
                return;
            }
#if WINDOWS
            //// Set the margin of the title for windows
            lblTitlePage.Margin = new Thickness(80, 15, 0, 0);
#endif
            //// Set the type of keyboard
            switch (Globals.cKeyboard)
            {
                case "Default":
                    entValue1.Keyboard = Keyboard.Default;
                    entValue2.Keyboard = Keyboard.Default;
                    break;
                case "Text":
                    entValue1.Keyboard = Keyboard.Text;
                    entValue2.Keyboard = Keyboard.Text;
                    break;
            }

            //// Character for the minus sign button
            btnMinusSign.Text = ClassEntryMethods.cNumNegativeSign;

            //// Set the Placeholder for the numeric entry field
            ClassEntryMethods.SetNumberEntryProperties(entValue1, "-999999999999", "9", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits);
            ClassEntryMethods.SetNumberEntryProperties(entValue2, "-999999999999", "9", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits);
        }

        /// <summary>
        /// Set focus to the first entry field 
        /// Add in the header of the xaml page: 'Loaded="OnPageLoaded"' 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageLoaded(object sender, EventArgs e)
        {
            _ = entValue1.Focus();
        }

        /// <summary>
        /// Entry focused event: format the text value for a numeric entry without the number separator and select the entire text value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberEntryFocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                _focusedEntry = entry;
                entry.MaxLength = 18;
                ClassEntryMethods.FormatDecimalNumberEntryFocused(entry);
            }
        }

        /// <summary>
        /// Entry unfocused event: format the text value for a numeric entry field with the number separator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberEntryUnfocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                entry.MaxLength = -1;
                ClassEntryMethods.FormatDecimalNumberEntryUnfocused(entry);
            }
        }

        /// <summary>
        /// Check if the value is numeric and clear result fields if the text have changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!ClassEntryMethods.IsDecimalNumber((Entry)sender, e.NewTextValue))
            {
                ((Entry)sender).Text = e.OldTextValue;
            }

            lblValueDifference.Text = "";
            lblValuePercDifference.Text = "";
            lblValuePercDiffValue1.Text = "";
            lblValuePercDiffValue2.Text = "";
        }

        /// <summary>
        /// Go to the next field when the return key have been pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToNextField(object sender, EventArgs e)
        {
            if (sender == entValue1)
            {
                _ = entValue2.Focus();
            }
        }

        /// <summary>
        /// Calculate the result 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculateResult(object sender, EventArgs e)
        {
            // Validate input values
            bool bIsNumber = decimal.TryParse(entValue1.Text, out decimal nValue1);
            if (!bIsNumber || nValue1 <= -1_000_000_000_000 || nValue1 >= 1_000_000_000_000)
            {
                entValue1.Text = "";
                _ = entValue1.Focus();
                return;
            }

            bIsNumber = decimal.TryParse(entValue2.Text, out decimal nValue2);
            if (!bIsNumber || nValue2 <= -1_000_000_000_000 || nValue2 >= 1_000_000_000_000)
            {
                entValue2.Text = "";
                _ = entValue2.Focus();
                return;
            }

            // Hide the keyboard
            ClassEntryMethods.HideSystemKeyboard(entValue2);

            // Convert string to int for number of decimal digits after decimal point
            int nNumDec = int.Parse(ClassEntryMethods.cNumDecimalDigits);
            int nPercDec = int.Parse(ClassEntryMethods.cPercDecimalDigits);

            // Calculate the difference
            decimal nValuePercDifference;
            decimal nValueTemp;

            decimal nValueDifference = nValue2 - nValue1;
            lblValueDifference.Text = ClassEntryMethods.RoundToNumDecimals(ref nValueDifference, nNumDec, "N");
            ClassEntryMethods.SetLabelTextColorForNumber(lblValueDifference);

            if (nValue1 == 0 && nValue2 == 0)
            {
                lblValuePercDifference.Text = "";
                lblValuePercDiffValue1.Text = "";
                lblValuePercDiffValue2.Text = "";

                _ = btnReset.Focus();
                return;
            }

            if (nValue1 == 0 && nValue2 != 0)
            {
                lblValuePercDifference.Text = "";
            
                nValueTemp = 0;
                lblValuePercDiffValue1.Text = ClassEntryMethods.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
                ClassEntryMethods.SetLabelTextColorForNumber(lblValuePercDiffValue1);
            
                lblValuePercDiffValue2.Text = "";

                _ = btnReset.Focus();
                return;
            }

            if (nValue1 != 0 && nValue2 == 0)
            {
                nValueTemp = -100;
                lblValuePercDifference.Text = ClassEntryMethods.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
                ClassEntryMethods.SetLabelTextColorForNumber(lblValuePercDifference);
            
                lblValuePercDiffValue1.Text = "";
            
                nValueTemp = 0;
                lblValuePercDiffValue2.Text = ClassEntryMethods.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
                ClassEntryMethods.SetLabelTextColorForNumber(lblValuePercDiffValue2);

                _ = btnReset.Focus();
                return;
            }

            if (nValue1 == nValue2)
            {
                nValueTemp = 0;
                lblValuePercDifference.Text = ClassEntryMethods.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
                ClassEntryMethods.SetLabelTextColorForNumber(lblValuePercDifference);
            
                nValueTemp = 100;
                lblValuePercDiffValue1.Text = ClassEntryMethods.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
                ClassEntryMethods.SetLabelTextColorForNumber(lblValuePercDiffValue1);
            
                lblValuePercDiffValue2.Text = ClassEntryMethods.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
                ClassEntryMethods.SetLabelTextColorForNumber(lblValuePercDiffValue2);

                _ = btnReset.Focus();
                return;
            }

            nValueTemp = nValue1 / nValue2 * 100;
            lblValuePercDiffValue1.Text = ClassEntryMethods.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
            ClassEntryMethods.SetLabelTextColorForNumber(lblValuePercDiffValue1);

            nValueTemp = nValue2 / nValue1 * 100;
            lblValuePercDiffValue2.Text = ClassEntryMethods.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
            ClassEntryMethods.SetLabelTextColorForNumber(lblValuePercDiffValue2);

            try
            {
                nValuePercDifference = nValueDifference / nValue1 * 100;
            }
            catch (Exception ex)
            {
#if DEBUG
                DisplayAlertAsync(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
#endif
                ResetEntryFields(null, null);
                return;
            }

            lblValuePercDifference.Text = ClassEntryMethods.RoundToNumDecimals(ref nValuePercDifference, nPercDec, "N");
            ClassEntryMethods.SetLabelTextColorForNumber(lblValuePercDifference);

            // Set focus
            _ = btnReset.Focus();
        }

        /// <summary>
        /// Reset the entry fields 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetEntryFields(object? sender, EventArgs? e)
        {
            entValue1.Text = "";
            entValue2.Text = "";

            _ = entValue1.Focus();
        }

        /// <summary>
        /// Handles the click event for the minus button, toggling the negative sign on the currently focused entry field
        /// </summary>
        /// <remarks>If no entry field is currently focused, this method does nothing. When invoked, it
        /// adds or removes the negative sign at the beginning of the entry's text and updates the cursor position
        /// accordingly.</remarks>
        /// <param name="sender">The source of the event, typically the minus button control.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void BtnMinusSign_Clicked(object sender, EventArgs e)
        {
            if (_focusedEntry == null)
            {
                return;
            }

            if (!_focusedEntry.Text.Contains(ClassEntryMethods.cNumNegativeSign))
            {
                _focusedEntry.Text = ClassEntryMethods.cNumNegativeSign + _focusedEntry.Text;
                _focusedEntry.CursorPosition = 1;
            }
            else
            {
                _focusedEntry.Text = _focusedEntry.Text.Replace(ClassEntryMethods.cNumNegativeSign, string.Empty);
                _focusedEntry.CursorPosition = 0;
            }

            ClassEntryMethods.SetEntryNumberColor(_focusedEntry);

            _focusedEntry.Focus();
            Task.Delay(100).Wait();
            _focusedEntry.SelectionLength = 0;
        }
    }
}
