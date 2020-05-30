using Microsoft.EntityFrameworkCore.Migrations;
using Common.Data;
using System.Linq;
using System.Collections.Generic;

namespace AdminUI.Migrations
{
    public partial class DbContext : Migration
    {
        private readonly SubmissionContext subContext;

        // Update all comma separated lists to json lists
        protected override void Up()
        {
            var entries = subContext.Submissions.ToList();
            foreach(var entry in entries)
            {
                // Failure indicates old formatting
                var currentString = entry.UriString;
                try
                {
                    var undone = System.Text.Json.JsonSerializer.Deserialize<List<string>>(currentString);
                }
                catch (System.Exception ex)
                {
                    var uList = currentString.Split(',').ToList();
                    entry.UriString = System.Text.Json.JsonSerializer.Serialize(uList);
                }
            }
        }

        // Revert json serialized list to comma separated
        protected override void Down()
        {
            var entries = subContext.Submissions.ToList();
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
        }
    }
}
