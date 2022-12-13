using Finance.Resources.Languages;

namespace Finance;

public partial class PageAmountGrossOfNet : ContentPage
{
	public PageAmountGrossOfNet()
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
        lblTitle.Text = FinLang.AmountGrossOfNetForm_Text;

        lblExplanation.Text = FinLang.AmountGrossOfNetExplanation_Text;
        lblNumDec.Text = FinLang.NumDec_Text;
        lblPercentage.Text = FinLang.Percentage_Text;
        lblAmountNet.Text = FinLang.AmountNet_Text;
        lblAmountDifference.Text = FinLang.AmountDifference_Text;
        lblAmountGross.Text = FinLang.AmountGross_Text;
        btnCalculate.Text = FinLang.Calculate_Text;
        btnReset.Text = FinLang.Reset_Text;

        // Set the type of keyboard.
        if (MainPage.cKeyboard == "Default")
        {
            entPercentage.Keyboard = Keyboard.Default;
            entAmountNet.Keyboard = Keyboard.Default;
        }
        else if (MainPage.cKeyboard == "Text")
        {
            entPercentage.Keyboard = Keyboard.Text;
            entAmountNet.Keyboard = Keyboard.Text;
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
        txtAmountDifference.Text = "";
        txtAmountGross.Text = "";
    }

    // Go to the next field when the return key have been pressed.
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == entNumDec)
        {
            entPercentage.Focus();
        }
        else if (sender == entPercentage)
        {
            entAmountNet.Focus();
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

        entPercentage.Text = MainPage.ReplaceDecimalPointComma(entPercentage.Text);
        bIsNumber = decimal.TryParse(entPercentage.Text, out decimal nPercentage);
        if (bIsNumber == false || nPercentage < 0 || nPercentage > 100)
        {
            entPercentage.Text = "";
            entPercentage.Focus();
            return;
        }

        entAmountNet.Text = MainPage.ReplaceDecimalPointComma(entAmountNet.Text);
        bIsNumber = decimal.TryParse(entAmountNet.Text, out decimal nAmountNet);
        if (bIsNumber == false || nAmountNet < 0 || nAmountNet > 9999999999)
        {
            entAmountNet.Text = "";
            entAmountNet.Focus();
            return;
        }

        // Close the keyboard.
        entAmountNet.IsEnabled = false;
        entAmountNet.IsEnabled = true;

        // Set decimal places for the Entry controls and values passed by reference.
        entPercentage.Text = MainPage.RoundDecimalToNumDecimals(ref nPercentage, nNumDec, "F");
        entAmountNet.Text = MainPage.RoundDecimalToNumDecimals(ref nAmountNet, nNumDec, "F");

        // Calculate the net amount.
        if (nPercentage == 0 || nPercentage == 100)
        {
            entPercentage.Text = "";
            entPercentage.Focus();
            return;
        }
        else if (nAmountNet > 0)
        {
            try
            {
                decimal nAmountGross = nAmountNet / ((100 - nPercentage) / 100);
                txtAmountGross.Text = MainPage.RoundDecimalToNumDecimals(ref nAmountGross, nNumDec, "N");
                decimal nAmountDifference = nAmountGross - nAmountNet;
                txtAmountDifference.Text = MainPage.RoundDecimalToNumDecimals(ref nAmountDifference, nNumDec, "N");
            }
            catch (Exception ex)
            {
                DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
                return;
            }
        }
        else
        {
            return;
        }

        // Set focus.
        btnReset.Focus();
    }

    // Reset the entry fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entNumDec.Text = "2";
        entPercentage.Text = "";
        entAmountNet.Text = "";

        entNumDec.Focus();
    }
}