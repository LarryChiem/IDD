using System;
using Newtonsoft.Json.Linq;
using Common.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace IDD
{
    public class FormToDbUtil
    {
        public FormToDbUtil(){}


        // Given a timesheet and a timesheetid referencing an existing timesheet
        // submission in the DB, insert the TimeEntry(s) in the TimeSheet. Get
        // back the number of records inserted.
        public int TimesheetEntriesToDB(Timesheet ts, int tsid)
        {
            string conn_str = "YourConnString";

            int totalInserts = 0;
            // Create connection for each entry
            // TODO bulk insert option? executemany?
            // FIXME add try/catch
            foreach (TimeEntry te in ts.TimeEntries)
            {
                string query = "INSERT INTO [dbo].[TimeEntry] ";
                query += "([Date],[Group],[Status],[In],[Out],[Hours],[TimesheetId]) VALUES('";
                query += te.Date.ToString() + "', ";
                query +=  "1, '";
                query += te.Status + "', '";
                query += te.In.ToString() + "', '";
                query += te.Out.ToString() + "', ";
                query += te.Hours + ", ";
                query += tsid + ") SELECT SCOPE_IDENTITY();";

                SqlConnection conn = new SqlConnection(conn_str);
                SqlCommand command = null;
                SqlDataReader reader = null;

                System.Console.WriteLine("ENTRY SQL: " + query);
                command = new SqlCommand(query, conn);
                conn.Open();

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    decimal id = reader.GetDecimal(0);
                    if (id > 0)
                    {
                        totalInserts += 1;
                    }
                }
                reader.Close();
            }
            return totalInserts;
        }

        // Submit the timesheet portion first to get the insert ID back.
        // TODO this could be improved by making 'timesheet' a variable in
        // the query portion, change the name of the method to something more
        // general, and it can acommodate multiple form types....?
        // FIXME add try/catch
        public string TimesheetToDB(Timesheet ts)
        {
            decimal insertId = 0;

            string query = "INSERT into Submissions (";
            query += "submitted, formtype, providername, providerid, clientname, ";
            query += "clientprime, servicegoal, progressnotes, status, useractivity, ";
            query += "rejectionreason, lockinfoid, uristring, discriminator, totalmiles, totalhours) ";
            query += "VALUES( '" + ts.Submitted.ToString() + "', '";
            query += ts.FormType + "', '";
            query += ts.ProviderName + "', '";
            query += ts.ProviderId + "', '";
            query += ts.ClientName + "', '";
            query += ts.ClientPrime + "', '";
            query += ts.ServiceGoal + "', '";
            query += ts.ProgressNotes + "', '";
            query += ts.Status + "', '";
            query += ts.UserActivity + "', '";
            query += ts.RejectionReason + "', ";
            query += "NULL, '";
            query += ts.UriString + "', '";
            query +=  "Timesheet', ";
            query += "0, ";
            query += ts.TotalHours + ") SELECT SCOPE_IDENTITY();";

            string conn_str = "YourConnString";

            using (SqlConnection conn = new SqlConnection(conn_str))
            {
                SqlCommand command = new SqlCommand(query, conn);
                conn.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        insertId = reader.GetDecimal(0);
                    }
                    reader.Close();
                }
            }

            return insertId.ToString();
        }

        // Give a timesheetform obj, get back a partially populated timesheet obj.
        // TODO fix UriString, confirm vals for // marks
        public Timesheet PopulateTimesheet(TimesheetForm tsf, Timesheet tsheet=null)
        {
            if(tsheet == null){tsheet = new Timesheet();}

            tsheet.ClientName = tsf.clientName;
            tsheet.ClientPrime = tsf.prime;
            tsheet.ProviderName = tsf.providerName;
            tsheet.ProviderId = tsf.providerNum;
            tsheet.ServiceGoal = tsf.serviceGoal;
            tsheet.ProgressNotes = tsf.progressNotes;
            tsheet.FormType = tsf.serviceAuthorized;
            tsheet.RejectionReason = null; //
            tsheet.Submitted = DateTime.Now; //
            tsheet.LockInfo = null;
            tsheet.UserActivity = null; //
            tsheet.UriString = "somehandle, someotherhandle"; //

            return tsheet;
        }

        // Convert the timesheet form row items into timesheet time entries. Makes
        // certain assumptions about start times, end times, and group. 
        public void PopulateTimesheetEntries(TimesheetForm tsf, Timesheet tsheet)
        {
            double totalHours = 0;
            var tl = new List<TimeEntry>();

            foreach (TimesheetRowItem tsri in tsf.Times)
            {
                var x = new TimeEntry();
                x.Date = Convert.ToDateTime(tsri.date);
                x.Hours = tsri.totalHours;
                totalHours += x.Hours;

                // Assume Group field is 'N'
                x.Group = true;

                // Assume starttime is AM, pad with leading zero if necessary
                string sdf = TimeFormatterPadding(tsri.starttime);
                string sd = tsri.date + " " + sdf + " AM";
                x.In = DateTime.ParseExact(sd, "yyyy-MM-dd HH:mm tt", null);

                // Assume endtime is PM, convert to 24hr.
                string edf = TimeFormatter24(tsri.endtime);
                string ed = tsri.date + " " + edf + " PM";
                x.Out = DateTime.ParseExact(ed, "yyyy-MM-dd HH:mm tt", null);

                tl.Add(x);
            }

            tsheet.TotalHours = totalHours;
            tsheet.TimeEntries = tl;   
        }


        // Convert PM time to 24hr time.
        // TODO make this not necessary?
        public string TimeFormatter24(string t)
        {
            var ts = t.Split(':');
            int hours = Convert.ToInt32(ts[0]);
            hours = hours + 12;
            return Convert.ToString(hours) + ":" + ts[1];
        }

        // Add leading zero if needed.
        // TODO make this not necessary?
        public string TimeFormatterPadding(string t)
        {
            var ts = t.Split(':');
            if(ts[0].Length < 2)
            {
                string x = "0" + ts[0];
                return x + ":" + ts[1];
            }
            return t;
        }

    }
}
