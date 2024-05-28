namespace Finance
{
    public sealed partial class PageInterestMonthDay : ContentPage
    {
    	public PageInterestMonthDay()
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
                entInterestRate.Keyboard = Keyboard.Default;
            }
            else if (Globals.cKeyboard == "Text")
            {
                entInterestRate.Keyboard = Keyboard.Text;
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
            entPercDec.Focus();
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
        /// Clear result fields 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryTextChanged(object sender, EventArgs e)
        {
            lblInterestMonth.Text = "";
            lblInterestDay365.Text = "";
            lblInterestDay366.Text = "";
        }

        /// <summary>
        /// Go to the next field when the return key have been pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToNextField(object sender, EventArgs e)
        {
            if (sender == entPercDec)
            {
                entInterestRate.Focus();
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
            bool bIsNumber = int.TryParse(entPercDec.Text, out int nPercDec);
            if (bIsNumber == false || nPercDec < 0 || nPercDec > 8)
            {
                entPercDec.Text = "";
                entPercDec.Focus();
                return;
            }

            entInterestRate.Text = Globals.ReplaceDecimalPointComma(entInterestRate.Text);
            bIsNumber = double.TryParse(entInterestRate.Text, out double nInterestRate);
            if (bIsNumber == false || nInterestRate < 0 || nInterestRate > 100)
            {
                entInterestRate.Text = "";
                entInterestRate.Focus();
                return;
            }

            // Close the keyboard
            entInterestRate.IsEnabled = false;
            entInterestRate.IsEnabled = true;

            // Set decimal places for the Entry controls and values passed by reference
            entInterestRate.Text = Globals.RoundToNumDecimals(ref nInterestRate, nPercDec, "F");

            double nInterestMonth = 0;
            double nInterestDay365 = 0;
            double nInterestDay366 = 0;

            if (nInterestRate > 0)
            {
                try
                {
                    nInterestMonth = (Math.Pow(1 + (nInterestRate / 100), (double)1 / 12) - 1) * 100;
                    nInterestDay365 = (Math.Pow(1 + (nInterestRate / 100), (double)1 / 365) - 1) * 100;
                    nInterestDay366 = (Math.Pow(1 + (nInterestRate / 100), (double)1 / 366) - 1) * 100;
                }
                catch (Exception ex)
                {
                    DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
                    return;
                }
            }

            // Rounding result
            lblInterestMonth.Text = Globals.RoundToNumDecimals(ref nInterestMonth, nPercDec, "N");
            lblInterestDay365.Text = Globals.RoundToNumDecimals(ref nInterestDay365, nPercDec, "N");
            lblInterestDay366.Text = Globals.RoundToNumDecimals(ref nInterestDay366, nPercDec, "N");

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
            entPercDec.Text = "6";
            entInterestRate.Text = "";
            lblInterestMonth.Text = "";
            lblInterestDay365.Text = "";
            lblInterestDay366.Text = "";

            entPercDec.Focus();
        }
    }
}