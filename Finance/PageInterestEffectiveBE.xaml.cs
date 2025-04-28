namespace Finance
{
    public sealed partial class PageInterestEffectiveBE : ContentPage
    {
    	public PageInterestEffectiveBE()
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
                entCapitalInitial.Keyboard = Keyboard.Default;
                entCapitalFinal.Keyboard = Keyboard.Default;
            }
            else if (Globals.cKeyboard == "Text")
            {
                entCapitalInitial.Keyboard = Keyboard.Text;
                entCapitalFinal.Keyboard = Keyboard.Text;
            }

            //// Set the Placeholder and MaxLength for the numeric entry field
            ClassEntryMethods.SetNumberEntryProperties(entCapitalInitial, "0", "0", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits);
            ClassEntryMethods.SetNumberEntryProperties(entCapitalFinal, "0", "0", "999999999999", "9", ClassEntryMethods.cNumDecimalDigits);
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
        private void NumberEntryFocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                entry.MaxLength = 17;
                ClassEntryMethods.FormatNumberEntryFocused(entry);
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
                ClassEntryMethods.FormatNumberEntryUnfocused(entry);
            }
        }

        /// <summary>
        /// Check if the value is numeric and clear result fields if the text have changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!ClassEntryMethods.IsNumeric((Entry)sender, e.NewTextValue))
            {
                ((Entry)sender).Text = e.OldTextValue;
            }

            lblAmountDifference.Text = "";
            lblInterestEffective.Text = "";
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
                _ = entCapitalFinal.Focus();
            }
            else if (sender == entCapitalFinal)
            {
                _ = entDurationYears.Focus();
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

            bIsNumber = double.TryParse(entCapitalFinal.Text, out double nCapitalFinal);
            if (!bIsNumber || nCapitalFinal < 0 || nCapitalFinal >= 1_000_000_000_000)
            {
                entCapitalFinal.Text = "";
                _ = entCapitalFinal.Focus();
                return;
            }

            bIsNumber = int.TryParse(entDurationYears.Text, out int nDurationYears);
            if (!bIsNumber || nDurationYears < 1 || nDurationYears > 100)
            {
                entDurationYears.Text = "";
                _ = entDurationYears.Focus();
                return;
            }

            // Hide the keyboard
            ClassEntryMethods.HideKeyboard(entDurationYears);

            // Convert string to int for number of decimal digits after decimal point
            int nNumDec = int.Parse(ClassEntryMethods.cNumDecimalDigits);
            int nPercDec = int.Parse(ClassEntryMethods.cPercDecimalDigits);

            // Calculating the effective interest
            double nAmountDifference;
            double nInterestEffective;

            if (nCapitalInitial > 0)
            {
                try
                {
                    nAmountDifference = nCapitalFinal - nCapitalInitial;
                    nInterestEffective = (Math.Pow(nCapitalFinal / nCapitalInitial, (double)1 / nDurationYears) - 1) * 100;
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
                nAmountDifference = 0;
                nInterestEffective = 0;
            }

            // Rounding result
            lblAmountDifference.Text = ClassEntryMethods.RoundToNumDecimals(ref nAmountDifference, nNumDec, "N");
            ClassEntryMethods.SetLabelTextColorForNumber(lblAmountDifference);
            lblInterestEffective.Text = ClassEntryMethods.RoundToNumDecimals(ref nInterestEffective, nPercDec, "N");
            ClassEntryMethods.SetLabelTextColorForNumber(lblInterestEffective);

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
            entCapitalInitial.Text = "";
            entCapitalFinal.Text = "";
            entDurationYears.Text = "1";
            lblInterestEffective.Text = "";

            _ = entCapitalInitial.Focus();
        }
    }
}
