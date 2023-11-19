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
            DisplayAlert("InitializeComponent", ex.Message, "OK");
            return;
        }

        // Set the type of keyboard.
        if (Globals.cKeyboard == "Default")
        {
            entPercentage.Keyboard = Keyboard.Default;
            entAmountNet.Keyboard = Keyboard.Default;
        }
        else if (Globals.cKeyboard == "Text")
        {
            entPercentage.Keyboard = Keyboard.Text;
            entAmountNet.Keyboard = Keyboard.Text;
        }
    }

    // Set focus to the first entry field.
    // Add in the header of the xaml page: 'Loaded="OnPageLoaded"'
    private void OnPageLoaded(object sender, EventArgs e)
    {
        entPercentage.Focus();
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
        lblAmountDifference.Text = "";
        lblAmountGross.Text = "";
    }

    // Go to the next field when the return key have been pressed.
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == entPercentage)
        {
            entAmountNet.Focus();
        }
    }

    // Calculate the result.
    private void CalculateResult(object sender, EventArgs e)
    {
        // Validate input values.
        entPercentage.Text = Globals.ReplaceDecimalPointComma(entPercentage.Text);
        bool bIsNumber = decimal.TryParse(entPercentage.Text, out decimal nPercentage);
        if (bIsNumber == false || nPercentage < 0 || nPercentage > 100)
        {
            entPercentage.Text = "";
            entPercentage.Focus();
            return;
        }

        entAmountNet.Text = Globals.ReplaceDecimalPointComma(entAmountNet.Text);
        bIsNumber = decimal.TryParse(entAmountNet.Text, out decimal nAmountNet);
        if (bIsNumber == false || nAmountNet < 0 || nAmountNet > 9_999_999_999)
        {
            entAmountNet.Text = "";
            entAmountNet.Focus();
            return;
        }

        // Close the keyboard.
        entAmountNet.IsEnabled = false;
        entAmountNet.IsEnabled = true;

        // Convert string to int for number of decimal digits after decimal point.
        int nNumDec = int.Parse(Globals.cNumDecimalDigits);
        int nPercDec = int.Parse(Globals.cPercDecimalDigits);

        // Set decimal places for the Entry controls and values passed by reference.
        entPercentage.Text = Globals.RoundDecimalToNumDecimals(ref nPercentage, nPercDec, "F");
        entAmountNet.Text = Globals.RoundDecimalToNumDecimals(ref nAmountNet, nNumDec, "F");

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
                lblAmountGross.Text = Globals.RoundDecimalToNumDecimals(ref nAmountGross, nNumDec, "N");
                decimal nAmountDifference = nAmountGross - nAmountNet;
                lblAmountDifference.Text = Globals.RoundDecimalToNumDecimals(ref nAmountDifference, nNumDec, "N");
            }
            catch (Exception ex)
            {
                DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
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
        entPercentage.Text = "";
        entAmountNet.Text = "";

        entPercentage.Focus();
    }
}