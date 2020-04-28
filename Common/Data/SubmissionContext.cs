using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Common.Data
{
    public class SubmissionContext : DbContext
    {
        public SubmissionContext(DbContextOptions<SubmissionContext> options)
            : base(options)
        {
        }

        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<MileageForm> MileageForms { get; set; }
    }
}