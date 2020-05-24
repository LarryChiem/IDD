using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Common.Models;
using Common.Data;

//TODO: Delete this file when releasing
namespace AdminUI.Models
{
    public static class MockData
    {
        public static void InitializeSubmissionDb(SubmissionContext context)
        {
            if (context.Submissions.Any())
            {
                return;   // DB has been seeded
            }

            var uris = new List<string>();
            uris.Add("https://iddstorageaccountdev.blob.core.windows.net/submissionfiles/2020-05-14-07-19-41-5642367_OR507_526_front.png");
            uris.Add("https://iddstorageaccountdev.blob.core.windows.net/submissionfiles/2020-05-14-07-19-41-5431700_OR507_526_back.png");
            uri_string = System.Text.Json.JsonSerializer.Serialize(uris);

            context.Submissions.AddRange(
                new Timesheet
                {
                    ClientName = "Minnie Mouse",
                    ClientPrime = "M0U5E",
                    Edited = true,
                    ProviderName = "Mickey Mouse",
                    ProviderId = "B1GM0U53",
                    TotalHours = 10,
                    FormType = "OR526 Attendant Care",
                    ServiceGoal = "To help her eat cheese",
                    ProgressNotes = "She ate the cheese",
                    Submitted = DateTime.Parse("4/2/20 2:03PM"),
                    RejectionReason = "",
                    Status = "Pending",
                    UriString = uri_string;

                    TimeEntries = new List<TimeEntry>
                    {
                            new TimeEntry{
                                Date = DateTime.Parse("3/27/20"),
                                In = DateTime.Parse("12:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 4,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("3/28/20"),
                                In = DateTime.Parse("3:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 1,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("3/29/20"),
                                In = DateTime.Parse("2:00pm"),
                                Out = DateTime.Parse("6:30pm"),
                                Hours = 4.5,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("3/30/20"),
                                In = DateTime.Parse("3:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 1,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("4/1/20"),
                                In = DateTime.Parse("3:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 1,
                                Group = true,
                                Status = "Pending"
                            }
                    }
                },
                new Timesheet
                {
                    ClientName = "Beast",
                    ClientPrime = "666",
                    Edited = false,
                    ProviderName = "Belle",
                    ProviderId = "B3113",
                    TotalHours = 80.7,
                    FormType = "OR526 Attendant Care",
                    ServiceGoal = "Transmogrification",
                    ProgressNotes = "Progress is a little hairy",
                    Submitted = DateTime.Parse("4/2/20 1:45PM"),
                    RejectionReason = "",
                    Status = "Pending",
                    UriString = uri_string;

                    TimeEntries = new List<TimeEntry>
                    {
                            new TimeEntry{
                                Date = DateTime.Parse("3/27/20"),
                                In = DateTime.Parse("12:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 4,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("3/28/20"),
                                In = DateTime.Parse("3:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 1,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("3/29/20"),
                                In = DateTime.Parse("2:00pm"),
                                Out = DateTime.Parse("6:30pm"),
                                Hours = 4.5,
                                Group = true,
                                Status = "Pending"
                            }
                    }
                },
                new Timesheet
                {
                    ClientName = "Cinderella",
                    ClientPrime = "5L1PP3R",
                    Edited = true,
                    ProviderName = "Prince Charming",
                    ProviderId = "H0TT13",
                    TotalHours = 54.5,
                    FormType = "OR526 Attendant Care",
                    ServiceGoal = "Shoe sizing",
                    ProgressNotes = "If the shoe fits, I must commit",
                    Submitted = DateTime.Parse("4/3/20 8:06AM"),
                    RejectionReason = "",
                    Status = "Pending",
                    UriString = uri_string
                    TimeEntries = new List<TimeEntry>
                    {
                            new TimeEntry{
                                Date = DateTime.Parse("3/27/20"),
                                In = DateTime.Parse("12:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 4,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("3/29/20"),
                                In = DateTime.Parse("2:00pm"),
                                Out = DateTime.Parse("6:30pm"),
                                Hours = 4.5,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("3/30/20"),
                                In = DateTime.Parse("3:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 1,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("4/1/20"),
                                In = DateTime.Parse("3:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 1,
                                Group = true,
                                Status = "Pending"
                            }
                    }
                },
                new Timesheet
                {
                    ClientName = "Anna",
                    ClientPrime = "R3DH3AD",
                    Edited = false,
                    ProviderName = "Elsa",
                    ProviderId = "L3T1TG0",
                    TotalHours = 12,
                    FormType = "OR526 Attendant Care",
                    ServiceGoal = "To help with her transition to queen",
                    ProgressNotes = "She aight",
                    Submitted = DateTime.Parse("4/4/20 5:13PM"),
                    RejectionReason = "",
                    Status = "Pending",
                    UriString = uri_string;

                    TimeEntries = new List<TimeEntry>
                    {
                            new TimeEntry{
                                Date = DateTime.Parse("3/28/20"),
                                In = DateTime.Parse("3:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 1,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("3/29/20"),
                                In = DateTime.Parse("2:00pm"),
                                Out = DateTime.Parse("6:30pm"),
                                Hours = 4.5,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("3/30/20"),
                                In = DateTime.Parse("3:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 1,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("4/1/20"),
                                In = DateTime.Parse("3:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 1,
                                Group = true,
                                Status = "Pending"
                            }
                    }
                },
                new Timesheet
                {
                    ClientName = "Snow White",
                    ClientPrime = "5L33PY",
                    Edited = false,
                    ProviderName = "Prince Florian",
                    ProviderId = "K155",
                    TotalHours = 89.2,
                    FormType = "OR526 Attendant Care",
                    ServiceGoal = "To wake her up",
                    ProgressNotes = "She needs an energy drink or something",
                    Submitted = DateTime.Parse("4/2/20 10:20AM"),
                    RejectionReason = "",
                    Status = "Pending",
                    UriString = uri_string;

                    TimeEntries = new List<TimeEntry>
                    {
                            new TimeEntry{
                                Date = DateTime.Parse("3/27/20"),
                                In = DateTime.Parse("12:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 4,
                                Group = true,
                                Status = "Pending"
                            },
                            new TimeEntry{
                                Date = DateTime.Parse("4/1/20"),
                                In = DateTime.Parse("3:00pm"),
                                Out = DateTime.Parse("4:00pm"),
                                Hours = 1,
                                Group = true,
                                Status = "Pending"
                            }
                    }
                });
                
            for (var i = 1; i <= 101; i++)
            {
                context.Submissions.Add(
                    new Timesheet
                    {
                        ClientName = "Dalmation " + i,
                        ClientPrime = "5P0T5",
                        Edited = false,
                        ProviderName = "Dalmation Dad",
                        ProviderId = "T1R3D",
                        TotalHours = 80.0,
                        ServiceGoal = "To make sure the kids survive",
                        ProgressNotes = "Still alive",
                        FormType = "OR526 Attendant Care",
                        Submitted = DateTime.Parse("4/1/20 10:20AM"),
                        RejectionReason = "",
                        Status = "Pending",
                        UriString = uri_string;

                        TimeEntries = new List<TimeEntry>
                        {
                                new TimeEntry
                                {
                                    Date = DateTime.Parse("3/27/20"),
                                    In = DateTime.Parse("12:00pm"),
                                    Out = DateTime.Parse("4:00pm"),
                                    Hours = 4,
                                    Group = true,
                                    Status = "Pending"
                                },
                                new TimeEntry
                                {
                                    Date = DateTime.Parse("3/28/20"),
                                    In = DateTime.Parse("3:00pm"),
                                    Out = DateTime.Parse("4:00pm"),
                                    Hours = 1,
                                    Group = true,
                                    Status = "Pending"
                                },
                                new TimeEntry
                                {
                                    Date = DateTime.Parse("3/29/20"),
                                    In = DateTime.Parse("2:00pm"),
                                    Out = DateTime.Parse("6:30pm"),
                                    Hours = 4.5,
                                    Group = true,
                                    Status = "Pending"
                                },
                                new TimeEntry
                                {
                                    Date = DateTime.Parse("3/30/20"),
                                    In = DateTime.Parse("3:00pm"),
                                    Out = DateTime.Parse("4:00pm"),
                                    Hours = 1,
                                    Group = true,
                                    Status = "Pending"
                                },
                                new TimeEntry
                                {
                                    Date = DateTime.Parse("4/1/20"),
                                    In = DateTime.Parse("3:00pm"),
                                    Out = DateTime.Parse("4:00pm"),
                                    Hours = 1,
                                    Group = true,
                                    Status = "Pending"
                                }
                        }
                    }

                );
                var rand = new Random();
                context.Submissions.Add(
                   new MileageForm
                   {
                       ClientName = "Hun Soldier " + i,
                       ClientPrime = "M0NG0L",
                       Edited = false,
                       ProviderName = "Atila The Hun",
                       ProviderId = "011700",
                       TotalMiles = 200 - i,
                       ServiceGoal = "To find Mulan",
                       ProgressNotes = "Rumor has it that he may actually be a woman in disguise",
                       FormType = "OR004 Mileage",
                       Submitted = DateTime.Parse("5/" + rand.Next(1,30) + "/20 " + rand.Next(1,12) + ":00AM"),
                       RejectionReason = "",
                       Status = "Pending",
                       UriString = uri_string;
                       
                       MileageEntries = new List<MileageEntry>
                       {
                                new MileageEntry
                                {
                                    Date = DateTime.Parse("3/27/20"),
                                    Miles = 4,
                                    Group = true,
                                    PurposeOfTrip = "Run to the grocery store",
                                    Status = "Pending"
                                },
                                new MileageEntry
                                {
                                    Date = DateTime.Parse("3/28/20"),
                                    Miles = 10,
                                    Group = true,
                                    PurposeOfTrip = "Dropping kids off at school",
                                    Status = "Pending"
                                },
                                new MileageEntry
                                {
                                    Date = DateTime.Parse("3/29/20"),
                                    Miles = 5,
                                    Group = true,
                                    PurposeOfTrip = "Run to the pharmacy",
                                    Status = "Pending"
                                },
                                new MileageEntry
                                {
                                    Date = DateTime.Parse("3/30/20"),
                                    Miles = 50,
                                    Group = true,
                                    PurposeOfTrip = "Roadtrip!",
                                    Status = "Pending"
                                },
                                new MileageEntry
                                {
                                    Date = DateTime.Parse("4/1/20"),
                                    Miles = 25,
                                    Group = true,
                                    PurposeOfTrip = "I'm just putting a really long string in here just to see what happens when the various modules try to display it",
                                    Status = "Pending"
                                },
                       }
                   }

               );
            }

            context.SaveChanges();
        }

    }
}
