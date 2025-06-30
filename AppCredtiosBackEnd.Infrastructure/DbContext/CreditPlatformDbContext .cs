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
        // Entidades existentes
        public DbSet<Users> Users { get; set; }
        public DbSet<CreditApplication> CreditApplications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Nuevas entidades bancarias
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<UserEmployment> UserEmployments { get; set; }
        public DbSet<CardApplication> CardApplications { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public CreditPlatformDbContext(DbContextOptions<CreditPlatformDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CreditPlatformDbContext).Assembly);

            // Configuraciones específicas para campos decimales
            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.Balance)
                    .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<UserEmployment>(entity =>
            {
                entity.Property(e => e.MonthlyIncome)
                    .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<CreditApplication>(entity =>
            {
                entity.Property(e => e.MonthlyIncome)
                    .HasColumnType("decimal(18,2)");
                
                entity.Property(e => e.RequestedAmount)
                    .HasColumnType("decimal(18,2)");
            });

            // Configurar relaciones de navegación para Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                // Relación con FromAccount (opcional)
                entity.HasOne(t => t.FromAccount)
                    .WithMany(a => a.FromTransactions)
                    .HasForeignKey(t => t.FromAccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con ToAccount (requerida)
                entity.HasOne(t => t.ToAccount)
                    .WithMany(a => a.ToTransactions)
                    .HasForeignKey(t => t.ToAccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Card (opcional)
                entity.HasOne(t => t.Card)
                    .WithMany()
                    .HasForeignKey(t => t.CardId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relación con CreditApplication (opcional)
                entity.HasOne(t => t.CreditApplication)
                    .WithMany()
                    .HasForeignKey(t => t.CreditApplicationId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configurar otras relaciones de navegación
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasOne(a => a.User)
                    .WithMany()
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Employer>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserEmployment>(entity =>
            {
                entity.HasOne(ue => ue.User)
                    .WithMany()
                    .HasForeignKey(ue => ue.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ue => ue.Employer)
                    .WithMany()
                    .HasForeignKey(ue => ue.EmployerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CardApplication>(entity =>
            {
                entity.HasOne(ca => ca.User)
                    .WithMany()
                    .HasForeignKey(ca => ca.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ca => ca.Account)
                    .WithMany(a => a.CardApplications)
                    .HasForeignKey(ca => ca.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ca => ca.ApprovedBy)
                    .WithMany()
                    .HasForeignKey(ca => ca.ApprovedById)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(ca => ca.CreditApplication)
                    .WithMany()
                    .HasForeignKey(ca => ca.CreditApplicationId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasOne(c => c.CardApplication)
                    .WithMany(ca => ca.Cards)
                    .HasForeignKey(c => c.CardApplicationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración específica para AuditLog - sin navegación para evitar conflictos
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(a => a.Id);
                
                entity.Property(a => a.Action)
                    .HasMaxLength(100)
                    .IsRequired();
                
                entity.Property(a => a.Details)
                    .HasMaxLength(500)
                    .IsRequired();
                
                entity.Property(a => a.Timestamp)
                    .HasDefaultValueSql("GETDATE()");
                
                // Solo configuramos las claves foráneas, sin navegación
                entity.Property(a => a.CreditApplicationId).IsRequired();
                entity.Property(a => a.UserId).IsRequired();
            });
        }
    }
}
