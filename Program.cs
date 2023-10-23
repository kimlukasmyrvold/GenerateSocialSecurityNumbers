using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace fnum;

// ======< Main Class >====== \\
public static partial class Program
{
    private static void Main()
    {
        GenerateFnums();
    }


    // ************************************
    // *            Generation            *
    // ************************************

    // Generate all fnums
    private static void GenerateFnums()
    {
        var fnums = new List<string>();
        var restStr = "00000";

        while (int.Parse(restStr) < 99999)
        {
            restStr = (int.Parse(restStr) + 1).ToString("00000");

            var str = restStr;
            Parallel.ForEach(Partitioner.Create(0, 100), range =>
            {
                for (var currentYear = range.Item1; currentYear < range.Item2; currentYear++)
                {
                    var daysInYear = new DaysInMonths(currentYear);
                    foreach (var item in daysInYear.MonthDays)
                    {
                        for (var i = 1; i <= item.Value; i++)
                        {
                            var day = i;
                            var month = int.Parse(item.Key);
                            var year = currentYear;

                            var dayStr = day.ToString("00");
                            var monthStr = month.ToString("00");
                            var yearStr = year.ToString("00");

                            var fnum = dayStr + monthStr + yearStr + str;

                            var fnumList = new List<int>();
                            foreach (var ch in fnum)
                            {
                                if (int.TryParse(ch.ToString(), out var result))
                                    fnumList.Add(result);
                            }

                            if (!CheckFnumValid(fnumList)) continue;
                            fnums.Add(fnum);
                            if (fnums.Count % 1000 == 0)
                            {
                                Console.WriteLine(fnum);
                            }
                        }
                    }
                }
            });
        }

        OutputToFile(fnums);
    }

    // ======< Ouput all fnums to text file >====== \\
    private static void OutputToFile(IEnumerable<string> fnums)
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fnums.txt");

        using var outputFile = new StreamWriter(path);
        foreach (var fnum in fnums) outputFile.WriteLine(fnum);

        Console.WriteLine("Fnums has been written to: " + path);
    }


    // ************************************
    // *            Validation            *
    // ************************************

    // ======< Checking if fnum is valid >====== \\
    private static bool CheckFnumValid(IReadOnlyList<int> fnumList)
    {
        if (fnumList.Count != 11)
            return false;

        var fnumBuilder = new StringBuilder(11);
        int[] check1 = { 3, 7, 6, 1, 8, 9, 4, 5, 2, 1, 0 };
        int[] check2 = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 1 };

        var checksum1 = 0;
        var checksum2 = 0;

        for (var i = 0; i < fnumList.Count; i++)
        {
            fnumBuilder.Append(fnumList[i]);
            checksum1 += fnumList[i] * check1[i];
            checksum2 += fnumList[i] * check2[i];
        }

        var fnum = fnumBuilder.ToString();

        if (!RegexOk(fnum))
            return false;

        if (!CheckDate(fnumList))
            return false;

        return CleanDivide(checksum1, 11) && CleanDivide(checksum2, 11);
    }

    // ======< Checking regex for fnum >====== \\
    private static bool RegexOk(string fnum)
    {
        return MyRegex().IsMatch(fnum);
    }

    // ======< Checking if date is valid >====== \\
    private static bool CheckDate(IReadOnlyList<int> fnumList)
    {
        var dateBuilder = new StringBuilder(6);
        for (var i = 0; i < 6; i++)
        {
            dateBuilder.Append(fnumList[i]);
        }

        var dateString = dateBuilder.ToString();
        return DateTime.TryParseExact(dateString, "ddMMyy", null, System.Globalization.DateTimeStyles.None,
            out _);
    }

    // ======< Checking if numbers divide evenly >====== \\
    private static bool CleanDivide(int n1, int n2)
    {
        return n1 % n2 == 0;
    }

    [GeneratedRegex("^(0[1-9]|[12][0-9]|3[01])(0[1-9]|1[012])\\d{7}$")]
    private static partial Regex MyRegex();
}

// ======< Days in Months class >====== \\
public class DaysInMonths
{
    public readonly Dictionary<string, int> MonthDays;

    public DaysInMonths(int yearInYyFormat)
    {
        int year;
        if (yearInYyFormat is >= 0 and <= 99)
        {
            year = yearInYyFormat;
            var currentYear = DateTime.Now.Year;
            var century = currentYear / 100;
            year += century * 100;
        }
        else
        {
            // ReSharper disable once NotResolvedInText
            throw new ArgumentOutOfRangeException("Year must be in YY format (0-99).");
        }

        MonthDays = new Dictionary<string, int>
        {
            { "1", 31 },
            { "2", DateTime.IsLeapYear(year) ? 29 : 28 },
            { "3", 31 },
            { "4", 30 },
            { "5", 31 },
            { "6", 30 },
            { "7", 31 },
            { "8", 31 },
            { "9", 30 },
            { "10", 31 },
            { "11", 30 },
            { "12", 31 }
        };
    }
}