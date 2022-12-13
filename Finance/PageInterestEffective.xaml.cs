using Finance.Resources.Languages;

namespace Finance;

public partial class PageInterestEffective : ContentPage
{
    public PageInterestEffective()
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

        // Set the current UI culture of the selected language.
        MainPage.SetCultureSelectedLanguage();

        // Put text in the chosen language in the controls.
        lblTitle.Text = FinLang.InterestEffectiveForm_Text;

        lblExplanation.Text = FinLang.InterestEffectiveExplanation_Text;
        lblNumDec.Text = FinLang.NumDec_Text;
        lblInterestRate.Text = FinLang.InterestRate_Text;
        lblPeriodsYear.Text = FinLang.PeriodsYear_Text;
        lblInterestEffective.Text = FinLang.InterestEffective_Text;
        btnCalculate.Text = FinLang.Calculate_Text;
        btnReset.Text = FinLang.Reset_Text;

        // Set the type of keyboard.
        if (MainPage.cKeyboard == "Default")
        {
            entInterestRate.Keyboard = Keyboard.Default;
        }
        else if (MainPage.cKeyboard == "Text")
        {
            entInterestRate.Keyboard = Keyboard.Text;
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
        txtInterestEffective.Text = "";
    }

    // Go to the next field when the return key have been pressed.
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == entNumDec)
        {
            entInterestRate.Focus();
        }
        else if (sender == entInterestRate)
        {
            entPeriodsYear.Focus();
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

        entInterestRate.Text = MainPage.ReplaceDecimalPointComma(entInterestRate.Text);
        bIsNumber = double.TryParse(entInterestRate.Text, out double nInterestRate);
        if (bIsNumber == false || nInterestRate < 0 || nInterestRate > 100)
        {
            entInterestRate.Text = "";
            entInterestRate.Focus();
            return;
        }

        bIsNumber = double.TryParse(entPeriodsYear.Text, out double nPeriodsYear);
        if (bIsNumber == false || nPeriodsYear < 1 || nPeriodsYear > 12)
        {
            entPeriodsYear.Text = "";
            entPeriodsYear.Focus();
            return;
        }

        // Close the keyboard.
        entPeriodsYear.IsEnabled = false;
        entPeriodsYear.IsEnabled = true;

        // Set decimal places for the Entry controls and values passed by reference.
        entInterestRate.Text = MainPage.RoundDoubleToNumDecimals(ref nInterestRate, nNumDec, "F");

        // Calculating the effective interest.
        double nInterestEffective;
        try
        {
            nInterestEffective = ((Math.Pow(1 + (nInterestRate / 100) / nPeriodsYear, nPeriodsYear) - 1) * 100);
        }
        catch (Exception ex)
        {
            DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
            return;
        }

        // Rounding result.
        txtInterestEffective.Text = MainPage.RoundDoubleToNumDecimals(ref nInterestEffective, nNumDec, "N");

        // Set focus.
        btnReset.Focus();
    }

    // Reset the entry fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entNumDec.Text = "2";
        entInterestRate.Text = "";
        entPeriodsYear.Text = "12";
        txtInterestEffective.Text = "";
        
        entNumDec.Focus();
    }
}