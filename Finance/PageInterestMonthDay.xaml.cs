using Finance.Resources.Languages;

namespace Finance;

public partial class PageInterestMonthDay : ContentPage
{
	public PageInterestMonthDay()
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
        lblTitle.Text = FinLang.InterestMonthDayForm_Text;

        lblExplanation.Text = FinLang.InterestMonthDayExplanation_Text;
        lblNumDec.Text = FinLang.NumDec_Text;
        lblInterestRate.Text = FinLang.InterestRate_Text;
        lblInterestMonth.Text = FinLang.InterestMonth_Text;
        lblInterestDay365.Text = FinLang.InterestDay365_Text;
        lblInterestDay366.Text = FinLang.InterestDay366_Text;
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

    // Clear result fields.
    private void EntryTextChanged(object sender, EventArgs e)
    {
        txtInterestMonth.Text = "";
        txtInterestDay365.Text = "";
        txtInterestDay366.Text = "";
    }

    // Go to the next field when the return key have been pressed.
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == entNumDec)
        {
            entInterestRate.Focus();
        }
    }

    // Calculate the result.
    private void CalculateResult(object sender, EventArgs e)
    {
        // Validate input values.
        bool bIsNumber = int.TryParse(entNumDec.Text, out int nNumDec);
        if (bIsNumber == false || nNumDec < 0 || nNumDec > 8)
        {
            entNumDec.Text = "";
            entNumDec.Focus();
            return;
        }

        entInterestRate.Text = MainPage.ReplaceDecimalPointComma(entInterestRate.Text);
        bIsNumber = double.TryParse(entInterestRate.Text, out double nInterestRate);
        if (bIsNumber == false || nInterestRate < 0 || nInterestRate > 100)
        {
            entInterestRate.Text = "";
            entInterestRate.Focus();
            return;
        }

        // Close the keyboard.
        entInterestRate.IsEnabled = false;
        entInterestRate.IsEnabled = true;

        // Set decimal places for the Entry controls and values passed by reference.
        entInterestRate.Text = MainPage.RoundDoubleToNumDecimals(ref nInterestRate, nNumDec, "F");

        double nInterestMonth = 0;
        double nInterestDay365 = 0;
        double nInterestDay366 = 0;

        if (nInterestRate > 0)
        {
            try
            {
                nInterestMonth = (Math.Pow(1 + (nInterestRate / 100), (double)1 / 12) - 1) * 100;
                nInterestDay365 = (Math.Pow(1 + (nInterestRate / 100), (double)1 / 365) - 1) * 100;
                nInterestDay366 = (Math.Pow(1 + (nInterestRate / 100), (double)1 / 366) - 1) * 100;
            }
            catch (Exception ex)
            {
                DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
                return;
            }
        }

        // Rounding result.
        txtInterestMonth.Text = MainPage.RoundDoubleToNumDecimals(ref nInterestMonth, nNumDec, "N");
        txtInterestDay365.Text = MainPage.RoundDoubleToNumDecimals(ref nInterestDay365, nNumDec, "N");
        txtInterestDay366.Text = MainPage.RoundDoubleToNumDecimals(ref nInterestDay366, nNumDec, "N");

        // Set focus.
        btnReset.Focus();
    }

    // Reset the entry fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entNumDec.Text = "6";
        entInterestRate.Text = "";
        txtInterestMonth.Text = "";
        txtInterestDay365.Text = "";
        txtInterestDay366.Text = "";

        entNumDec.Focus();
    }
}