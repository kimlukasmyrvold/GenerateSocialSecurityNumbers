using System.Text.RegularExpressions;

namespace fnum;

public static partial class Program
{
    [GeneratedRegex("^(0[1-9]|[12][0-9]|3[01])(0[1-9]|1[012])\\d{7}$")]
    private static partial Regex FnumRegex();
    
    // Check if date is valid
    private static bool CheckDate(IReadOnlyList<ushort> fnumList)
    {
        var date = "";
        for (ushort i = 0; i < 6; i++)
        {
            date += fnumList[i];
        }
        return DateTime.TryParseExact(date, "ddMMyy", null, System.Globalization.DateTimeStyles.None, out _);
    }

    // Check if a Fnum is valid    
    private static bool CheckFnumValid(IReadOnlyList<ushort> fnumList)
    {
        var fnum = "";
        ushort[] check1 = { 3, 7, 6, 1, 8, 9, 4, 5, 2, 1, 0 };
        ushort[] check2 = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2, 1 };

        ushort checksum1 = 0;
        ushort checksum2 = 0;

        for (ushort i = 0; i < fnumList.Count; i++)
        {
            fnum += fnumList[i];
            checksum1 += (ushort)(fnumList[i] * check1[i]);
            checksum2 += (ushort)(fnumList[i] * check2[i]);
        }

        var regexOk = FnumRegex().IsMatch(fnum);
        return regexOk && CheckDate(fnumList) && CleanDivide(checksum1, 11) && CleanDivide(checksum2, 11);
    }

    // Checks if there's an clean divide between two numbers.
    private static bool CleanDivide(ushort n1, ushort n2)
    {
        // ReSharper disable once PossibleLossOfFraction
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return (float)n1 / n2 == n1 / n2; // Same as (return n1 % n2 == 0)
    }

    // Increase the value of fnum
    private static string IncreaseFnum(string fnum)
    {
        var num = ulong.Parse(fnum) + 1;
        return num.ToString("00000000000");
    }

    // Generate all fnums
    private static void GenerateFnums()
    {
        var fnum = "01010000000";

        do
        {
            // Increase the fnum by 1
            fnum = IncreaseFnum(fnum);

            // Adding each number in fnum to fnumList
            var fnumList = new List<ushort>();
            foreach (var ch in fnum)
            {
                if (ushort.TryParse(ch.ToString(), out var result)) fnumList.Add(result);
            }

            // Check if month is 13 then change the fnum to be 01 again to not try for months above 13
            var month = int.Parse(fnumList[2] + "" + fnumList[3]);
            if (month == 13)
            {
                var date = int.Parse(fnumList[0] + "" + fnumList[1]) + 1;
                fnum = date + "010000000";
            }

            // Skipping rest of code if fnum is invalid
            if (!CheckFnumValid(fnumList)) continue;

            Console.WriteLine(fnum);
        } while (ulong.Parse(fnum) <= 31130000000);
    }


    // Checking if a Fnum is valid and writing to Console
    private static void WriteFnumIsValid()
    {
        const string fNum = "01129988483";
        var fnumList = new List<ushort>();
        foreach (var ch in fNum)
        {
            if (ushort.TryParse(ch.ToString(), out var result))
            {
                fnumList.Add(result);
            }
        }

        Console.WriteLine(CheckFnumValid(fnumList));
    }

    private static void Main()
    {
        // WriteFnumIsValid();
        GenerateFnums();
    }
}