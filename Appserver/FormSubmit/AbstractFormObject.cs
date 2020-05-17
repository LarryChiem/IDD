using Appserver.TextractDocument;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class AbstractFormObject{
    public static AbstractFormObject FromTextract(TextractDocument doc)
    {
        // Here we'll Determine the type of object (timesheet or mileage form) and then
        // return the correct type.
        // For now, assume TimesheetForm

        TimesheetForm t = new TimesheetForm();

        // Grab the first page and make sure it is the front
        if( doc.PageCount() < 2)
        {
            throw new ArgumentException();
        }

        // Do a silly assignment because C# won't let me assign the variable in the foreach loop instead
        // and there is no default constructor
        Page frontpage = doc.GetPage(0);
        bool frontfound = false;
        List<Page> backpages = new List<Page>();

        foreach( var page in doc.Pages)
        {
            if (page.GetTables().Count >= 1)
            {
                frontpage = page;
                frontfound = true;
            }
            else
            {
                backpages.Add(page);
            }
        }

        if( !frontfound )
        {
            throw new ArgumentException();
        }

        var formitems = frontpage.GetFormItems();

        // Top Form Information
        
        t.clientName        = formitems[0].Value.ToString().Trim(); // Customer Name 
        t.prime             = formitems[1].Value.ToString().Trim(); // Prime 
        t.providerName      = formitems[2].Value.ToString().Trim(); // Provider Name 
        t.providerNum       = formitems[3].Value.ToString().Trim(); // Provider Num 
        t.brokerage         = formitems[4].Value.ToString().Trim(); // CM Organization
        t.scpaName          = formitems[5].Value.ToString().Trim(); // SC/PA Name
        t.serviceAuthorized = formitems[6].Value.ToString().Trim(); // Service

        // Table
        var tables = frontpage.GetTables();
        if(tables.Count == 0)
        {
            Console.WriteLine("No Table Information");
            return t;
        }

        var table = tables[0].GetTable();
        // Remove first row
        table.RemoveAt(0);

        // Grab last row for total
        var lastrow = table.Last();
        // Now remove it
        table.RemoveAt(table.Count - 1);

        foreach( var row in table)
        {
            t.addTimeRow(row[0].ToString().Trim(),
                FixHours(row[1].ToString()).ToString().Trim(),
                FixHours(row[2].ToString()).ToString().Trim(),
                FixHours(row[3].ToString()).ToString().Trim(), 
                ConvertInt(row[4].ToString()).ToString().Trim());
        }

        if (lastrow.Count > 3)
        {
            try
            {
                t.totalHours = FixHours(lastrow[2].ToString()).Trim();
            }catch(FormatException)
            {
                t.totalHours = lastrow[2].ToString();
            }
        }

        // Populate back form objects
        formitems = backpages[0].GetFormItems();

        t.serviceGoal = formitems[6].Value.ToString().Trim();
        t.progressNotes = formitems[7].Value.ToString().Trim();
        t.employerSignDate = formitems[8].Value.ToString().Trim();
        t.employerSignature = !string.IsNullOrEmpty(t.employerSignDate);
        t.providerSignDate = formitems[10].Value.ToString().Trim();
        t.providerSignature = !string.IsNullOrEmpty(t.providerSignDate);
        // t.authorization

        return t;
    }
    public static DateTime ConvertDate( string s )
    {
        return DateTime.Now;
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
}