using System;
using System.Collections.Generic;

namespace Common.Models
{
    public abstract class PWAsubmission
    {
        public int id { get; set; }
        public int formChoice { get; set; }
        public PWAsubmissionVals clientName { get; set; }
        public PWAsubmissionVals prime { get; set; }
        public PWAsubmissionVals submissionDate { get; set; }
        public PWAsubmissionVals providerName { get; set; }
        public PWAsubmissionVals providerNum { get; set; }
        public PWAsubmissionVals serviceAuthorized { get; set; }
        public PWAsubmissionVals serviceGoal { get; set; }
        public PWAsubmissionVals progressNotes { get; set; }
        public PWAsubmissionVals employerSignDate { get; set; }
        public PWAsubmissionVals employerSignature { get; set; }
        public PWAsubmissionVals providerSignDate { get; set; }
        public PWAsubmissionVals providerSignature { get; set; }
        public PWAsubmissionVals authorization { get; set; }
        public PWAsubmissionVals approval { get; set; }
        public PWAsubmissionVals scpaName { get; set; }
        public PWAsubmissionVals brokerage { get; set; }

        public static PWAsubmission FromForm(AbstractFormObject form, AbstractFormObject.FormType formType)
        {
            PWAsubmission submission;
            switch (formType)
            {
                case AbstractFormObject.FormType.OR526_ATTENDANT:
                case AbstractFormObject.FormType.OR507_RELIEF:
                    submission = new PWATimesheet();
                    break;
                case AbstractFormObject.FormType.OR004_MILEAGE:
                    submission = new PWAMileage();
                    break;
                default:
                    throw new ArgumentException();
            }

            submission.approval = new PWAsubmissionVals(form.approval.ToString());
            submission.authorization = new PWAsubmissionVals(form.authorization.ToString());
            submission.brokerage = new PWAsubmissionVals(form.brokerage);
            submission.clientName = new PWAsubmissionVals(form.clientName);
            submission.employerSignature = new PWAsubmissionVals(form.employerSignature.ToString());
            submission.employerSignDate = new PWAsubmissionVals(form.employerSignDate);
            submission.id = form.id;
            submission.prime = new PWAsubmissionVals(form.prime);
            submission.progressNotes = new PWAsubmissionVals(form.progressNotes);
            submission.providerName = new PWAsubmissionVals(form.providerName);
            submission.providerNum = new PWAsubmissionVals(form.providerNum);
            submission.providerSignature = new PWAsubmissionVals(form.employerSignature.ToString());
            submission.providerSignDate = new PWAsubmissionVals(form.employerSignDate);
            submission.scpaName = new PWAsubmissionVals(form.scpaName);
            submission.serviceAuthorized = new PWAsubmissionVals(form.serviceAuthorized);
            submission.serviceGoal = new PWAsubmissionVals(form.serviceGoal);

            submission.ConvertForm(form);
            return submission;
        }
        protected abstract void ConvertForm(AbstractFormObject form);
    }

    public class PWAsubmissionVals
    {
        public string value { get; set; }
        public bool wasEdited { get; set; }

        public PWAsubmissionVals( string s, bool edited=false) => (value, wasEdited) = (s, edited);
    }

    public class PWATimesheet : PWAsubmission
    {
        public PWAsubmissionVals totalHours { get; set; }
        public PWAtimesheetEntries timesheet { get; set; }
        protected override void ConvertForm(AbstractFormObject form)
        {
            TimesheetForm ts = (TimesheetForm)form;
            totalHours = new PWAsubmissionVals(ts.totalHours);

            timesheet = new PWAtimesheetEntries();
            var entries = new List<PWAtimesheetVals>();
            foreach (var entry in ts.Times)
            {
                entries.Add(new PWAtimesheetVals
                {
                    date = entry.date,
                    starttime = entry.starttime,
                    endtime = entry.endtime,
                    group = entry.group,
                    totalHours = entry.totalHours,
                    wasEdited = false
                });
            }

            timesheet.value = entries;
        }
    }

    public class PWAtimesheetEntries
    {
        public bool wasEdited { get; set; }
        public ICollection<PWAtimesheetVals> value { get; set; }
    }

    public class PWAtimesheetVals
    {
        public string date { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public string totalHours { get; set; }
        public string group { get; set; }
        public bool wasEdited { get; set; }
    }

    public class PWAMileage : PWAsubmission
    {
        public PWAsubmissionVals totalMiles { get; set; }
        public PWAmileageEntries mileage { get; set; }
        protected override void ConvertForm(AbstractFormObject form)
        {
            Appserver.FormSubmit.MileageForm m = (Appserver.FormSubmit.MileageForm)form;
            totalMiles = new PWAsubmissionVals(m.totalMiles);

            mileage = new PWAmileageEntries();
            var entries = new List<PWAmileageVals>();
            foreach (var entry in m.Mileage)
            {
                entries.Add(new PWAmileageVals
                {
                    date = entry.date,
                    totalMiles = entry.totalMiles,
                    group = entry.group,
                    purpose = entry.purpose,
                    wasEdited = false
                });
            }

            mileage.value = entries;
        }
    }
    public class PWAmileageEntries
    {
        public bool wasEdited { get; set; }
        public ICollection<PWAmileageVals> value { get; set; }
    }
    public class PWAmileageVals
    {
        public string date { get; set; }
        public string totalMiles { get; set; }
        public string group { get; set; }
        public string purpose { get; set; }
        public bool wasEdited { get; set; }
    }
}
