using System;
using Common.Models;
using Common.Data;
using System.Collections.Generic;
using Appserver.Data;
using convUtil = IDD.FormConversionUtils;

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


        // Give a timesheetform obj, get back a partially populated timesheet obj.
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
            tsheet.RejectionReason = "";
            tsheet.Submitted = DateTime.UtcNow;
            tsheet.LockInfo = null;
            tsheet.UserActivity = "";
            tsheet.Edited = convUtil.wasPWAedited(tsf);
            
            tsheet.UriString = _sscontext.Stagings.Find(tsf.id).UriString;
            PopulateTimesheetEntries(tsf, tsheet);
            return tsheet;
        }
        // Convert the timesheet form row items into timesheet time entries. Makes
        // certain assumptions about start times, end times, and group. 
        private void PopulateTimesheetEntries(PWATimesheet timesheetForm, Timesheet timesheet)
        {
            timesheet.TotalHours = Convert.ToDouble(convUtil.TimeToDecimal(timesheetForm.totalHours.value));
            var timeEntryList = new List<TimeEntry>();

            if (timesheetForm.timesheet.wasEdited)
                timesheet.Edited = true;

            foreach (var row in timesheetForm.timesheet.value)
            {
                var timeEntry = new TimeEntry();
                try
                {
                    timeEntry.Date = Convert.ToDateTime(row.date);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Hey, {0} is not a valid Date!", row.date);
                    timeEntry.Date = DateTime.Parse("1/1/1900");
                }
                try
                {
                    timeEntry.Hours = double.Parse(convUtil.TimeToDecimal(row.totalHours));
                }
                catch (FormatException)
                {
                    Console.WriteLine("Hey, {0} is not a valid Time!", row.totalHours);
                    timeEntry.Hours = -1;
                }

                timeEntry.Group = row.group.Equals("1", StringComparison.CurrentCultureIgnoreCase);

                try
                {
                    timeEntry.In = DateTime.Parse(row.starttime);
                }
                catch ( FormatException) 
                {
                    Console.WriteLine("Hey, {0} is not a valid Time!", row.starttime);
                    timeEntry.In = DateTime.Parse("12:00 AM");
                }

                try
                {
                    timeEntry.Out = DateTime.Parse(row.endtime);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Hey, {0} is not a valid Time!", row.endtime);
                    timeEntry.Out = DateTime.Parse("12:00 AM");
                }

                timeEntryList.Add(timeEntry);
            }
            timesheet.TimeEntries = timeEntryList;   
        }

        // Give a timesheetform obj, get back a partially populated timesheet obj.
        public MileageForm PopulateMileage(PWAMileage pwaForm, MileageForm mileageForm = null)
        {
            if (mileageForm == null) { mileageForm = new MileageForm(); }

            mileageForm.ClientName      = pwaForm.clientName.value;
            mileageForm.ClientPrime     = pwaForm.prime.value;
            mileageForm.ProviderName    = pwaForm.providerName.value;
            mileageForm.ProviderId      = pwaForm.providerNum.value;
            mileageForm.ServiceGoal     = pwaForm.serviceGoal.value;
            mileageForm.ProgressNotes   = pwaForm.progressNotes.value;
            mileageForm.FormType        = pwaForm.serviceAuthorized.value;
            mileageForm.RejectionReason = "";
            mileageForm.Submitted       = DateTime.UtcNow;
            mileageForm.LockInfo        = null;
            mileageForm.UserActivity    = "";
            mileageForm.Edited = convUtil.wasPWAedited(pwaForm);

            mileageForm.UriString = _sscontext.Stagings.Find(pwaForm.id).UriString;
            PopulateMileageEntries(pwaForm, mileageForm);
            return mileageForm;
        }
        // Convert the timesheet form row items into timesheet time entries. Makes
        // certain assumptions about start times, end times, and group. 
        private void PopulateMileageEntries(PWAMileage pwaForm, MileageForm mileageForm)
        {
            mileageForm.TotalMiles = Convert.ToDouble(convUtil.TimeToDecimal(pwaForm.totalMiles.value));
            var tl = new List<MileageEntry>();

            // Only update if true
            if(pwaForm.mileagesheet.wasEdited == true)
            {
                mileageForm.Edited = true;
            }

            foreach (var row in pwaForm.mileagesheet.value)
            {
                var entry = new MileageEntry();
                try
                {
                    entry.Date = Convert.ToDateTime(row.date);
                }
                catch (FormatException)
                {
                    entry.Date = DateTime.Now;
                }
                try
                {
                    entry.Miles = float.Parse(row.totalMiles);
                }
                catch (FormatException)
                {
                    entry.Miles = 0;
                }

                entry.Group = row.group.Equals("1", StringComparison.CurrentCultureIgnoreCase);

                // Assume starttime is AM, pad with leading zero if necessary
                entry.PurposeOfTrip = row.purpose;
                tl.Add(entry);
            }
            mileageForm.MileageEntries = tl;
        }

    }
}
