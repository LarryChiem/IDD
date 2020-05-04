using Appserver.TextractDocument;
using System;

public abstract class AbstractFormObject{
    static public AbstractFormObject FromTextract(TextractDocument doc)
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

        
        t.clientName        = formitems[0].Value.ToString(); // Customer Name
        t.providerName      = formitems[1].Value.ToString(); // Provider Name
        t.prime             = formitems[2].Value.ToString(); // Prime
        t.providerNum       = formitems[3].Value.ToString(); // Provider Num
        t.brokerage         = formitems[4].Value.ToString(); // CM Organization
        t.scpaName          = formitems[5].Value.ToString(); // SC/PA Name
        t.serviceAuthorized = formitems[6].Value.ToString(); // Service
        
        //t.units = Int32.Parse( formitems[9].Value.ToString() );
        //t.type = formitems[10].Value.ToString();
        //t.frequency = formitems[11].Value.ToString();

        return t;
    }
}