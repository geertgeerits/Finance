namespace Finance
{
    public sealed partial class PageAmountGrossOfNet : ContentPage
    {
        public PageAmountGrossOfNet()
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
                entPercentage.Keyboard = Keyboard.Default;
                entAmountNet.Keyboard = Keyboard.Default;
            }
            else if (Globals.cKeyboard == "Text")
            {
                entPercentage.Keyboard = Keyboard.Text;
                entAmountNet.Keyboard = Keyboard.Text;
            }

            //// Set the Placeholder and MaxLength for the numeric entry field
            Globals.SetEntryProperties(entPercentage, "0", "0", "99", "9", Globals.cPercDecimalDigits, Globals.cPercDecimalDigits);
        }

        /// <summary>
        /// Set focus to the first entry field
        /// Add in the header of the xaml page: 'Loaded="OnPageLoaded"' 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageLoaded(object sender, EventArgs e)
        {
            _ = entPercentage.Focus();

            //Dispatcher.Dispatch(() =>
            //{
            //    _ = entPercentage.Focus();
            //});
        }

        /// <summary>
        /// Entry focused event: format the text value for a numeric entry without the number separator and select the entire text value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryFocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                Globals.FormatTextEntryFocused(entry);
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
                Globals.FormatTextEntryUnfocused(entry);
            }
        }

        /// <summary>
        /// Clear result fields if the text have changed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryTextChanged(object sender, TextChangedEventArgs e)
        {
            lblAmountDifference.Text = "";
            lblAmountGross.Text = "";
        }

        /// <summary>
        /// Go to the next field when the return key have been pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToNextField(object sender, EventArgs e)
        {
            if (sender == entPercentage)
            {
                _ = entAmountNet.Focus();
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
            bool bIsNumber = decimal.TryParse(entPercentage.Text, out decimal nPercentage);
            if (!bIsNumber || nPercentage < 0 || nPercentage >= 100)
            {
                entPercentage.Text = "";
                _ = entPercentage.Focus();
                return;
            }

            bIsNumber = decimal.TryParse(entAmountNet.Text, out decimal nAmountNet);
            if (!bIsNumber || nAmountNet < 0 || nAmountNet >= 1_000_000_000_000)
            {
                entAmountNet.Text = "";
                _ = entAmountNet.Focus();
                return;
            }

            // Close the keyboard
            entAmountNet.IsEnabled = false;
            entAmountNet.IsEnabled = true;

            // Convert string to int for number of decimal digits after decimal point
            int nNumDec = int.Parse(Globals.cNumDecimalDigits);
            int nPercDec = int.Parse(Globals.cPercDecimalDigits);

            // Calculate the net amount
            if (nPercentage == 0)
            {
                decimal nAmountGross = nAmountNet;
                lblAmountGross.Text = Globals.RoundToNumDecimals(ref nAmountGross, nNumDec, "N");
                decimal nAmountDifference = nAmountGross - nAmountNet;
                lblAmountDifference.Text = Globals.RoundToNumDecimals(ref nAmountDifference, nNumDec, "N");
            }
            else if (nPercentage == 100)
            {
                entPercentage.Text = "";
                _ = entPercentage.Focus();
                return;
            }
            else if (nAmountNet > 0)
            {
                try
                {
                    decimal nAmountGross = nAmountNet / (1 - (nPercentage / 100));
                    lblAmountGross.Text = Globals.RoundToNumDecimals(ref nAmountGross, nNumDec, "N");
                    decimal nAmountDifference = nAmountGross - nAmountNet;
                    lblAmountDifference.Text = Globals.RoundToNumDecimals(ref nAmountDifference, nNumDec, "N");
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
            else
            {
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
            entPercentage.Text = "";
            entAmountNet.Text = "";

            _ = entPercentage.Focus();
        }
    }
}