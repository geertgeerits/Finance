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
            DisplayAlert("InitializeComponent", ex.Message, "OK");
            return;
        }

        // Set the date properties for the DatePicker.
        dtpDate1.MinimumDate = new DateTime(1583, 1, 1);
        dtpDate1.MaximumDate = new DateTime(3999, 12, 31);
        dtpDate2.MinimumDate = new DateTime(1583, 1, 1);
        dtpDate2.MaximumDate = new DateTime(3999, 12, 31);

        dtpDate1.Format = Globals.cDateFormat;
        dtpDate2.Format = Globals.cDateFormat;

        CalculateResult(null, null);

        // Set focus to the first entry field.
        dtpDate1.Focus();
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
        // Calculate the date difference in years, months, weeks and days (Sameer Saini).
        DateDifferenceInYearsMonthsDays(dtpDate1.Date, dtpDate2.Date);

        // Calculate the date difference in years, months and days (Mohammed Ali Babu).
        //DateDifference dateDifference = new(dtpDate1.Date, dtpDate2.Date);
        //txtDateDifferenceYearMonthDay.Text = dateDifference.ToString();

        // Calculate the date difference in years, months and days (???).
        //Age age = new(dtpDate1.Date, dtpDate2.Date);
        //txtDateDifferenceYearMonthDay.Text = $"{age.Years.ToString()}, {age.Months.ToString()}, {age.Days.ToString()}";

        // Calculate age from start date (date of birth) to end date (Viorel).
        txtDateDifferenceYearMonthDay.Text = DateAgeInYearsMonthsDays(dtpDate1.Date, dtpDate2.Date);

        // Calculate the date difference in days, hours, minutes and seconds.
        long nDateDifference = (dtpDate2.Date - dtpDate1.Date).Days;
        txtDateDifferenceDays.Text = nDateDifference.ToString("N0");

        nDateDifference *= 24;
        txtDateDifferenceHours.Text = nDateDifference.ToString("N0");

        nDateDifference *= 60;
        txtDateDifferenceMinutes.Text = nDateDifference.ToString("N0");

        nDateDifference *= 60;
        txtDateDifferenceSeconds.Text = nDateDifference.ToString("N0");
        //--------------------------------------------------------------------------------------------

        // Define two dates.
        //DateTime date1 = new DateTime(2010, 1, 1, 8, 0, 15);
        //DateTime date2 = new DateTime(2010, 8, 18, 13, 30, 30);

        //DateTime date1 = dtpDate1.Date;
        //DateTime date2 = dtpDate2.Date;

        // Calculate the interval between the two dates.
        //TimeSpan interval = date2 - date1;
        //Console.WriteLine("{0} - {1} = {2}", date2, date1, interval.ToString());

        // Display individual properties of the resulting TimeSpan object.
        //Console.WriteLine("   {0,-35} {1,20}", "Value of Days Component:", interval.Days);
        //Console.WriteLine("   {0,-35} {1,20}", "Total Number of Days:", interval.TotalDays);
        //Console.WriteLine("   {0,-35} {1,20}", "Value of Hours Component:", interval.Hours);
        //Console.WriteLine("   {0,-35} {1,20}", "Total Number of Hours:", interval.TotalHours);
        //Console.WriteLine("   {0,-35} {1,20}", "Value of Minutes Component:", interval.Minutes);
        //Console.WriteLine("   {0,-35} {1,20}", "Total Number of Minutes:", interval.TotalMinutes);
        //Console.WriteLine("   {0,-35} {1,20:N0}", "Value of Seconds Component:", interval.Seconds);
        //Console.WriteLine("   {0,-35} {1,20:N0}", "Total Number of Seconds:", interval.TotalSeconds);
        //Console.WriteLine("   {0,-35} {1,20:N0}", "Value of Milliseconds Component:", interval.Milliseconds);
        //Console.WriteLine("   {0,-35} {1,20:N0}", "Total Number of Milliseconds:", interval.TotalMilliseconds);
        //Console.WriteLine("   {0,-35} {1,20:N0}", "Ticks:", interval.Ticks);

        // This example displays the following output:
        //       8/18/2010 1:30:30 PM - 1/1/2010 8:00:15 AM = 229.05:30:15
        //          Value of Days Component:                             229
        //          Total Number of Days:                   229.229340277778
        //          Value of Hours Component:                              5
        //          Total Number of Hours:                  5501.50416666667
        //          Value of Minutes Component:                           30
        //          Total Number of Minutes:                       330090.25
        //          Value of Seconds Component:                           15
        //          Total Number of Seconds:                      19,805,415
        //          Value of Milliseconds Component:                       0
        //          Total Number of Milliseconds:             19,805,415,000
        //          Ticks:                               198,054,150,000,000

        //txtDateDifferenceMonths.Text = "";
        //txtDateDifferenceWeeks.Text = $"{interval.Days / 7:N0}";
        //txtDateDifferenceDays.Text = $"{interval.TotalDays:N0}";
        //txtDateDifferenceHours.Text = $"{interval.TotalHours:N0}";
        //txtDateDifferenceMinutes.Text = $"{interval.TotalMinutes:N0}";
        //txtDateDifferenceSeconds.Text = $"{interval.TotalSeconds:N0}";


        // Set focus.
        btnReset.Focus();
    }

    // Reset the entry and result fields.
    private void ResetEntryFields(object sender, EventArgs e)
    {
        dtpDate1.Date = DateTime.Today;
        dtpDate2.Date = DateTime.Today;

        CalculateResult(sender, e);
    }

    // Calculate age from start date (date of birth) to end date.
    // by: Sameer Saini
    // Source: https://codelikeadev.com/blog/calculate-person-age-from-date-of-birth-c-sharp#:~:text=Subtract()%20method%20that%20is,Years%2C%20Months%2C%20Days%20etc.
    private void DateDifferenceInYearsMonthsDays(DateTime d1, DateTime d2)
    {
        DateTime fromDate;
        DateTime toDate;

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

        TimeSpan difference = toDate.Subtract(fromDate);

        var firstDay = new DateTime(1, 1, 1);

        //Console.WriteLine($"Age in seconds: {Math.Round(difference.TotalSeconds)}");
        //Console.WriteLine($"Age in minutes: {Math.Round(difference.TotalMinutes)}");
        //Console.WriteLine($"Age in hours: {Math.Round(difference.TotalHours)}");
        //Console.WriteLine($"Age in days: {Math.Round(difference.TotalDays)}");
        //Console.WriteLine($"Age in weeks up: {Math.Ceiling(difference.TotalDays / 7)}");
        //Console.WriteLine($"Age in full up/down: {Math.Round(difference.TotalDays / 7)}");
        //Console.WriteLine($"Age in full down: {Math.Floor(difference.TotalDays / 7)}");

        int totalYears = (firstDay + difference).Year - 1;

        int totalMonths = (totalYears * 12) + (firstDay + difference).Month - 1;
        int runningMonths = totalMonths - (totalYears * 12);

        int runningWeeks = Convert.ToInt32(Math.Floor(difference.TotalDays / 7));

        int runningDays = (toDate - fromDate.AddMonths((totalYears * 12) + runningMonths)).Days;

        string cYear = totalYears == 1 ? FinLang.DateYear_Text : FinLang.DateYears_Text;
        string cMonth = runningMonths == 1 ? FinLang.DateMonth_Text : FinLang.DateMonths_Text;
        string cDay = runningDays == 1 ? FinLang.DateDay_Text : FinLang.DateDays_Text;

        txtDateDifferenceYearMonthDay.Text = $"{totalYears:N0} {cYear}, {runningMonths} {cMonth}, {runningDays} {cDay}";
        txtDateDifferenceMonths.Text = $"{totalMonths:N0}";
        txtDateDifferenceWeeks.Text = $"{runningWeeks:N0}";
    }
    // Calculate age from start date (date of birth) to end date.
    // by: Viorel
    // https://learn.microsoft.com/en-us/answers/questions/608004/find-difference-between-dates-c
    private static string DateAgeInYearsMonthsDays(DateTime d1, DateTime d2)
    {
        DateTime StartDate;
        DateTime EndDate;

        if (d1 > d2)
        {
            StartDate = d2;
            EndDate = d1;
        }
        else
        {
            StartDate = d1;
            EndDate = d2;
        }

        int years;
        int months;
        int days;

        for (var i = 1; ; ++i)
        {
            if (StartDate.AddYears(i) > EndDate)
            {
                years = i - 1;

                break;
            }
        }

        for (var i = 1; ; ++i)
        {
            if (StartDate.AddYears(years).AddMonths(i) > EndDate)
            {
                months = i - 1;

                break;
            }
        }

        for (var i = 1; ; ++i)
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
        return $"{years:N0} {cYear}, {months} {cMonth}, {days} {cDay}";
    }
}

