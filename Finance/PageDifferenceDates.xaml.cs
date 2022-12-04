using Finance.Resources.Languages;

namespace Finance;
public partial class PageDifferenceDates : ContentPage
{
    public PageDifferenceDates()
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
        lblTitle.Text = FinLang.DifferenceDatesForm_Text;

        lblExplanation.Text = FinLang.DifferenceExplanationDates_Text;
        lblDate1.Text = FinLang.Date1_Text;
        lblDate2.Text = FinLang.Date2_Text;
        lblDateDifferenceDays.Text = FinLang.DateDifferenceDays_Text;
        lblDateDifferenceHours.Text = FinLang.DateDifferenceHours_Text;
        lblDateDifferenceMinutes.Text = FinLang.DateDifferenceMinutes_Text;
        lblDateDifferenceSeconds.Text = FinLang.DateDifferenceSeconds_Text;
        lblDateDifferenceYearMonthDay.Text = FinLang.DateDifferenceYearMonthDay_Text;

        btnCalculate.Text = FinLang.Calculate_Text;
        btnReset.Text = FinLang.Reset_Text;

        // Set the date properties for the DatePicker.
        dtpDate1.MinimumDate = new DateTime(1583, 1, 1);
        dtpDate1.MaximumDate = new DateTime(3999, 12, 31);
        dtpDate2.MinimumDate = new DateTime(1583, 1, 1);
        dtpDate2.MaximumDate = new DateTime(3999, 12, 31);

        dtpDate1.Format = MainPage.cDateFormat;
        dtpDate2.Format = MainPage.cDateFormat;

        // Set focus to the first entry field.
        dtpDate1.Focus();
    }

    // Clear result fields if the date have changed.
    private void DatePickerDateSelected(object sender, EventArgs e)
    {
        txtDateDifferenceDays.Text = "";
        txtDateDifferenceHours.Text = "";
        txtDateDifferenceMinutes.Text = "";
        txtDateDifferenceSeconds.Text = "";
        txtDateDifferenceYearMonthDay.Text = "";
    }

    // Go to the next field when the return key have been pressed.
    private void GoToNextField(object sender, EventArgs e)
    {
        if (sender == dtpDate1)
        {
            dtpDate2.Focus();
        }
    }

    // Calculate the result.
    private void CalculateResult(object sender, EventArgs e)
    {
        // Calculate the date difference in days, hours, minutes and seconds.
        decimal nDateDifference = (dtpDate2.Date - dtpDate1.Date).Days;
        txtDateDifferenceDays.Text = MainPage.RoundDecimalToNumDecimals(ref nDateDifference, 0, "N");
        
        nDateDifference *= 24;
        txtDateDifferenceHours.Text = MainPage.RoundDecimalToNumDecimals(ref nDateDifference, 0, "N");

        nDateDifference *= 60;
        txtDateDifferenceMinutes.Text = MainPage.RoundDecimalToNumDecimals(ref nDateDifference, 0, "N");

        nDateDifference *= 60;
        txtDateDifferenceSeconds.Text = MainPage.RoundDecimalToNumDecimals(ref nDateDifference, 0, "N");

        // Calculate the date difference in years, months and days.
        DateDifference dateDifference = new(dtpDate1.Date, dtpDate2.Date);
        txtDateDifferenceYearMonthDay.Text = dateDifference.ToString();

        // Set focus.
        btnReset.Focus();
    }

    // Reset the entry and result fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        dtpDate1.Date = DateTime.Today;
        dtpDate2.Date = DateTime.Today;
        txtDateDifferenceDays.Text = "";
        txtDateDifferenceHours.Text = "";
        txtDateDifferenceMinutes.Text = "";
        txtDateDifferenceSeconds.Text = "";
        txtDateDifferenceYearMonthDay.Text = "";
    }
}

// Class to calculate the date difference in years, months and days.
// Source: https://www.codeproject.com/Articles/28837/Calculating-Duration-Between-Two-Dates-in-Years-Mo
// by: Mohammed Ali Babu
public class DateDifference
{
    // Defining Number of days in month; index 0=> january and 11=> december.
    // February contain either 28 or 29 days, that's why here value is -1 which wil be calculate later.
    private readonly int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    // Contain from date.
    private readonly DateTime fromDate;

    // Contain to date.
    private readonly DateTime toDate;

    // This three variable for output representation.
    private readonly int year;
    private readonly int month;
    private readonly int day;

    public DateDifference(DateTime d1, DateTime d2)
    {
        int increment;

        if (d1 > d2)
        {
            fromDate = d2;
            toDate = d1;
        }
        else
        {
            fromDate = d1;
            toDate = d2;
        }

        // Day Calculation.
        increment = 0;

        if (fromDate.Day > toDate.Day)
        {
            increment = monthDay[fromDate.Month - 1];
        }

        // If it is february month.
        // If it's to day is less then from day.
        if (increment == -1)
        {
            if (DateTime.IsLeapYear(fromDate.Year))
            {
                // Leap year february contain 29 days.
                increment = 29;
            }
            else
            {
                increment = 28;
            }
        }
        
        if (increment != 0)
        {
            day = (toDate.Day + increment) - fromDate.Day;
            increment = 1;
        }
        else
        {
            day = toDate.Day - fromDate.Day;
        }

        // Month calculation.
        if ((fromDate.Month + increment) > toDate.Month)
        {
            month = (toDate.Month + 12) - (fromDate.Month + increment);
            increment = 1;
        }
        else
        {
            month = (toDate.Month) - (fromDate.Month + increment);
            increment = 0;
        }

        // Year calculation.
        year = toDate.Year - (fromDate.Year + increment);
    }

    public override string ToString()
    {
        string cYear;
        string cMonth;
        string cDay;

        if (year == 1)
        {
            cYear = " " + FinLang.DateYear_Text + ", ";
        }
        else
        {
            cYear = " " + FinLang.DateYears_Text + ", ";
        }

        if (month == 1)
        {
            cMonth = " " + FinLang.DateMonth_Text + ", ";
        }
        else
        {
            cMonth = " " + FinLang.DateMonths_Text + ", ";
        }

        if (day == 1)
        {
            cDay = " " + FinLang.DateDay_Text;
        }
        else
        {
            cDay = " " + FinLang.DateDays_Text;
        }

        return year.ToString() + cYear + month.ToString() + cMonth + day.ToString() + cDay;
    }

    public int Years
    {
        get
        {
            return year;
        }
    }

    public int Months
    {
        get
        {
            return month;
        }
    }

    public int Days
    {
        get
        {
            return day;
        }
    }
}
