using Finance.Resources.Languages;

namespace Finance;

public partial class PageInvestmentReturn : ContentPage
{
	public PageInvestmentReturn()
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
        lblTitle.Text = FinLang.InvestmentReturnForm_Text;

        lblExplanation.Text = FinLang.InvestmentReturnExplanation_Text + "\n";  // BUG auto sizing label for small screens.  Add a new line to solve the bug.
        lblNumDec.Text = FinLang.NumDec_Text;
        lblAmountPurchase.Text = FinLang.AmountPurchase_Text;
        lblAmountCost.Text = FinLang.AmountCost_Text;
        lblAmountTotal.Text = FinLang.AmountTotal_Text;
        lblAmountRevenueYear.Text = FinLang.AmountRevenueYear_Text;
        lblPercentageReturnYear.Text = FinLang.PercentageReturnYear_Text;
        btnCalculate.Text = FinLang.Calculate_Text;
        btnReset.Text = FinLang.Reset_Text;

        // Set the type of keyboard.
        if (MainPage.cKeyboard == "Default")
        {
            entAmountPurchase.Keyboard = Keyboard.Default;
            entAmountCost.Keyboard = Keyboard.Default;
            entAmountRevenueYear.Keyboard = Keyboard.Default;
            entPercentageReturnYear.Keyboard = Keyboard.Default;
        }
        else if (MainPage.cKeyboard == "Text")
        {
            entAmountPurchase.Keyboard = Keyboard.Text;
            entAmountCost.Keyboard = Keyboard.Text;
            entAmountRevenueYear.Keyboard = Keyboard.Text;
            entPercentageReturnYear.Keyboard = Keyboard.Text;
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
        txtAmountTotal.Text = "";
    }

    // Go to the next field when the return key have been pressed.
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == entNumDec)
        {
            entAmountPurchase.Focus();
        }
        else if (sender == entAmountPurchase)
        {
            entAmountCost.Focus();
        }
        else if (sender == entAmountCost)
        {
            entAmountRevenueYear.Focus();
        }
        else if (sender == entAmountRevenueYear)
        {
            entPercentageReturnYear.Focus();
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

        entAmountPurchase.Text = MainPage.ReplaceDecimalPointComma(entAmountPurchase.Text);
        bIsNumber = decimal.TryParse(entAmountPurchase.Text, out decimal nAmountPurchase);
        if (bIsNumber == false || nAmountPurchase < 0 || nAmountPurchase > 9999999999)
        {
            entAmountPurchase.Text = "";
            entAmountPurchase.Focus();
            return;
        }

        entAmountCost.Text = MainPage.ReplaceDecimalPointComma(entAmountCost.Text);
        bIsNumber = decimal.TryParse(entAmountCost.Text, out decimal nAmountCost);
        if (bIsNumber == false || nAmountCost < 0 || nAmountCost > 9999999999)
        {
            entAmountCost.Text = "";
            entAmountCost.Focus();
            return;
        }

        entAmountRevenueYear.Text = MainPage.ReplaceDecimalPointComma(entAmountRevenueYear.Text);
        bIsNumber = decimal.TryParse(entAmountRevenueYear.Text, out decimal nAmountRevenueYear);
        if (bIsNumber == false || nAmountRevenueYear < 0 || nAmountRevenueYear > 9999999999)
        {
            entAmountRevenueYear.Text = "";
            entAmountRevenueYear.Focus();
            return;
        }

        entPercentageReturnYear.Text = MainPage.ReplaceDecimalPointComma(entPercentageReturnYear.Text);
        bIsNumber = decimal.TryParse(entPercentageReturnYear.Text, out decimal nPercentageReturnYear);
        if (bIsNumber == false || nPercentageReturnYear < 0 || nPercentageReturnYear > 9999)
        {
            entPercentageReturnYear.Text = "";
            entPercentageReturnYear.Focus();
            return;
        }

        // Close the keyboard.
        entAmountRevenueYear.IsEnabled = false;
        entAmountRevenueYear.IsEnabled = true;
        entPercentageReturnYear.IsEnabled = false;
        entPercentageReturnYear.IsEnabled = true;

        // Set decimal places for the Entry controls and values passed by reference.
        entAmountPurchase.Text = MainPage.RoundDecimalToNumDecimals(ref nAmountPurchase, nNumDec, "F");
        entAmountCost.Text = MainPage.RoundDecimalToNumDecimals(ref nAmountCost, nNumDec, "F");
        entAmountRevenueYear.Text = MainPage.RoundDecimalToNumDecimals(ref nAmountRevenueYear, nNumDec, "F");
        entPercentageReturnYear.Text = MainPage.RoundDecimalToNumDecimals(ref nPercentageReturnYear, nNumDec, "F");

        // Calculate the results.
        decimal nAmountTotal = nAmountPurchase + nAmountCost;

        if (nAmountTotal == 0)
        {
            txtAmountTotal.Text = MainPage.RoundDecimalToNumDecimals(ref nAmountTotal, nNumDec, "N");
        }

        if (nAmountRevenueYear == 0)
        {
            decimal nNumberTemp = 0;
            entPercentageReturnYear.Text = MainPage.RoundDecimalToNumDecimals(ref nNumberTemp, nNumDec, "F");
        }

        try
        {
            if (nAmountPurchase + nAmountCost > 0)
            {
                nPercentageReturnYear = nAmountRevenueYear / nAmountTotal * 100;
                entPercentageReturnYear.Text = MainPage.RoundDecimalToNumDecimals(ref nPercentageReturnYear, nNumDec, "F");
            }
            else if (nPercentageReturnYear > 0)
            {
                nAmountTotal = nAmountRevenueYear / nPercentageReturnYear * 100;
            }
            else
            {
                return;
            }

            txtAmountTotal.Text = MainPage.RoundDecimalToNumDecimals(ref nAmountTotal, nNumDec, "N");
        }
        catch (Exception ex)
        {
            DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
            return;
        }

        // Set focus.
        btnReset.Focus();
    }

    // Reset the entry fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entNumDec.Text = "2";
        entAmountPurchase.Text = "0";
        entAmountCost.Text = "0";
        txtAmountTotal.Text = "0";
        entAmountRevenueYear.Text = "0";
        entPercentageReturnYear.Text = "0";

        entNumDec.Focus();
    }
}