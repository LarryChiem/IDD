using Appserver.TextractDocument;
using System;

public abstract class AbstractFormObject{
    public static AbstractFormObject FromTextract(TextractDocument doc)
    {
        // Here we'll Determine the type of object (timesheet or mileage form) and then
        // return the correct type.
        // For now, assume TimesheetForm

        TimesheetForm t = new TimesheetForm();

        // Grab the first page
        Page p;
        try
        {
            p = doc.GetPage(0);
        }catch( System.ArgumentOutOfRangeException e)
        {
            Console.WriteLine(e.Message);
            return t;
        }

        // Assume we got the page
        var formitems = p.GetFormItems();

        // Top Form Information
        
        t.clientName        = formitems[0].Value.ToString(); // Customer Name
        t.providerName      = formitems[1].Value.ToString(); // Provider Name
        t.prime             = formitems[2].Value.ToString(); // Prime
        t.providerNum       = formitems[3].Value.ToString(); // Provider Num
        t.brokerage         = formitems[4].Value.ToString(); // CM Organization
        t.scpaName          = formitems[5].Value.ToString(); // SC/PA Name
        t.serviceAuthorized = formitems[6].Value.ToString(); // Service

        // Table
        var tables = p.GetTables();
        if(tables.Count == 0)
        {
            Console.WriteLine("No Table Information");
            return t;
        }

        var table = tables[0].GetTable();
        // Remove first row
        table.RemoveAt(0);
        foreach( var row in table)
        {
            t.addTimeRow(row[0].ToString(), row[1].ToString(), row[2].ToString(), ConvertHours(row[3].ToString()), ConvertInt(row[4].ToString()));
        }
        //t.units = Int32.Parse( formitems[9].Value.ToString() );
        //t.type = formitems[10].Value.ToString();
        //t.frequency = formitems[11].Value.ToString();

        return t;
    }
    public static DateTime ConvertDate( string s )
    {
        return DateTime.Now;
    }
    public static float ConvertHours( string s )
    {
        // Try to read in only the number portion
        string num = "";
        bool encounteredDecimal = false;
        bool invalid = false;
        foreach( var c in s.ToCharArray())
        {
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    num += c;
                    break;
                case '.':
                    if (encounteredDecimal)
                    {
                        invalid = true;
                        break;
                    }
                    encounteredDecimal = true;
                    num += c;
                    break;
                default:
                    invalid = true;
                    break;
            }
            // If we've encountered a second decimal then we break out of loop
            if (invalid) break;
        }
        if (num.Length == 0)
            return 0;
        return int.Parse(num);
    }

    public static int ConvertInt( string s)
    {
        s = s.ToLower();
        if (s == "y" || s == "yes" )
            return 1;
        return 0;
    }
}