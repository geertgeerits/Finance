namespace Finance;
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
    }

    // Set focus to the first entry field.
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
        lblValueDifference.Text = "";
        lblValuePercDifference.Text = "";
        lblValuePercDiffValue1.Text = "";
        lblValuePercDiffValue2.Text = "";
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

        entValue2.Text = Globals.ReplaceDecimalPointComma(entValue2.Text);
        bIsNumber = decimal.TryParse(entValue2.Text, out decimal nValue2);
        if (bIsNumber == false || nValue2 < -9_999_999_999 || nValue2 > 9_999_999_999)
        {
            entValue2.Text = "";
            entValue2.Focus();
            return;
        }

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
        lblValueDifference.Text = Globals.RoundDecimalToNumDecimals(ref nValueDifference, nNumDec, "N");
        Globals.SetLabelTextColorForNumber(lblValueDifference);

        if (nValue1 == 0 && nValue2 == 0)
        {
            lblValuePercDifference.Text = "";
            lblValuePercDiffValue1.Text = "";
            lblValuePercDiffValue2.Text = "";

            btnReset.Focus();
            return;
        }

        if (nValue1 == 0 && nValue2 != 0)
        {
            lblValuePercDifference.Text = "";
            
            nValueTemp = 0;
            lblValuePercDiffValue1.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
            Globals.SetLabelTextColorForNumber(lblValuePercDiffValue1);
            
            lblValuePercDiffValue2.Text = "";

            btnReset.Focus();
            return;
        }

        if (nValue1 != 0 && nValue2 == 0)
        {
            nValueTemp = -100;
            lblValuePercDifference.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
            Globals.SetLabelTextColorForNumber(lblValuePercDifference);
            
            lblValuePercDiffValue1.Text = "";
            
            nValueTemp = 0;
            lblValuePercDiffValue2.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
            Globals.SetLabelTextColorForNumber(lblValuePercDiffValue2);

            btnReset.Focus();
            return;
        }

        if (nValue1 == nValue2)
        {
            nValueTemp = 0;
            lblValuePercDifference.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
            Globals.SetLabelTextColorForNumber(lblValuePercDifference);
            
            nValueTemp = 100;
            lblValuePercDiffValue1.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
            Globals.SetLabelTextColorForNumber(lblValuePercDiffValue1);
            
            lblValuePercDiffValue2.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
            Globals.SetLabelTextColorForNumber(lblValuePercDiffValue2);

            btnReset.Focus();
            return;
        }

        nValueTemp = nValue1 / nValue2 * 100;
        lblValuePercDiffValue1.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
        Globals.SetLabelTextColorForNumber(lblValuePercDiffValue1);

        nValueTemp = nValue2 / nValue1 * 100;
        lblValuePercDiffValue2.Text = Globals.RoundDecimalToNumDecimals(ref nValueTemp, nPercDec, "N");
        Globals.SetLabelTextColorForNumber(lblValuePercDiffValue2);

        try
        {
            nValuePercDifference = nValueDifference / nValue1 * 100;
        }
        catch (Exception ex)
        {
            DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
            return;
        }

        lblValuePercDifference.Text = Globals.RoundDecimalToNumDecimals(ref nValuePercDifference, nPercDec, "N");
        Globals.SetLabelTextColorForNumber(lblValuePercDifference);

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
