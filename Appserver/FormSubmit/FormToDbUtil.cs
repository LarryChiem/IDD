using System;
using Newtonsoft.Json.Linq;
using Common.Models;
using Common.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Collections;
using Appserver.Data;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IDD
{
    public class FormToDbUtil
    {
        private SubmissionContext _scontext;
        private SubmissionStagingContext _sscontext;

        public FormToDbUtil(SubmissionContext context, SubmissionStagingContext sscontext)
        {
            _scontext = context;
            _sscontext = sscontext;
        }


        // Use EF core to send data to DB
        public int TimesheetEFtoDB(Timesheet ts)
        {
            _scontext.Add(ts);
            _scontext.SaveChanges();
            _sscontext.Remove(ts.Id);
            _sscontext.SaveChanges();
            return ts.Id;
        }


        // Given a timesheet and a timesheetid referencing an existing timesheet
        // submission in the DB, insert the TimeEntry(s) in the TimeSheet. Get
        // back the number of records inserted.
        public int TimesheetEntriesToDB(Timesheet ts, int tsid)
        {
            string conn_str = Environment.GetEnvironmentVariable("test-db-conn-str");
            int totalInserts = 0;
            // Create connection for each entry
            // TODO bulk insert option? executemany?
            // FIXME add try/catch
            foreach (TimeEntry te in ts.TimeEntries)
            {

                string query = "INSERT INTO [dbo].[TimeEntry] ([Date],[Group],[Status],[In],[Out],[Hours],[TimesheetId]) ";
                query += "VALUES(@date, @group, @status, @in, @out, @hours, @timesheetid) SELECT SCOPE_IDENTITY();";

                SqlConnection conn = new SqlConnection(conn_str);
                SqlCommand command = null;
                SqlDataReader reader = null;

                System.Console.WriteLine("ENTRY SQL: " + query);
                command = new SqlCommand(query, conn);
                command.Parameters.Add("@date", System.Data.SqlDbType.DateTime2);
                command.Parameters["@date"].Value = te.Date;

                command.Parameters.Add("@group", System.Data.SqlDbType.Bit);
                command.Parameters["@group"].Value = 1;

                command.Parameters.Add("@status", System.Data.SqlDbType.NVarChar, 25);
                command.Parameters["@status"].Value = te.Status;

                command.Parameters.Add("@in", System.Data.SqlDbType.DateTime2);
                command.Parameters["@in"].Value = te.In;

                command.Parameters.Add("@out", System.Data.SqlDbType.DateTime2);
                command.Parameters["@out"].Value = te.Out;

                command.Parameters.Add("@hours", System.Data.SqlDbType.Float);
                command.Parameters["@hours"].Value = te.Hours;

                command.Parameters.Add("@timesheetid", System.Data.SqlDbType.Int);
                command.Parameters["@timesheetid"].Value = tsid;

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

            string query = "INSERT INTO [dbo].[Submissions] ([Submitted],[FormType],[ProviderName]";
            query += ",[ProviderId],[ClientName],[ClientPrime],[ServiceGoal],[ProgressNotes],[Status]";
            query += ",[UserActivity],[RejectionReason],[LockInfoId],[UriString],[Discriminator],[TotalMiles],[TotalHours]) ";
            query += "VALUES(@submitted, @formtype, @providername, @providerid, @clientname, @clientprime, @servicegoal,";
            query += "@progressnotes, @status, @useractivity, @rejectionreason, @lockinfoid, @uristring, @discriminator,";
            query += "@totalmiles, @totalhours) SELECT SCOPE_IDENTITY();";

            string conn_str = Environment.GetEnvironmentVariable("test-db-conn-str");
            using (SqlConnection conn = new SqlConnection(conn_str))
            {
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.Add("@submitted", System.Data.SqlDbType.DateTime2);
                command.Parameters["@submitted"].Value = ts.Submitted;

                command.Parameters.Add("@formtype", System.Data.SqlDbType.NVarChar, 50);
                command.Parameters["@formtype"].Value = ts.FormType;

                command.Parameters.Add("@providername", System.Data.SqlDbType.NVarChar, 50);
                command.Parameters["@providername"].Value = ts.ProviderName;

                command.Parameters.Add("@providerid", System.Data.SqlDbType.NVarChar, 50);
                command.Parameters["@providerid"].Value = ts.ProviderId;

                command.Parameters.Add("@clientname", System.Data.SqlDbType.NVarChar, 50);
                command.Parameters["@clientname"].Value = ts.ClientName;

                command.Parameters.Add("@clientprime", System.Data.SqlDbType.NVarChar, 50);
                command.Parameters["@clientprime"].Value = ts.ClientPrime;

                command.Parameters.Add("@servicegoal", System.Data.SqlDbType.NVarChar, 100);
                command.Parameters["@servicegoal"].Value = ts.ServiceGoal;

                command.Parameters.Add("@progressnotes", System.Data.SqlDbType.NVarChar, 500);
                command.Parameters["@progressnotes"].Value = ts.ProgressNotes;

                command.Parameters.Add("@status", System.Data.SqlDbType.NVarChar, 50);
                command.Parameters["@status"].Value = ts.Status;

                command.Parameters.Add("@useractivity", System.Data.SqlDbType.NVarChar, 100);
                command.Parameters["@useractivity"].Value = ts.UserActivity;

                command.Parameters.Add("@rejectionreason", System.Data.SqlDbType.NVarChar, 250);
                command.Parameters["@rejectionreason"].Value = ts.RejectionReason;

                command.Parameters.Add("@lockinfoid", System.Data.SqlDbType.Int);
                command.Parameters["@lockinfoid"].Value = DBNull.Value;

                command.Parameters.Add("@uristring", System.Data.SqlDbType.NVarChar, 500);
                command.Parameters["@uristring"].Value = ts.UriString;

                command.Parameters.Add("@discriminator", System.Data.SqlDbType.NVarChar, 50);
                command.Parameters["@discriminator"].Value = "Timesheet";

                command.Parameters.Add("@totalmiles", System.Data.SqlDbType.Float);
                command.Parameters["@totalmiles"].Value = 0;

                command.Parameters.Add("@totalhours", System.Data.SqlDbType.Float);
                command.Parameters["@totalhours"].Value = ts.TotalHours;

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
            tsheet.RejectionReason = ""; //
            tsheet.Submitted = DateTime.Now; //
            tsheet.LockInfo = null;
            tsheet.UserActivity = ""; //
            
            tsheet.UriString = _sscontext.Stagings.Find(tsf.id).UriString;

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
