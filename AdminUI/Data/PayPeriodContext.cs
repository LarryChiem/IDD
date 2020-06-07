using AdminUI.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminUI.Data
{
    public class PayPeriodContext : DbContext
    {
        public PayPeriodContext(DbContextOptions<PayPeriodContext> options)
            : base(options)
        {
        }

        public DbSet<PayPeriod> PayPeriods { get; set; }

    }
}