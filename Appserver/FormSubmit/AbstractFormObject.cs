using Appserver.FormSubmit;
using Appserver.TextractDocument;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
    /// Front Fields
    public int id { get; set; }
    public string clientName { get; set; }
    public string prime { get; set; }
    public string providerName { get; set; }
    public string providerNum { get; set; }
    public string brokerage { get; set; }
    public string scpaName { get; set; }
    public string serviceAuthorized { get; set; }
    /// Back Fields
    public string serviceGoal { get; set; }
    public string progressNotes { get; set; }
    public bool employerSignature { get; set; }
    public string employerSignDate { get; set; }
    public bool providerSignature { get; set; }
    public string providerSignDate { get; set; }
    public bool authorization { get; set; }
    public bool approval { get; set; }
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
                // Search for Service Delivered On:
                foreach (var line in page.GetLines())
                {
                    // Ever form has "Service Delivered On:" on the front page, so we use
                    // this to determine if this is the front or back.
                    frontfound = line.ToString().Contains("vice Delivered O");
                    if (frontfound)
                        break;
                }
                if (frontfound)
                {
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
        var formitems = frontpage.GetFormItems();

        // Top Form Information

        form.clientName = formitems[0].Value.ToString().Trim(); // Customer Name 
        form.prime = formitems[1].Value.ToString().Trim(); // Prime 
        form.providerName = formitems[2].Value.ToString().Trim(); // Provider Name 
        form.providerNum = formitems[3].Value.ToString().Trim(); // Provider Num 
        form.brokerage = formitems[4].Value.ToString().Trim(); // CM Organization
        form.scpaName = formitems[5].Value.ToString().Trim(); // SC/PA Name
        form.serviceAuthorized = formitems[6].Value.ToString().Trim(); // Service
        
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

    public static string ConvertDate(string s)
    {
        string date;
        try
        {
            return DateTime.Parse(s).ToString("yyyy-MM-dd");
        }
        catch (FormatException)
        {
            return s;
        }
    }

    // If one of the rows in the cell matches the regex
    // and there is also a row that can be parsed into hours,
    // then we probably have a total hours cell. 
    public static bool isTotalTimeRow(List<Cell> lastrow)
    {
        // The letters of "total/unit/hours" without
        // "yes/no" for group field.
        Regex rxTotal = new Regex(@"([uthrli])+");
        Regex rxTime = new Regex(@"([0-9])+");
        bool matchRex = false;
        bool matchTime = false;

        foreach(var entry in lastrow)
        {
            // TotalHours match?
            var s = entry.ToString().Trim().ToLower();
            var totalmatches = rxTotal.Matches(s);
            if(totalmatches.Count >= 1){ matchRex = true; }

            // Time match?
            var time = FixHours(entry.ToString()).ToString().Trim();
            var timeMatches = rxTime.Matches(time.ToLower());
            if(timeMatches.Count >=1) { matchTime = true; }
        }

        return matchRex && matchTime;
    }


    public static bool isTotalMilesRow(List<Cell> lastrow)
    {
        Regex rxNumberGroup = new Regex(@"([0-9])+");
        int confidence = 0;
        bool foundDate = false;

        foreach(var entry in lastrow)
        {
            var s = entry.ToString().Trim().ToLower();

            // If we can parse the entry into a valid
            // DateTime format, probably areen't dealing
            // with a total miles row.
            try
            {
                var date = DateTime.Parse(s).ToString("yyyy-MM-dd");
                foundDate = true;           
            }
            catch (FormatException){}

            // Increase confidence we have a total miles row
            if (s.Contains("total"))
            {
                confidence++;
            }

            if (s.Contains("miles"))
            {
                confidence++;
            }

            var matches = rxNumberGroup.Matches(s);
            confidence += matches.Count;
        }

        // Most likely a mileage entry
        if (foundDate)
        {
            return false;
        }

        // Did we at least find a contains words
        // and at least one group of numbers that could
        // be a mileage total?
        return confidence >= 2;
    }


    protected abstract void AddTables(List<Table> tables);
    protected abstract void AddBackForm(Page page);
}