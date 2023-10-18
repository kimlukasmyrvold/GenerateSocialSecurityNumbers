using System.Text.RegularExpressions;

namespace fnum;

public static partial class Program
{
    // Check if a Fnum is valid    
    private static bool FnumIsValid(IReadOnlyList<ushort> fnumList)
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

        var regexOk = MyRegex().IsMatch(fnum);
        return regexOk && CleanDivide(checksum1, 11) && CleanDivide(checksum2, 11);
    }

    // Checks if there's an clean divide between two numbers.
    private static bool CleanDivide(ushort n1, ushort n2)
    {
        // ReSharper disable once PossibleLossOfFraction
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return (float)n1 / n2 == n1 / n2; // Same as (return n1 % n2 == 0)
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

        Console.WriteLine(FnumIsValid(fnumList));
    }

    // Generates a fnum
    private static string GenerateFnum(string fnum)
    {
        var num = int.Parse(fnum) + 1;
        return num.ToString("00000000000");
    }

    private static void Main()
    {
        // WriteFnumIsValid();
        
        
        var fnum = "01120000000";
        
        // for (float i = 0; i < 99999999999; i++)
        do
        {
            fnum = GenerateFnum(fnum);
        
            var fnumList = new List<ushort>();
            foreach (var ch in fnum)
            {
                if (ushort.TryParse(ch.ToString(), out var result))
                {
                    fnumList.Add(result);
                }
            }
        
            if (!FnumIsValid(fnumList)) continue;
        
            Console.WriteLine(fnum);
            Console.WriteLine(fnum[2] + "" + fnum[3]);
        } while (long.Parse(fnum) <= 31130000000);
    }

    [GeneratedRegex("^(0[1-9]|[12][0-9]|3[01])(0[1-9]|1[012])\\d{7}$")]
    private static partial Regex MyRegex();
}