﻿namespace Finance;
public partial class PageDifferenceNumbers : ContentPage
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

        // Set the type of keyboard.
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

        // Set focus to the first entry field.
        //entValue1.Focus();
    }

    // Set focus to the first entry field (workaround for !!!BUG!!! ?).
    // Add in the header of the xaml page: 'Loaded="OnPageLoaded"'
    private void OnPageLoaded(object sender, EventArgs e)
    {
        entValue1.Focus();
    }

    // Select all the text in the entry field.
    private void EntryFocused(object sender, EventArgs e)
    {
        var entry = (Entry)sender;

        entry.CursorPosition = entry.Text.Length;
        entry.CursorPosition = 0;
        entry.SelectionLength = entry.Text.Length;
    }

    // Clear result fields if the text have changed.
    private void EntryTextChanged(object sender, EventArgs e)
    {
        txtValueDifference.Text = "";
        txtValuePercDifference.Text = "";
        txtValuePercDiffValue1.Text = "";
        txtValuePercDiffValue2.Text = "";
    }

    // Go to the next field when the return key have been pressed.
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == entValue1)
        {
            entValue2.Focus();
        }
    }

    // Calculate the result.
    private void CalculateResult(object sender, EventArgs e)
    {
        // Validate input values.
        entValue1.Text = Globals.ReplaceDecimalPointComma(entValue1.Text);
        bool bIsNumber = decimal.TryParse(entValue1.Text, out decimal nValue1);
        if (bIsNumber == false || nValue1 < -9_999_999_999 || nValue1 > 9_999_999_999)
        {
            entValue1.Text = "";
            entValue1.Focus();
            return;
        }
        //Globals.SetDecimalNumberColor(entValue1, nValue1);
        entValue1.TextColor = nValue1 < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);

        entValue2.Text = Globals.ReplaceDecimalPointComma(entValue2.Text);
        bIsNumber = decimal.TryParse(entValue2.Text, out decimal nValue2);
        if (bIsNumber == false || nValue2 < -9_999_999_999 || nValue2 > 9_999_999_999)
        {
            entValue2.Text = "";
            entValue2.Focus();
            return;
        }
        entValue2.TextColor = nValue2 < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);

        // Close the keyboard.
        entValue2.IsEnabled = false;
        entValue2.IsEnabled = true;
        //await entValue2.HideSoftInputAsync(default);

        // Convert string to int for number of decimal digits after decimal point.
        int nNumDec = int.Parse(Globals.cNumDecimalDigits);
        int nPercDec = int.Parse(Globals.cPercDecimalDigits);

        // Set decimal places for the Entry controls and values passed by reference.
        entValue1.Text = Globals.RoundDecimalToNumDecimals(ref nValue1, nNumDec, "F");
        entValue2.Text = Globals.RoundDecimalToNumDecimals(ref nValue2, nNumDec, "F");

        // Calculate the difference.
        decimal nValuePercDifference;
        decimal nValueTemp;

        decimal nValueDifference = nValue2 - nValue1;
        txtValueDifference.Text = Globals.RoundDecimalToNumDecimals(ref nValueDifference, nNumDec, "N");
        txtValueDifference.TextColor = nValueDifference < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);

        if (nValue1 == 0 && nValue2 == 0)
        {
            txtValuePercDifference.Text = "";
            txtValuePercDiffValue1.Text = "";
            txtValuePercDiffValue2.Text = "";

            btnReset.Focus();
            return;
        }

        if (nValue1 == 0 && nValue2 != 0)
        {
            txtValuePercDifference.Text = "";
            
            nValueTemp = 0;
            txtValuePercDiffValue1.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
            txtValuePercDiffValue1.TextColor = nValueTemp < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);
            
            txtValuePercDiffValue2.Text = "";

            btnReset.Focus();
            return;
        }

        if (nValue1 != 0 && nValue2 == 0)
        {
            nValueTemp = -100;
            txtValuePercDifference.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
            txtValuePercDifference.TextColor = nValueTemp < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);
            
            txtValuePercDiffValue1.Text = "";
            
            nValueTemp = 0;
            txtValuePercDiffValue2.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
            txtValuePercDiffValue2.TextColor = nValueTemp < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);

            btnReset.Focus();
            return;
        }

        if (nValue1 == nValue2)
        {
            nValueTemp = 0;
            txtValuePercDifference.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
            txtValuePercDifference.TextColor = nValueTemp < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);
            
            nValueTemp = 100;
            txtValuePercDiffValue1.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
            txtValuePercDiffValue1.TextColor = nValueTemp < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);
            
            txtValuePercDiffValue2.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
            txtValuePercDiffValue2.TextColor = nValueTemp < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);

            btnReset.Focus();
            return;
        }

        nValueTemp = nValue1 / nValue2 * 100;
        txtValuePercDiffValue1.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
        txtValuePercDiffValue1.TextColor = nValueTemp < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);

        nValueTemp = nValue2 / nValue1 * 100;
        txtValuePercDiffValue2.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
        txtValuePercDiffValue2.TextColor = nValueTemp < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);

        try
        {
            nValuePercDifference = nValueDifference / nValue1 * 100;
        }
        catch (Exception ex)
        {
            DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
            return;
        }

        txtValuePercDifference.Text = Globals.RoundDecimalToNumDecimals(ref nValuePercDifference, nPercDec, "N");
        txtValuePercDifference.TextColor = nValuePercDifference < 0 ? Color.FromArgb(Globals.cColorNegNumber) : Color.FromArgb(Globals.cColorPosNumber);

        // Set focus.
        btnReset.Focus();
    }

    // Reset the entry fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entValue1.Text = "";
        entValue2.Text = "";

        entValue1.Focus();
    }
}
