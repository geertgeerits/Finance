using Finance.Resources.Languages;

namespace Finance;

public partial class PageInterestPayDiscount : ContentPage
{
	public PageInterestPayDiscount()
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
        lblTitle.Text = FinLang.InterestPayDiscountForm_Text;

        lblExplanation.Text = FinLang.InterestPayDiscountExplanation_Text;
        lblNumDec.Text = FinLang.NumDec_Text;
        lblPaymentDiscount.Text = FinLang.PaymentDiscount_Text;
        lblExpiryDaysWithDiscount.Text = FinLang.ExpiryDaysWithDiscount_Text;
        lblExpiryDaysWithoutDiscount.Text = FinLang.ExpiryDaysWithoutDiscount_Text;
        lblInterestEffective.Text = FinLang.InterestEffective_Text;
        btnCalculate.Text = FinLang.Calculate_Text;
        btnReset.Text = FinLang.Reset_Text;

        // Set the type of keyboard.
        if (MainPage.cKeyboard == "Default")
        {
            entPaymentDiscount.Keyboard = Keyboard.Default;
        }
        else if (MainPage.cKeyboard == "Text")
        {
            entPaymentDiscount.Keyboard = Keyboard.Text;
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
        txtInterestEffective.Text = "";
    }

    // Go to the next field when the return key have been pressed.
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == entNumDec)
        {
            entPaymentDiscount.Focus();
        }
        else if (sender == entPaymentDiscount)
        {
            entExpiryDaysWithDiscount.Focus();
        }
        else if (sender == entExpiryDaysWithDiscount)
        {
            entExpiryDaysWithoutDiscount.Focus();
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

        entPaymentDiscount.Text = MainPage.ReplaceDecimalPointComma(entPaymentDiscount.Text);
        bIsNumber = decimal.TryParse(entPaymentDiscount.Text, out decimal nPaymentDiscount);
        if (bIsNumber == false || nPaymentDiscount < 0 || nPaymentDiscount > 100)
        {
            entPaymentDiscount.Text = "";
            entPaymentDiscount.Focus();
            return;
        }

        bIsNumber = int.TryParse(entExpiryDaysWithDiscount.Text, out int nExpiryDaysWithDiscount);
        if (bIsNumber == false || nExpiryDaysWithDiscount < 0 || nExpiryDaysWithDiscount > 999)
        {
            entExpiryDaysWithDiscount.Text = "";
            entExpiryDaysWithDiscount.Focus();
            return;
        }

        bIsNumber = int.TryParse(entExpiryDaysWithoutDiscount.Text, out int nExpiryDaysWithoutDiscount);
        if (bIsNumber == false || nExpiryDaysWithoutDiscount < nExpiryDaysWithDiscount || nExpiryDaysWithoutDiscount > 999)
        {
            entExpiryDaysWithoutDiscount.Text = "";
            entExpiryDaysWithoutDiscount.Placeholder = Convert.ToString(nExpiryDaysWithDiscount) + " - 999";
            entExpiryDaysWithoutDiscount.Focus();
            return;
        }

        // Close the keyboard.
        entExpiryDaysWithoutDiscount.IsEnabled = false;
        entExpiryDaysWithoutDiscount.IsEnabled = true;

        // Set decimal places for the Entry controls and values passed by reference.
        entPaymentDiscount.Text = MainPage.RoundDecimalToNumDecimals(ref nPaymentDiscount, nNumDec, "F");

        decimal nInterestEffective;

        if (nExpiryDaysWithoutDiscount - nExpiryDaysWithDiscount > 0)
        {
            try
            {
                nInterestEffective = nPaymentDiscount * 365 / (nExpiryDaysWithoutDiscount - nExpiryDaysWithDiscount);
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
        txtInterestEffective.Text = MainPage.RoundDecimalToNumDecimals(ref nInterestEffective, nNumDec, "N");

        // Set focus.
        btnReset.Focus();
    }

    // Reset the entry fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entNumDec.Text = "2";
        entPaymentDiscount.Text = "";
        entExpiryDaysWithDiscount.Text = "7";
        entExpiryDaysWithoutDiscount.Text = "30";
        txtInterestEffective.Text = "";

        entExpiryDaysWithoutDiscount.Placeholder = "0 - 999";

        entNumDec.Focus();
    }
}