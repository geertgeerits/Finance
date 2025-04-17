﻿namespace Finance
{
    public sealed partial class PageDifferenceNumbers : ContentPage
    {
        public PageDifferenceNumbers()
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
                entValue1.Keyboard = Keyboard.Default;
                entValue2.Keyboard = Keyboard.Default;
            }
            else if (Globals.cKeyboard == "Text")
            {
                entValue1.Keyboard = Keyboard.Text;
                entValue2.Keyboard = Keyboard.Text;
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
            _ = entValue1.Focus();
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

            lblValueDifference.Text = "";
            lblValuePercDifference.Text = "";
            lblValuePercDiffValue1.Text = "";
            lblValuePercDiffValue2.Text = "";
        }

        /// <summary>
        /// Go to the next field when the return key have been pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToNextField(object sender, EventArgs e)
        {
            if (sender == entValue1)
            {
                _ = entValue2.Focus();
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
            bool bIsNumber = decimal.TryParse(entValue1.Text, out decimal nValue1);
            if (!bIsNumber || nValue1 <= -1_000_000_000_000 || nValue1 >= 1_000_000_000_000)
            {
                entValue1.Text = "";
                _ = entValue1.Focus();
                return;
            }

            bIsNumber = decimal.TryParse(entValue2.Text, out decimal nValue2);
            if (!bIsNumber || nValue2 <= -1_000_000_000_000 || nValue2 >= 1_000_000_000_000)
            {
                entValue2.Text = "";
                _ = entValue2.Focus();
                return;
            }

            // Close the keyboard
            entValue2.IsEnabled = false;
            entValue2.IsEnabled = true;
            //await entValue2.HideSoftInputAsync(default);

            // Convert string to int for number of decimal digits after decimal point
            int nNumDec = int.Parse(Globals.cNumDecimalDigits);
            int nPercDec = int.Parse(Globals.cPercDecimalDigits);

            // Calculate the difference
            decimal nValuePercDifference;
            decimal nValueTemp;

            decimal nValueDifference = nValue2 - nValue1;
            lblValueDifference.Text = Globals.RoundToNumDecimals(ref nValueDifference, nNumDec, "N");
            Globals.SetLabelTextColorForNumber(lblValueDifference);

            if (nValue1 == 0 && nValue2 == 0)
            {
                lblValuePercDifference.Text = "";
                lblValuePercDiffValue1.Text = "";
                lblValuePercDiffValue2.Text = "";

                _ = btnReset.Focus();
                return;
            }

            if (nValue1 == 0 && nValue2 != 0)
            {
                lblValuePercDifference.Text = "";
            
                nValueTemp = 0;
                lblValuePercDiffValue1.Text = Globals.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
                Globals.SetLabelTextColorForNumber(lblValuePercDiffValue1);
            
                lblValuePercDiffValue2.Text = "";

                _ = btnReset.Focus();
                return;
            }

            if (nValue1 != 0 && nValue2 == 0)
            {
                nValueTemp = -100;
                lblValuePercDifference.Text = Globals.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
                Globals.SetLabelTextColorForNumber(lblValuePercDifference);
            
                lblValuePercDiffValue1.Text = "";
            
                nValueTemp = 0;
                lblValuePercDiffValue2.Text = Globals.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
                Globals.SetLabelTextColorForNumber(lblValuePercDiffValue2);

                _ = btnReset.Focus();
                return;
            }

            if (nValue1 == nValue2)
            {
                nValueTemp = 0;
                lblValuePercDifference.Text = Globals.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
                Globals.SetLabelTextColorForNumber(lblValuePercDifference);
            
                nValueTemp = 100;
                lblValuePercDiffValue1.Text = Globals.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
                Globals.SetLabelTextColorForNumber(lblValuePercDiffValue1);
            
                lblValuePercDiffValue2.Text = Globals.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
                Globals.SetLabelTextColorForNumber(lblValuePercDiffValue2);

                _ = btnReset.Focus();
                return;
            }

            nValueTemp = nValue1 / nValue2 * 100;
            lblValuePercDiffValue1.Text = Globals.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
            Globals.SetLabelTextColorForNumber(lblValuePercDiffValue1);

            nValueTemp = nValue2 / nValue1 * 100;
            lblValuePercDiffValue2.Text = Globals.RoundToNumDecimals(ref nValueTemp, nPercDec, "N");
            Globals.SetLabelTextColorForNumber(lblValuePercDiffValue2);

            try
            {
                nValuePercDifference = nValueDifference / nValue1 * 100;
            }
            catch (Exception ex)
            {
#if DEBUG
                DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
#endif
                ResetEntryFields(null, null);
                return;
            }

            lblValuePercDifference.Text = Globals.RoundToNumDecimals(ref nValuePercDifference, nPercDec, "N");
            Globals.SetLabelTextColorForNumber(lblValuePercDifference);

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
            entValue1.Text = "";
            entValue2.Text = "";

            _ = entValue1.Focus();
        }
    }
}
