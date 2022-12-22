using Finance.Resources.Languages;
namespace Finance;

public partial class PageLoanDetail : ContentPage
{
    // Variables for loan detail.
    private readonly string[] aColHeader = new string[7];
    private readonly string[,] aLoanDetail = new string[1201, 7];

    // Variables for export / e-mail.
    private readonly string cDocumentName;
    private readonly string cDocTitleAnnuity;
    private readonly string cDocTitleLinear;
    private string cExportType;
    private readonly string cFileOpenQuestion;
    //private readonly string cEmailAddressQuestion;
    //private readonly string cEmailBody;
    //private readonly string cEmailSendQuestion;
    //private readonly string cEmailSubject;
    //private readonly string cEmailTitle;
    //private readonly string cEmailMessage;
    private readonly string cShareQuestion;
    private readonly string cYes;
    private readonly string cNo;
    private bool bReCalculateResult;

    public PageLoanDetail()
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
        lblTitle.Text = FinLang.LoanDetailForm_Text;

        lblExplanation.Text = FinLang.LoanDetailExplanation_Text;
        lblNumDec.Text = FinLang.NumDec_Text;
        lblInterestRate.Text = FinLang.InterestRate_Text;
        lblCapitalInitial.Text = FinLang.CapitalInitial_Text;
        lblDurationYears.Text = FinLang.DurationYears_Text;
        lblPeriodsYear.Text = FinLang.PeriodsYear_Text;
        lblAmountPeriod.Text = FinLang.AmountPeriod_Text;
        lblInterestTotal.Text = FinLang.InterestTotal_Text;
        lblCapitalInterest.Text = FinLang.CapitalInterest_Text;
        rbnLoanAnnuity.Content = FinLang.LoanAnnuity_Text;
        rbnLoanLinear.Content = FinLang.LoanLinear_Text;
        lblExpirationDate.Text = FinLang.DateFirstRepayment_Text;
        lblDayEndMonth.Text = FinLang.DayEndMonth_Text;
        lblCurrencyCode.Text = FinLang.CurrencyCode_Text;

        btnCalculate.Text = FinLang.Calculate_Text;
        btnReset.Text = FinLang.Reset_Text;
        //btnPrint.Text = FinLang.Print_Text;
        //btnExport.Text = FinLang.ExportFile_Text + " ->";
        btnExport.Text = FinLang.ButtonShare_Text + " ->";

        aColHeader[0] = FinLang.LoanDetailColumns_0_Text;
        aColHeader[1] = FinLang.LoanDetailColumns_1_Text;
        aColHeader[2] = FinLang.LoanDetailColumns_2_Text;
        aColHeader[3] = FinLang.LoanDetailColumns_3_Text;
        aColHeader[4] = FinLang.LoanDetailColumns_4_Text;
        aColHeader[5] = FinLang.LoanDetailColumns_5_Text;
        aColHeader[6] = FinLang.LoanDetailColumns_6_Text;

        cDocumentName = FinLang.LoanDetailDocumentName_Text;
        cDocTitleAnnuity = FinLang.LoanDetailDocTitleAnnuity_Text;
        cDocTitleLinear = FinLang.LoanDetailDocTitleLinear_Text;

        cFileOpenQuestion = FinLang.FileOpenQuestion_Text;
        //cEmailAddressQuestion = FinLang.EmailAddressQuestion_Text;
        //cEmailBody = FinLang.EmailBody_Text;
        //cEmailSendQuestion = FinLang.EmailSendQuestion_Text;
        //cEmailSubject = FinLang.EmailSubject_Text;
        //cEmailTitle = FinLang.EmailTitle_Text;
        //cEmailMessage = FinLang.EmailMessage_Text;
        cShareQuestion = FinLang.ShareQuestion_Text;
        cYes = FinLang.Yes_Text;
        cNo = FinLang.No_Text;

        // Set the type of keyboard.
        if (MainPage.cKeyboard == "Default")
        {
            entInterestRate.Keyboard = Keyboard.Default;
            entCapitalInitial.Keyboard = Keyboard.Default;
        }
        else if (MainPage.cKeyboard == "Text")
        {
            entInterestRate.Keyboard = Keyboard.Text;
            entCapitalInitial.Keyboard = Keyboard.Text;
        }

        // Set the current date format and date for the DatePicker.
        dtpExpirationDate.Format = MainPage.cDateFormat;

        // Set the currency code.
        entCurrencyCode.Text = MainPage.cISOCurrencyCode;

        // Set the default export format.
        pickerExportType.SelectedIndex = 1;

        // Test variable to recalculate the loan.
        bReCalculateResult = true;

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
        txtAmountPeriod.Text = "";
        txtInterestTotal.Text = "";
        txtCapitalInterest.Text = "";

