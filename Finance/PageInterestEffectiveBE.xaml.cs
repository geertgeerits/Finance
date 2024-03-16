namespace Finance;

public partial class PageInterestEffectiveBE : ContentPage
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

        // Set the type of keyboard
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
    }

    // Set focus to the first entry field
    // Add in the header of the xaml page: 'Loaded="OnPageLoaded"'
    private void OnPageLoaded(object sender, EventArgs e)
    {
        entCapitalInitial.Focus();
    }

    // Select all the text in the entry field
    private void EntryFocused(object sender, EventArgs e)
    {
        var entry = (Entry)sender;

        entry.CursorPosition = entry.Text.Length;
        entry.CursorPosition = 0;
        entry.SelectionLength = entry.Text.Length;
    }

    // Clear result fields if the text have changed
    private void EntryTextChanged(object sender, EventArgs e)
    {
        lblAmountDifference.Text = "";
        lblInterestEffective.Text = "";
    }

    // Go to the next field when the return key have been pressed
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == entCapitalInitial)
        {
            entCapitalFinal.Focus();
        }
        else if (sender == entCapitalFinal)
        {
            entDurationYears.Focus();
        }
    }

    // Calculate the result
    private void CalculateResult(object sender, EventArgs e)
    {
        // Validate input values
        entCapitalInitial.Text = Globals.ReplaceDecimalPointComma(entCapitalInitial.Text);
        bool bIsNumber = double.TryParse(entCapitalInitial.Text, out double nCapitalInitial);
        if (bIsNumber == false || nCapitalInitial < 0 || nCapitalInitial > 9_999_999_999)
        {
            entCapitalInitial.Text = "";
            entCapitalInitial.Focus();
            return;
        }

        entCapitalFinal.Text = Globals.ReplaceDecimalPointComma(entCapitalFinal.Text);
        bIsNumber = double.TryParse(entCapitalFinal.Text, out double nCapitalFinal);
        if (bIsNumber == false || nCapitalFinal < 0 || nCapitalFinal > 9_999_999_999)
        {
            entCapitalFinal.Text = "";
            entCapitalFinal.Focus();
            return;
        }

        bIsNumber = int.TryParse(entDurationYears.Text, out int nDurationYears);
        if (bIsNumber == false || nDurationYears < 1 || nDurationYears > 100)
        {
            entDurationYears.Text = "";
            entDurationYears.Focus();
            return;
        }

        // Close the keyboard
        entDurationYears.IsEnabled = false;
        entDurationYears.IsEnabled = true;

        // Convert string to int for number of decimal digits after decimal point
        int nNumDec = int.Parse(Globals.cNumDecimalDigits);
        int nPercDec = int.Parse(Globals.cPercDecimalDigits);

        // Set decimal places for the Entry controls and values passed by reference
        entCapitalInitial.Text = Globals.RoundDoubleToNumDecimals(ref nCapitalInitial, nNumDec, "F");
        entCapitalFinal.Text = Globals.RoundDoubleToNumDecimals(ref nCapitalFinal, nNumDec, "F");

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
                DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
                return;
            }
        }
        else
        {
            nAmountDifference = 0;
            nInterestEffective = 0;
        }

        // Rounding result
        lblAmountDifference.Text = Globals.RoundDoubleToNumDecimals(ref nAmountDifference, nNumDec, "N");
        Globals.SetLabelTextColorForNumber(lblAmountDifference);
        lblInterestEffective.Text = Globals.RoundDoubleToNumDecimals(ref nInterestEffective, nPercDec, "N");
        Globals.SetLabelTextColorForNumber(lblInterestEffective);

        // Set focus
        btnReset.Focus();
    }

    // Reset the entry fields
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entCapitalInitial.Text = "";
        entCapitalFinal.Text = "";
        entDurationYears.Text = "1";
        lblInterestEffective.Text = "";

        entCapitalInitial.Focus();
    }
}