// Class to calculate the date difference in years, months and days.
// by: Mohammed Ali Babu
// Source: https://www.codeproject.com/Articles/28837/Calculating-Duration-Between-Two-Dates-in-Years-Mo
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
        string cYear = year == 1 ? FinLang.DateYear_Text : FinLang.DateYears_Text;
        string cMonth = month == 1 ? FinLang.DateMonth_Text : FinLang.DateMonths_Text;
        string cDay = day == 1 ? FinLang.DateDay_Text : FinLang.DateDays_Text;

        return $"{year:N0} {cYear}, {month} {cMonth}, {day} {cDay}";
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

// Calculate age from start date (date of birth) to end date.
// by: ???
// Source: https://www.csharp-console-examples.com/csharp-console/age-calculator-in-c-years-months-days/
public class Age
{
    public int Years;
    public int Months;
    public int Days;

    public Age(DateTime Bday)
    {
        this.Count(Bday);
    }

    public Age(DateTime Bday, DateTime Cday)
    {
        this.Count(Bday, Cday);
    }

    public Age Count(DateTime Bday)
    {
        return this.Count(Bday, DateTime.Today);
    }

    public Age Count(DateTime Bday, DateTime Cday)
    {
        //DateTime Bday;
        //DateTime Cday;

        //if (d1 > d2)
        //{
        //    Bday = d2;
        //    Cday = d1;
        //}
        //else
        //{
        //    Bday = d1;
        //    Cday = d2;
        //}

        if ((Cday.Year - Bday.Year) > 0 ||
            (((Cday.Year - Bday.Year) == 0) && ((Bday.Month < Cday.Month) ||
              ((Bday.Month == Cday.Month) && (Bday.Day <= Cday.Day)))))
        {
            int DaysInBdayMonth = DateTime.DaysInMonth(Bday.Year, Bday.Month);
            int DaysRemain = Cday.Day + (DaysInBdayMonth - Bday.Day);

            if (Cday.Month > Bday.Month)
            {
                this.Years = Cday.Year - Bday.Year;
                this.Months = Cday.Month - (Bday.Month + 1) + Math.Abs(DaysRemain / DaysInBdayMonth);
                this.Days = (DaysRemain % DaysInBdayMonth + DaysInBdayMonth) % DaysInBdayMonth;
            }
            else if (Cday.Month == Bday.Month)
            {
                if (Cday.Day >= Bday.Day)
                {
                    this.Years = Cday.Year - Bday.Year;
                    this.Months = 0;
                    this.Days = Cday.Day - Bday.Day;
                }
                else
                {
                    this.Years = (Cday.Year - 1) - Bday.Year;
                    this.Months = 11;
                    this.Days = DateTime.DaysInMonth(Bday.Year, Bday.Month) - (Bday.Day - Cday.Day);
                }
            }
            else
            {
                this.Years = (Cday.Year - 1) - Bday.Year;
                this.Months = Cday.Month + (11 - Bday.Month) + Math.Abs(DaysRemain / DaysInBdayMonth);
                this.Days = (DaysRemain % DaysInBdayMonth + DaysInBdayMonth) % DaysInBdayMonth;
            }
        }
        else
        {
            throw new ArgumentException("Birthday date must be earlier than current date");
        }
        return this;
    }
}