        bReCalculateResult = true;
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
            entCapitalInitial.Focus();
        }
        else if (sender == entCapitalInitial)
        {
            entDurationYears.Focus();
        }
        else if (sender == entDurationYears)
        {
            entPeriodsYear.Focus();
        }
        else if (sender == entCurrencyCode)
        {
            // Close the keyboard.
            entCurrencyCode.IsEnabled = false;
            entCurrencyCode.IsEnabled = true;

            btnExport.Focus();
        }
    }

    // Calculate the result with detail per period.
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

        entInterestRate.Text = MainPage.ReplaceDecimalPointComma(entInterestRate.Text);
        bIsNumber = double.TryParse(entInterestRate.Text, out double nInterestRate);
        if (bIsNumber == false || nInterestRate < 0 || nInterestRate > 100)
        {
            entInterestRate.Text = "";
            entInterestRate.Focus();
            return;
        }

        entCapitalInitial.Text = MainPage.ReplaceDecimalPointComma(entCapitalInitial.Text);
        bIsNumber = double.TryParse(entCapitalInitial.Text, out double nCapitalInitial);
        if (bIsNumber == false || nCapitalInitial < 1 || nCapitalInitial > 9999999999)
        {
            entCapitalInitial.Text = "";
            entCapitalInitial.Focus();
            return;
        }

        bIsNumber = int.TryParse(entDurationYears.Text, out int nDurationYears);
        if (bIsNumber == false || nDurationYears < 1 || nDurationYears > 100)
        {
            entDurationYears.Text = "";
            entDurationYears.Focus();
            return;
        }

        bIsNumber = int.TryParse(entPeriodsYear.Text, out int nPeriodsYear);
        if (bIsNumber == false || nPeriodsYear < 0 || nPeriodsYear > 12)
        {
            entPeriodsYear.Text = "";
            entPeriodsYear.Focus();
            return;
        }

        if ((12 % nPeriodsYear) != 0)
        {
            entPeriodsYear.Text = "";
            entPeriodsYear.Focus();
            return;
        }

        // Close the keyboard.
        entPeriodsYear.IsEnabled = false;
        entPeriodsYear.IsEnabled = true;

        // Set decimal places for the entry controls and values passed by reference.
        entInterestRate.Text = MainPage.RoundDoubleToNumDecimals(ref nInterestRate, nNumDec, "F");
        entCapitalInitial.Text = MainPage.RoundDoubleToNumDecimals(ref nCapitalInitial, nNumDec, "F");

        // Clear result fields.
        txtAmountPeriod.Text = "";
        txtInterestTotal.Text = "";
        txtCapitalInterest.Text = "";

        // Clear the array.
        Array.Clear(aLoanDetail);

        // Setup variables.
        double nCapitalRemainder;
        double nCapitalPeriod = 0;
        double nInterestPeriod = 0;
        double nInterestTotal = 0;
        double nInterestRatePeriod;
        double nPaymentPeriod = 0;
        int nRow;

        // Set up the loan calculations.
        DateTime dExpirationDate = dtpExpirationDate.Date;              // Expiration date

        int nNumberMonthsAdd = 12 / nPeriodsYear;                       // Number of months to add
        int nNumberMonthsAddCumul = nNumberMonthsAdd;                   // Cumul of number of months to add
        int nNumberPeriods = nDurationYears * nPeriodsYear;             // Number of periods

        // Calculate the interest per month.
        try
        {
            nInterestRatePeriod = Math.Pow(1 + (nInterestRate / 100), (double)1 / nPeriodsYear) - 1;  // Interest rate per period
        }
        catch (Exception ex)
        {
            DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
            return;
        }

        // Rounding to 6 digits after decimal point
        if (MainPage.cRoundNumber == "AwayFromZero")
        {
            nInterestRatePeriod = Math.Round(nInterestRatePeriod, 6, MidpointRounding.AwayFromZero);
        }
        else if (MainPage.cRoundNumber == "ToEven")
        {
            nInterestRatePeriod = Math.Round(nInterestRatePeriod, 6, MidpointRounding.ToEven);  // Rounding to 6 digits after decimal point
        }

        //DisplayAlert("nInterestRatePeriod", nInterestRatePeriod.ToString(), MainPage.cButtonCloseText);  // For testing

        // Calculate annuity loan per period.
        if (rbnLoanAnnuity.IsChecked)
        {
            nInterestTotal += nInterestPeriod;                          // Interest total

            if (nInterestRate > 0)
            {
                nPaymentPeriod = (nCapitalInitial * nInterestRatePeriod) / (1 - Math.Pow(1 + nInterestRatePeriod, -nNumberPeriods));  // Payment per period
            }
            else
            {
                nPaymentPeriod = nCapitalInitial / nNumberPeriods;
            }

            nCapitalRemainder = nCapitalInitial;                        // Remainder of capital
            txtAmountPeriod.Text = MainPage.RoundDoubleToNumDecimals(ref nPaymentPeriod, nNumDec, "N");
        }

        // Calculate linear loan per period.
        else
        {                      
            // Amount of capital per period
            if (MainPage.cRoundNumber == "AwayFromZero")
            {
                nCapitalPeriod = Math.Round(nCapitalInitial / nNumberPeriods, nNumDec, MidpointRounding.AwayFromZero);
            }
            else if (MainPage.cRoundNumber == "ToEven")
            {
                nCapitalPeriod = Math.Round(nCapitalInitial / nNumberPeriods, nNumDec, MidpointRounding.ToEven);
            }

            nCapitalRemainder = nCapitalInitial;                        // Remainder of capital
        }

        // Add the data in elements of the array.
        for (nRow = 0; nRow < nNumberPeriods; nRow++)
        {
            // Calculate loan annuity per period.
            if (rbnLoanAnnuity.IsChecked)
            {
                if (MainPage.cRoundNumber == "AwayFromZero")
                {
                    nInterestPeriod = Math.Round(nCapitalRemainder * nInterestRatePeriod, nNumDec, MidpointRounding.AwayFromZero);
                }
                else if (MainPage.cRoundNumber == "ToEven")
                {
                    nInterestPeriod = Math.Round(nCapitalRemainder * nInterestRatePeriod, nNumDec, MidpointRounding.ToEven);
                }

                nInterestTotal += nInterestPeriod;
                nCapitalPeriod = nPaymentPeriod - nInterestPeriod;
                nCapitalRemainder -= nCapitalPeriod;
            }
            // Calculate loan linear per period.
            else
            {
                if (MainPage.cRoundNumber == "AwayFromZero")
                {
                    nInterestPeriod = Math.Round(nCapitalRemainder * nInterestRatePeriod, nNumDec, MidpointRounding.AwayFromZero);
                }
                else if (MainPage.cRoundNumber == "ToEven")
                {
                    nInterestPeriod = Math.Round(nCapitalRemainder * nInterestRatePeriod, nNumDec, MidpointRounding.ToEven);
                }

                nPaymentPeriod = nCapitalPeriod + nInterestPeriod;
                nInterestTotal += nInterestPeriod;
                nCapitalRemainder -= nCapitalPeriod;
            }

            // Correction rounding differences interest and capital last period.
            if (nRow == nNumberPeriods - 1)
            {
                // Correction rounding differences interest last period.
                if (nInterestPeriod < 0)
                {
                    nInterestPeriod = 0;
                }

                // Correction rounding differences capital last period.
                //DisplayAlert("nCapitalRemainder", Convert.ToString(nCapitalRemainder), "OK");  //For testing
                nCapitalPeriod += nCapitalRemainder;
                nPaymentPeriod += nCapitalRemainder;
                nCapitalRemainder = 0;
            }

            // If selected set last day of month.
            if (ckbDayEndMonth.IsChecked)
            {
                DateTime dDateLastDayMonth = new(dExpirationDate.Year, dExpirationDate.Month, DateTime.DaysInMonth(dExpirationDate.Year, dExpirationDate.Month));
                dExpirationDate = dDateLastDayMonth;
            }

            // Fill the array with data.
            aLoanDetail[nRow, 0] = Convert.ToString(nRow + 1);
            aLoanDetail[nRow, 1] = dExpirationDate.ToString(MainPage.cDateFormat);
            aLoanDetail[nRow, 2] = MainPage.RoundDoubleToNumDecimals(ref nPaymentPeriod, nNumDec, "N");
            aLoanDetail[nRow, 3] = MainPage.RoundDoubleToNumDecimals(ref nCapitalPeriod, nNumDec, "N");
            aLoanDetail[nRow, 4] = MainPage.RoundDoubleToNumDecimals(ref nInterestPeriod, nNumDec, "N");
            aLoanDetail[nRow, 5] = MainPage.RoundDoubleToNumDecimals(ref nInterestTotal, nNumDec, "N");
            aLoanDetail[nRow, 6] = MainPage.RoundDoubleToNumDecimals(ref nCapitalRemainder, nNumDec, "N");

            dExpirationDate = dtpExpirationDate.Date.AddMonths(nNumberMonthsAddCumul);
            nNumberMonthsAddCumul += nNumberMonthsAdd;

            //DisplayAlert("Row number", nRow.ToString(), "OK");  // For testing
        }
        //DisplayAlert("Row number", nRow.ToString(), "OK");  // For testing
        
        // Rounding and formatting result.
        txtInterestTotal.Text = MainPage.RoundDoubleToNumDecimals(ref nInterestTotal, nNumDec, "N");
        double nCapitalInterest = nCapitalInitial + nInterestTotal;
        txtCapitalInterest.Text = MainPage.RoundDoubleToNumDecimals(ref nCapitalInterest, nNumDec, "N");

        // Store totals in row of array aLoanDetail (ex. nNumberPeriods = 12 -> nRow in for loop = 0-11 -> nRow after for loop = 12).
        aLoanDetail[nRow, 2] = txtCapitalInterest.Text;
        aLoanDetail[nRow, 3] = MainPage.RoundDoubleToNumDecimals(ref nCapitalInitial, nNumDec, "N");
        aLoanDetail[nRow, 4] = txtInterestTotal.Text;

        // Test variable te recalculate the loan.
        bReCalculateResult = false;

        // Set focus.
        btnReset.Focus();
    }

    // Export loan with detail per period.
    private async void ExportDetailLoan(object sender, EventArgs e)
    {
        // Recalculate the loan if needed.
        if (bReCalculateResult)
        {
            CalculateResult(sender, e);
        }

        // Number of periods.
        bool bIsNumber = int.TryParse(entDurationYears.Text, out int nDurationYears);
        if (bIsNumber == false)
        {
            return;
        }

        bIsNumber = int.TryParse(entPeriodsYear.Text, out int nPeriodsYear);
        if (bIsNumber == false)
        {
            return;
        }

        int nNumberPeriods = nDurationYears * nPeriodsYear;

        // Currency.
        string cCurrency = entCurrencyCode.Text.Trim();
        
        if (cCurrency.Length < 3)
        {
            cCurrency = MainPage.cISOCurrencyCode;
            entCurrencyCode.Text = cCurrency;
        }

        cCurrency = cCurrency.ToUpper();

        // Document title: loan annuity or linear.
        string cDocTitle;

        if (rbnLoanAnnuity.IsChecked)
        {
            cDocTitle = cDocTitleAnnuity;
        }
        else
        {
            cDocTitle = cDocTitleLinear;
        }

        // File name.
        //string cFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), cDocumentName);
        //string cFileName = Path.Combine(FileSystem.AppDataDirectory, cDocumentName);
        string cFileName = Path.Combine(FileSystem.CacheDirectory, cDocumentName);

        // Export.
        activityIndicator.IsRunning = true;

        if (cExportType == "CSV ;")
        {
            cFileName += ".csv";
            ExportDetailLoanCSV(nNumberPeriods, cCurrency, cDocTitle, cFileName);
        }
        else if (cExportType == "HTML")
        {
            cFileName += ".html";
            ExportDetailLoanHTML(nNumberPeriods, cCurrency, cDocTitle, cFileName);
        }
        else if (cExportType == "PDF")
        {
            cFileName += ".pdf";
            ExportDetailLoanPDF(nNumberPeriods, cCurrency, cDocTitle, cFileName);
        }

        // !!! BUG on iOS and Mac !!!
        // SendEmail() does not work.
        // OpenDocumentFile() and OpenShareInterface(): only one of the two is working (not both).

#if ANDROID || WINDOWS
        // Open the file.
        await OpenDocumentFile(cFileName);

        // Send file as attachment to e-mail address (not used anymore !!!).
        //await SendEmail(cFileName);
#endif
        // Open the share interface to share the document file+.
        await OpenShareInterface(cFileName);

        activityIndicator.IsRunning = false;
    }

    // Export loan with detail per period as a CSV file with a semicolon delimiter.
    private void ExportDetailLoanCSV(int nNumberPeriods, string cCurrency, string cDocTitle, string cFileName)
    {
        int nRow;

        try
        {
            using StreamWriter sw = new(cFileName, false);
            
            // Current date.
            sw.WriteLine(DateTime.Today.ToString(MainPage.cDateFormat));

            // Page number.
            //sw.WriteLine(nPagNo.ToString());

            sw.WriteLine(cDocTitle);
            sw.WriteLine("");

            // Data loan.
            sw.WriteLine(lblInterestRate.Text + " " + entInterestRate.Text + " %");
            double nCapitalInitial = Convert.ToDouble(entCapitalInitial.Text);
            sw.WriteLine(lblCapitalInitial.Text + " " + MainPage.RoundDoubleToNumDecimals(ref nCapitalInitial, Convert.ToInt32(entNumDec.Text), "N") + " " + cCurrency);
            sw.WriteLine(lblDurationYears.Text + " " + entDurationYears.Text);
            sw.WriteLine(lblPeriodsYear.Text + " " + entPeriodsYear.Text);
            sw.WriteLine("");

            // Detail per period.
            sw.WriteLine(aColHeader[0] + ";" + aColHeader[1] + ";" + aColHeader[2] + ";" + aColHeader[3] + ";" + aColHeader[4] + ";" + aColHeader[5] + ";" + aColHeader[6]);

            for (nRow = 0; nRow < nNumberPeriods; nRow++)
            {
                sw.WriteLine(aLoanDetail[nRow, 0] + ";" + aLoanDetail[nRow, 1] + ";" + aLoanDetail[nRow, 2] + ";" + aLoanDetail[nRow, 3] + ";" + aLoanDetail[nRow, 4] + ";" + aLoanDetail[nRow, 5] + ";" + aLoanDetail[nRow, 6]);
            }

            // Totals.
            sw.WriteLine("");
            sw.WriteLine(" ; ;" + aLoanDetail[nRow, 2] + ";" + aLoanDetail[nRow, 3] + ";" + aLoanDetail[nRow, 4]);

            // Close the StreamWriter object.
            sw.Close();
        }
        catch (Exception ex)
        {
            DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
            return;
        }
    }

    // Export loan with detail per period as a HTML file.
    private void ExportDetailLoanHTML(int nNumberPeriods, string cCurrency, string cDocTitle, string cFileName)
    {
        int nRow;

        try
        {
            using StreamWriter sw = new(cFileName, false);
            
            sw.WriteLine("<!DOCTYPE html>");
            sw.WriteLine("<html>");

            sw.WriteLine("<head>");
            sw.WriteLine("<meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\"/>");
            sw.WriteLine("<title>Finance</title>");
            sw.WriteLine("<meta name=\"author\" content=\"Geert Geerits\"/>");
            sw.WriteLine("<meta name=\"created\" content=\"2022-06-06T11:18:37.52\"/>");
            sw.WriteLine("<meta name=\"changedby\" content=\"Geert Geerits\"/>");
            sw.WriteLine("<meta name=\"changed\" content=\"2022-06-09T16:11:00.00\"/>");
            sw.WriteLine("</head>");

            sw.WriteLine("<body>");

            // Current date.
            sw.WriteLine(DateTime.Today.ToString(MainPage.cDateFormat) + "<br>");

            // Page number.
            //sw.WriteLine(nPagNo.ToString() + "<br>");

            sw.WriteLine("<p align=\"center\"><font size=\"4\"><b>" + cDocTitle + "</b></font></p>");
            sw.WriteLine("<hr>");

            // Data loan.
            sw.WriteLine("<p>" + lblInterestRate.Text + " " + entInterestRate.Text + " %" + "<br>");
            double nCapitalInitial = Convert.ToDouble(entCapitalInitial.Text);
            sw.WriteLine(lblCapitalInitial.Text + " " + MainPage.RoundDoubleToNumDecimals(ref nCapitalInitial, Convert.ToInt32(entNumDec.Text), "N") + " " + cCurrency + "<br>");
            sw.WriteLine(lblDurationYears.Text + " " + entDurationYears.Text + "<br>");
            sw.WriteLine(lblPeriodsYear.Text + " " + entPeriodsYear.Text + "</p>");

            // Detail per period.
            sw.WriteLine("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"0\">");
            sw.WriteLine("<col width=\"37*\"/>");
            sw.WriteLine("<col width=\"37*\"/>");
            sw.WriteLine("<col width=\"37*\"/>");
            sw.WriteLine("<col width=\"37*\"/>");
            sw.WriteLine("<col width=\"37*\"/>");
            sw.WriteLine("<col width=\"37*\"/>");
            sw.WriteLine("<col width=\"37*\"/>");

            // Column headings.
            sw.WriteLine("<tr valign=\"top\">");
            sw.WriteLine("<td width=\"6%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0.05cm; padding-bottom: 0.05cm; padding-left: 0.05cm; padding-right: 0cm\"><p align=\"right\">" + aColHeader[0] + "</p></td>");
            sw.WriteLine("<td width=\"12%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">" + aColHeader[1] + "</p></td>");
            sw.WriteLine("<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">" + aColHeader[2] + "</p></td>");
            sw.WriteLine("<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">" + aColHeader[3] + "</p></td>");
            sw.WriteLine("<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">" + aColHeader[4] + "</p></td>");
            sw.WriteLine("<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">" + aColHeader[5] + "</p></td>");
            sw.WriteLine("<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: 1px solid #000000; padding-top: 0.05cm; padding-bottom: 0.05cm; padding-left: 0cm; padding-right: 0.05cm\"><p align=\"right\">" + aColHeader[6] + "</p></td>");
            sw.WriteLine("</tr>");

            // Detail per period.
            for (nRow = 0; nRow < nNumberPeriods; nRow++)
            {
                sw.WriteLine("<tr valign=\"top\">");
                sw.WriteLine("<td width=\"6%\" style=\"border-top: none; border-bottom: none; border-left: 1px solid #000000; border-right: none; padding-top: 0cm; padding-bottom: 0cm; padding-left: 0.05cm; padding-right: 0cm\"><p align=\"right\">" + aLoanDetail[nRow, 0] + "</p></td>");
                sw.WriteLine("<td width=\"12%\" style=\border: none; padding: 0cm\"><p align=\"right\">" + aLoanDetail[nRow, 1] + "</p></td>");
                sw.WriteLine("<td width=\"16%\" style=\border: none; padding: 0cm\"><p align=\"right\">" + aLoanDetail[nRow, 2] + "</p></td>");
                sw.WriteLine("<td width=\"16%\" style=\border: none; padding: 0cm\"><p align=\"right\">" + aLoanDetail[nRow, 3] + "</p></td>");
                sw.WriteLine("<td width=\"16%\" style=\border: none; padding: 0cm\"><p align=\"right\">" + aLoanDetail[nRow, 4] + "</p></td>");
                sw.WriteLine("<td width=\"16%\" style=\border: none; padding: 0cm\"><p align=\"right\">" + aLoanDetail[nRow, 5] + "</p></td>");
                sw.WriteLine("<td width=\"16%\" style=\"border-top: none; border-bottom: none; border-left: none; border-right: 1px solid #000000; padding-top: 0cm; padding-bottom: 0cm; padding-left: 0cm; padding-right: 0.05cm\"><p align=\"right\">" + aLoanDetail[nRow, 6] + "</p></td>");
                sw.WriteLine("</tr>");
            }

            // Totals.
            sw.WriteLine("<tr valign=\"top\">");
            sw.WriteLine("<td width=\"6%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: none; padding-top: 0.05cm; padding-bottom: 0.05cm; padding-left: 0.05cm; padding-right: 0cm\"><p align=\"right\"></p></td>");
            sw.WriteLine("<td width=\"12%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\"></p></td>");
            sw.WriteLine("<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">" + aLoanDetail[nRow, 2] + "</p></td>");
            sw.WriteLine("<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">" + aLoanDetail[nRow, 3] + "</p></td>");
            sw.WriteLine("<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\">" + aLoanDetail[nRow, 4] + "</p></td>");
            sw.WriteLine("<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: none; padding: 0.05cm 0cm\"><p align=\"right\"></p></td>");
            sw.WriteLine("<td width=\"16%\" style=\"border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: none; border-right: 1px solid #000000; padding-top: 0.05cm; padding-bottom: 0.05cm; padding-left: 0cm; padding-right: 0.05cm\"><p align=\"right\"></p></td>");
            sw.WriteLine("</tr>");

            sw.WriteLine("</table>");
            sw.WriteLine("</body>");
            sw.WriteLine("</html>");

            // Close the StreamWriter object.
            sw.Close();
        }
        catch (Exception ex)
        {
            DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
            return;
        }
    }

    // Export loan with detail per period as a PDF file.
    private void ExportDetailLoanPDF(int nNumberPeriods, string cCurrency, string cDocTitle, string cFileName)
    {
        // Counters.     
        int nRow;
        int nPagNo;
        int nRowStart = 0;
        int nRowsPage;
        int nPosFirstRow;
        int nNumObjects;
        string cNoObject;
        string cNoContents;

        // Others.
        string cPageSize;
        bool bPageLandscape;
        string cFontSize;
        string[] cTextLines = new string[2000];
        int nTextLine;

        // Begin and end location of the horizontal line in points.
        string cHorPointsBeg;
        string cHorPointsEnd;

        // Begin and end location of the document in characters (Font: Courier ; Fontsize: 12).
        int nTabBeg12;
        int nTabEnd12;

        // Begin and end location of the document in characters (Font: Courier ; Fontsize: 10).
        int nTabBeg10;
        int nTabEnd10;

        // Begin location of the document and columns in characters (Font: Courier ; Fontsize: 9 and 8).
        int nTabBeg;
        int nTab0;
        int nTab1;
        int nTab2;
        int nTab3;
        int nTab4;
        int nTab5;
        int nTab6;

        // Portrait.
        if (entCapitalInitial.Text.Length < 14)
        {
            bPageLandscape = false;
            nRowsPage = 36;
        }
        // Landscape.
        else
        {
            bPageLandscape = true;
            nRowsPage = 24;
        }

        // Number of pages.
        string cPageObj = "[3 0 R";
        int nNumPages = 1;
        long[] aPosObject = new long[150];

        if (nNumberPeriods > nRowsPage)
        {
            if (nNumberPeriods % nRowsPage == 0)
            {
                nNumPages = nNumberPeriods / nRowsPage;
            }
            else
            {
                nNumPages = nNumberPeriods / nRowsPage + 1;
            }
        }

        // Set the page objects.
        if (nNumPages > 1)
        {
            int nPage;
            int nPageObj = 7;
            for (nPage = 2; nPage < nNumPages + 1; nPage++)
            {
                cPageObj = cPageObj + " " + Convert.ToString(nPageObj) + " 0 R";
                nPageObj += 2;
            }
        }
        
        cPageObj = cPageObj + "] /Count " + Convert.ToString(nNumPages);
        //DisplayAlert("cPageObj", cPageObj, "OK");

        // Format settings portrait.
        if (bPageLandscape == false)
        {
            // Paper size in PostScript points A4: 595 x 842 ; Letter: 612 x 792.
            if (MainPage.cPageFormat == "A4")
            {
                cPageSize = "0 0 595 842";
                nPosFirstRow = 782;
            }
            else
            {
                cPageSize = "0 0 612 792";
                nPosFirstRow = 732;
            }

            // Begin and end location of a horizontal line in points.
            cHorPointsBeg = "20";
            cHorPointsEnd = "575";

            // Begin and end location of the document in characters (Font: Courier ; Fontsize: 12).
            nTabBeg12 = 4;
            nTabEnd12 = 81;

            // Begin and end location of the document in characters (Font: Courier ; Fontsize: 10).
            nTabBeg10 = 4;
            nTabEnd10 = 96;

            // Fontsize (Font: Courier ; Fontsize: 8).
            cFontSize = "8";
            
            // Begin and end location of the document in characters.
            nTabBeg = 5;    // 20;

            // End location of the columns on the document in characters.
            nTab0 = 12;
            nTab1 = 26;
            nTab2 = 44;
            nTab3 = 63;
            nTab4 = 82;
            nTab5 = 101;
            nTab6 = 120;

            // End location of the column on the document (Font: Courier ; Fontsize: 9).
            //int nTab0 = 10;     // 68;
            //int nTab1 = 22;     // 135;
            //int nTab2 = 39;     // 225;
            //int nTab3 = 56;     // 315;
            //int nTab4 = 73;     // 405;
            //int nTab5 = 90;     // 495;
            //int nTab6 = 107;    // 585;
        }
        // Format settings Landscape.
        else
        {
            // Paper size in PostScript points A4: 842 x 595 ; Letter: 792 x 612.
            if (MainPage.cPageFormat == "A4")
            {
                cPageSize = "0 0 842 595";
                nPosFirstRow = 555;
            }
            else
            {
                cPageSize = "0 0 792 612";
                nPosFirstRow = 572;
            }

            // Begin and end location of a horizontal line in points.
            cHorPointsBeg = "30";
            cHorPointsEnd = "766";

            // Begin and end location of the document in characters (Font: Courier ; Fontsize: 12).
            nTabBeg12 = 10;
            nTabEnd12 = 112;

            // Begin and end location of the document in characters (Font: Courier ; Fontsize: 10).
            nTabBeg10 = 10;
            nTabEnd10 = 132;

            // Fontsize (Font: Courier ; Fontsize: 9).
            cFontSize = "9";

            // Begin and end location of the document in characters.
            nTabBeg = 10;
            //nTabEnd = 146;

            // End location of the columns on the document in characters.
            nTab0 = 17;
            nTab1 = 31;
            nTab2 = 54;
            nTab3 = 77;
            nTab4 = 100;
            nTab5 = 123;
            nTab6 = 146;
        }

        int nHeight = nPosFirstRow;

        // Variables.
        string cText;
        string cColText;
        string cColTextPart2;

        int nLeftPadding = (nTabEnd12 - nTabBeg12 + 1 - cDocTitle.Length) / 2;
        cDocTitle = new string(' ', nLeftPadding) + cDocTitle;
        cDocTitle = ReplaceCharacters(cDocTitle);

        // Column headings.
        string cColText0Part2 = "";
        string cColText1Part2 = "";
        string cColText2Part2 = "";
        string cColText3Part2 = "";
        string cColText4Part2 = "";
        string cColText5Part2 = "";
        string cColText6Part2 = "";

        string cColText0 = SplitStringInTwo(aColHeader[0], ref cColText0Part2, nTab0 - nTabBeg);
        string cColText1 = SplitStringInTwo(aColHeader[1], ref cColText1Part2, nTab1 - nTab0);
        string cColText2 = SplitStringInTwo(aColHeader[2], ref cColText2Part2, nTab2 - nTab1);
        string cColText3 = SplitStringInTwo(aColHeader[3], ref cColText3Part2, nTab3 - nTab2);
        string cColText4 = SplitStringInTwo(aColHeader[4], ref cColText4Part2, nTab4 - nTab3);
        string cColText5 = SplitStringInTwo(aColHeader[5], ref cColText5Part2, nTab5 - nTab4);
        string cColText6 = SplitStringInTwo(aColHeader[6], ref cColText6Part2, nTab6 - nTab5);

        cColText = cColText0 + cColText1 + cColText2 + cColText3 + cColText4 + cColText5 + cColText6;
        cColText = ReplaceCharacters(cColText);
        cColTextPart2 = cColText0Part2 + cColText1Part2 + cColText2Part2 + cColText3Part2 + cColText4Part2 + cColText5Part2 + cColText6Part2;
        cColTextPart2 = ReplaceCharacters(cColTextPart2);

        // Decoding filter: /FlateDecode ; /ASCII85Decode ; [/ASCII85Decode /FlateDecode] ; /ASCIIHexDecode ; /LZWDecode
        string cFilter = "";
        //string cFilter = " /Filter /FlateDecode";

        // Make PDF file.
        try
        {
            // Start writing to PDF array.
            nTextLine = 1;

            cText = "%PDF-1.7";
            cTextLines[nTextLine] = cText;
            nTextLine++;

            cText = "1 0 obj <</Type /Catalog /Pages 2 0 R>>";
            cTextLines[nTextLine] = cText;
            nTextLine++;
            cTextLines[nTextLine] = "endobj";
            nTextLine++;

            cText = "2 0 obj <</Type /Pages /Kids " + cPageObj + ">>";
            cTextLines[nTextLine] = cText;
            nTextLine++;
            cTextLines[nTextLine] = "endobj";
            nTextLine++;

            cText = "3 0 obj <</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox ["+ cPageSize + "] /Contents 6 0 R>>";
            cTextLines[nTextLine] = cText;
            nTextLine++;
            cTextLines[nTextLine] = "endobj";
            nTextLine++;

            //cText = "4 0 obj <</Font <</F1 5 0 R>>>>";
            cText = "4 0 obj <</Font <</F1 5 0 R>>/ProcSet[/PDF/Text]>>";
            cTextLines[nTextLine] = cText;
            nTextLine++;
            cTextLines[nTextLine] = "endobj";
            nTextLine++;

            // Encoding: /StandardEncoding ; /MacRomanEncoding ; /WinAnsiEncoding ; /PDFDocEncoding ; /Unicode ; /UTF-8
            cText = "5 0 obj <</Type /Font /Subtype /Type1 /BaseFont /Courier /Encoding /WinAnsiEncoding>>";
            cTextLines[nTextLine] = cText;
            nTextLine++;
            cTextLines[nTextLine] = "endobj";
            nTextLine++;

            cText = "6 0 obj";
            cTextLines[nTextLine] = cText;
            nTextLine++;

            nNumObjects = 7;

            cText = "<</Length 0000" + cFilter + ">>";
            cTextLines[nTextLine] = cText;
            nTextLine++;

            for (nPagNo = 1; nPagNo < nNumPages + 1; nPagNo++)
            {
                cTextLines[nTextLine] = "stream";
                nTextLine++;

                // Current date and page number.
                int nSpaceLength = nTabEnd10 - nTabBeg10 - DateTime.Today.ToString(MainPage.cDateFormat).Length - nPagNo.ToString().Length;
                string cDatePageNo = DateTime.Today.ToString(MainPage.cDateFormat) + new string(' ', nSpaceLength) + nPagNo.ToString();
                nHeight = nPosFirstRow;
                cText = "BT /F1 10 Tf " + cHorPointsBeg + " " + Convert.ToString(nHeight) + " Td (" + cDatePageNo + ")Tj ET";
                cTextLines[nTextLine] = cText;
                nTextLine++;

                //// Document title.
                nHeight -= 20;
                cText = "BT /F1 12 Tf " + cHorPointsBeg + " " + Convert.ToString(nHeight) + " Td (" + cDocTitle + ")Tj ET";
                cTextLines[nTextLine] = cText;
                nTextLine++;

                // Horizontal line.
                nHeight -= 8;
                cText = cHorPointsBeg + " " + Convert.ToString(nHeight) + " m " + cHorPointsEnd + " " + Convert.ToString(nHeight) + " l h S";
                cTextLines[nTextLine] = cText;
                nTextLine++;

                // Data loan.
                if (nPagNo == 1)
                {
                    nHeight -= 12;
                    cText = "BT /F1 10 Tf " + cHorPointsBeg + " " + Convert.ToString(nHeight) + " Td (" + ReplaceCharacters(lblInterestRate.Text) + " " + entInterestRate.Text + " %" + ")Tj ET";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;

                    double nCapitalInitial = Convert.ToDouble(entCapitalInitial.Text);
                    nHeight -= 15;
                    cText = "BT /F1 10 Tf " + cHorPointsBeg + " " + Convert.ToString(nHeight) + " Td (" + ReplaceCharacters(lblCapitalInitial.Text) + " " + MainPage.RoundDoubleToNumDecimals(ref nCapitalInitial, Convert.ToInt32(entNumDec.Text), "N") + " " + cCurrency + ")Tj ET";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;

                    nHeight -= 15;
                    cText = "BT /F1 10 Tf " + cHorPointsBeg + " " + Convert.ToString(nHeight) + " Td (" + ReplaceCharacters(lblDurationYears.Text) + " " + entDurationYears.Text + ")Tj ET";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;

                    nHeight -= 15;
                    cText = "BT /F1 10 Tf " + cHorPointsBeg + " " + Convert.ToString(nHeight) + " Td (" + ReplaceCharacters(lblPeriodsYear.Text) + " " + entPeriodsYear.Text + ")Tj ET";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;

                    // Horizontal line.
                    nHeight -= 5;
                    cText = cHorPointsBeg + " " + Convert.ToString(nHeight) + " m " + cHorPointsEnd + " " + Convert.ToString(nHeight) + " l h S";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;
                }

                // Print test line to calculate the number of characters on 1 line.
                //nHeight -= 15;
                //cText = "BT /F1 " + cFontSize + " Tf " + cHorPointsBeg + " " + Convert.ToString(nHeight) + " Td (123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 )Tj ET";
                //cTextLines[nTextLine] = cText;
                //nTextLine++;

                // Column heading line 1.
                nHeight -= 15;
                cText = "BT /F1 " + cFontSize + " Tf " + cHorPointsBeg + " " + Convert.ToString(nHeight) + " Td (" + cColText + ")Tj ET";
                cTextLines[nTextLine] = cText;
                nTextLine++;

                // Column headings line 2.
                if (cColTextPart2.Trim() != "")
                {
                    nHeight -= 10;
                    cText = "BT /F1 " + cFontSize + " Tf " + cHorPointsBeg + " " + Convert.ToString(nHeight) + " Td (" + cColTextPart2 + ")Tj ET";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;
                }

                // Horizontal line.
                nHeight -= 5;
                cText = cHorPointsBeg + " " + Convert.ToString(nHeight) + " m " + cHorPointsEnd + " " + Convert.ToString(nHeight) + " l h S";
                cTextLines[nTextLine] = cText;
                nTextLine++;

                // Detail per period.
                for (nRow = nRowStart; nRow < nNumberPeriods; nRow++)
                {
                    cColText0 = new String(' ', nTab0 - nTabBeg - aLoanDetail[nRow, 0].Length) + aLoanDetail[nRow, 0];
                    cColText1 = new String(' ', nTab1 - nTab0 - aLoanDetail[nRow, 1].Length) + aLoanDetail[nRow, 1];
                    cColText2 = new String(' ', nTab2 - nTab1 - aLoanDetail[nRow, 2].Length) + aLoanDetail[nRow, 2];
                    cColText3 = new String(' ', nTab3 - nTab2 - aLoanDetail[nRow, 3].Length) + aLoanDetail[nRow, 3];
                    cColText4 = new String(' ', nTab4 - nTab3 - aLoanDetail[nRow, 4].Length) + aLoanDetail[nRow, 4];
                    cColText5 = new String(' ', nTab5 - nTab4 - aLoanDetail[nRow, 5].Length) + aLoanDetail[nRow, 5];
                    cColText6 = new String(' ', nTab6 - nTab5 - aLoanDetail[nRow, 6].Length) + aLoanDetail[nRow, 6];

                    cText = cColText0 + cColText1 + cColText2 + cColText3 + cColText4 + cColText5 + cColText6;
                    nHeight -= 15;
                    cText = "BT /F1 " + cFontSize + " Tf " + cHorPointsBeg + " " + Convert.ToString(nHeight) + " Td (" + cText + ")Tj ET";
                    cTextLines[nTextLine] = cText;
                    nTextLine++;

                    if (nRow == nNumberPeriods - 1)
                    {
                        break;
                    }

                    else if ((nRow + 1) % nRowsPage == 0 && nRow > nRowsPage - 2)
                    {
                        cTextLines[nTextLine] = "endstream";
                        nTextLine++;
                        cTextLines[nTextLine] = "endobj";
                        nTextLine++;

                        nRowStart = nRow + 1;

                        //DisplayAlert("nNumObjects", Convert.ToString(nNumObjects), "OK");

                        cNoObject = Convert.ToString(nNumObjects);
                        cNoContents = Convert.ToString(nNumObjects + 1);

                        cText = cNoObject + " 0 obj<</Type /Page /Parent 2 0 R /Resources 4 0 R /MediaBox [" + cPageSize + "] /Contents " + cNoContents + " 0 R>>";
                        cTextLines[nTextLine] = cText;
                        nTextLine++;
                        cTextLines[nTextLine] = "endobj";
                        nTextLine++;
                        nNumObjects++;

                        cNoObject = Convert.ToString(nNumObjects);
                        cTextLines[nTextLine] = cNoObject + " 0 obj";
                        nTextLine++;
                        nNumObjects++;

                        //cText = "<</Length 0000>>";
                        //cTextLines[nTextLine] = cText;
                        //nTextLine++;

                        cText = "<</Length 0000" + cFilter + ">>";
                        cTextLines[nTextLine] = cText;
                        nTextLine++;

                        break;
                    }
                }
            }

            // End.
            // Horizontal line.
            nHeight -= 7;
            cText = cHorPointsBeg + " " + Convert.ToString(nHeight) + " m " + cHorPointsEnd + " " + Convert.ToString(nHeight) + " l h S";
            cTextLines[nTextLine] = cText;
            nTextLine++;

            // Totals.
            cColText0 = new String(' ', nTab0 - nTabBeg);
            cColText1 = new String(' ', nTab1 - nTab0);
            cColText2 = new String(' ', nTab2 - nTab1 - aLoanDetail[nNumberPeriods, 2].Length) + aLoanDetail[nNumberPeriods, 2];
            cColText3 = new String(' ', nTab3 - nTab2 - aLoanDetail[nNumberPeriods, 3].Length) + aLoanDetail[nNumberPeriods, 3];
            cColText4 = new String(' ', nTab4 - nTab3 - aLoanDetail[nNumberPeriods, 4].Length) + aLoanDetail[nNumberPeriods, 4];

            cText = cColText0 + cColText1 + cColText2 + cColText3 + cColText4;
            nHeight -= 12;
            cText = "BT /F1 " + cFontSize + " Tf " + cHorPointsBeg + " " + Convert.ToString(nHeight) + " Td (" + cText + ")Tj ET";
            cTextLines[nTextLine] = cText;
            nTextLine++;

            // Horizontal line.
            nHeight -= 7;
            cText = cHorPointsBeg + " " + Convert.ToString(nHeight) + " m " + cHorPointsEnd + " " + Convert.ToString(nHeight) + " l h S";
            cTextLines[nTextLine] = cText;
            nTextLine++;

            cTextLines[nTextLine] = "endstream";
            nTextLine++;
            cTextLines[nTextLine] = "endobj";
            nTextLine++;

            // PDF Xref table.
            cTextLines[nTextLine] = "xref";
            nTextLine++;
            cTextLines[nTextLine] = "0 " + Convert.ToString(nNumObjects);
            nTextLine++;
            cTextLines[nTextLine] = "0000000000 65535 f ";
            nTextLine++;

            for (int nObject = 1; nObject < nNumObjects; nObject++)
            {
                cText = "0000000000 00000 n ";
                cTextLines[nTextLine] = cText;
                nTextLine++;
            }

            cTextLines[nTextLine] = "trailer <</Size " + Convert.ToString(nNumObjects) + " /Root 1 0 R>>";
            nTextLine++;
            cTextLines[nTextLine] = "startxref";
            nTextLine++;
            cTextLines[nTextLine] = Convert.ToString(aPosObject[nNumObjects]);
            nTextLine++;
            cTextLines[nTextLine] = "%%EOF";

            // Count the number of characters, set the object locations and set the size of the streams.
            long nNumChar = 0;
            nNumObjects = 1;
            aPosObject[0] = 0;
            int nRowStream = 0;

            // Calulate the length from the streams.
            for (nRow = 1; nRow < nTextLine + 1; nRow++)
            {
                if (cTextLines[nRow].Contains("stream") && cTextLines[nRow][..6] == "stream")
                {
                    nNumChar = 0;
                    nRowStream = nRow;
                }
                else
                {
                    nNumChar = nNumChar + cTextLines[nRow].Length + 1;
                }

                if (cTextLines[nRow].Contains("endstream") && cTextLines[nRow][..9] == "endstream")
                {
                    cTextLines[nRowStream - 1] = "<</Length " + Convert.ToString(nNumChar - 10) + cFilter + ">>";
                }
            }

            // Calulate the position of the objects and the xref.
            nNumChar = 0;

            for (nRow = 1; nRow < nTextLine + 1; nRow++)
            {
                if (cTextLines[nRow].Contains(" 0 obj"))
                {
                    aPosObject[nNumObjects] = nNumChar + 1;
                    nNumObjects++;
                }

                if (cTextLines[nRow].Contains("xref") && cTextLines[nRow][..4] == "xref")
                {
                    aPosObject[nNumObjects] = nNumChar + 1;
                    break;
                }

                nNumChar = nNumChar + cTextLines[nRow].Length + 1;
            }

            // Set the xref data.
            nRow += 3;
            for (int nObject = 1; nObject < nNumObjects; nObject++)
            {
                cText = new String('0', 10) + Convert.ToString(aPosObject[nObject]);
                cText = string.Concat(cText.AsSpan(cText.Length - 10, 10), " 00000 n ");
                cTextLines[nRow] = cText;
                nRow++;
            }

            cTextLines[nRow + 2] = Convert.ToString(aPosObject[nNumObjects]);

            // Save the PDF file.
            using StreamWriter sw = new(cFileName, false);
            
            for (nRow = 1; nRow < nTextLine + 1; nRow++)
            {
                sw.WriteLine(cTextLines[nRow]);
            }

            // Close the StreamWriter object.
            sw.Close();
            sw.Dispose();
        }

        catch (Exception ex)
        {
            DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
            return;
        }
    }

    // Replace special characters in strings for output PDF.
    static string ReplaceCharacters(string cText)
    {
        cText = cText.Replace("ã", "\\343");    // ASCII = d 227   o 343   h E3
        cText = cText.Replace("ä", "\\344");    // ASCII = d 228   o 344   h E4
        cText = cText.Replace("ç", "\\347");    // ASCII = d 231   o 347   h E7
        cText = cText.Replace("é", "\\351");    // ASCII = d 233   o 351   h E9
        cText = cText.Replace("ê", "\\352");    // ASCII = d 234   o 352   h EA
        cText = cText.Replace("í", "\\355");    // ASCII = d 237   o 355   h ED
        cText = cText.Replace("ï", "\\357");    // ASCII = d 239   o 357   h EF
        cText = cText.Replace("ñ", "\\361");    // ASCII = d 241   o 361   h F1
        cText = cText.Replace("ó", "\\363");    // ASCII = d 243   o 363   h F3

        //cText = cText.Replace('ã', 'a');
        //cText = cText.Replace('ä', 'a');
        //cText = cText.Replace('ç', 'c');
        //cText = cText.Replace('é', 'e');
        //cText = cText.Replace('ê', 'e');
        //cText = cText.Replace('í', 'i');
        //cText = cText.Replace('ï', 'i');
        //cText = cText.Replace('ñ', 'n');
        //cText = cText.Replace('ó', 'o');

        //int i = (int)'ä';
        //DisplayAlert("ä", Convert.ToString(i), "OK");

        return cText;
    }
    
    // Split string in two parts.
    private string SplitStringInTwo(string cText, ref string cTextPart2, int nMaxTextLength)
    {
        try
        {
            int nPos = cText.LastIndexOf(" ");

            if (nPos > 0 && cText.Length > nMaxTextLength)
            {
                cTextPart2 = cText[(nPos + 1)..];
                cText = cText[..nPos];
            }

            if (cText.Length > nMaxTextLength)
            {
                cText = cText[..nMaxTextLength];
            }

            if (cTextPart2.Length > nMaxTextLength)
            {
                cTextPart2 = cTextPart2[..nMaxTextLength];
            }

            cText = new String(' ', nMaxTextLength - cText.Length) + cText;
            cTextPart2 = new String(' ', nMaxTextLength - cTextPart2.Length) + cTextPart2;

            return cText;
        }
        
        catch (Exception ex)
        {
            DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
            return cText;
        }
    }

    // Open the document file.
    private async Task OpenDocumentFile(string cFile)
    {
        bool answer = await DisplayAlert("Finance", Path.GetFileName(cFile) + "\n\n" + cFileOpenQuestion, cYes, cNo);
        if (answer == false)
        {
            return;
        }

        try
        {
            await Launcher.Default.OpenAsync(new OpenFileRequest(cDocumentName, new ReadOnlyFile(cFile)));
        }
        catch (Exception ex)
        {
            await DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
        }
    }

    // Send e-mail with attachment.
    //private async Task SendEmail(string cFile)
    //{
    //    Task.Delay(800).Wait();

    //    bool answer = await DisplayAlert("Finance", Path.GetFileName(cFile) + "\n\n" + cEmailSendQuestion, cYes, cNo);
    //    if (answer == false)
    //    {
    //        return;
    //    }

    //    string cEmailAddress = await DisplayPromptAsync(cEmailTitle, cEmailAddressQuestion, keyboard: Keyboard.Email);
    //    if (cEmailAddress == null)
    //    {
    //        return;
    //    }

    //    try
    //    {
    //        if (Email.Default.IsComposeSupported)
    //        {
    //            string subject = cEmailSubject;
    //            string body = cEmailBody;
    //            string[] recipients = new[] { cEmailAddress };

    //            var message = new EmailMessage
    //            {
    //                Subject = subject,
    //                Body = body,
    //                BodyFormat = EmailBodyFormat.PlainText,
    //                To = new List<string>(recipients)
    //            };

    //            message.Attachments.Add(new EmailAttachment(cFile));

    //            await Email.Default.ComposeAsync(message);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
    //        return;
    //    }
    //}

    // Open the share interface.
    private async Task OpenShareInterface(string cFile)
    {
#if ANDROID || WINDOWS        
        Task.Delay(800).Wait();

        bool answer = await DisplayAlert("Finance", Path.GetFileName(cFile) + "\n\n" + cShareQuestion, cYes, cNo);
        if (answer == false)
        {
            return;
        }
#endif
        try
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Finance",
                File = new ShareFile(cFile)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert(MainPage.cErrorTitleText, ex.Message, MainPage.cButtonCloseText);
        }
    }

    // Reset the entry fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        entNumDec.Text = "2";
        entInterestRate.Text = "";
        entCapitalInitial.Text = "";
        entDurationYears.Text = "";
        entPeriodsYear.Text = "12";
        txtAmountPeriod.Text = "";
        txtInterestTotal.Text = "";
        txtCapitalInterest.Text = "";
        
        dtpExpirationDate.Date = DateTime.Today;
        ckbDayEndMonth.IsChecked = false;
        entCurrencyCode.Text = MainPage.cISOCurrencyCode;

        bReCalculateResult = true;

        entNumDec.Focus();
    }

    // Radio button checked changed event.
    private void OnRbnLoanCheckedChanged(object sender, EventArgs e)
    {
        CalculateResult(sender, e);
    }

    // DatePicker and CheckBox event.
    private void OnDateDataChanged(object sender, EventArgs e)
    {
        bReCalculateResult = true;
    }

    // Picker and export type SelectedIndexChanged event.
    private void OnPickerExportTypeChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            cExportType = picker.Items[selectedIndex];
            //DisplayAlert("cExportType", cExportType, "OK");  // For testing
        }
    }
}
