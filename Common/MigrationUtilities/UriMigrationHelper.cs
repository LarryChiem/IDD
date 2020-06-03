using Common.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Common.MigrationUtilities
{
    public class UriMigrationHelper
    {
        public static void UpgradeCommaFixSubmissions(SubmissionContext subcontext)
        {
            // Order by ID asc
            var entries = subcontext.Submissions.OrderBy(x => x.Id).ToList();
            foreach (var entry in entries)
            {
                // Failure indicates old formatting. If the first result can deserialize
                // successfully, then the follow entries will have been formatted correctly.
                // Thus, the break.
                var currentString = entry.UriString;
                try
                {
                    var undone = System.Text.Json.JsonSerializer.Deserialize<List<string>>(currentString);
                    break;
                }
                catch (System.Exception ex)
                {
                    var uList = currentString.Split(',').ToList();
                    entry.UriString = System.Text.Json.JsonSerializer.Serialize(uList);
                }
            }
        }


        public static void DowngradeCommaFixSubmissions(SubmissionContext subcontext)
        {
            // Order by ID asc
            var entries = subcontext.Submissions.OrderBy(x => x.Id).ToList();
            foreach (var entry in entries)
            {
                var currentString = entry.UriString;
                try
                {
                    var undone = System.Text.Json.JsonSerializer.Deserialize<List<string>>(currentString);
                    var replaceString = string.Join(",", undone);
                    entry.UriString = replaceString;
                }
                // Failure indicates the correct formatting. As the results are
                // ordered ASC, exit early if exception is encountered as the
                // following entries should be in the correct format.
                catch (System.Exception ex)
                {
                    break;
                }
            }
        }
    }
}
