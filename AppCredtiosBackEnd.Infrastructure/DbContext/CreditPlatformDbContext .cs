using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppCreditosBackEnd.Infrastructure.DbContext
{
    public class CreditPlatformDbContext: Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<CreditApplication> CreditApplications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public CreditPlatformDbContext(DbContextOptions<CreditPlatformDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CreditPlatformDbContext).Assembly);
        }
    }
}
