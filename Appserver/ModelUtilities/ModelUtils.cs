using System.Collections.Generic;
using System.Linq;
using Appserver.Data;

namespace Common.MigrationUtilities
{
    public class ModelUtils
    {
        // Convert from comma to json formatting
        public static void UpgradeCommaFixStaging(SubmissionStagingContext subcontext)
        {
            var entries = subcontext.Stagings;
            foreach(var entry in entries)
            {
                var currentString = entry.UriString;
                // Fail on old formatting
                try
                {
                    var undone = System.Text.Json.JsonSerializer.Deserialize<List<string>>(currentString);
                }
                catch(System.Exception ex)
                {
                    var uList = currentString.Split(',').ToList();
                    entry.UriString = System.Text.Json.JsonSerializer.Serialize(uList);
                }
            }

            subcontext.SaveChanges();

        }


        public static void DowngradeCommaFixStaging(SubmissionStagingContext subcontext)
        {
            var entries = subcontext.Stagings;
            foreach(var entry in entries)
            {
                var currentString = entry.UriString;
                try
                {
                    var undone = System.Text.Json.JsonSerializer.Deserialize<List<string>>(currentString);
                    var replaceString = string.Join(",", undone);
                    entry.UriString = replaceString;
                }
                catch (System.Exception ex)
                {
                    continue;
                }
            }

            subcontext.SaveChanges();
        }
    }
}
