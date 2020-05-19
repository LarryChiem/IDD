using Appserver.FormSubmit;
using Appserver.TextractDocument;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

        foreach (var page in doc.Pages)
        {
            if (page.GetTables().Count >= 1 && page.GetFormItems().Count < 8)
            {
                frontpage = page;
                frontfound = true;
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
        formitems = backpages[0].GetFormItems();

        form.serviceGoal = formitems[6].Value.ToString().Trim();
        form.progressNotes = formitems[7].Value.ToString().Trim();
        form.employerSignDate = formitems[8].Value.ToString().Trim();
        form.employerSignature = !string.IsNullOrEmpty(form.employerSignDate);
        form.providerSignDate = formitems[10].Value.ToString().Trim();
        form.providerSignature = !string.IsNullOrEmpty(form.providerSignDate);

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

    protected abstract void AddTables(List<Table> tables);
}