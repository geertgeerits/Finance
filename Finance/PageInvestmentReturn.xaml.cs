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
        }

        /// <summary>
        /// Set focus to the first entry field 
        /// Add in the header of the xaml page: 'Loaded="OnPageLoaded"' 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageLoaded(object sender, EventArgs e)
        {
            entAmountPurchase.Focus();
        }

        /// <summary>
        /// Select all the text in the entry field 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryFocused(object sender, EventArgs e)
        {
            var entry = (Entry)sender;

            entry.CursorPosition = entry.Text.Length;
            entry.CursorPosition = 0;
            entry.SelectionLength = entry.Text.Length;
        }

        /// <summary>
        /// Clear result fields if the text have changed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryTextChanged(object sender, EventArgs e)
        {
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
                entAmountCost.Focus();
            }
            else if (sender == entAmountCost)
            {
                entAmountRevenueYear.Focus();
            }
            else if (sender == entAmountRevenueYear)
            {
                entPercentageReturnYear.Focus();
            }
        }

        /// <summary>
        /// Set the value of a another field to '0' if the current field is unfocused 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryUnfocused(object sender, EventArgs e)
        {
            if (sender == entAmountPurchase && entAmountPurchase.Text != "0")
            {
                entPercentageReturnYear.Text = "0";
            }
            else if (sender == entAmountCost && entAmountCost.Text != "0")
            {
                entPercentageReturnYear.Text = "0";
            }
            else if (sender == entPercentageReturnYear && entPercentageReturnYear.Text != "0")
            {
                entAmountPurchase.Text = "0";
                entAmountCost.Text = "0";
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
            entAmountPurchase.Text = Globals.ReplaceDecimalPointComma(entAmountPurchase.Text);
            bool bIsNumber = decimal.TryParse(entAmountPurchase.Text, out decimal nAmountPurchase);
            if (bIsNumber == false || nAmountPurchase < 0 || nAmountPurchase > 9_999_999_999)
            {
                entAmountPurchase.Text = "";
                entAmountPurchase.Focus();
                return;
            }

            entAmountCost.Text = Globals.ReplaceDecimalPointComma(entAmountCost.Text);
            bIsNumber = decimal.TryParse(entAmountCost.Text, out decimal nAmountCost);
            if (bIsNumber == false || nAmountCost < 0 || nAmountCost > 9_999_999_999)
            {
                entAmountCost.Text = "";
                entAmountCost.Focus();
                return;
            }

            entAmountRevenueYear.Text = Globals.ReplaceDecimalPointComma(entAmountRevenueYear.Text);
            bIsNumber = decimal.TryParse(entAmountRevenueYear.Text, out decimal nAmountRevenueYear);
            if (bIsNumber == false || nAmountRevenueYear < 0 || nAmountRevenueYear > 9_999_999_999)
            {
                entAmountRevenueYear.Text = "";
                entAmountRevenueYear.Focus();
                return;
            }

            entPercentageReturnYear.Text = Globals.ReplaceDecimalPointComma(entPercentageReturnYear.Text);
            bIsNumber = decimal.TryParse(entPercentageReturnYear.Text, out decimal nPercentageReturnYear);
            if (bIsNumber == false || nPercentageReturnYear < 0 || nPercentageReturnYear > 9_999)
            {
                entPercentageReturnYear.Text = "";
                entPercentageReturnYear.Focus();
                return;
            }

            // Close the keyboard
            entAmountRevenueYear.IsEnabled = false;
            entAmountRevenueYear.IsEnabled = true;
            entPercentageReturnYear.IsEnabled = false;
            entPercentageReturnYear.IsEnabled = true;

            // Convert string to int for number of decimal digits after decimal point
            int nNumDec = int.Parse(Globals.cNumDecimalDigits);
            int nPercDec = int.Parse(Globals.cPercDecimalDigits);

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
        
            // Set decimal places for the Entry controls and values passed by reference
            entAmountPurchase.Text = Globals.RoundToNumDecimals(ref nAmountPurchase, nNumDec, "F");
            entAmountCost.Text = Globals.RoundToNumDecimals(ref nAmountCost, nNumDec, "F");
            entAmountRevenueYear.Text = Globals.RoundToNumDecimals(ref nAmountRevenueYear, nNumDec, "F");
            entPercentageReturnYear.Text = Globals.RoundToNumDecimals(ref nPercentageReturnYear, nPercDec, "F");

            // Calculate the results
            decimal nAmountTotal = nAmountPurchase + nAmountCost;

            if (nAmountTotal == 0)
            {
                lblAmountTotal.Text = Globals.RoundToNumDecimals(ref nAmountTotal, nNumDec, "N");
            }

            if (nAmountRevenueYear == 0)
            {
                decimal nNumberTemp = 0;
                entPercentageReturnYear.Text = Globals.RoundToNumDecimals(ref nNumberTemp, nPercDec, "F");
            }

            try
            {
                if (nAmountPurchase + nAmountCost > 0)
                {
                    nPercentageReturnYear = nAmountRevenueYear / nAmountTotal * 100;
                    entPercentageReturnYear.Text = Globals.RoundToNumDecimals(ref nPercentageReturnYear, nPercDec, "F");
                }
                else if (nPercentageReturnYear > 0)
                {
                    nAmountTotal = nAmountRevenueYear / nPercentageReturnYear * 100;
                }
                else
                {
                    return;
                }

                lblAmountTotal.Text = Globals.RoundToNumDecimals(ref nAmountTotal, nNumDec, "N");
            }
            catch (Exception ex)
            {
                DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
                return;
            }

            // Set focus
            btnReset.Focus();
        }

        /// <summary>
        /// Reset the entry fields 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetEntryFields(object sender, EventArgs e)
        {
            entAmountPurchase.Text = "0";
            entAmountCost.Text = "0";
            lblAmountTotal.Text = "";
            entAmountRevenueYear.Text = "0";
            entPercentageReturnYear.Text = "0";

            entAmountPurchase.Focus();
        }
    }
}