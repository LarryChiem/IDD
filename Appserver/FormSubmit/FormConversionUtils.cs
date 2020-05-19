using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace IDD
{
    public static class FormConversionUtils
    {
        // Convert PM time to 24hr time.
        // TODO make this not necessary?
        public static string TimeFormatter24(string t)
        {
            var ts = t.Split(':');
            int hours;
            try
            {
                hours = Convert.ToInt32(ts[0]);
            }
            catch (FormatException)
            {
                hours = 0;
            }
            hours = hours + 12;
            if (ts.Length < 2)
                return Convert.ToString(hours) + ":" + "00";
            else
                return Convert.ToString(hours) + ":" + ts[1];
        }

        // Add leading zero if needed.
        // TODO make this not necessary?
        public static string TimeFormatterPadding(string t)
        {
            var ts = t.Split(':');
            if (ts.Length < 2)
            {
                return "00:00";
            }
            if (ts[0].Length < 2)
            {
                string x = "0" + ts[0];
                return x + ":" + ts[1];
            }
            return t;
        }

        public static bool PWABoolConverter(string val)
        {
            val = val.ToLower();
            if (val == "true" || val == "yes")
            {
                return true;
            }

            return false;
        }


        // Removes spaces, replaces commas with hyphens,
        // and attempts to replace things like 1st, 2nd with
        // just the numeric equivalent.
        private static string DateStringCleaning(string s)
        {
            // Remove spaces, normal chars to lowercase
            s = s.ToLower();
            s = s.Replace(" ", "");
            s = s.Replace(",", "-");

            // Get rid of 1st, 2nd, 14th, etc. chars in string.
            // A bit hacky, but since regex.replace would replace the entire matched
            // pattern. August is the month that gets in the way...
            s = s.Replace("august", "aug");
            if (Regex.IsMatch(s, @"[1-9]+((st)|(rd)|(nd)|(th))"))
            {
                s = s.Replace("st", "");
                s = s.Replace("rd", "");
                s = s.Replace("nd", "");
                s = s.Replace("th", "");
            }

            s = NormalizeMonth(s);
            return s;
        }

        // Assumes a-b-c format, attemps to convert short
        // and long spelling of months into their numbered
        // equivalent.
        private static string NormalizeMonth(string s)
        {
            var l = s.Split("-");
            foreach (string x in l)
            {
                DateTime xd;
                if (DateTime.TryParseExact(
                    x,
                    new[] { "MMM", "MMMM" },
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out xd))
                {

                    // Switch out name for integer of month
                    var i = Array.IndexOf(l, x);
                    l[i] = xd.ToString("MM");
                    s = String.Join("-", l);

                    //System.Console.WriteLine("PARSED MONTH " + x);
                    //System.Console.WriteLine("TO: " + xd.ToString("MM"));
                    //System.Console.WriteLine("RETURNING: " + s);
                    return s;

                }
            }

            // Return original if no replacements were made
            return s;
        }

        // Attempts to parse the passed string into a valid
        // DateTime obj. via several methods. 
        private static bool DateStringCustomParser(string ts, out DateTime dt)
        {
            // Second pass try more specific formats
            try
            {
                DateTime.TryParseExact(
                    ts,
                    new[] { "yyyy-M-dd", "MMMM-D-yyyy", "y-M-dd", "y-M-d",
                            "yyyy-M-dd HH:mm tt", "MMMM-D-yyyy HH:mm tt",
                            "y-M-dd HH:mm tt", "y-M-d HH:mm tt"},
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out dt);
                return true;
            }
            catch (FormatException) { }

            dt = default;
            return false;
        }


        // Takes a string meant to represent a date and
        // attempts to convert it to a DateTime object. Otherwise,
        // a FormatException is thrown.
        public static DateTime DateStringConvertUtil(string s)
        {

            // Try the default parser
            DateTime dt;
            if (DateTime.TryParse(s, out dt))
            {
                // Did we at least parse a year in
                // the second millenia?
                if (dt.Year.ToString().Contains("20"))
                {
                    //System.Console.WriteLine("\nOUT: " + dt.ToString());
                    //System.Console.WriteLine("FROM: " + s);
                    return dt;
                }
                else
                {
                    throw new FormatException();
                }
            }

            // Clean up the passed string and try again
            var cleanx = DateStringCleaning(s);
            if (DateTime.TryParse(cleanx, out dt))
            {
                if (dt.Year.ToString().Contains("20"))
                {
                    //System.Console.WriteLine("\nOut Clean: " + dt.ToString());
                    //System.Console.WriteLine("FROM : " + s);
                    return dt;
                }
                else
                {
                    throw new FormatException();
                }
            }

            // Try more specific parser
            if (DateStringCustomParser(s, out dt))
            {
                if (dt.Year.ToString().Contains("20"))
                {
                    //System.Console.WriteLine("\nOUT Custom: " + dt.ToString());
                    //System.Console.WriteLine("FROM: " + s);
                    return dt;
                }
                else
                {
                    throw new FormatException();
                }
            }

            // Unable to parse string into DateTime
            throw new FormatException();
        }
    
        public static string TimeToDecimal( string s)
        {
            var stime = s.Split(':');
            double phour;
            if (stime.Length == 1)
            {
                try
                {
                    phour = double.Parse(stime[0]);
                }
                catch (FormatException)
                {
                    return s;
                }
            }
            else {
                phour = (double.Parse(stime[1]) / 60.0) + double.Parse(stime[0]);
            }
            return string.Format("{0:0.00}", phour);
        }
    }
}
