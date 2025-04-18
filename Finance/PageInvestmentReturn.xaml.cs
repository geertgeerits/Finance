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
            // Set the margin of the title for windows
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

            //// Set the Placeholder and MaxLength for the numeric entry field
            ClassEntryMethods.SetEntryProperties(entAmountPurchase, "0", "0", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits, ClassEntryMethods.cNumDecimalDigits);
            ClassEntryMethods.SetEntryProperties(entAmountCost, "0", "0", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits, ClassEntryMethods.cNumDecimalDigits);
            ClassEntryMethods.SetEntryProperties(entAmountRevenueYear, "0", "0", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits, ClassEntryMethods.cNumDecimalDigits);
            ClassEntryMethods.SetEntryProperties(entPercentageReturnYear, "0", "0", "999", "9", ClassEntryMethods.cPercDecimalDigits, ClassEntryMethods.cPercDecimalDigits);

            //// Format the text value for a numeric entry without the number separator and select the entire text value
            //   This is necessary if the default value of the field is 0
            //EntryFocused(entPercentageReturnYear, null);

            // Reset the entry fields
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
        private void EntryFocused(object sender, FocusEventArgs? e)
        {
            if (sender is Entry entry)
            {
                ClassEntryMethods.FormatTextEntryFocused(entry);
            }
        }

        /// <summary>
        /// Entry unfocused event: format the text value for a numeric entry field with the number separator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryUnfocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                ClassEntryMethods.FormatTextEntryUnfocused(entry);
            }
        }

        /// <summary>
        /// Check if the value is numeric and clear result fields if the text have changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!ClassEntryMethods.IsNumeric((Entry)sender, e.NewTextValue))
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
            // Validate input values
            bool bIsNumber = decimal.TryParse(entAmountPurchase.Text, out decimal nAmountPurchase);
            if (!bIsNumber || nAmountPurchase < 0 || nAmountPurchase >= 1_000_000_000_000)
            {
                entAmountPurchase.Text = "";
                _ = entAmountPurchase.Focus();
                return;
            }

            bIsNumber = decimal.TryParse(entAmountCost.Text, out decimal nAmountCost);
            if (!bIsNumber || nAmountCost < 0 || nAmountCost >= 1_000_000_000_000)
            {
                entAmountCost.Text = "";
                _ = entAmountCost.Focus();
                return;
            }

            bIsNumber = decimal.TryParse(entAmountRevenueYear.Text, out decimal nAmountRevenueYear);
            if (!bIsNumber || nAmountRevenueYear < 0 || nAmountRevenueYear >= 1_000_000_000_000)
            {
                entAmountRevenueYear.Text = "";
                _ = entAmountRevenueYear.Focus();
                return;
            }

            bIsNumber = decimal.TryParse(entPercentageReturnYear.Text, out decimal nPercentageReturnYear);
            if (!bIsNumber || nPercentageReturnYear < 0 || nPercentageReturnYear >= 1_000)
            {
                entPercentageReturnYear.Text = "";
                _ = entPercentageReturnYear.Focus();
                return;
            }

            // Hide the keyboard
            ClassEntryMethods.CloseKeyboard(entPercentageReturnYear);

            // Convert string to int for number of decimal digits after decimal point
            int nNumDec = int.Parse(ClassEntryMethods.cNumDecimalDigits);
            int nPercDec = int.Parse(ClassEntryMethods.cPercDecimalDigits);

            // Check what needs to be calculated first
            if (nPercentageReturnYear > 0)
            {
                nAmountPurchase = 0;
                nAmountCost = 0;
            }

            if (nAmountPurchase + nAmountCost > 0)
            {
                nPercentageReturnYear = 0;
            }
        
            // Calculate the results
            decimal nAmountTotal = nAmountPurchase + nAmountCost;

            if (nAmountTotal == 0)
            {
                lblAmountTotal.Text = ClassEntryMethods.RoundToNumDecimals(ref nAmountTotal, nNumDec, "N");
            }

            if (nAmountRevenueYear == 0)
            {
                decimal nNumberTemp = 0;
                entPercentageReturnYear.Text = ClassEntryMethods.RoundToNumDecimals(ref nNumberTemp, nPercDec, "N");
            }

            try
            {
                if (nAmountPurchase + nAmountCost > 0)
                {
                    nPercentageReturnYear = nAmountRevenueYear / nAmountTotal * 100;
                    entPercentageReturnYear.Text = ClassEntryMethods.RoundToNumDecimals(ref nPercentageReturnYear, nPercDec, "N");
                }
                else if (nPercentageReturnYear > 0)
                {
                    nAmountTotal = nAmountRevenueYear / nPercentageReturnYear * 100;
                }
                else
                {
                    return;
                }

                lblAmountTotal.Text = ClassEntryMethods.RoundToNumDecimals(ref nAmountTotal, nNumDec, "N");
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