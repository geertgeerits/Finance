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
            entVATPercentage.Text = Globals.ReplaceDecimalPointComma(entVATPercentage.Text);
            bool bIsNumber = decimal.TryParse(entVATPercentage.Text, out decimal nVATPercentage);
            if (bIsNumber == false || nVATPercentage < 0 || nVATPercentage > 100)
            {
                entVATPercentage.Text = "";
                _ = entVATPercentage.Focus();
                return;
            }

            entVATAmountExclusive.Text = Globals.ReplaceDecimalPointComma(entVATAmountExclusive.Text);
            bIsNumber = decimal.TryParse(entVATAmountExclusive.Text, out decimal nVATAmountExclusive);
            if (bIsNumber == false || nVATAmountExclusive < 0 || nVATAmountExclusive > 9_999_999_999)
            {
                entVATAmountExclusive.Text = "";
                _ = entVATAmountExclusive.Focus();
                return;
            }

            entVATAmount.Text = Globals.ReplaceDecimalPointComma(entVATAmount.Text);
            bIsNumber = decimal.TryParse(entVATAmount.Text, out decimal nVATAmount);
            if (bIsNumber == false || nVATAmount < 0 || nVATAmount > 9_999_999_999)
            {
                entVATAmount.Text = "";
                _ = entVATAmount.Focus();
                return;
            }

            entVATAmountIncluded.Text = Globals.ReplaceDecimalPointComma(entVATAmountIncluded.Text);
            bIsNumber = decimal.TryParse(entVATAmountIncluded.Text, out decimal nVATAmountIncluded);
            if (bIsNumber == false || nVATAmountIncluded < 0 || nVATAmountIncluded > 9_999_999_999)
            {
                entVATAmountIncluded.Text = "";
                _ = entVATAmountIncluded.Focus();
                return;
            }

            // Close the keyboard
            entVATAmountExclusive.IsEnabled = false;
            entVATAmountExclusive.IsEnabled = true;
            entVATAmountIncluded.IsEnabled = false;
            entVATAmountIncluded.IsEnabled = true;

            // Convert string to int for number of decimal digits after decimal point
            int nNumDec = int.Parse(Globals.cNumDecimalDigits);
            int nPercDec = int.Parse(Globals.cPercDecimalDigits);

            // Set decimal places for the Entry controls and values passed by reference
            entVATPercentage.Text = Globals.RoundToNumDecimals(ref nVATPercentage, nPercDec, "F");
            entVATAmountExclusive.Text = Globals.RoundToNumDecimals(ref nVATAmountExclusive, nNumDec, "F");
            entVATAmount.Text = Globals.RoundToNumDecimals(ref nVATAmount, nNumDec, "F");
            entVATAmountIncluded.Text = Globals.RoundToNumDecimals(ref nVATAmountIncluded, nNumDec, "F");

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
               3+4 Amount VAT + amount VAT included */

            try
            {
                // 1+2 VAT percentage + amount VAT exclusive - Calculate VAT amount and VAT amount included
                if (nVATPercentage > 0 && nVATAmountExclusive > 0 && nVATAmount == 0 && nVATAmountIncluded == 0)
                {
                    nVATAmount = nVATAmountExclusive * nVATPercentage / 100;
                    nVATAmountIncluded = nVATAmountExclusive + nVATAmount;

                    entVATAmount.Text = Globals.RoundToNumDecimals(ref nVATAmount, nNumDec, "F");
                    entVATAmountIncluded.Text = Globals.RoundToNumDecimals(ref nVATAmountIncluded, nNumDec, "F");
                }

                // 1+3 VAT percentage + amount VAT - Calculate VAT amaount exclusive and VAT amount included
                else if (nVATPercentage > 0 && nVATAmountExclusive == 0 && nVATAmount > 0 && nVATAmountIncluded == 0)
                {
                    nVATAmountExclusive = nVATAmount / nVATPercentage * 100;
                    nVATAmountIncluded = nVATAmountExclusive + nVATAmount;

                    entVATAmountExclusive.Text = Globals.RoundToNumDecimals(ref nVATAmountExclusive, nNumDec, "F");
                    entVATAmountIncluded.Text = Globals.RoundToNumDecimals(ref nVATAmountIncluded, nNumDec, "F");
                }

                // 1+4 VAT percentage + amount VAT included - Calculate VAT amount exclusive and VAT amount
                else if (nVATPercentage > 0 && nVATAmountExclusive == 0 && nVATAmount == 0 && nVATAmountIncluded > 0)
                {
                    nVATAmountExclusive = nVATAmountIncluded / (1 + nVATPercentage / 100);
                    nVATAmount = nVATAmountIncluded - nVATAmountExclusive;

                    entVATAmountExclusive.Text = Globals.RoundToNumDecimals(ref nVATAmountExclusive, nNumDec, "F");
                    entVATAmount.Text = Globals.RoundToNumDecimals(ref nVATAmount, nNumDec, "F");
                }

                // 2+3 Amount VAT exclusive + amount VAT - Calculate VAT percentage and VAT amount included
                else if (nVATPercentage == 0 && nVATAmount > 0 && nVATAmountExclusive > nVATAmount && nVATAmountIncluded == 0)
                {
                    nVATAmountIncluded = nVATAmountExclusive + nVATAmount;
                    nVATPercentage = nVATAmount / nVATAmountExclusive * 100;

                    entVATPercentage.Text = Globals.RoundToNumDecimals(ref nVATPercentage, nPercDec, "F");
                    entVATAmountIncluded.Text = Globals.RoundToNumDecimals(ref nVATAmountIncluded, nNumDec, "F");
                }

                // 2+4 Amount VAT exclusive + amount VAT included - Calculate VAT percentage and VAT amount
                else if (nVATPercentage == 0 && nVATAmountExclusive > 0 && nVATAmount == 0 && nVATAmountIncluded > nVATAmountExclusive)
                {
                    nVATAmount = nVATAmountIncluded - nVATAmountExclusive;
                    nVATPercentage = nVATAmount / nVATAmountExclusive * 100;

                    entVATPercentage.Text = Globals.RoundToNumDecimals(ref nVATPercentage, nPercDec, "F");
                    entVATAmount.Text = Globals.RoundToNumDecimals(ref nVATAmount, nPercDec, "F");
                }

                // 3+4 Amount VAT +amount VAT included - Calculate VAT percentage and VAT amount exclusive
                else if (nVATPercentage == 0 && nVATAmountExclusive == 0 && nVATAmount > 0 && nVATAmountIncluded > nVATAmount)
                {
                    nVATAmountExclusive = nVATAmountIncluded - nVATAmount;
                    nVATPercentage = nVATAmount / nVATAmountExclusive * 100;

                    entVATPercentage.Text = Globals.RoundToNumDecimals(ref nVATPercentage, nPercDec, "F");
                    entVATAmountExclusive.Text = Globals.RoundToNumDecimals(ref nVATAmountExclusive, nNumDec, "F");
                }

                // Invalid combination of values
                else
                {
                    ResetEntryFields(null, null);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
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
            entVATPercentage.Text = "0";
            entVATAmountExclusive.Text = "0";
            entVATAmount.Text = "0";
            entVATAmountIncluded.Text = "0";

            _ = entVATPercentage.Focus();
        }
    }
}