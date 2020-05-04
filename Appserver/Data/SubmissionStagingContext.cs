using Appserver.Models;
using Microsoft.EntityFrameworkCore;

namespace Appserver.Data
{
    public class SubmissionStagingContext : DbContext
    {
        public SubmissionStagingContext(DbContextOptions<SubmissionStagingContext> options)
            : base(options)
        {
        }

        public DbSet<SubmissionStaging> Stagings { get; set; }
    }
}
