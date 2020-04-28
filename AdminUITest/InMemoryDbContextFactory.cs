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

        public UserContext GetUserContext()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryArticleDatabase")
                .Options;
            var dbContext = new UserContext(options);
            return dbContext;
        }
    }
}
