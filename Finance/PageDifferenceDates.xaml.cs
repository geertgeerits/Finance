namespace Finance
{
    public sealed partial class PageDifferenceDates : ContentPage
    {
        public PageDifferenceDates()
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

            // Set the date properties for the DatePicker
            dtpDate1.MinimumDate = new DateTime(1583, 1, 1);
            dtpDate1.MaximumDate = new DateTime(5000, 1, 1);
            dtpDate2.MinimumDate = new DateTime(1583, 1, 1);
            dtpDate2.MaximumDate = new DateTime(5000, 1, 1);

            dtpDate1.Format = Globals.cDateFormat;
            dtpDate2.Format = Globals.cDateFormat;

            CalculateResult(null, null);
        }

        // Go to the next field when the return key have been pressed
        private void GoToNextField(object sender, EventArgs e)
        {
            if (sender == dtpDate1)
            {
                dtpDate2.Focus();
            }
        }

        // Calculate the result
        private void CalculateResult(object sender, EventArgs e)
        {
            // Set the smallest date to the first date
            DateTime fromDate;
            DateTime toDate;

            if (dtpDate1.Date > dtpDate2.Date)
            {
                fromDate = dtpDate2.Date;
                toDate = dtpDate1.Date;
            }
            else
            {
                fromDate = dtpDate1.Date;
                toDate = dtpDate2.Date;
            }

            // Calculate age from start date (date of birth) to end date in years, months, weeks and days (Viorel)
            DateAgeInYearsMonthsWeeksDays(fromDate, toDate);

            // Calculate the date difference in days, hours, minutes and seconds
            long nDateDifference = (toDate - fromDate).Days;
            lblDateDifferenceDays.Text = nDateDifference.ToString("N0");

            nDateDifference *= 24;
            lblDateDifferenceHours.Text = nDateDifference.ToString("N0");

            nDateDifference *= 60;
            lblDateDifferenceMinutes.Text = nDateDifference.ToString("N0");

            nDateDifference *= 60;
            lblDateDifferenceSeconds.Text = nDateDifference.ToString("N0");

            // Set focus.
            btnReset.Focus();
        }

        // Calculate age from start date (date of birth) to end date in years, months, weeks and days
        // by: Viorel
        // https://learn.microsoft.com/en-us/answers/questions/608004/find-difference-between-dates-c
        private void DateAgeInYearsMonthsWeeksDays(DateTime StartDate, DateTime EndDate)
        {
            int years;
            int months;
            int days;

            for (int i = 1; ; ++i)
            {
                if (StartDate.AddYears(i) > EndDate)
                {
                    years = i - 1;
                    break;
                }
            }

            for (int i = 1; ; ++i)
            {
                if (StartDate.AddYears(years).AddMonths(i) > EndDate)
                {
                    months = i - 1;
                    break;
                }
            }

            for (int i = 1; ; ++i)
            {
                if (StartDate.AddYears(years).AddMonths(months).AddDays(i) > EndDate)
                {
                    days = i - 1;
                    break;
                }
            }

            string cYear = years == 1 ? FinLang.DateYear_Text : FinLang.DateYears_Text;
            string cMonth = months == 1 ? FinLang.DateMonth_Text : FinLang.DateMonths_Text;
            string cDay = days == 1 ? FinLang.DateDay_Text : FinLang.DateDays_Text;

            lblDateDifferenceYearMonthDay.Text = $"{years:N0} {cYear}, {months} {cMonth}, {days} {cDay}";
            lblDateDifferenceMonths.Text = $"{(years * 12) + months:N0}";
            lblDateDifferenceWeeks.Text = $"{(EndDate - StartDate).Days / 7:N0}";
        }

        // Reset the entry and result fields
        private void ResetEntryFields(object sender, EventArgs e)
        {
            dtpDate1.Date = DateTime.Today;
            dtpDate2.Date = DateTime.Today;

            CalculateResult(sender, e);
        }
    }
}
