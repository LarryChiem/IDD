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
using convUtil = IDD.FormConversionUtils;
using System.Linq;

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

        // Give a timesheetform obj, get back a partially populated timesheet obj.
        // TODO fix UriString, confirm vals for // marks
        public Timesheet PopulateTimesheet(PWATimesheet tsf, Timesheet tsheet=null)
        {
            if(tsheet == null){tsheet = new Timesheet();}

            tsheet.ClientName = tsf.clientName.value;
            tsheet.ClientPrime = tsf.prime.value;
            tsheet.ProviderName = tsf.providerName.value;
            tsheet.ProviderId = tsf.providerNum.value;
            tsheet.ServiceGoal = tsf.serviceGoal.value;
            tsheet.ProgressNotes = tsf.progressNotes.value;
            tsheet.FormType = tsf.serviceAuthorized.value;
            tsheet.RejectionReason = ""; //
            tsheet.Submitted = DateTime.UtcNow; //
            tsheet.LockInfo = null;
            tsheet.UserActivity = ""; //
            tsheet.Edited = convUtil.wasPWAedited(tsf);
            
            tsheet.UriString = _sscontext.Stagings.Find(tsf.id).UriString;
            PopulateTimesheetEntries(tsf, tsheet);
            return tsheet;
        }
        // Convert the timesheet form row items into timesheet time entries. Makes
        // certain assumptions about start times, end times, and group. 
        private void PopulateTimesheetEntries(PWATimesheet tsf, Timesheet tsheet)
        {
            tsheet.TotalHours = Convert.ToDouble(convUtil.TimeToDecimal(tsf.totalHours.value));
            var tl = new List<TimeEntry>();

            // Only update if true
            if(tsf.timesheet.wasEdited == true)
            {
                tsheet.Edited = true;
            }

            foreach (var row in tsf.timesheet.value)
            {
                var x = new TimeEntry();
                try
                {
                    x.Date = Convert.ToDateTime(row.date);
                }
                catch (FormatException)
                {
                    x.Date = DateTime.Now;
                }
                try
                {
                    x.Hours = float.Parse(row.totalHours);
                }
                catch (FormatException)
                {
                    x.Hours = 0;
                }

                // Assume Group field is 'N'
                x.Group = false;

                // Assume starttime is AM, pad with leading zero if necessary
                string sdf = convUtil.TimeFormatterPadding(row.starttime);
                string sd;
                if (!sdf.Contains("AM"))
                {
                    sd = row.date + " " + sdf + " AM";
                }
                else
                {
                    sd = row.date + " " + sdf;

                }
                try
                {
                    x.In = DateTime.ParseExact(sd, "yyyy-MM-dd HH:mm tt", null);
                }
                catch ( FormatException) 
                {
                    x.In = DateTime.Now;
                }

                // Assume endtime is PM, convert to 24hr.
                string edf = convUtil.TimeFormatter24(row.endtime);
                string ed;
                if (!sdf.Contains("AM"))
                {
                    ed = row.date + " " + edf + " PM";
                }
                else
                {
                    ed = row.date + " " + edf;

                }
                try
                {
                    x.Out = DateTime.ParseExact(ed, "yyyy-MM-dd HH:mm tt", null);
                }
                catch (FormatException)
                {
                    x.Out = DateTime.Now;
                }

                tl.Add(x);
            }
            tsheet.TimeEntries = tl;   
        }

        // Give a timesheetform obj, get back a partially populated timesheet obj.
        // TODO fix UriString, confirm vals for // marks
        public MileageForm PopulateMileage(PWAMileage m, MileageForm mf = null)
        {
            if (mf == null) { mf = new MileageForm(); }

            mf.ClientName      = m.clientName.value;
            mf.ClientPrime     = m.prime.value;
            mf.ProviderName    = m.providerName.value;
            mf.ProviderId      = m.providerNum.value;
            mf.ServiceGoal     = m.serviceGoal.value;
            mf.ProgressNotes   = m.progressNotes.value;
            mf.FormType        = m.serviceAuthorized.value;
            mf.RejectionReason = ""; //
            mf.Submitted       = DateTime.UtcNow; //
            mf.LockInfo        = null;
            mf.UserActivity    = ""; //
            mf.Edited = convUtil.wasPWAedited(m);

            mf.UriString = _sscontext.Stagings.Find(m.id).UriString;
            PopulateMileageEntries(m, mf);
            return mf;
        }
        // Convert the timesheet form row items into timesheet time entries. Makes
        // certain assumptions about start times, end times, and group. 
        private void PopulateMileageEntries(PWAMileage m, MileageForm mf)
        {
            mf.TotalMiles = Convert.ToDouble(convUtil.TimeToDecimal(m.totalMiles.value));
            var tl = new List<MileageEntry>();

            // Only update if true
            if(m.mileagesheet.wasEdited == true)
            {
                mf.Edited = true;
            }

            foreach (var row in m.mileagesheet.value)
            {
                var x = new MileageEntry();
                try
                {
                    x.Date = Convert.ToDateTime(row.date);
                }
                catch (FormatException)
                {
                    x.Date = DateTime.Now;
                }
                try
                {
                    x.Miles = float.Parse(row.totalMiles);
                }
                catch (FormatException)
                {
                    x.Miles = 0;
                }

                // Assume Group field is 'N'
                x.Group = false;

                // Assume starttime is AM, pad with leading zero if necessary
                x.PurposeOfTrip = row.purpose;
                tl.Add(x);
            }
            mf.MileageEntries = tl;
        }

    }
}
