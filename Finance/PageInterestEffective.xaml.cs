namespace Finance
{
    public sealed partial class PageInterestEffective : ContentPage
    {
        public PageInterestEffective()
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
            // Set the left margin of the title for windows
            lblTitlePage.Margin = new Thickness(40, 0, 0, 0);
#endif
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
            entInterestRate.Focus();
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
            lblInterestEffective.Text = "";
        }

        /// <summary>
        /// Go to the next field when the return key have been pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToNextField(object sender, EventArgs e)
        {
            if (sender == entInterestRate)
            {
                entPeriodsYear.Focus();
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
            entInterestRate.Text = Globals.ReplaceDecimalPointComma(entInterestRate.Text);
            bool bIsNumber = double.TryParse(entInterestRate.Text, out double nInterestRate);
            if (bIsNumber == false || nInterestRate < 0 || nInterestRate > 100)
            {
                entInterestRate.Text = "";
                entInterestRate.Focus();
                return;
            }

            bIsNumber = double.TryParse(entPeriodsYear.Text, out double nPeriodsYear);
            if (bIsNumber == false || nPeriodsYear < 1 || nPeriodsYear > 12)
            {
                entPeriodsYear.Text = "";
                entPeriodsYear.Focus();
                return;
            }

            // Close the keyboard
            entPeriodsYear.IsEnabled = false;
            entPeriodsYear.IsEnabled = true;

            // Convert string to int for number of decimal digits after decimal point
            int nPercDec = int.Parse(Globals.cPercDecimalDigits);

            // Set decimal places for the Entry controls and values passed by reference
            entInterestRate.Text = Globals.RoundToNumDecimals(ref nInterestRate, nPercDec, "F");

            // Calculating the effective interest
            double nInterestEffective;
            try
            {
                nInterestEffective = ((Math.Pow(1 + (nInterestRate / 100) / nPeriodsYear, nPeriodsYear) - 1) * 100);
            }
            catch (Exception ex)
            {
                DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
                return;
            }

            // Rounding result
            lblInterestEffective.Text = Globals.RoundToNumDecimals(ref nInterestEffective, nPercDec, "N");

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
            entInterestRate.Text = "";
            entPeriodsYear.Text = "12";
            lblInterestEffective.Text = "";

            entInterestRate.Focus();
        }
    }
}