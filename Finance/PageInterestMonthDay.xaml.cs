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
#if WINDOWS
            //// Set the margin of the title for windows
            lblTitlePage.Margin = new Thickness(80, 15, 0, 0);
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

            //// Set the Placeholder for the numeric entry field
            ClassEntryMethods.SetNumberEntryProperties(entInterestRate, "0", "0", "100", "0", ClassEntryMethods.cPercDecimalDigits);
        }

        /// <summary>
        /// Set focus to the first entry field 
        /// Add in the header of the xaml page: 'Loaded="OnPageLoaded"' 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageLoaded(object sender, EventArgs e)
        {
            _ = entPercDec.Focus();
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
                entInterestRate.MaxLength = 11;
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
                _ = entInterestRate.Focus();
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
            if (!bIsNumber || nPercDec < 0 || nPercDec > 8)
            {
                entPercDec.Text = "";
                _ = entPercDec.Focus();
                return;
            }

            bIsNumber = double.TryParse(entInterestRate.Text, out double nInterestRate);
            if (!bIsNumber || nInterestRate < 0 || nInterestRate > 100)
            {
                entInterestRate.Text = "";
                _ = entInterestRate.Focus();
                return;
            }

            // Hide the keyboard
            ClassEntryMethods.HideSystemKeyboard(entInterestRate);

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
#if DEBUG                    
                    DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
#endif
                    ResetEntryFields(null, null);
                    return;
                }
            }

            // Rounding result
            lblInterestMonth.Text = ClassEntryMethods.RoundToNumDecimals(ref nInterestMonth, nPercDec, "N");
            lblInterestDay365.Text = ClassEntryMethods.RoundToNumDecimals(ref nInterestDay365, nPercDec, "N");
            lblInterestDay366.Text = ClassEntryMethods.RoundToNumDecimals(ref nInterestDay366, nPercDec, "N");

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
            entPercDec.Text = "6";
            entInterestRate.Text = "";
            lblInterestMonth.Text = "";
            lblInterestDay365.Text = "";
            lblInterestDay366.Text = "";

            _ = entPercDec.Focus();
        }
    }
}