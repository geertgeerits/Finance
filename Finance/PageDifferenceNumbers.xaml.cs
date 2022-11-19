using Finance.Resources.Languages;

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
            DisplayAlert("InitializeComponent", ex.Message, MainPage.cButtonCloseText);
            return;
        }

        // Put text in the chosen language in the controls.
        lblTitle.Text = FinLang.DifferenceNumbersForm_Text;

        lblExplanation.Text = FinLang.DifferenceExplanationNumbers_Text;
        lblNumDec.Text = FinLang.NumDec_Text;
        lblValue1.Text = FinLang.Value1_Text;
        lblValue2.Text = FinLang.Value2_Text;
        lblValueDifference.Text = FinLang.ValueDifference_Text;
        lblValuePercDifference.Text = FinLang.ValuePercDifference_Text;
        lblValuePercDiffValue1.Text = FinLang.ValuePercDiffValue1_Text;
        lblValuePercDiffValue2.Text = FinLang.ValuePercDiffValue2_Text;

        btnCalculate.Text = FinLang.Calculate_Text;
        btnReset.Text = FinLang.Reset_Text;

        // Set the type of keyboard.
        if (MainPage.cKeyboard == "Default")
        {
            entValue1.Keyboard = Keyboard.Default;
            entValue2.Keyboard = Keyboard.Default;
        }
        else if (MainPage.cKeyboard == "Text")
        {
            entValue1.Keyboard = Keyboard.Text;
            entValue2.Keyboard = Keyboard.Text;
        }

        // Set focus to the first entry field.
        entNumDec.Focus();
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
        if (sender == entNumDec)
        {
            entValue1.Focus();
        }
        else if (sender == entValue1)
        {
            entValue2.Focus();
        }
    }

    // Calculate the result.
    private void CalculateResult(object sender, EventArgs e)
    {
        // Validate input values.
        bool bIsNumber = int.TryParse(entNumDec.Text, out int nNumDec);
        if (bIsNumber == false || nNumDec < 0 || nNumDec > 6)
        {
            entNumDec.Text = "";
            entNumDec.Focus();
            return;
        }

        entValue1.Text = MainPage.ReplaceDecimalPointComma(entValue1.Text);
        bIsNumber = decimal.TryParse(entValue1.Text, out decimal nValue1);
        if (bIsNumber == false || nValue1 < 1 || nValue1 > 9999999999)
        {
            entValue1.Text = "";
            entValue1.Focus();
            return;
        }

        entValue2.Text = MainPage.ReplaceDecimalPointComma(entValue2.Text);
        bIsNumber = decimal.TryParse(entValue2.Text, out decimal nValue2);
        if (bIsNumber == false || nValue2 < 1 || nValue2 > 9999999999)
        {
            entValue2.Text = "";
            entValue2.Focus();
            return;
        }

        // Close the keyboard.
        entValue2.IsEnabled = false;
        entValue2.IsEnabled = true;

        // Set decimal places for the Entry controls and values passed by reference.
        entValue1.Text = MainPage.RoundDecimalToNumDecimals(ref nValue1, nNumDec, "F");
        entValue2.Text = MainPage.RoundDecimalToNumDecimals(ref nValue2, nNumDec, "F");

        // Calculate the difference.
        decimal nValuePercDifference;

        if (nValue1 == nValue2)
        {
            return;
        }
        else
        {
            decimal nValueTemp;
            nValueTemp = nValue1 / nValue2 * 100;
            txtValuePercDiffValue1.Text = MainPage.RoundDecimalToNumDecimals(ref nValueTemp, nNumDec, "N");

            nValueTemp = nValue2 / nValue1 * 100;
            txtValuePercDiffValue2.Text = MainPage.RoundDecimalToNumDecimals(ref nValueTemp, nNumDec, "N");
        }

        decimal nValueDifference = nValue2 - nValue1;

        try
        {
            nValuePercDifference = nValueDifference / nValue1 * 100;
        }
        catch (Exception ex)
        {
            DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
            return;
        }

        txtValueDifference.Text = MainPage.RoundDecimalToNumDecimals(ref nValueDifference, nNumDec, "N");
        txtValuePercDifference.Text = MainPage.RoundDecimalToNumDecimals(ref nValuePercDifference, nNumDec, "N");

        // Set focus.
        entNumDec.Focus();
    }

    // Reset the entry fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entNumDec.Text = "2";
        entValue1.Text = "";
        entValue2.Text = "";

        entNumDec.Focus();
    }
}
