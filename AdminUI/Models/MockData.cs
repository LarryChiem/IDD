using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminUI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdminUI.Models
{
    public static class MockData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new TimesheetContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<TimesheetContext>>()))
            {
                // Look for any movies.
                if (context.Timesheet.Any())
                {
                    return;   // DB has been seeded
                }

                context.Timesheet.AddRange(
                    new Timesheet
                    {
                        ClientName = "Minnie Mouse",
                        ClientPrime = "M0U5E",
                        ProviderName = "Mickey Mouse",
                        ProviderID = "B1GM0U53",
                        CMOrg = "Multnomah Case Management",
                        SCPAName = "Walt Disney",
                        Service = "SE49 Family Support",
                        Hours = 34.3,
                        ServiceGoal = "To help her eat cheese",
                        ProgressNotes = "She ate the cheese",
                        ClientSignature = true,
                        ClientSigned = DateTime.Parse("03/25/2020"),
                        ProviderSignature = true,
                        ProviderSigned = DateTime.Parse("3/25/20"),
                        Type = "OR526 Attendant Care",
                        Submitted = DateTime.Parse("4/2/20 2:03PM"),
                        RejectionReason = "",
                        Status = "Pending"
                    },
                    new Timesheet
                    {
                        ClientName = "Beast",
                        ClientPrime = "666",
                        ProviderName = "Belle",
                        ProviderID = "B3113",
                        CMOrg = "Gaston's Monster Management",
                        SCPAName = "Walt Disney",
                        Service = "SE151 In Home Support Children",
                        Hours = 80.7,
                        ServiceGoal = "Transmogrification",
                        ProgressNotes = "Progress is a little hairy",
                        ClientSignature = true,
                        ClientSigned = DateTime.Parse("03/24/2020"),
                        ProviderSignature = true,
                        ProviderSigned = DateTime.Parse("3/25/20"),
                        Type = "OR526 Attendant Care",
                        Submitted = DateTime.Parse("4/2/20 1:45PM"),
                        RejectionReason = "",
                        Status = "Pending"
                    },
                    new Timesheet
                    {
                        ClientName = "Cinderella",
                        ClientPrime = "5L1PP3R",
                        ProviderName = "Prince Charming",
                        ProviderID = "H0TT13",
                        CMOrg = "Party City",
                        SCPAName = "Evil Stepmother",
                        Service = "SE49 In Home Support Adults",
                        Hours = 54.5,
                        ServiceGoal = "Shoe sizing",
                        ProgressNotes = "If the shoe fits, I must commit",
                        ClientSignature = true,
                        ClientSigned = DateTime.Parse("03/27/2020"),
                        ProviderSignature = true,
                        ProviderSigned = DateTime.Parse("3/28/20"),
                        Type = "OR526 Attendant Care",
                        Submitted = DateTime.Parse("4/3/20 8:06AM"),
                        RejectionReason = "",
                        Status = "Pending"
                    },
                    new Timesheet
                    {
                        ClientName = "Anna",
                        ClientPrime = "R3DH3AD",
                        ProviderName = "Elsa",
                        ProviderID = "L3T1TG0",
                        CMOrg = "Arendelle Government",
                        SCPAName = "Kristoff",
                        Service = "SE49 In Home Support Adults",
                        Hours = 12,
                        ServiceGoal = "To help with her transition to queen",
                        ProgressNotes = "She aight",
                        ClientSignature = false,
                        ProviderSignature = true,
                        ProviderSigned = DateTime.Parse("3/25/20"),
                        Type = "OR526 Attendant Care",
                        Submitted = DateTime.Parse("4/4/20 5:13PM"),
                        RejectionReason = "",
                        Status = "Pending"
                    },
                    new Timesheet
                    {
                        ClientName = "Snow White",
                        ClientPrime = "5L33PY",
                        ProviderName = "Prince Florian",
                        ProviderID = "K155",
                        CMOrg = "Dwarven Housekeeping",
                        SCPAName = "Walt Disney",
                        Service = "SE151 In Home Support Children",
                        Hours = 89.2,
                        ServiceGoal = "To wake her up",
                        ProgressNotes = "She needs an energy drink or something",
                        ClientSignature = true,
                        ClientSigned = DateTime.Parse("03/27/2020"),
                        ProviderSignature = false,
                        Type = "OR526 Attendant Care",
                        Submitted = DateTime.Parse("4/2/20 10:20AM"),
                        RejectionReason = "",
                        Status = "Pending"
                    });
                for (var i = 1; i <= 101; i++)
                {
                    context.Timesheet.Add(
                        new Timesheet
                        {
                            ClientName = "Dalmation " + i,
                            ClientPrime = "5P0T5",
                            ProviderName = "Dalmation Dad",
                            ProviderID = "T1R3D",
                            CMOrg = "Disney",
                            SCPAName = "Walt Disney",
                            Service = "SE151 In Home Support Children",
                            Hours = 80.0,
                            ServiceGoal = "To make sure the kids survive",
                            ProgressNotes = "Still alive",
                            ClientSignature = true,
                            ClientSigned = DateTime.Parse("03/27/2020"),
                            ProviderSignature = false,
                            Type = "OR526 Attendant Care",
                            Submitted = DateTime.Parse("4/1/20 10:20AM"),
                            RejectionReason = "",
                            Status = "Pending"
                        }
                    );
                }

                context.SaveChanges();
            }
        }
    }
}
