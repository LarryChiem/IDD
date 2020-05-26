using System;
using System.Threading.Tasks;
using NUnit.Framework;
using AdminUI.Controllers;
using Microsoft.AspNetCore.Mvc;
using AdminUI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Common.Models;
using Common.Data;
using System.Collections.Generic;

namespace AdminUITest
{
    [TestFixture]
    public class SubmissionControllerTests
    {
        private readonly SubmissionContext _scontext;
        private readonly ILogger<SubmissionController> _logger;
        
        public SubmissionControllerTests()
        {
            var uriList = new List<string>();
            uriList.Add("hi");
            uriList.Add("hello");
            uriList.Add("how are you");
            _scontext = new InMemoryDbContextFactory().GetSubmissionContext();
            _logger = new NullLogger<SubmissionController>();
            _scontext.Add(new Timesheet {
                ClientName = "client",
                ClientPrime = "prime",
                ProviderName = "provider",
                ProviderId = "id",
                TotalHours = 34.3,
                ServiceGoal = "To help her eat cheese",
                ProgressNotes = "She ate the cheese",
                FormType = "OR526 Attendant Care",
                Submitted = DateTime.Parse("4/2/20 2:03PM"),
                RejectionReason = "",
                Status = "Pending",
                UriString = System.Text.Json.JsonSerializer.Serialize(uriList),
                LockInfo = new Lock
                {
                    User = "steve",
                    LastActivity = DateTime.Now
                }
            }
            ) ;
            _scontext.Add(new Timesheet{ 
                ClientName = "client",
                ClientPrime = "prime",
                ProviderName = "provider",
                ProviderId = "id",
                TotalHours = 34.3,
                ServiceGoal = "To help her eat cheese",
                ProgressNotes = "She ate the cheese",
                FormType = "OR526 Attendant Care",
                Submitted = DateTime.Parse("4/2/20 2:03PM"),
                RejectionReason = "",
                Status = "Pending",
                UriString = System.Text.Json.JsonSerializer.Serialize(uriList)
                }
            );
            _scontext.SaveChanges();
        }

        [Test]
        public async Task ModalProcess_SubmissionNotFound()
        {
            var tc = new SubmissionController(_logger, _scontext);
            var result = await tc.ModalProcess(-1, "Accept", "Nah", "id", "04/01/2020", "04/15/2020", "pName", "cName", "PRIME", "timesheetIndex", 1, "providerId");
            Assert.IsInstanceOf(typeof(NotFoundResult), result);
        }
        [Test]
        public async Task ModalProcess_LockNotFound()
        { 
            var tc = new SubmissionController(_logger, _scontext);
            var result = await tc.ModalProcess(2, "Accept", "Nah", "id", "04/01/2020", "04/15/2020", "pName", "cName", "PRIME", "timesheetIndex", 1, "providerId") as ViewResult;
            Assert.AreEqual("NoPermission", result.ViewName);
        }
        [Test]
        public async Task ModalProcess_LockDoesntMatchUser()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "bob"),
                new Claim(ClaimTypes.Name, "bob")
            }));

            var tc = new SubmissionController(_logger, _scontext)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                }
            };

            var result = await tc.ModalProcess(1, "Accept", "Nah", "id", "04/01/2020", "04/15/2020", "pName", "cName", "PRIME", "timesheetIndex", 1, "providerId") as ViewResult;
            Assert.AreEqual("NoPermission", result.ViewName);
        }
        [Test]
        public async Task ModalProcess_AllGood()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "steve"),
                new Claim(ClaimTypes.Name, "steve")
            }));

            var tc = new SubmissionController(_logger, _scontext)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
                {
                    HttpContext = new DefaultHttpContext() { User = user }
                }
            };

            var result = await tc.ModalProcess(1, "Accept", "Nah", "id", "04/01/2020", "04/15/2020", "pName", "cName", "PRIME", "timesheetIndex", 1, "providerId") as RedirectToActionResult;
            Assert.AreEqual("Index", result.ActionName);
        }

    }
}
