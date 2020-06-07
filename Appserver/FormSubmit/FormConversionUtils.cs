using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IDD
{
    public static class FormConversionUtils
    {

        /*******************************************************************************
        /// Methods
        *******************************************************************************/


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


        // Attempts to return a timestamp converted
        // to decimal. 
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


        // Given a PWAmodel, return bool reflecting if
        // the form was edited as all.
        public static bool wasPWAedited(PWAsubmission submission)
        {
            // Keys that could be edited
            var keyList = new List<string>();
            keyList.Add("clientName");
            keyList.Add("prime");
            keyList.Add("submissionDate");
            keyList.Add("providerName");
            keyList.Add("providerNum");
            keyList.Add("serviceAuthorized");
            keyList.Add("serviceGoal");
            keyList.Add("progressNotes");
            keyList.Add("employerSignDate");
            keyList.Add("employerSignature");
            keyList.Add("providerSignDate");
            keyList.Add("providerSignature");
            keyList.Add("authorization");
            keyList.Add("approval");
            keyList.Add("scpaName");
            keyList.Add("brokerage");
            keyList.Add("totalHours");
            keyList.Add("totalMiles");

            // Convert submission to JSON object
            string jsonString = JsonConvert.SerializeObject(submission);
            JObject obj = JObject.Parse(jsonString);

            // Iterate over obj with keys
            for (int i = 0; i < keyList.Count; i++)
            {
                // Exit early on first instance of true
                try
                {
                    string s = keyList[i];
                    bool x = (bool)obj[s]["wasEdited"];
                    if(x == true)
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            // Default case
            return false;
        }
    }
}
