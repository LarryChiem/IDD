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
            p = doc.GetPage(1);
        }catch( System.ArgumentOutOfRangeException e)
        {
            Console.WriteLine(e.Message);
            return t;
        }

        // Assume we got the page
        var formitems = p.GetFormItems();

        t.clientName = formitems[0].Value.ToString();
        t.providerName = formitems[1].Value.ToString();
        t.prime = formitems[3].Value.ToString();
    t.providerName = formitems[4].Value.ToString();
    t.providerNum = formitems[5].Value.ToString();
    t.brokerage = formitems[6].Value.ToString();
    t.scpaName = formitems[7].Value.ToString();
    t.serviceAuthorized = formitems[8].Value.ToString();
    t.units = Int32.Parse( formitems[9].Value.ToString() );
    t.type = formitems[10].Value.ToString();
    t.frequency = formitems[11].Value.ToString();

        return t;
    }
}