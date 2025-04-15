namespace Finance
{
    public sealed partial class PageLoanDetail : ContentPage
    {
        //// Variables for loan detail
        public static readonly string[] aColHeader = new string[7];
        public static readonly string[,] aLoanDetail = new string[1201, 7];

        //// Variables for export / e-mail
        private string cExportType = "";
        private bool bReCalculateResult;

        public PageLoanDetail()
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
#if WINDOWS
            //// Set the margin of the title for windows
            lblTitlePage.Margin = new Thickness(80, 15, 0, 0);
#endif
#if IOS
            //// Set the scale of the activity indicator for iOS
            activityIndicator.Scale = 2;
#endif
            //// Set the date properties for the DatePicker
            dtpExpirationDate.MinimumDate = new DateTime(1583, 1, 1);
            dtpExpirationDate.MaximumDate = new DateTime(5000, 1, 1);

            //// Put text for the column headers in the chosen language in the array
            aColHeader[0] = FinLang.LoanDetailColumns_0_Text;
            aColHeader[1] = FinLang.LoanDetailColumns_1_Text;
            aColHeader[2] = FinLang.LoanDetailColumns_2_Text;
            aColHeader[3] = FinLang.LoanDetailColumns_3_Text;
            aColHeader[4] = FinLang.LoanDetailColumns_4_Text;
            aColHeader[5] = FinLang.LoanDetailColumns_5_Text;
            aColHeader[6] = FinLang.LoanDetailColumns_6_Text;

            //// Set the type of keyboard
            if (Globals.cKeyboard == "Default")
            {
                entInterestRate.Keyboard = Keyboard.Default;
                entCapitalInitial.Keyboard = Keyboard.Default;
            }
            else if (Globals.cKeyboard == "Text")
            {
                entInterestRate.Keyboard = Keyboard.Text;
                entCapitalInitial.Keyboard = Keyboard.Text;
            }

            //// Set the current date format and date for the DatePicker
            dtpExpirationDate.Format = Globals.cDateFormat;

            //// Set the currency code
            entCurrencyCode.Text = Globals.cISOCurrencyCode;

            //// Set the default export format
            pickerExportType.SelectedIndex = 1;

            //// Test variable to recalculate the loan
            bReCalculateResult = true;
        }

        /// <summary>
        /// Set focus to the first entry field 
        /// Add in the header of the xaml page: 'Loaded="OnPageLoaded"' 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageLoaded(object sender, EventArgs e)
        {
            _ = entInterestRate.Focus();
        }

        /// <summary>
        /// Entry focused event: format the text value for a numeric entry without the number separator and select the entire text value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryFocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                Globals.FormatTextEntryFocused(entry);
            }
        }

        /// <summary>
        /// Entry unfocused event: format the text value for a numeric entry field with the number separator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryUnfocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                Globals.FormatTextEntryUnfocused(entry);
            }
        }

        /// <summary>
        /// Go to the next field when the return key have been pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToNextField(object sender, EventArgs e)
        {
            if (sender == entInterestRate)
            {
                _ = entCapitalInitial.Focus();
            }
            else if (sender == entCapitalInitial)
            {
                _ = entDurationYears.Focus();
            }
            else if (sender == entDurationYears)
            {
                _ = entPeriodsYear.Focus();
            }
            else if (sender == entCurrencyCode)
            {
                // Close the keyboard
                entCurrencyCode.IsEnabled = false;
                entCurrencyCode.IsEnabled = true;

                _ = btnExport.Focus();
            }
        }

        /// <summary>
        /// Calculate the result with detail per period 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculateResult(object sender, EventArgs e)
        {
            // Validate input values
            bool bIsNumber = double.TryParse(entInterestRate.Text, out double nInterestRate);
            if (!bIsNumber || nInterestRate < 0 || nInterestRate > 100)
            {
                entInterestRate.Text = "";
                _ = entInterestRate.Focus();
                return;
            }

            bIsNumber = double.TryParse(entCapitalInitial.Text, out double nCapitalInitial);
            if (!bIsNumber || nCapitalInitial < 1 || nCapitalInitial >= 1_000_000_000_000)
            {
                entCapitalInitial.Text = "";
                _ = entCapitalInitial.Focus();
                return;
            }

            bIsNumber = int.TryParse(entDurationYears.Text, out int nDurationYears);
            if (!bIsNumber || nDurationYears < 1 || nDurationYears > 100)
            {
                entDurationYears.Text = "";
                _ = entDurationYears.Focus();
                return;
            }

            bIsNumber = int.TryParse(entPeriodsYear.Text, out int nPeriodsYear);
            if (!bIsNumber || nPeriodsYear < 1 || nPeriodsYear > 12)
            {
                entPeriodsYear.Text = "";
                _ = entPeriodsYear.Focus();
                return;
            }

            if ((12 % nPeriodsYear) != 0)
            {
                entPeriodsYear.Text = "";
                _ = entPeriodsYear.Focus();
                return;
            }

            // Close the keyboard
            entPeriodsYear.IsEnabled = false;
            entPeriodsYear.IsEnabled = true;

            // Convert string to int for number of decimal digits after decimal point
            int nNumDec = int.Parse(Globals.cNumDecimalDigits);
            int nPercDec = int.Parse(Globals.cPercDecimalDigits);

            // Clear result fields
            lblAmountPeriod.Text = "";
            lblInterestTotal.Text = "";
            lblCapitalInterest.Text = "";

            // Clear the array
            Array.Clear(aLoanDetail);

            // Setup variables
            double nCapitalRemainder;
            double nCapitalPeriod = 0;
            double nInterestPeriod = 0;
            double nInterestTotal = 0;
            double nInterestRatePeriod;
            double nPaymentPeriod = 0;
            int nRow;

            // Set up the loan calculations
            DateTime dExpirationDate = dtpExpirationDate.Date;              // Expiration date

            int nNumberMonthsAdd = 12 / nPeriodsYear;                       // Number of months to add
            int nNumberMonthsAddCumul = nNumberMonthsAdd;                   // Cumul of number of months to add
            int nNumberPeriods = nDurationYears * nPeriodsYear;             // Number of periods

            // Calculate the interest per month
            try
            {
                nInterestRatePeriod = Math.Pow(1 + (nInterestRate / 100), (double)1 / nPeriodsYear) - 1;  // Interest rate per period
            }
            catch (Exception ex)
            {
#if DEBUG                
                _ = DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
#endif
                ResetEntryFields(null, null);
                return;
            }

            // Rounding to 6 digits after decimal point
            if (Globals.cRoundNumber == "AwayFromZero")
            {
                nInterestRatePeriod = Math.Round(nInterestRatePeriod, 6, MidpointRounding.AwayFromZero);
            }
            else if (Globals.cRoundNumber == "ToEven")
            {
                nInterestRatePeriod = Math.Round(nInterestRatePeriod, 6, MidpointRounding.ToEven);  // Rounding to 6 digits after decimal point
            }

            //Debug.WriteLine("nInterestRatePeriod: " + nInterestRatePeriod);  // For testing

            // Calculate annuity loan per period
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
                lblAmountPeriod.Text = Globals.RoundToNumDecimals(ref nPaymentPeriod, nNumDec, "N");
            }

            // Calculate linear loan per period
            else
            {                      
                // Amount of capital per period
                if (Globals.cRoundNumber == "AwayFromZero")
                {
                    nCapitalPeriod = Math.Round(nCapitalInitial / nNumberPeriods, nNumDec, MidpointRounding.AwayFromZero);
                }
                else if (Globals.cRoundNumber == "ToEven")
                {
                    nCapitalPeriod = Math.Round(nCapitalInitial / nNumberPeriods, nNumDec, MidpointRounding.ToEven);
                }

                nCapitalRemainder = nCapitalInitial;                        // Remainder of capital
            }

            // Add the data in elements of the array
            for (nRow = 0; nRow < nNumberPeriods; nRow++)
            {
                // Calculate loan annuity per period
                if (rbnLoanAnnuity.IsChecked)
                {
                    if (Globals.cRoundNumber == "AwayFromZero")
                    {
                        nInterestPeriod = Math.Round(nCapitalRemainder * nInterestRatePeriod, nNumDec, MidpointRounding.AwayFromZero);
                    }
                    else if (Globals.cRoundNumber == "ToEven")
                    {
                        nInterestPeriod = Math.Round(nCapitalRemainder * nInterestRatePeriod, nNumDec, MidpointRounding.ToEven);
                    }

                    nInterestTotal += nInterestPeriod;
                    nCapitalPeriod = nPaymentPeriod - nInterestPeriod;
                    nCapitalRemainder -= nCapitalPeriod;
                }
                // Calculate loan linear per period
                else
                {
                    if (Globals.cRoundNumber == "AwayFromZero")
                    {
                        nInterestPeriod = Math.Round(nCapitalRemainder * nInterestRatePeriod, nNumDec, MidpointRounding.AwayFromZero);
                    }
                    else if (Globals.cRoundNumber == "ToEven")
                    {
                        nInterestPeriod = Math.Round(nCapitalRemainder * nInterestRatePeriod, nNumDec, MidpointRounding.ToEven);
                    }

                    nPaymentPeriod = nCapitalPeriod + nInterestPeriod;
                    nInterestTotal += nInterestPeriod;
                    nCapitalRemainder -= nCapitalPeriod;
                }

                // Correction rounding differences interest and capital last period
                if (nRow == nNumberPeriods - 1)
                {
                    // Correction rounding differences interest last period
                    if (nInterestPeriod < 0)
                    {
                        nInterestPeriod = 0;
                    }

                    // Correction rounding differences capital last period
                    //Debug.WriteLine("nCapitalRemainder: " + nCapitalRemainder);  // For testing
                    nCapitalPeriod += nCapitalRemainder;
                    nPaymentPeriod += nCapitalRemainder;
                    nCapitalRemainder = 0;
                }

                // If selected set last day of month
                if (ckbDayEndMonth.IsChecked)
                {
                    DateTime dDateLastDayMonth = new(dExpirationDate.Year, dExpirationDate.Month, DateTime.DaysInMonth(dExpirationDate.Year, dExpirationDate.Month));
                    dExpirationDate = dDateLastDayMonth;
                }

                // Fill the array with data
                aLoanDetail[nRow, 0] = Convert.ToString(nRow + 1);
                aLoanDetail[nRow, 1] = dExpirationDate.ToString(Globals.cDateFormat);
                aLoanDetail[nRow, 2] = Globals.RoundToNumDecimals(ref nPaymentPeriod, nNumDec, "N");
                aLoanDetail[nRow, 3] = Globals.RoundToNumDecimals(ref nCapitalPeriod, nNumDec, "N");
                aLoanDetail[nRow, 4] = Globals.RoundToNumDecimals(ref nInterestPeriod, nNumDec, "N");
                aLoanDetail[nRow, 5] = Globals.RoundToNumDecimals(ref nInterestTotal, nNumDec, "N");
                aLoanDetail[nRow, 6] = Globals.RoundToNumDecimals(ref nCapitalRemainder, nNumDec, "N");

                dExpirationDate = dtpExpirationDate.Date.AddMonths(nNumberMonthsAddCumul);
                nNumberMonthsAddCumul += nNumberMonthsAdd;

                //Debug.WriteLine("Row number: " + nRow);  // For testing
            }
            //Debug.WriteLine("Row number: " + nRow);  // For testing

            // Rounding and formatting result
            lblInterestTotal.Text = Globals.RoundToNumDecimals(ref nInterestTotal, nNumDec, "N");
            double nCapitalInterest = nCapitalInitial + nInterestTotal;
            lblCapitalInterest.Text = Globals.RoundToNumDecimals(ref nCapitalInterest, nNumDec, "N");

            // Store totals in row of array aLoanDetail (ex. nNumberPeriods = 12 -> nRow in for loop = 0-11 -> nRow after for loop = 12)
            aLoanDetail[nRow, 2] = lblCapitalInterest.Text;
            aLoanDetail[nRow, 3] = Globals.RoundToNumDecimals(ref nCapitalInitial, nNumDec, "N");
            aLoanDetail[nRow, 4] = lblInterestTotal.Text;

            // Test variable te recalculate the loan
            bReCalculateResult = false;

            // Set focus
            _ = btnReset.Focus();
        }

        /// <summary>
        /// Export loan with detail per period 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ExportDetailLoan(object sender, EventArgs e)
        {
            // Recalculate the loan if needed
            if (bReCalculateResult)
            {
                CalculateResult(sender, e);
            }

            // Number of periods
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

            // Currency
            string cCurrency = entCurrencyCode.Text.Trim();
        
            if (cCurrency.Length < 3)
            {
                cCurrency = Globals.cISOCurrencyCode;
                entCurrencyCode.Text = cCurrency;
            }

            cCurrency = cCurrency.ToUpper();

            // Document title: loan annuity or linear
            string cDocTitle;

            if (rbnLoanAnnuity.IsChecked)
            {
                cDocTitle = FinLang.LoanDetailDocTitleAnnuity_Text;
            }
            else
            {
                cDocTitle = FinLang.LoanDetailDocTitleLinear_Text;
            }

            // File name
            //string cFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), FinLang.LoanDetailDocumentName_Text);
            //string cFileName = Path.Combine(FileSystem.Current.AppDataDirectory, FinLang.LoanDetailDocumentName_Text);
            string cFileName = Path.Combine(FileSystem.Current.CacheDirectory, FinLang.LoanDetailDocumentName_Text);

            // Initialize named tuple from labels and entry fields
            (string lblInterestRateText, string entInterestRateText, string lblCapitalInitialText, string entCapitalInitialText,
                string lblDurationYearsText, string entDurationYearsText, string lblPeriodsYearText, string entPeriodsYear) tLblEnt
                = (lblInterestRate.Text, entInterestRate.Text, lblCapitalInitial.Text, entCapitalInitial.Text,
                lblDurationYears.Text, entDurationYears.Text, lblPeriodsYear.Text, entPeriodsYear.Text);

            // Export
            activityIndicator.IsRunning = true;

            if (cExportType == "CSV ;")
            {
                cFileName += ".csv";
                ClassPageLoanDetailExport.ExportDetailLoanCSV(nNumberPeriods, cCurrency, cDocTitle, cFileName, tLblEnt);
            }
            else if (cExportType == "HTML")
            {
                cFileName += ".html";
                ClassPageLoanDetailExport.ExportDetailLoanHTML(nNumberPeriods, cCurrency, cDocTitle, cFileName, tLblEnt);
            }
            else if (cExportType == "PDF")
            {
                cFileName += ".pdf";
                ClassPageLoanDetailExport.ExportDetailLoanPDF(nNumberPeriods, cCurrency, cDocTitle, cFileName, tLblEnt);
            }

            // Open the document file
            await OpenDocumentFileAsync(cFileName);

            // Open the share interface to share the document file
            await OpenShareInterfaceAsync(cFileName);

            activityIndicator.IsRunning = false;
        }

        /// <summary>
        /// Open the document file 
        /// </summary>
        /// <param name="cFile"></param>
        /// <returns></returns>
        private async Task OpenDocumentFileAsync(string cFile)
        {
            bool answer = await DisplayAlert("Finance", $"{Path.GetFileName(cFile)}\n\n{FinLang.FileOpenQuestion_Text}", FinLang.Yes_Text, FinLang.No_Text);
            if (answer == false)
            {
                return;
            }

            try
            {
                // Workaround for !!!BUG!!! on Android !!! - Webpage not available
                // The webpage at file:///data/user/0/com.companyname.finance/cache/FinanceDocument.html could not be loaded becauce:
                // net::ERR_ACCESS_DENIED
#if ANDROID
                await Launcher.Default.OpenAsync(new OpenFileRequest(FinLang.LoanDetailDocumentName_Text, new ReadOnlyFile(cFile)));
#else
                await Navigation.PushAsync(new PageLoanDetailHtml(cFile));
#endif
                // Wait 1 second before opening the share interface
                Task.Delay(1000).Wait();
            }
            catch (Exception ex)
            {
#if DEBUG                
                await DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
#endif
            }
        }

        /// <summary>
        /// Open the share interface 
        /// </summary>
        /// <param name="cFile"></param>
        /// <returns></returns>
        private async Task OpenShareInterfaceAsync(string cFile)
        {
            bool answer = await DisplayAlert("Finance", $"{Path.GetFileName(cFile)}\n\n{FinLang.ShareQuestion_Text}", FinLang.Yes_Text, FinLang.No_Text);
            if (answer == false)
            {
                return;
            }

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
#if DEBUG                
                await DisplayAlert(FinLang.ErrorTitle_Text, ex.Message, FinLang.ButtonClose_Text);
#endif
            }
        }

        /// <summary>
        /// Reset the entry fields 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetEntryFields(object? sender, EventArgs? e)
        {
            entInterestRate.Text = "";
            entCapitalInitial.Text = "";
            entDurationYears.Text = "";
            entPeriodsYear.Text = "12";
            lblAmountPeriod.Text = "";
            lblInterestTotal.Text = "";
            lblCapitalInterest.Text = "";
        
            dtpExpirationDate.Date = DateTime.Today;
            ckbDayEndMonth.IsChecked = false;
            entCurrencyCode.Text = Globals.cISOCurrencyCode;

            bReCalculateResult = true;

            _ = entInterestRate.Focus();
        }

        /// <summary>
        /// Radio button checked changed event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRbnLoanCheckedChanged(object sender, EventArgs e)
        {
            CalculateResult(sender, e);
        }

        /// <summary>
        /// DatePicker and CheckBox event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDateDataChanged(object sender, EventArgs e)
        {
            bReCalculateResult = true;
        }

        /// <summary>
        /// Picker and export type SelectedIndexChanged event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPickerExportTypeChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                cExportType = picker.Items[selectedIndex];
                //Debug.WriteLine("cExportType: " + cExportType);  // For testing
            }
        }
    }
}
