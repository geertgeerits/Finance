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
            DisplayAlert("InitializeComponent", ex.Message, "OK");
            return;
        }

        // Set the type of keyboard
        if (Globals.cKeyboard == "Default")
        {
            entVATPercentage.Keyboard = Keyboard.Default;
            entVATAmountExclusive.Keyboard = Keyboard.Default;
            entVATAmountIncluded.Keyboard = Keyboard.Default;
        }
        else if (Globals.cKeyboard == "Text")
        {
            entVATPercentage.Keyboard = Keyboard.Text;
            entVATAmountExclusive.Keyboard = Keyboard.Text;
            entVATAmountIncluded.Keyboard = Keyboard.Text;
        }
    }

    // Set focus to the first entry field
    // Add in the header of the xaml page: 'Loaded="OnPageLoaded"'
    private void OnPageLoaded(object sender, EventArgs e)
    {
        entVATPercentage.Focus();
    }

    // Select all the text in the entry field
    private void EntryFocused(object sender, EventArgs e)
    {
        Entry entry = (Entry)sender;

        entry.CursorPosition = entry.Text.Length;
        entry.CursorPosition = 0;
        entry.SelectionLength = entry.Text.Length;
    }

    // Clear result fields if the text have changed
    private void EntryTextChanged(object sender, EventArgs e)
    {
        lblVATAmount.Text = "";
    }

    // Go to the next field when the return key have been pressed
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == entVATPercentage)
        {
            entVATAmountExclusive.Focus();
        }
        else if (sender == entVATAmountExclusive)
        {
            entVATAmountIncluded.Focus();
        }
    }

    // Set the value of a another field to '0' if the current field is unfocused
    private void EntryUnfocused(object sender, EventArgs e)
    {
        entVATPercentage.Text = Globals.ReplaceDecimalPointComma(entVATPercentage.Text);
        bool bIsNumber = decimal.TryParse(entVATPercentage.Text, out decimal nVATPercentage);
        if (!bIsNumber)
        {
            return;
        }

        if (sender == entVATPercentage && nVATPercentage == 0)
        {
            lblVATAmount.Text = "0";
        }
        else if (sender == entVATAmountExclusive && entVATAmountExclusive.Text != "0" && nVATPercentage != 0)
        {
            entVATAmountIncluded.Text = "0";
        }
        else if (sender == entVATAmountIncluded && entVATAmountIncluded.Text != "0" && nVATPercentage != 0)
        {
            entVATAmountExclusive.Text = "0";
        }
    }

    // Calculate the result
    private void CalculateResult(object sender, EventArgs e)
    {
        entVATPercentage.Text = Globals.ReplaceDecimalPointComma(entVATPercentage.Text);
        bool bIsNumber = decimal.TryParse(entVATPercentage.Text, out decimal nVATPercentage);
        if (bIsNumber == false || nVATPercentage < 0 || nVATPercentage > 100)
        {
            entVATPercentage.Text = "";
            entVATPercentage.Focus();
            return;
        }

        entVATAmountExclusive.Text = Globals.ReplaceDecimalPointComma(entVATAmountExclusive.Text);
        bIsNumber = decimal.TryParse(entVATAmountExclusive.Text, out decimal nVATAmountExclusive);
        if (bIsNumber == false || nVATAmountExclusive < 0 || nVATAmountExclusive > 9_999_999_999)
        {
            entVATAmountExclusive.Text = "";
            entVATAmountExclusive.Focus();
            return;
        }

        entVATAmountIncluded.Text = Globals.ReplaceDecimalPointComma(entVATAmountIncluded.Text);
        bIsNumber = decimal.TryParse(entVATAmountIncluded.Text, out decimal nVATAmountIncluded);
        if (bIsNumber == false || nVATAmountIncluded < 0 || nVATAmountIncluded > 9_999_999_999)
        {
            entVATAmountIncluded.Text = "";
            entVATAmountIncluded.Focus();
            return;
        }

        // Close the keyboard
        entVATAmountExclusive.IsEnabled = false;
        entVATAmountExclusive.IsEnabled = true;
        entVATAmountIncluded.IsEnabled = false;
        entVATAmountIncluded.IsEnabled = true;

        // Convert string to int for number of decimal digits after decimal point
        int nNumDec = int.Parse(Globals.cNumDecimalDigits);
        int nPercDec = int.Parse(Globals.cPercDecimalDigits);

        // Set decimal places for the Entry controls and values passed by reference
        entVATPercentage.Text = Globals.RoundDecimalToNumDecimals(ref nVATPercentage, nPercDec, "F");
        entVATAmountExclusive.Text = Globals.RoundDecimalToNumDecimals(ref nVATAmountExclusive, nNumDec, "F");
        entVATAmountIncluded.Text = Globals.RoundDecimalToNumDecimals(ref nVATAmountIncluded, nNumDec, "F");

        // Calculate the VAT
        decimal nVATAmount;

        try
        {
            // Calculate the VAT percentage
            if (nVATPercentage == 0 && nVATAmountExclusive > 0 && nVATAmountIncluded > nVATAmountExclusive)
            {
                nVATAmount = nVATAmountIncluded - nVATAmountExclusive;
                nVATPercentage = nVATAmount / nVATAmountExclusive * 100;

                entVATPercentage.Text = Globals.RoundDecimalToNumDecimals(ref nVATPercentage, nPercDec, "F");
            }
            
            // Calculate the amount VAT exclusieve
            else if (nVATAmountIncluded > 0)
            {
                nVATAmount = nVATAmountIncluded * nVATPercentage / (100 + nVATPercentage);

                if (Globals.cRoundNumber == "AwayFromZero")
                {
                    nVATAmount = Math.Round(nVATAmount, nNumDec, MidpointRounding.AwayFromZero);
                }
                else if (Globals.cRoundNumber == "ToEven")
                {
                    nVATAmount = Math.Round(nVATAmount, nNumDec, MidpointRounding.ToEven);
                }

                nVATAmountExclusive = nVATAmountIncluded - nVATAmount;
                entVATAmountExclusive.Text = Globals.RoundDecimalToNumDecimals(ref nVATAmountExclusive, nNumDec, "F");
            }
            
            // Calculate the amount VAT included
            else if (nVATAmountExclusive > 0)
            {
                nVATAmount = nVATAmountExclusive * nVATPercentage / 100;
                
                if (Globals.cRoundNumber == "AwayFromZero")
                {
                    nVATAmount = Math.Round(nVATAmount, nNumDec, MidpointRounding.AwayFromZero);
                }
                else if (Globals.cRoundNumber == "ToEven")
                {
                    nVATAmount = Math.Round(nVATAmount, nNumDec, MidpointRounding.ToEven);
                }

                nVATAmountIncluded = nVATAmountExclusive + nVATAmount;
                entVATAmountIncluded.Text = Globals.RoundDecimalToNumDecimals(ref nVATAmountIncluded, nNumDec, "F");
            }
            
            else
            {
                return;
            }
        }
        catch (Exception ex)
        {
            DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
            return;
        }

        // Rounding result
        lblVATAmount.Text = Globals.RoundDecimalToNumDecimals(ref nVATAmount, nNumDec, "N");

        // Set focus
        btnReset.Focus();
    }

    // Reset the entry fields
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entVATPercentage.Text = "";
        entVATAmountExclusive.Text = "0";
        lblVATAmount.Text = "";
        entVATAmountIncluded.Text = "0";

        entVATPercentage.Focus();
    }
}