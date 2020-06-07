using Appserver.FormSubmit;
using Appserver.TextractDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public abstract class AbstractFormObject{

    /*******************************************************************************
    /// Enums
    *******************************************************************************/
    public enum FormType
    {
        OR004_MILEAGE = 1,
        OR507_RELIEF = 2,
        OR526_ATTENDANT = 3,
    }

    /*******************************************************************************
    /// Fields
    *******************************************************************************/
    private static  List<string> keys = new List<string>{
                "Customer Name:",
                "Provider Name:",
                "CM Organization:",
                "Service:",
                "Prime:",
                "Provider Num:",
                "SC/PA Name:"
        };
    public static double tolerance = 0.25; // Allows for 5 edits
    /*******************************************************************************
    /// Properties
    *******************************************************************************/
    /// Front Fields
    public int id { get; set; } = -1;
    public string guid { get; set; } = "";
    public string clientName { get; set; } = "";
    public string prime { get; set; } = "";
    public string providerName { get; set; } = "";
    public string providerNum { get; set; } = "";
    public string brokerage { get; set; } = "";
    public string scpaName { get; set; } = "";
    public string serviceAuthorized { get; set; } = "";
    /// Back Fields
    public string serviceGoal { get; set; } = "";
    public string progressNotes { get; set; } = "";
    public bool employerSignature { get; set; } = false;
    public string employerSignDate { get; set; } = "";
    public bool providerSignature { get; set; } = false;
    public string providerSignDate { get; set; } = "";
    public bool authorization { get; set; } = false;
    public string approval { get; set; } = "";
    public string review_status { get; set; } = "Pending";

    /*******************************************************************************
    /// Static Methods
    *******************************************************************************/

    public static AbstractFormObject FromTextract(TextractDocument doc, FormType formType)
    {
        // Here we'll Determine the type of object (timesheet or mileage form) and then
        // return the correct type.

        // Grab the first page and make sure it is the front
        if (doc.PageCount() < 2)
        {
            throw new ArgumentException();
        }

        AbstractFormObject form;
        switch (formType)
        {
            case FormType.OR526_ATTENDANT:
            case FormType.OR507_RELIEF:
                form = new TimesheetForm();
                break;
            case FormType.OR004_MILEAGE:
                form = new MileageForm();
                break;
            default:
                throw new ArgumentException();
        }
        // Do a silly assignment because C# won't let me assign the variable in the foreach loop instead
        // and there is no default constructor
        Page frontpage = doc.GetPage(0);
        bool frontfound = false;
        List<Page> backpages = new List<Page>();

        // Improve front page detection
        foreach (var page in doc.Pages)
        {
            if (!frontfound)
            {
                bool servicefound = false;
                // Search for Service Delivered On:
                foreach (var line in page.GetLines())
                {
                    // Ever form has "Service Delivered On:" on the front page, so we use
                    // this to determine if this is the front or back.
                    // We check if the distance of the string is within tolerance of NGLD.
                    if( !servicefound )
                        servicefound = NGLD("Service Delivered On:", line.ToString()) < tolerance;

                    // To protect against accidentally finding service delivered on the back we check
                    // if this is the back also. We take the first 14 characters as this is common on
                    // all versions of the back sheet.
                    if( NGLD("PROGRESS NOTES", Substring( line.ToString(),0,13)) < tolerance)
                    {
                        servicefound = false;
                        break; // Quit the loop as we found something on the back page.
                    }
                }
                if (servicefound)
                {
                    frontfound = true;
                    frontpage = page;
                }
                else
                {
                    backpages.Add(page);
                }
            }
            else
            {
                backpages.Add(page);
            }
        }

        if (!frontfound)
        {
            throw new ArgumentException();
        }
        // Top Form Information
        // grab the list of form keys and match
        var matches = MatchKeyValuePairs(keys, frontpage.GetKeys());
        var mapping = new Dictionary<string, string>(matches);

        var formitems = frontpage.GetFormItems();
        var formDict = new Dictionary<string, string>(formitems.Count);
        foreach(var item in formitems)
        {
            formDict.Add(item.Key.ToString(), item.Value.ToString().Trim());
        }

        if (!formDict.ContainsKey(""))
            formDict.Add("", ""); // Add in empty string match for missing items

        form.clientName         = formDict[mapping[keys[0]]]; // Customer Name 
        form.providerName       = formDict[mapping[keys[1]]]; // Provider Name 
        form.brokerage          = formDict[mapping[keys[2]]]; // CM Organization
        form.serviceAuthorized  = formDict[mapping[keys[3]]]; // Service
        form.prime              = formDict[mapping[keys[4]]]; // Prime 
        form.providerNum        = formDict[mapping[keys[5]]]; // Provider Num 
        form.scpaName           = formDict[mapping[keys[6]]]; // SC/PA Name

        // Table
        var tables = frontpage.GetTables();
        if (tables.Count == 0)
        {
            Console.WriteLine("No Table Information");
            return form;
        }
        form.AddTables(tables);
        // Populate back form objects
        form.AddBackForm(backpages[0]);

        return form;
    }
    public static string FixHours( string s )
    {
        return s.Replace(".", ":");
    }

    public static int ConvertInt( string s)
    {
        s = s.ToLower();
        if (s == "y" || s == "yes" )
            return 1;
        return 0;
    }

    // Converts a date if possible, or returns the string for user to edit
    public static string ConvertDate(string s)
    {
        try
        {
            return DateTime.Parse(s).ToString("yyyy-MM-dd");
        }
        catch (FormatException)
        {
            return s;
        }
    }

    // Returns true if the timesheet row is likely to be empty.
    // Not concerned with Group value.
    public static bool isEmptyTimesheetRow(List<Cell> row)
    {
        var subdate = ConvertDate(row[0].ToString().Trim());
        var starttime = FixHours(row[1].ToString()).ToString().Trim();
        var endtime = FixHours(row[2].ToString()).ToString().Trim();
        var totalHours = FixHours(row[3].ToString()).ToString().Trim();

        bool emptyDate = (subdate == "");
        bool emptyStart = (starttime == "AM PM" || starttime == "PM AM");
        bool emptyEnd = (endtime == "AM PM" || endtime == "PM AM");
        bool emptyHours = (totalHours == "");

        int count = 0;

        if (emptyDate) { count++; }
        if (emptyStart) { count++; }
        if (emptyEnd) { count++; }
        if (emptyHours) { count++; }

        // It's possible someone forget to write the total hours,
        // or the starting date for the row. But, too many misses (>1)
        // and it's likely to be an empty row
        if(count >=3) { return true; }

        // Default Case
        return false;
    }


    // Returns true if the mileage form row is likely to be empty.
    // Not concerned with the Group value.
    public static bool isEmptyMileageRow(List<Cell> row)
    {
        var subdate = ConvertDate(row[0].ToString().Trim());
        var miles = row[1].ToString().Trim();
        var purpose = row[3].ToString().Trim();

        bool emptyDate = (subdate == "");
        bool emptyMiles = (miles == "");
        bool emptyPurpose = (purpose == "");

        int count = 0;

        if (emptyDate) { count++; };
        if (emptyMiles) { count++; }
        if (emptyPurpose) { count++; }

        // Leave some room for error
        if(count >= 2) { return true; }

        // Default Case
        return false;
    }

    public static int minimum(params int[] rest)
    {
        int min = int.MaxValue;
        foreach( var y in rest)
        {
            if (y < min)
                min = y;
        }
        return min;
    }
    // This takes two strings and returns the generalized levenshtein distance which is
    // the number of edits (inserts, deletions, and substitutions) required to convert 
    // one string to the other.
    public static int LevenshteinDistance(string s, string t)
    {
        // Initialize rows
        List<int> v0 = new List<int>(Enumerable.Range(0, t.Length+1).ToList<int>());
        List<int> v1 = new List<int>(Enumerable.Repeat(0,t.Length+1).ToList<int>());

        for ( int i = 0; i < s.Length; ++i)
        {
            // Calculate distances to v1, first column is number of deletions compared
            // to an empty string

            v1[0] = i + 1;

            for( int j = 0; j < t.Length; ++j)
            {
                int sub = s[i] == t[j] ? v0[j] : v0[j] + 1;
                v1[j + 1] = minimum( v0[j + 1] + 1, v1[j] + 1, s[i] == t[j] ? v0[j] : v0[j] + 1);
            }

            // Copy current row to previous row
            // No need to swap as v1 is invalidated
            v0 = new List<int>(v1);
        }

        return v0[t.Length];
    }

    // Takes two strings and returns the Normalized General Levenshtein Distance
    public static double NGLD(string s, string t)
    {
        double gld = LevenshteinDistance(s, t);
        return (2.0 * gld/((double)(s.Length + t.Length + gld)));
    }

    // This takes two lists of strings and creates a mapping between keys and values.
    // If the set of values is less than the set of keys then it removes duplicates
    // and replaces them with an empty string.
    public static List<KeyValuePair<string,string>> MatchKeyValuePairs(List<string> keys, List<string> values)
    {
        var matches = new List<KeyValuePair<string, string>>(keys.Count);

        // NGLD forms a metric on the space which means that NGLD(X,Y)==NGLD(Y,X)

        // For each key we want to find minimum along the row
        // First create distance matrix
        List<List<double>> matrix = new List<List<double>>(keys.Count);
        List<int> matchedIndex = new List<int>(keys.Count);

        for(int i = 0; i < keys.Count; ++i)
        {
            matrix.Add(new List<double>(Enumerable.Repeat(double.MaxValue, values.Count)));
        }

        // Now calculate distances
        for( int i = 0; i < keys.Count; ++i)
        {
            // Track minimum index
            int minIndex = 0;
            for( int j = 0; j < values.Count; ++j)
            {
                matrix[i][j] = NGLD(keys[i], values[j]);
                if (matrix[i][j] < matrix[i][minIndex])
                    minIndex = j;
            }

            // Add to matches
            matches.Add(new KeyValuePair<string, string>(keys[i], values[minIndex]));
            matchedIndex.Add(minIndex);
        }

        // If fewer values than keys, keep only the best match, replacing others with
        // empty string
        if (values.Count < keys.Count) {
            for (int i = 0; i < keys.Count - 1; ++i)
            {
                // This is going to be inefficient as we'll assign twice to the one which
                // is further away, once the first time we find it, and again the second time
                // we encounter it
                var index1 = matchedIndex.IndexOf(i);
                if (index1 != keys.Count - 1)
                {
                    var index2 = matchedIndex.IndexOf(i, index1 + 1);

                    // No second index was found then continue on, or else find best index
                    if( index2 != -1)
                    {
                        if( matrix[index1][i] < matrix[index2][i])
                        {
                            matches[index2] = new KeyValuePair<string, string>(matches[index2].Key,"");
                        }
                        else
                        {
                            matches[index1] = new KeyValuePair<string, string>(matches[index1].Key, "");
                        }
                    }
                }
            }
        }
        return matches;
    }


    protected abstract void AddTables(List<Table> tables);
    protected void AddBackForm(Page page)
    {
        var keys = new List<string>()
        {
            "SERVICE GOAL:",
            "PROGRESS NOTES (attach additional pages if needed):",
            "Date:",
            "Date:"
        };
        // We'll need to take care of the repeated "Date" key
        var matches = MatchKeyValuePairs(keys, page.GetKeys());
        matches[keys.Count - 1] = new KeyValuePair<string,string>("Date2:",matches[keys.Count-1].Value);
        var mapping = new Dictionary<string, string>(matches);

        // We'll need to handle the repeated key here also
        var formitems = page.GetFormItems();
        var formDict = new Dictionary<string, string>(formitems.Count);

        foreach (var item in formitems)
        {
            try
            {
                formDict.Add(item.Key.ToString(), item.Value.ToString().Trim());
            }
            catch (ArgumentException)
            {
                // Assuming this is the second date on the form.
                if (String.IsNullOrEmpty( providerSignDate))
                    providerSignDate = ConvertDate(item.Value.ToString().Trim());
            }
        }

        if( !formDict.ContainsKey("") )
            formDict.Add("", ""); // Add in empty string match for missing items

        serviceGoal = formDict[mapping[keys[0]]];
        progressNotes = formDict[mapping[keys[1]]];
        employerSignDate = ConvertDate(formDict[mapping[keys[2]]]);
        employerSignature = !string.IsNullOrEmpty(employerSignDate);
        // Provider Sign Date taken care of above
        providerSignature = !string.IsNullOrEmpty(providerSignDate);
    }

    // This returns a substring starting at start and ending at either end
    // or Count.
    public static string Substring(string s, int start, int end)
    {
        if( start < 0 || end < 0)
        {
            throw new ArgumentException();
        }
        if( s.Length < start)
        {
            throw new IndexOutOfRangeException();
        }

        if( end == 0)
        {
            return "";
        }
        if( start + end >= s.Length)
        {
            return s.Substring(start, s.Length - start);
        }
        return s.Substring(start, end);

        // Calculate end
    }
}
