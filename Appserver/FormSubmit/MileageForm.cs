using Newtonsoft.Json;
using System.Collections.Generic;

namespace Appserver.FormSubmit
{
    public class MileageForm : AbstractFormObject
    {
        private List<MileageRowItem> miles = new List<MileageRowItem>();
        public MileageForm() { }
        public int id { get; set; }
        public string clientName { get; set; }
        public string prime { get; set; }
        public string providerName { get; set; }
        public string providerNum { get; set; }
        public string brokerage { get; set; }
        public string scpaName { get; set; }
        public string serviceAuthorized { get; set; }

        [JsonProperty("mileagesheet")]
        [JsonConverter(typeof(MileageRowConverter))]
        internal List<MileageRowItem> Mileage { get => miles; set => miles = value; }
        public string totalMiles { get; set; }
        public string serviceGoal { get; set; }
        public string progressNotes { get; set; }
        public bool employerSignature { get; set; }
        public string employerSignDate { get; set; }
        public bool providerSignature { get; set; }
        public string providerSignDate { get; set; }
        public bool authorization { get; set; }
        public bool approval { get; set; }
        public string review_status { get; set; } = "Pending";
        public void addMileRow(string date, string miles, string group, string purpose) =>
            this.Mileage.Add(new MileageRowItem(date, miles, group, purpose));
    }
}
