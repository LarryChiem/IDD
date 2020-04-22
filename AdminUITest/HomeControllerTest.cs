using NUnit.Framework;
using AdminUI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AdminUI.Data;
using Microsoft.Extensions.Logging.Abstractions;

namespace AdminUI.Tests
{

    [TestFixture]
    public class HomeControllerTests
    {

        private readonly TimesheetContext _dbcontext;
        private readonly LockTableContext _ltcontext;
        private readonly Controller _controller;
        private readonly ILogger<HomeController> _logger;

        public HomeControllerTests()
        {
            _dbcontext = new InMemoryDbContextFactory().GetTimesheetContext();
            _ltcontext = new InMemoryDbContextFactory().GetLockTableContext();
            _logger = new NullLogger<HomeController>();
            _controller = new HomeController(_logger, _dbcontext, _ltcontext);
        }

       [Test]
        public void IndexTest_IsNotNull()
        {
            var hc = new HomeController(_logger, _dbcontext,_ltcontext);
            var result = (ViewResult)hc.Index("sortOrder", "pName", "cName", "01/01/2020", "01/14/2020", "123456", "1", "P123456", "pending");
            Assert.IsNotNull(result.ViewData);
        }
    }
}