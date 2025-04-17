namespace Finance
{
    public sealed partial class PageInterestAnnual : ContentPage
    {
        public PageInterestAnnual()
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
            // Set the margin of the title for windows
            lblTitlePage.Margin = new Thickness(80, 15, 0, 0);
#endif
            //// Set the type of keyboard
            if (Globals.cKeyboard == "Default")
            {
                entCapitalInitial.Keyboard = Keyboard.Default;
                entAmountPeriod.Keyboard = Keyboard.Default;
                entCapitalFinal.Keyboard = Keyboard.Default;
            }
            else if (Globals.cKeyboard == "Text")
            {
                entCapitalInitial.Keyboard = Keyboard.Text;
                entAmountPeriod.Keyboard = Keyboard.Text;
                entCapitalFinal.Keyboard = Keyboard.Text;
            }

            // Reset the entry fields
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
            _ = entCapitalInitial.Focus();
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
        /// Check if the value is numeric and clear result fields if the text have changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Globals.IsNumeric((Entry)sender, e.NewTextValue))
            {
                ((Entry)sender).Text = e.OldTextValue;
            }

            lblInterestRate.Text = "";
        }

        /// <summary>
        /// Go to the next field when the return key have been pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToNextField(object sender, EventArgs e)
        {
            if (sender == entCapitalInitial)
            {
                _ = entDurationYears.Focus();
            }
            else if (sender == entDurationYears)
            {
                _ = entAmountPeriod.Focus();
            }
            else if (sender == entAmountPeriod)
            {
                _ = entCapitalFinal.Focus();
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
            bool bIsNumber = double.TryParse(entCapitalInitial.Text, out double nCapitalInitial);
            if (!bIsNumber || nCapitalInitial < 0 || nCapitalInitial >= 1_000_000_000_000)
            {
                entCapitalInitial.Text = "";
                _ = entCapitalInitial.Focus();
                return;
            }

            bIsNumber = int.TryParse(entDurationYears.Text, out int nDurationYears);
            if (!bIsNumber || nDurationYears < 1 || nDurationYears > 100)
            {
                entDurationYears.Text = "";
                _ = entDurationYears.Focus();
                return;
            }

            bIsNumber = double.TryParse(entAmountPeriod.Text, out double nAmountPeriod);
            if (!bIsNumber || nAmountPeriod < 0 || nAmountPeriod >= 1_000_000_000_000)
            {
                entAmountPeriod.Text = "";
                _ = entAmountPeriod.Focus();
                return;
            }

            bIsNumber = double.TryParse(entCapitalFinal.Text, out double nCapitalFinal);
            if (!bIsNumber || nCapitalFinal < 0 || nCapitalFinal >= 1_000_000_000_000)
            {
                entCapitalFinal.Text = "";
                _ = entCapitalFinal.Focus();
                return;
            }

            // Close the keyboard
            entAmountPeriod.IsEnabled = false;
            entAmountPeriod.IsEnabled = true;
            entCapitalFinal.IsEnabled = false;
            entCapitalFinal.IsEnabled = true;

            // Convert string to int for number of decimal digits after decimal point
            int nNumDec = int.Parse(Globals.cNumDecimalDigits);
            int nPercDec = int.Parse(Globals.cPercDecimalDigits);

            // Check what needs to be calculated first
            if (nCapitalFinal > 0)
            {
                nAmountPeriod = 0;
            }

            if (nAmountPeriod > 0)
            {
                nCapitalFinal = 0;
            }

            // Initialize variables
            double nInterestAmount;
            double nInterestRate;
            double nInterimCalculation;
            double nRenteTemp;

            // Calculate annual interest
            try
            {
                if (nCapitalInitial == 0)
                {
                    _ = entCapitalInitial.Focus();
                    return;
                }
                else if (nAmountPeriod > 0)
                {
                    nInterestAmount = nDurationYears * nAmountPeriod - nCapitalInitial;
                    nInterimCalculation = nAmountPeriod * nDurationYears;
                    entCapitalFinal.Text = Globals.RoundToNumDecimals(ref nInterimCalculation, nNumDec, "N");
                }
                else if (nCapitalFinal != 0)
                {
                    nInterestAmount = nCapitalFinal - nCapitalInitial;
                    nInterimCalculation = nCapitalFinal / nDurationYears;
                    entAmountPeriod.Text = Globals.RoundToNumDecimals(ref nInterimCalculation, nNumDec, "N");
                }
                else
                {
                    return;
                }

                nRenteTemp = (Math.Pow((nInterestAmount + nCapitalInitial) / nCapitalInitial, (double)1 / nDurationYears) - 1) * 100;
                nInterestRate = nRenteTemp;
            }
            catch (Exception ex)
            {
#if DEBUG                
                DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
#endif
                ResetEntryFields(null, null);
                return;
            }

            // Rounding interest
            lblInterestRate.Text = Globals.RoundToNumDecimals(ref nInterestRate, nPercDec, "N");
            Globals.SetLabelTextColorForNumber(lblInterestRate);

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
            entCapitalInitial.Text = 0.ToString("F" + Globals.cNumDecimalDigits);
            entDurationYears.Text = "1";
            entAmountPeriod.Text = 0.ToString("F" + Globals.cNumDecimalDigits);
            entCapitalFinal.Text = 0.ToString("F" + Globals.cNumDecimalDigits);
            lblInterestRate.Text = "";

            _ = entCapitalInitial.Focus();
        }
    }
}