using Finance.Resources.Languages;

namespace Finance;

public partial class PageVATCalculation : ContentPage
{
	public PageVATCalculation()
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
        lblTitle.Text = FinLang.VATCalculationForm_Text;

        lblExplanation.Text = FinLang.VATCalculationExplanation_Text;
        lblNumDec.Text = FinLang.NumDec_Text;
        lblVATPercentage.Text = FinLang.VATPercentage_Text;
        lblVATAmountExclusive.Text = FinLang.VATAmountExclusive_Text;
        lblVATAmount.Text = FinLang.VATAmount_Text;
        lblVATAmountIncluded.Text = FinLang.VATAmountIncluded_Text;
        btnCalculate.Text = FinLang.Calculate_Text;
        btnReset.Text = FinLang.Reset_Text;

        // Set the type of keyboard.
        if (MainPage.cKeyboard == "Default")
        {
            entVATPercentage.Keyboard = Keyboard.Default;
            entVATAmountExclusive.Keyboard = Keyboard.Default;
            entVATAmountIncluded.Keyboard = Keyboard.Default;
        }
        else if (MainPage.cKeyboard == "Text")
        {
            entVATPercentage.Keyboard = Keyboard.Text;
            entVATAmountExclusive.Keyboard = Keyboard.Text;
            entVATAmountIncluded.Keyboard = Keyboard.Text;
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
        txtVATAmount.Text = "";
    }

    // Go to the next field when the return key have been pressed.
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == entNumDec)
        {
            entVATPercentage.Focus();
        }
        else if (sender == entVATPercentage)
        {
            entVATAmountExclusive.Focus();
        }
        else if (sender == entVATAmountExclusive)
        {
            entVATAmountIncluded.Focus();
        }
    }

    // Set the value of a another field to '0' if the current field is unfocused.
    private void EntryUnfocused(object sender, EventArgs e)
    {
        if (sender == entVATAmountExclusive && entVATAmountExclusive.Text != "0")
        {
            entVATAmountIncluded.Text = "0";
        }
        else if (sender == entVATAmountIncluded && entVATAmountIncluded.Text != "0")
        {
            entVATAmountExclusive.Text = "0";
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

        entVATPercentage.Text = MainPage.ReplaceDecimalPointComma(entVATPercentage.Text);
        bIsNumber = decimal.TryParse(entVATPercentage.Text, out decimal nVATPercentage);
        if (bIsNumber == false || nVATPercentage < 0 || nVATPercentage > 100)
        {
            entVATPercentage.Text = "";
            entVATPercentage.Focus();
            return;
        }

        entVATAmountExclusive.Text = MainPage.ReplaceDecimalPointComma(entVATAmountExclusive.Text);
        bIsNumber = decimal.TryParse(entVATAmountExclusive.Text, out decimal nVATAmountExclusive);
        if (bIsNumber == false || nVATAmountExclusive < 0 || nVATAmountExclusive > 9999999999)
        {
            entVATAmountExclusive.Text = "";
            entVATAmountExclusive.Focus();
            return;
        }

        entVATAmountIncluded.Text = MainPage.ReplaceDecimalPointComma(entVATAmountIncluded.Text);
        bIsNumber = decimal.TryParse(entVATAmountIncluded.Text, out decimal nVATAmountIncluded);
        if (bIsNumber == false || nVATAmountIncluded < 0 || nVATAmountIncluded > 9999999999)
        {
            entVATAmountIncluded.Text = "";
            entVATAmountIncluded.Focus();
            return;
        }

        // Close the keyboard.
        entVATAmountExclusive.IsEnabled = false;
        entVATAmountExclusive.IsEnabled = true;
        entVATAmountIncluded.IsEnabled = false;
        entVATAmountIncluded.IsEnabled = true;

        // Set decimal places for the Entry controls and values passed by reference.
        entVATPercentage.Text = MainPage.RoundDecimalToNumDecimals(ref nVATPercentage, nNumDec, "F");
        entVATAmountExclusive.Text = MainPage.RoundDecimalToNumDecimals(ref nVATAmountExclusive, nNumDec, "F");
        entVATAmountIncluded.Text = MainPage.RoundDecimalToNumDecimals(ref nVATAmountIncluded, nNumDec, "F");

        // Calculate the VAT.
        decimal nVATAmount;

        try
        {
            if (nVATAmountIncluded > 0)
            {
                nVATAmount = nVATAmountIncluded * nVATPercentage / (100 + nVATPercentage);

                if (MainPage.cRoundNumber == "AwayFromZero")
                {
                    nVATAmount = Math.Round(nVATAmount, nNumDec, MidpointRounding.AwayFromZero);
                }
                else if (MainPage.cRoundNumber == "ToEven")
                {
                    nVATAmount = Math.Round(nVATAmount, nNumDec, MidpointRounding.ToEven);
                }

                nVATAmountExclusive = nVATAmountIncluded - nVATAmount;
                entVATAmountExclusive.Text = MainPage.RoundDecimalToNumDecimals(ref nVATAmountExclusive, nNumDec, "F");
            }
            else if (nVATAmountExclusive > 0)
            {
                nVATAmount = nVATAmountExclusive * nVATPercentage / 100;
                
                if (MainPage.cRoundNumber == "AwayFromZero")
                {
                    nVATAmount = Math.Round(nVATAmount, nNumDec, MidpointRounding.AwayFromZero);
                }
                else if (MainPage.cRoundNumber == "ToEven")
                {
                    nVATAmount = Math.Round(nVATAmount, nNumDec, MidpointRounding.ToEven);
                }

                nVATAmountIncluded = nVATAmountExclusive + nVATAmount;
                entVATAmountIncluded.Text = MainPage.RoundDecimalToNumDecimals(ref nVATAmountIncluded, nNumDec, "F");
            }
            else
            {
                return;
            }
        }
        catch (Exception ex)
        {
            DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
            return;
        }

        // Rounding result.
        txtVATAmount.Text = MainPage.RoundDecimalToNumDecimals(ref nVATAmount, nNumDec, "N");

        // Set focus.
        btnReset.Focus();
    }

    // Reset the entry fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entNumDec.Text = "2";
        entVATPercentage.Text = "";
        entVATAmountExclusive.Text = "0";
        txtVATAmount.Text = "";
        entVATAmountIncluded.Text = "0";

        entNumDec.Focus();
    }
}