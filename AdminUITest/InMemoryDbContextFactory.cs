using AdminUI.Data;
using Common.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminUITest
{
    public class InMemoryDbContextFactory
    {
        public SubmissionContext GetSubmissionContext()
        {
            var options = new DbContextOptionsBuilder<SubmissionContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryArticleDatabase")
                .Options;
            var dbContext = new SubmissionContext(options);
            return dbContext;
        }

        public PayPeriodContext GetPayPeriodContext()
        {
            var options = new DbContextOptionsBuilder<PayPeriodContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryArticleDatabase")
                .Options;
            var dbContext = new PayPeriodContext(options);
            return dbContext;
        }

    }
}
