namespace Finance
{
    public sealed partial class PageVATCalculation : ContentPage
    {
    	public PageVATCalculation()
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
                entVATPercentage.Keyboard = Keyboard.Default;
                entVATAmountExclusive.Keyboard = Keyboard.Default;
                entVATAmount.Keyboard = Keyboard.Default;
                entVATAmountIncluded.Keyboard = Keyboard.Default;
            }
            else if (Globals.cKeyboard == "Text")
            {
                entVATPercentage.Keyboard = Keyboard.Text;
                entVATAmountExclusive.Keyboard = Keyboard.Text;
                entVATAmount.Keyboard = Keyboard.Text;
                entVATAmountIncluded.Keyboard = Keyboard.Text;
            }

            //// Set the Placeholder and MaxLength for the numeric entry field
            ClassEntryMethods.SetNumberEntryProperties(entVATPercentage, "0", "0", "999", "9", ClassEntryMethods.cPercDecimalDigits, ClassEntryMethods.cPercDecimalDigits, false);
            ClassEntryMethods.SetNumberEntryProperties(entVATAmountExclusive, "0", "0", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits, ClassEntryMethods.cNumDecimalDigits, false);
            ClassEntryMethods.SetNumberEntryProperties(entVATAmount, "0", "0", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits, ClassEntryMethods.cNumDecimalDigits, true);
            ClassEntryMethods.SetNumberEntryProperties(entVATAmountIncluded, "0", "0", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits, ClassEntryMethods.cNumDecimalDigits, true);

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
            _ = entVATPercentage.Focus();
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
                ClassEntryMethods.FormatNumberEntryFocused(entry);
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
                ClassEntryMethods.FormatNumberEntryUnfocused(entry);
            }
        }

        /// <summary>
        /// Check if the value is numeric and clear result fields if the text have changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!ClassEntryMethods.IsNumeric((Entry)sender, e.NewTextValue))
            {
                ((Entry)sender).Text = e.OldTextValue;
            }
        }

        /// <summary>
        /// Go to the next field when the return key have been pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToNextField(object sender, EventArgs e)
        {
            if (sender == entVATPercentage)
            {
                _ = entVATAmountExclusive.Focus();
            }
            else if (sender == entVATAmountExclusive)
            {
                _ = entVATAmount.Focus();
            }
            else if (sender == entVATAmount)
            {
                _ = entVATAmountIncluded.Focus();
            }
        }

        /// <summary>
        /// Calculate the result 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculateResult(object sender, EventArgs e)
        {
            bool bIsNumber = decimal.TryParse(entVATPercentage.Text, out decimal nVATPercentage);
            if (!bIsNumber || nVATPercentage < 0 || nVATPercentage >= 1_000)
            {
                entVATPercentage.Text = 0.ToString("F" + ClassEntryMethods.cPercDecimalDigits);
                _ = entVATPercentage.Focus();
                return;
            }

            bIsNumber = decimal.TryParse(entVATAmountExclusive.Text, out decimal nVATAmountExclusive);
            if (!bIsNumber || nVATAmountExclusive < 0 || nVATAmountExclusive >= 1_000_000_000_000)
            {
                entVATAmountExclusive.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
                _ = entVATAmountExclusive.Focus();
                return;
            }

            bIsNumber = decimal.TryParse(entVATAmount.Text, out decimal nVATAmount);
            if (!bIsNumber || nVATAmount < 0 || nVATAmount >= 1_000_000_000_000)
            {
                entVATAmount.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
                _ = entVATAmount.Focus();
                return;
            }

            bIsNumber = decimal.TryParse(entVATAmountIncluded.Text, out decimal nVATAmountIncluded);
            if (!bIsNumber || nVATAmountIncluded < 0 || nVATAmountIncluded >= 1_000_000_000_000)
            {
                entVATAmountIncluded.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
                _ = entVATAmountIncluded.Focus();
                return;
            }

            // Hide the keyboard
            ClassEntryMethods.HideKeyboard(entVATAmountIncluded);

            // Show the formatted number in the entry field
            ClassEntryMethods.bShowFormattedNumber = true;

            // Convert string to int for number of decimal digits after decimal point
            int nNumDec = int.Parse(ClassEntryMethods.cNumDecimalDigits);
            int nPercDec = int.Parse(ClassEntryMethods.cPercDecimalDigits);

            // Calculate the VAT fields
            /* Possible combinations
               1+2.         1+3..           1+4...
               (2+1.)       2+3....         2+4.....
               (3+1..)      (3+2....)       3+4......
               (4+1...)     (4+2.....)      (4+3......)

               1+2 VAT percentage + amount VAT exclusive
               1+3 VAT percentage + amount VAT
               1+4 VAT percentage + amount VAT included
               2+3 Amount VAT exclusive + amount VAT
               2+4 Amount VAT exclusive + amount VAT included
               3+4 Amount VAT + amount VAT included
            */
            try
            {
                // 1+2 VAT percentage + amount VAT exclusive - Calculate VAT amount and VAT amount included
                if (nVATPercentage > 0 && nVATAmountExclusive > 0 && nVATAmount == 0 && nVATAmountIncluded == 0)
                {
                    nVATAmount = nVATAmountExclusive * nVATPercentage / 100;
                    nVATAmountIncluded = nVATAmountExclusive + nVATAmount;

                    entVATAmount.Text = ClassEntryMethods.RoundToNumDecimals(ref nVATAmount, nNumDec, "N");
                    entVATAmountIncluded.Text = ClassEntryMethods.RoundToNumDecimals(ref nVATAmountIncluded, nNumDec, "N");
                }

                // 1+3 VAT percentage + amount VAT - Calculate VAT amaount exclusive and VAT amount included
                else if (nVATPercentage > 0 && nVATAmountExclusive == 0 && nVATAmount > 0 && nVATAmountIncluded == 0)
                {
                    nVATAmountExclusive = nVATAmount / nVATPercentage * 100;
                    nVATAmountIncluded = nVATAmountExclusive + nVATAmount;

                    entVATAmountExclusive.Text = ClassEntryMethods.RoundToNumDecimals(ref nVATAmountExclusive, nNumDec, "N");
                    entVATAmountIncluded.Text = ClassEntryMethods.RoundToNumDecimals(ref nVATAmountIncluded, nNumDec, "N");
                }

                // 1+4 VAT percentage + amount VAT included - Calculate VAT amount exclusive and VAT amount
                else if (nVATPercentage > 0 && nVATAmountExclusive == 0 && nVATAmount == 0 && nVATAmountIncluded > 0)
                {
                    nVATAmountExclusive = nVATAmountIncluded / (1 + nVATPercentage / 100);
                    nVATAmount = nVATAmountIncluded - nVATAmountExclusive;

                    entVATAmountExclusive.Text = ClassEntryMethods.RoundToNumDecimals(ref nVATAmountExclusive, nNumDec, "N");
                    entVATAmount.Text = ClassEntryMethods.RoundToNumDecimals(ref nVATAmount, nNumDec, "N");
                }

                // 2+3 Amount VAT exclusive + amount VAT - Calculate VAT percentage and VAT amount included
                else if (nVATPercentage == 0 && nVATAmountExclusive > 0 && nVATAmount > 0 && nVATAmountIncluded == 0)
                {
                    nVATAmountIncluded = nVATAmountExclusive + nVATAmount;
                    nVATPercentage = nVATAmount / nVATAmountExclusive * 100;

                    entVATPercentage.Text = ClassEntryMethods.RoundToNumDecimals(ref nVATPercentage, nPercDec, "N");
                    entVATAmountIncluded.Text = ClassEntryMethods.RoundToNumDecimals(ref nVATAmountIncluded, nNumDec, "N");
                }

                // 2+4 Amount VAT exclusive + amount VAT included - Calculate VAT percentage and VAT amount
                else if (nVATPercentage == 0 && nVATAmountExclusive > 0 && nVATAmount == 0 && nVATAmountIncluded >= nVATAmountExclusive)
                {
                    nVATAmount = nVATAmountIncluded - nVATAmountExclusive;
                    nVATPercentage = nVATAmount / nVATAmountExclusive * 100;

                    entVATPercentage.Text = ClassEntryMethods.RoundToNumDecimals(ref nVATPercentage, nPercDec, "N");
                    entVATAmount.Text = ClassEntryMethods.RoundToNumDecimals(ref nVATAmount, nNumDec, "N");
                }

                // 3+4 Amount VAT +amount VAT included - Calculate VAT percentage and VAT amount exclusive
                else if (nVATPercentage == 0 && nVATAmountExclusive == 0 && nVATAmount > 0 && nVATAmountIncluded > nVATAmount)
                {
                    nVATAmountExclusive = nVATAmountIncluded - nVATAmount;
                    nVATPercentage = nVATAmount / nVATAmountExclusive * 100;

                    entVATPercentage.Text = ClassEntryMethods.RoundToNumDecimals(ref nVATPercentage, nPercDec, "N");
                    entVATAmountExclusive.Text = ClassEntryMethods.RoundToNumDecimals(ref nVATAmountExclusive, nNumDec, "N");
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
            entVATPercentage.Text = 0.ToString("F" + ClassEntryMethods.cPercDecimalDigits);
            entVATAmountExclusive.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);
            entVATAmount.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits );
            entVATAmountIncluded.Text = 0.ToString("F" + ClassEntryMethods.cNumDecimalDigits);

            _ = entVATPercentage.Focus();
        }
    }
}