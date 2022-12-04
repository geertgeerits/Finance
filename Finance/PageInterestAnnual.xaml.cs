using Finance.Resources.Languages;

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
            DisplayAlert("InitializeComponent", ex.Message, MainPage.cButtonCloseText);
            return;
        }

        // Put text in the chosen language in the controls.
        lblTitle.Text = FinLang.InterestAnnualForm_Text;

        lblExplanation.Text = FinLang.InterestAnnualExplanation_Text + "\n";  // BUG auto sizing label for small screens.  Add a new line to solve the bug.;
        lblNumDec.Text = FinLang.NumDec_Text;
        lblCapitalInitial.Text = FinLang.CapitalInitial_Text;
        lblDurationMonths.Text = FinLang.DurationMonths_Text;
        lblAmountPeriod.Text = FinLang.AmountPeriod_Text;
        lblCapitalFinal.Text = FinLang.CapitalFinal_Text;
        lblInterestRate.Text = FinLang.InterestRate_Text;
        btnCalculate.Text = FinLang.Calculate_Text;
        btnReset.Text = FinLang.Reset_Text;

        // Set the type of keyboard.
        if (MainPage.cKeyboard == "Default")
        {
            entCapitalInitial.Keyboard = Keyboard.Default;
            entAmountPeriod.Keyboard = Keyboard.Default;
            entCapitalFinal.Keyboard = Keyboard.Default;
        }
        else if (MainPage.cKeyboard == "Text")
        {
            entCapitalInitial.Keyboard = Keyboard.Text;
            entAmountPeriod.Keyboard = Keyboard.Text;
            entCapitalFinal.Keyboard = Keyboard.Text;
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
        txtInterestRate.Text = "";
    }

    // Go to the next field when the return key have been pressed.
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == entNumDec)
        {
            entCapitalInitial.Focus();
        }
        else if (sender == entCapitalInitial)
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
        //DisplayAlert("nNumDec", Convert.ToString(nNumDec), MainPage.cButtonCloseText);

        entCapitalInitial.Text = MainPage.ReplaceDecimalPointComma(entCapitalInitial.Text);
        bIsNumber = double.TryParse(entCapitalInitial.Text, out double nCapitalInitial);
        if (bIsNumber == false || nCapitalInitial < 0 || nCapitalInitial > 9999999999)
        {
            entCapitalInitial.Text = "";
            entCapitalInitial.Focus();
            return;
        }

        bIsNumber = int.TryParse(entDurationMonths.Text, out int nDurationMonths);
        if (bIsNumber == false || nDurationMonths < 0 || nDurationMonths > 1200)
        {
            entDurationMonths.Text = "";
            entDurationMonths.Focus();
            return;
        }

        entAmountPeriod.Text = MainPage.ReplaceDecimalPointComma(entAmountPeriod.Text);
        bIsNumber = double.TryParse(entAmountPeriod.Text, out double nAmountPeriod);
        if (bIsNumber == false || nAmountPeriod < 0 || nAmountPeriod > 9999999999)
        {
            entAmountPeriod.Text = "";
            entAmountPeriod.Focus();
            return;
        }

        entCapitalFinal.Text = MainPage.ReplaceDecimalPointComma(entCapitalFinal.Text);
        bIsNumber = double.TryParse(entCapitalFinal.Text, out double nCapitalFinal);
        if (bIsNumber == false || nCapitalFinal < 0 || nCapitalFinal > 9999999999)
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

        // Set decimal places for the Entry controls and values passed by reference.
        entCapitalInitial.Text = MainPage.RoundDoubleToNumDecimals(ref nCapitalInitial, nNumDec, "F");
        entAmountPeriod.Text = MainPage.RoundDoubleToNumDecimals(ref nAmountPeriod, nNumDec, "F");
        entCapitalFinal.Text = MainPage.RoundDoubleToNumDecimals(ref nCapitalFinal, nNumDec, "F");

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
            entCapitalFinal.Text = MainPage.RoundDoubleToNumDecimals(ref nInterimCalculation, nNumDec, "F");
        }
        else if (nCapitalFinal != 0)
        {
            nInterestAmount = nCapitalFinal - nCapitalInitial;
            nInterimCalculation = nCapitalFinal / nDurationMonths;
            entAmountPeriod.Text = MainPage.RoundDoubleToNumDecimals(ref nInterimCalculation, nNumDec, "F");
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
            DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
            return;
        }

        // Rounding interest.
        txtInterestRate.Text = MainPage.RoundDoubleToNumDecimals(ref nInterestRate, nNumDec, "N");

        // Set focus.
        btnReset.Focus();
    }

    // Reset the entry fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entNumDec.Text = "2";
        entCapitalInitial.Text = "";
        entDurationMonths.Text = "12";
        entAmountPeriod.Text = "0";
        entCapitalFinal.Text = "0";
        txtInterestRate.Text = "";

        entNumDec.Focus();
    }
}