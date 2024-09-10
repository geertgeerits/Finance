namespace Finance
{
    public sealed partial class PageInterestPayDiscount : ContentPage
    {
    	public PageInterestPayDiscount()
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
                entPaymentDiscount.Keyboard = Keyboard.Default;
            }
            else if (Globals.cKeyboard == "Text")
            {
                entPaymentDiscount.Keyboard = Keyboard.Text;
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
            entPaymentDiscount.Focus();
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
            lblInterestEffective.Text = "";
        }

        /// <summary>
        /// Go to the next field when the return key have been pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToNextField(object sender, EventArgs e)
        {
            if (sender == entPaymentDiscount)
            {
                entExpiryDaysWithDiscount.Focus();
            }
            else if (sender == entExpiryDaysWithDiscount)
            {
                entExpiryDaysWithoutDiscount.Focus();
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
            entPaymentDiscount.Text = Globals.ReplaceDecimalPointComma(entPaymentDiscount.Text);
            bool bIsNumber = decimal.TryParse(entPaymentDiscount.Text, out decimal nPaymentDiscount);
            if (bIsNumber == false || nPaymentDiscount < 0 || nPaymentDiscount > 100)
            {
                entPaymentDiscount.Text = "";
                entPaymentDiscount.Focus();
                return;
            }

            bIsNumber = int.TryParse(entExpiryDaysWithDiscount.Text, out int nExpiryDaysWithDiscount);
            if (bIsNumber == false || nExpiryDaysWithDiscount < 0 || nExpiryDaysWithDiscount > 999)
            {
                entExpiryDaysWithDiscount.Text = "";
                entExpiryDaysWithDiscount.Focus();
                return;
            }

            bIsNumber = int.TryParse(entExpiryDaysWithoutDiscount.Text, out int nExpiryDaysWithoutDiscount);
            if (bIsNumber == false || nExpiryDaysWithoutDiscount < nExpiryDaysWithDiscount || nExpiryDaysWithoutDiscount > 999)
            {
                entExpiryDaysWithoutDiscount.Text = "";
                entExpiryDaysWithoutDiscount.Placeholder = $"{Convert.ToString(nExpiryDaysWithDiscount)} - 999";
                entExpiryDaysWithoutDiscount.Focus();
                return;
            }

            // Close the keyboard
            entExpiryDaysWithoutDiscount.IsEnabled = false;
            entExpiryDaysWithoutDiscount.IsEnabled = true;

            // Convert string to int for number of decimal digits after decimal point
            int nPercDec = int.Parse(Globals.cPercDecimalDigits);

            // Set decimal places for the Entry controls and values passed by reference
            entPaymentDiscount.Text = Globals.RoundToNumDecimals(ref nPaymentDiscount, nPercDec, "F");

            decimal nInterestEffective;

            if (nExpiryDaysWithoutDiscount - nExpiryDaysWithDiscount > 0)
            {
                try
                {
                    nInterestEffective = nPaymentDiscount * 365 / (nExpiryDaysWithoutDiscount - nExpiryDaysWithDiscount);
                }
                catch (Exception ex)
                {
                    DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
                    return;
                }
            }
            else
            {
                nInterestEffective = 0;
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
            entPaymentDiscount.Text = "";
            entExpiryDaysWithDiscount.Text = "7";
            entExpiryDaysWithoutDiscount.Text = "30";
            lblInterestEffective.Text = "";

            entExpiryDaysWithoutDiscount.Placeholder = "0 - 999";

            entPaymentDiscount.Focus();
        }
    }
}