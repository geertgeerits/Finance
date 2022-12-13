using Finance.Resources.Languages;

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
            DisplayAlert("InitializeComponent", ex.Message, MainPage.cButtonCloseText);
            return;
        }

        // Set the current UI culture of the selected language.
        MainPage.SetCultureSelectedLanguage();

        // Put text in the chosen language in the controls.
        lblTitle.Text = FinLang.InterestEffectiveBEForm_Text;

        lblExplanation.Text = FinLang.InterestEffectiveBEExplanation_Text;
        lblNumDec.Text = FinLang.NumDec_Text;
        lblCapitalInitial.Text = FinLang.CapitalInitial_Text;
        lblCapitalFinal.Text = FinLang.CapitalFinal_Text;
        lblDurationYears.Text = FinLang.DurationYears_Text;
        lblInterestEffective.Text = FinLang.InterestEffective_Text;
        btnCalculate.Text = FinLang.Calculate_Text;
        btnReset.Text = FinLang.Reset_Text;

        // Set the type of keyboard.
        if (MainPage.cKeyboard == "Default")
        {
            entCapitalInitial.Keyboard = Keyboard.Default;
            entCapitalFinal.Keyboard = Keyboard.Default;
        }
        else if (MainPage.cKeyboard == "Text")
        {
            entCapitalInitial.Keyboard = Keyboard.Text;
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
        txtInterestEffective.Text = "";
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
            entCapitalFinal.Focus();
        }
        else if (sender == entCapitalFinal)
        {
            entDurationYears.Focus();
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

        entCapitalFinal.Text = MainPage.ReplaceDecimalPointComma(entCapitalFinal.Text);
        bIsNumber = double.TryParse(entCapitalFinal.Text, out double nCapitalFinal);
        if (bIsNumber == false || nCapitalFinal < 0 || nCapitalFinal > 9999999999)
        {
            entCapitalFinal.Text = "";
            entCapitalFinal.Focus();
            return;
        }

        bIsNumber = double.TryParse(entDurationYears.Text, out double nDurationYears);
        if (bIsNumber == false || nDurationYears < 1 || nDurationYears > 100)
        {
            entDurationYears.Text = "";
            entDurationYears.Focus();
            return;
        }

        // Close the keyboard.
        entDurationYears.IsEnabled = false;
        entDurationYears.IsEnabled = true;

        // Set decimal places for the Entry controls and values passed by reference.
        entCapitalInitial.Text = MainPage.RoundDoubleToNumDecimals(ref nCapitalInitial, nNumDec, "F");
        entCapitalFinal.Text = MainPage.RoundDoubleToNumDecimals(ref nCapitalFinal, nNumDec, "F");

        // Calculating the effective interest.
        double nInterestEffective;

        if (nCapitalInitial > 0)
        {
            try
            {
                nInterestEffective = (Math.Pow((nCapitalFinal / nCapitalInitial), (1 / nDurationYears)) - 1) * 100;
            }
            catch (Exception ex)
            {
                DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
                return;
            }
        }
        else
        {
            nInterestEffective = 0;
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
        entCapitalInitial.Text = "";
        entCapitalFinal.Text = "";
        entDurationYears.Text = "";
        txtInterestEffective.Text = "";

        entNumDec.Focus();
    }
}
