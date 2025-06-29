namespace Finance
{
    public sealed partial class PageInvestmentReturn : ContentPage
    {
    	public PageInvestmentReturn()
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
#if WINDOWS
            //// Set the margin of the title for windows
            lblTitlePage.Margin = new Thickness(80, 15, 0, 0);
#endif
            //// Set the type of keyboard
            if (Globals.cKeyboard == "Default")
            {
                entAmountPurchase.Keyboard = Keyboard.Default;
                entAmountCost.Keyboard = Keyboard.Default;
                entAmountRevenueYear.Keyboard = Keyboard.Default;
                entPercentageReturnYear.Keyboard = Keyboard.Default;
            }
            else if (Globals.cKeyboard == "Text")
            {
                entAmountPurchase.Keyboard = Keyboard.Text;
                entAmountCost.Keyboard = Keyboard.Text;
                entAmountRevenueYear.Keyboard = Keyboard.Text;
                entPercentageReturnYear.Keyboard = Keyboard.Text;
            }

            //// Set the Placeholder for the numeric entry field
            ClassEntryMethods.SetNumberEntryProperties(entAmountPurchase, "0", "0", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits);
            ClassEntryMethods.SetNumberEntryProperties(entAmountCost, "0", "0", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits);
            ClassEntryMethods.SetNumberEntryProperties(entAmountRevenueYear, "0", "0", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits);
            ClassEntryMethods.SetNumberEntryProperties(entPercentageReturnYear, "0", "0", "999", "9", ClassEntryMethods.cPercDecimalDigits);

            //// Reset the entry fields
            ResetEntryFields(null, null);
        }

        /// <summary>
        /// Set focus to the first entry field 
        /// Add in the header of the xaml page: 'Loaded="OnPageLoaded"' 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageLoaded(object sender, EventArgs e)
        {
            _ = entAmountPurchase.Focus();
        }

        /// <summary>
        /// Entry focused event: format the text value for a numeric entry without the number separator and select the entire text value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberEntryFocused(object sender, FocusEventArgs? e)
        {
            if (sender is Entry entry)
            {
                if (sender == entPercentageReturnYear)
                {
                    entPercentageReturnYear.MaxLength = 12;
                }
                else
                {
                    entry.MaxLength = 17;
                }

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

            lblAmountTotal.Text = "";
        }

        /// <summary>
        /// Go to the next field when the return key have been pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToNextField(object sender, EventArgs e)
        {
            if (sender == entAmountPurchase)
            {
                _ = entAmountCost.Focus();
            }
            else if (sender == entAmountCost)
            {
                _ = entAmountRevenueYear.Focus();
            }
            else if (sender == entAmountRevenueYear)
            {
                _ = entPercentageReturnYear.Focus();
            }
        }

        /// <summary>
        /// Calculate the result 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculateResult(object sender, EventArgs e)
        {
            // Replace null or empty values with 0
            if (string.IsNullOrEmpty(entAmountPurchase.Text))
            {
                entAmountPurchase.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
            }

            if (string.IsNullOrEmpty(entAmountCost.Text))
            {
                entAmountCost.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
            }

            if (string.IsNullOrEmpty(entAmountRevenueYear.Text))
            {
                entAmountRevenueYear.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
            }

            if (string.IsNullOrEmpty(entPercentageReturnYear.Text))
            {
                entPercentageReturnYear.Text = 0.ToString("F" + ClassEntryMethods.cPercDecimalDigits);
            }

            // Check if the values are numeric and in the correct range            
            bool bIsNumber = decimal.TryParse(entAmountPurchase.Text, out decimal nAmountPurchase);
            if (!bIsNumber || nAmountPurchase < 0 || nAmountPurchase >= 1_000_000_000_000)
            {
                entAmountPurchase.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
                _ = entAmountPurchase.Focus();
                return;
            }

            bIsNumber = decimal.TryParse(entAmountCost.Text, out decimal nAmountCost);
            if (!bIsNumber || nAmountCost < 0 || nAmountCost >= 1_000_000_000_000)
            {
                entAmountCost.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
                _ = entAmountCost.Focus();
                return;
            }

            bIsNumber = decimal.TryParse(entAmountRevenueYear.Text, out decimal nAmountRevenueYear);
            if (!bIsNumber || nAmountRevenueYear < 0 || nAmountRevenueYear >= 1_000_000_000_000)
            {
                entAmountRevenueYear.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
                _ = entAmountRevenueYear.Focus();
                return;
            }

            bIsNumber = decimal.TryParse(entPercentageReturnYear.Text, out decimal nPercentageReturnYear);
            if (!bIsNumber || nPercentageReturnYear < 0 || nPercentageReturnYear >= 1_000)
            {
                entPercentageReturnYear.Text = 0.ToString("F" + ClassEntryMethods.cPercDecimalDigits);
                _ = entPercentageReturnYear.Focus();
                return;
            }

            // Hide the keyboard
            ClassEntryMethods.HideSystemKeyboard(entPercentageReturnYear);

            // Show the formatted number in the entry field
            ClassEntryMethods.bShowFormattedNumber = true;

            // Convert string to int for number of decimal digits after decimal point
            int nNumDec = int.Parse(ClassEntryMethods.cNumDecimalDigits);
            int nPercDec = int.Parse(ClassEntryMethods.cPercDecimalDigits);

            // Calculate the results
            decimal nAmountTotal = nAmountPurchase + nAmountCost;

            try
            {
                // Calculate the return in percentage
                if (nAmountTotal > 0 && nAmountRevenueYear > 0 && nPercentageReturnYear == 0)
                {
                    nPercentageReturnYear = nAmountRevenueYear / nAmountTotal * 100;
                    entPercentageReturnYear.Text = ClassEntryMethods.RoundToNumDecimals(ref nPercentageReturnYear, nPercDec, "N");
                    lblAmountTotal.Text = ClassEntryMethods.RoundToNumDecimals(ref nAmountTotal, nNumDec, "N");
                }

                // Calculate the amount total of the investment
                else if (nAmountTotal == 0 && nAmountRevenueYear > 0 && nPercentageReturnYear > 0)
                {
                    nAmountTotal = nAmountRevenueYear / nPercentageReturnYear * 100;
                    lblAmountTotal.Text = ClassEntryMethods.RoundToNumDecimals(ref nAmountTotal, nNumDec, "N");
                }

                // Invalid values or combination of values
                else
                {
                    ResetEntryFields(null, null);
                }
            }

            catch (Exception ex)
            {
#if DEBUG                
                DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
#endif
                ResetEntryFields(null, null);
                return;
            }

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
            entAmountPurchase.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
            entAmountCost.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
            lblAmountTotal.Text = "";
            entAmountRevenueYear.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
            entPercentageReturnYear.Text = 0.ToString("F" + ClassEntryMethods.cPercDecimalDigits);

            _ = entAmountPurchase.Focus();
        }
    }
}