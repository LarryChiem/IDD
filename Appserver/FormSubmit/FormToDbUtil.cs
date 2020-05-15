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
            tsheet.Submitted = DateTime.UtcNow; //
            tsheet.LockInfo = null;
            tsheet.UserActivity = ""; //
            
            tsheet.UriString = _sscontext.Stagings.Find(tsf.id).UriString;

            return tsheet;
        }

        // PWA to TimesheetForm converter
        public TimesheetForm PWAtoTimesheetFormConverter(PWAsubmission pwasub)
        {
            TimesheetForm tsf = new TimesheetForm();
            List<TimesheetRowItem> tsl = new List<TimesheetRowItem>();
            tsf.clientName = pwasub.clientName.value;
            tsf.prime = pwasub.prime.value;
            tsf.providerName = pwasub.providerName.value;
            tsf.providerNum = pwasub.providerNum.value;
            tsf.brokerage = pwasub.brokerage.value;
            tsf.scpaName = pwasub.scpaName.value;
            tsf.serviceAuthorized = pwasub.serviceAuthorized.value;
            tsf.serviceGoal = pwasub.serviceGoal.value;
            tsf.progressNotes = pwasub.progressNotes.value;
            tsf.employerSignature = convUtil.PWABoolConverter(pwasub.employerSignature.value);
            tsf.employerSignDate = pwasub.employerSignDate.value;
            tsf.providerSignature = convUtil.PWABoolConverter(pwasub.providerSignature.value);
            tsf.providerSignDate = pwasub.providerSignDate.value;
            tsf.authorization = convUtil.PWABoolConverter(pwasub.authorization.value);
            tsf.id = pwasub.id;


            foreach(PWAtimesheetVals lsv in pwasub.timesheet.value)
            {
                // Convert from HH:MM to HH.hh
                var stime = lsv.totalHours.Split(':');
                double phour = (double.Parse(stime[1]) / 60.0) + double.Parse(stime[0]);
                
                tsf.addTimeRow(lsv.date, lsv.starttime, lsv.endtime, string.Format("{0:0.00}", phour), "true");
            }

            return tsf;
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
                try
                {
                    x.Date = Convert.ToDateTime(tsri.date);
                }
                catch (FormatException)
                {
                    x.Date = DateTime.Now;
                }
                try
                {
                    x.Hours = float.Parse(tsri.totalHours);
                }
                catch (FormatException)
                {
                    x.Hours = 0;
                }
                totalHours += x.Hours;

                // Assume Group field is 'N'
                x.Group = true;

                // Assume starttime is AM, pad with leading zero if necessary
                string sdf = convUtil.TimeFormatterPadding(tsri.starttime);
                string sd;
                if (!sdf.Contains("AM"))
                {
                    sd = tsri.date + " " + sdf + " AM";
                }
                else
                {
                    sd = tsri.date + " " + sdf;

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
                string edf = convUtil.TimeFormatter24(tsri.endtime);
                string ed;
                if (!sdf.Contains("AM"))
                {
                    ed = tsri.date + " " + edf + " PM";
                }
                else
                {
                    ed = tsri.date + " " + edf;

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

            tsheet.TotalHours = Math.Round(totalHours, 2);
            tsheet.TimeEntries = tl;   
        }

    }
}
