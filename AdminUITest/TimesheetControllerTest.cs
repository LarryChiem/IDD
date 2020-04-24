using System;
using System.Threading.Tasks;
using NUnit.Framework;
using AdminUI.Controllers;
using Microsoft.AspNetCore.Mvc;
using AdminUI.Data;
using AdminUI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;

namespace AdminUI.Tests
{
    [TestFixture]
    public class TimesheetControllerTests
    {
        private readonly TimesheetContext _dbcontext;
        private readonly LockTableContext _Lcontext;
        private readonly ILogger<TimesheetController> _logger;
        
        public TimesheetControllerTests()
        {
            _dbcontext = new InMemoryDbContextFactory().GetTimesheetContext();
            _Lcontext = new InMemoryDbContextFactory().GetLockTableContext();
            _logger = new NullLogger<TimesheetController>();
            _dbcontext.Add(new Timesheet{ 
                ClientName = "client",
                ClientPrime = "prime",
                ProviderName = "provider",
                ProviderID = "id",
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
                }
            );
            _dbcontext.Add(new Timesheet{ 
                ClientName = "client",
                ClientPrime = "prime",
                ProviderName = "provider",
                ProviderID = "id",
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
                }
            );
            _Lcontext.Add(new LockTableRow
            {
                TimesheetID = 1,
                User = "steve",
                LastInteraction = DateTime.Now,
                TimeLocked = DateTime.Now
            });
            _dbcontext.SaveChanges();
            _Lcontext.SaveChanges();
        }

        [Test]
        public async Task Process_TimesheetNotFound()
        {
            var tc = new TimesheetController(_logger, _dbcontext, _Lcontext);
            var result = await tc.Process(-1, "Accept", "Nah");
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }
        [Test]
        public async Task Process_LockRowNotFound()
        { 
            var tc = new TimesheetController(_logger, _dbcontext, _Lcontext);
            var result = await tc.Process(2, "Accept", "Nah") as ViewResult;
            Assert.AreEqual("NoPermission", result.ViewName);
        }
        [Test]
        public async Task Process_LockRowDoesntMatchUser()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "bob"),
                new Claim(ClaimTypes.Name, "bob")
            }));

            var tc = new TimesheetController(_logger, _dbcontext, _Lcontext)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                }
            };

            var result = await tc.Process(1, "Accept", "Nah") as ViewResult;
            Assert.AreEqual("NoPermission", result.ViewName);
        }
        [Test]
        public async Task Process_AllGood()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "steve"),
                new Claim(ClaimTypes.Name, "steve")
            }));

            var tc = new TimesheetController(_logger, _dbcontext, _Lcontext)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                }
            };

            var result = await tc.Process(1, "Accept", "Nah") as RedirectToActionResult;
            Assert.AreEqual("Index", result.ActionName);
        }

    }
}
