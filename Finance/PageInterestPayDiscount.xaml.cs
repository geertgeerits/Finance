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
                    entPaymentDiscount.Keyboard = Keyboard.Default;
                    entExpiryDaysWithDiscount.Keyboard = Keyboard.Default;
                    entExpiryDaysWithoutDiscount.Keyboard = Keyboard.Default;
                    break;
                case "Text":
                    entPaymentDiscount.Keyboard = Keyboard.Text;
                    entExpiryDaysWithDiscount.Keyboard = Keyboard.Text;
                    entExpiryDaysWithoutDiscount.Keyboard = Keyboard.Text;
                    break;
            }

            //// Set the Placeholder text for the numeric entry fields
            //// The ValidationTriggerActionDecimal MinValue and MaxValue has to be set but not the MaxDecimalPlaces
            ClassEntryMethods.SetNumberEntryProperties(entPaymentDiscount, ClassEntryMethods.cPercDecimalDigits);
        }

        /// <summary>
        /// Set focus to the first entry field 
        /// Add in the header of the xaml page: 'Loaded="OnPageLoaded"' 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageLoaded(object sender, EventArgs e)
        {
            _ = entPaymentDiscount.Focus();
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
                entPaymentDiscount.MaxLength = 11;
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
            if (sender == entPaymentDiscount)
            {
                _ = entExpiryDaysWithDiscount.Focus();
            }
            else if (sender == entExpiryDaysWithDiscount)
            {
                _ = entExpiryDaysWithoutDiscount.Focus();
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
            entPaymentDiscount.Unfocus();
            entExpiryDaysWithDiscount.Unfocus();
            entExpiryDaysWithoutDiscount.Unfocus();

            // Validate input values
            bool bIsNumber = decimal.TryParse(entPaymentDiscount.Text, out decimal nPaymentDiscount);
            if (!bIsNumber || nPaymentDiscount < 0 || nPaymentDiscount > 100)
            {
                entPaymentDiscount.Text = "";
                _ = entPaymentDiscount.Focus();
                return;
            }

            bIsNumber = int.TryParse(entExpiryDaysWithDiscount.Text, out int nExpiryDaysWithDiscount);
            if (!bIsNumber || nExpiryDaysWithDiscount < 0 || nExpiryDaysWithDiscount >= 1000)
            {
                entExpiryDaysWithDiscount.Text = "";
                _ = entExpiryDaysWithDiscount.Focus();
                return;
            }

            bIsNumber = int.TryParse(entExpiryDaysWithoutDiscount.Text, out int nExpiryDaysWithoutDiscount);
            if (!bIsNumber || nExpiryDaysWithoutDiscount < nExpiryDaysWithDiscount || nExpiryDaysWithoutDiscount >= 1000)
            {
                entExpiryDaysWithoutDiscount.Text = "";
                entExpiryDaysWithoutDiscount.Placeholder = $"{Convert.ToString(nExpiryDaysWithDiscount)} - 999";
                _ = entExpiryDaysWithoutDiscount.Focus();
                return;
            }

            // Hide the keyboard
            await ClassEntryMethods.HideSystemKeyboard(entExpiryDaysWithoutDiscount);

            // Convert string to int for number of decimal digits after decimal point
            int nPercDec = int.Parse(ClassEntryMethods.cPercDecimalDigits);

            decimal nInterestEffective;

            if (nExpiryDaysWithoutDiscount - nExpiryDaysWithDiscount > 0)
            {
                try
                {
                    nInterestEffective = nPaymentDiscount * 365 / (nExpiryDaysWithoutDiscount - nExpiryDaysWithDiscount);
                }
                catch (Exception ex)
                {
#if DEBUG                    
                    await DisplayAlertAsync(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
#endif
                    ResetEntryFields(null, null);
                    return;
                }
            }
            else
            {
                nInterestEffective = 0;
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
            entPaymentDiscount.Text = "";
            entExpiryDaysWithDiscount.Text = "7";
            entExpiryDaysWithoutDiscount.Text = "30";
            lblInterestEffective.Text = "";

            entExpiryDaysWithoutDiscount.Placeholder = "0 - 999";

            _ = entPaymentDiscount.Focus();
        }
    }
}