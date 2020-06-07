
namespace Appserver.FormSubmit
{
    public class MileageRowItem
    {
        public string date = "2000-01-01";
        public string totalMiles = "12";
        public string group;
        public string purpose;

        public MileageRowItem(string date, string totalMiles, string group, string purpose) =>
            (this.date, this.totalMiles, this.group, this.purpose) =
            (date, totalMiles, group, purpose);
    }
}
