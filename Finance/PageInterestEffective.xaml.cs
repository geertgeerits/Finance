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
                    entInterestRate.Keyboard = Keyboard.Default;
                    entPeriodsYear.Keyboard = Keyboard.Default;
                    break;
                case "Text":
                    entInterestRate.Keyboard = Keyboard.Text;
                    entPeriodsYear.Keyboard = Keyboard.Text;
                    break;
            }

            //// Set the Placeholder text for the numeric entry fields
            //// The ValidationTriggerActionDecimal MinValue and MaxValue has to be set but not the MaxDecimalPlaces
            ClassEntryMethods.SetNumberEntryProperties(entInterestRate, ClassEntryMethods.cPercDecimalDigits);
        }

        /// <summary>
        /// Set focus to the first entry field 
        /// Add in the header of the xaml page: 'Loaded="OnPageLoaded"' 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageLoaded(object sender, EventArgs e)
        {
            _ = entInterestRate.Focus();
        }

        /// <summary>
        /// Entry focused event: format the text value for a numeric entry without the number separator and select the entire text value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NumberEntryFocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                entInterestRate.MaxLength = 11;
                await ClassEntryMethods.FormatDecimalNumberEntryFocused(entry);
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
                _ = entPeriodsYear.Focus();
            }
        }

        /// <summary>
        /// Calculate the result 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CalculateResult(object sender, EventArgs e)
        {
            // Unfocus the entry controls when the Calculate button has been pressed
            entInterestRate.Unfocus();
            entPeriodsYear.Unfocus();

            // Validate input values
            bool bIsNumber = double.TryParse(entInterestRate.Text, out double nInterestRate);
            if (!bIsNumber || nInterestRate < 0 || nInterestRate > 100)
            {
                entInterestRate.Text = "";
                _ = entInterestRate.Focus();
                return;
            }

            bIsNumber = double.TryParse(entPeriodsYear.Text, out double nPeriodsYear);
            if (!bIsNumber || nPeriodsYear < 1 || nPeriodsYear > 12)
            {
                entPeriodsYear.Text = "";
                _ = entPeriodsYear.Focus();
                return;
            }

            // Hide the keyboard
            await ClassEntryMethods.HideSystemKeyboard(entPeriodsYear);

            // Convert string to int for number of decimal digits after decimal point
            int nPercDec = int.Parse(ClassEntryMethods.cPercDecimalDigits);

            // Calculating the effective interest
            double nInterestEffective;
            try
            {
                nInterestEffective = ((Math.Pow(1 + (nInterestRate / 100) / nPeriodsYear, nPeriodsYear) - 1) * 100);
            }
            catch (Exception ex)
            {
#if DEBUG                
                await DisplayAlertAsync(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
#endif
                ResetEntryFields(null, null);
                return;
            }

            // Rounding result
            lblInterestEffective.Text = ClassEntryMethods.RoundToNumDecimals(ref nInterestEffective, nPercDec, "N");

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
            entInterestRate.Text = "";
            entPeriodsYear.Text = "12";
            lblInterestEffective.Text = "";

            _ = entInterestRate.Focus();
        }
    }
}