using NUnit.Framework;
using AdminUI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Common.Data;
using Microsoft.Extensions.Logging.Abstractions;
using AdminUI.Data;

namespace AdminUITest
{

    [TestFixture]
    public class HomeControllerTests
    {

        private readonly ILogger<HomeController> _logger;
        private readonly SubmissionContext _scontext;
        private readonly PayPeriodContext _pcontext;

        public HomeControllerTests()
        {
            _logger = new NullLogger<HomeController>();
            _scontext = new InMemoryDbContextFactory().GetSubmissionContext();
            _pcontext = new InMemoryDbContextFactory().GetPayPeriodContext();
        }

    }
}