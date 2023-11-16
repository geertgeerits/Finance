namespace Finance;

public partial class PageInterestAnnual : ContentPage
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

        // Set the type of keyboard.
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

        // Set focus to the first entry field.
        //entCapitalInitial.Focus();
    }

    // Set focus to the first entry field (workaround for !!!BUG!!! ?).
    // Add in the header of the xaml page: 'Loaded="OnPageLoaded"'
    private void OnPageLoaded(object sender, EventArgs e)
    {
        entCapitalInitial.Focus();
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
        lblInterestRate.Text = "";
    }

    // Go to the next field when the return key have been pressed.
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == entCapitalInitial)
        {
            entDurationMonths.Focus();
        }
        else if (sender == entDurationMonths)
        {
            entAmountPeriod.Focus();
        }
        else if (sender == entAmountPeriod)
        {
            entCapitalFinal.Focus();
        }
    }

    // Set the value of a another field to '0' if the current field is unfocused.
    private void EntryUnfocused(object sender, EventArgs e)
    {
        if (sender == entAmountPeriod && entAmountPeriod.Text != "0")
        {
            entCapitalFinal.Text = "0";
        }
        else if (sender == entCapitalFinal && entCapitalFinal.Text != "0")
        {
            entAmountPeriod.Text = "0";
        }
    }

    // Calculate the result.
    private void CalculateResult(object sender, EventArgs e)
    {
        // Validate input values.
        entCapitalInitial.Text = Globals.ReplaceDecimalPointComma(entCapitalInitial.Text);
        bool bIsNumber = double.TryParse(entCapitalInitial.Text, out double nCapitalInitial);
        if (bIsNumber == false || nCapitalInitial < 0 || nCapitalInitial > 9_999_999_999)
        {
            entCapitalInitial.Text = "";
            entCapitalInitial.Focus();
            return;
        }

        bIsNumber = int.TryParse(entDurationMonths.Text, out int nDurationMonths);
        if (bIsNumber == false || nDurationMonths < 1 || nDurationMonths > 1_200)
        {
            entDurationMonths.Text = "";
            entDurationMonths.Focus();
            return;
        }

        entAmountPeriod.Text = Globals.ReplaceDecimalPointComma(entAmountPeriod.Text);
        bIsNumber = double.TryParse(entAmountPeriod.Text, out double nAmountPeriod);
        if (bIsNumber == false || nAmountPeriod < 0 || nAmountPeriod > 9_999_999_999)
        {
            entAmountPeriod.Text = "";
            entAmountPeriod.Focus();
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

        // Close the keyboard.
        entAmountPeriod.IsEnabled = false;
        entAmountPeriod.IsEnabled = true;
        entCapitalFinal.IsEnabled = false;
        entCapitalFinal.IsEnabled = true;

        // Convert string to int for number of decimal digits after decimal point.
        int nNumDec = int.Parse(Globals.cNumDecimalDigits);
        int nPercDec = int.Parse(Globals.cPercDecimalDigits);

        // Check what needs to be calculated first.
        if (nCapitalFinal > 0)
        {
            nAmountPeriod = 0;
        }

        if (nAmountPeriod > 0)
        {
            nCapitalFinal = 0;
        }

        // Set decimal places for the Entry controls and values passed by reference.
        entCapitalInitial.Text = Globals.RoundDoubleToNumDecimals(ref nCapitalInitial, nNumDec, "F");
        entAmountPeriod.Text = Globals.RoundDoubleToNumDecimals(ref nAmountPeriod, nNumDec, "F");
        entCapitalFinal.Text = Globals.RoundDoubleToNumDecimals(ref nCapitalFinal, nNumDec, "F");

        // Initialize variables.
        double nInterestAmount;
        double nInterestRate;
        double nInterimCalculation;
        double nRenteTemp;

        // Calculate annual interest.
        if (nCapitalInitial == 0)
        {
            _ = entCapitalInitial.Focus();
            return;
        }
        else if (nAmountPeriod > 0)
        {
            nInterestAmount = nDurationMonths * nAmountPeriod - nCapitalInitial;
            nInterimCalculation = nAmountPeriod * nDurationMonths;
            entCapitalFinal.Text = Globals.RoundDoubleToNumDecimals(ref nInterimCalculation, nNumDec, "F");
        }
        else if (nCapitalFinal != 0)
        {
            nInterestAmount = nCapitalFinal - nCapitalInitial;
            nInterimCalculation = nCapitalFinal / nDurationMonths;
            entAmountPeriod.Text = Globals.RoundDoubleToNumDecimals(ref nInterimCalculation, nNumDec, "F");
        }
        else
        {
            return;
        }

        try
        {
            nRenteTemp = (Math.Pow((nInterestAmount + nCapitalInitial) / nCapitalInitial, (double)1 / (nDurationMonths / 12)) - 1) * 100;
            nInterestRate = nRenteTemp;
        }
        catch (Exception ex)
        {
            DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
            return;
        }

        // Rounding interest.
        lblInterestRate.Text = Globals.RoundDoubleToNumDecimals(ref nInterestRate, nPercDec, "N");
        Globals.SetLabelTextColorForNumber(lblInterestRate);

        // Set focus.
        btnReset.Focus();
    }

    // Reset the entry fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entCapitalInitial.Text = "";
        entDurationMonths.Text = "12";
        entAmountPeriod.Text = "0";
        entCapitalFinal.Text = "0";
        lblInterestRate.Text = "";

        entCapitalInitial.Focus();
    }
}