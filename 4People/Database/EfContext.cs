using System;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using _4People.Database.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace _4People.Database
{
    public class EfContext : DbContext
    {
        public static readonly string CreatingString = @"Server=DESKTOP-491MMIU\SQLDEV1101;Database=_4People;User ID=sa;Password=Tok_Vol583;TrustServerCertificate=True;Trusted_Connection=True;";
        public static bool IsAlreadyExists;
        private static SqliteConnection sqlConnection = new ();

        public DbSet<Company> Companies { get; set; }
        public DbSet<Subdivision> Subdivisions { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public EfContext() : base()
        {
            Database.Migrate();
            Database.EnsureCreated();
        }

        public EfContext(DbContextOptions<EfContext> options) : base(options)
        {
            Database.Migrate();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(CreatingString);
        }

        public static async Task<EfContext> CreateInstanceAsync()
        {
            DbContextOptionsBuilder<EfContext> builder = new();
            builder.UseSqlServer(CreatingString);
            var context = new EfContext(builder.Options);
            return context;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                        .HasMany(company => company.Subdivisions)
                        .WithOne(subdivision => subdivision.Company)
                        .HasForeignKey(subdivision => subdivision.CompanyId);

            modelBuilder.Entity<Subdivision>()
                        .HasOne(subdivision => subdivision.Leader)
                        .WithOne()
                        .HasForeignKey<Subdivision>(subdivision => subdivision.LeaderId);

            modelBuilder.Entity<Subdivision>()
                        .HasMany(subdivision => subdivision.Employees)
                        .WithOne(employee => employee.Subdivision)
                        .HasForeignKey(employee => employee.SubdivisionId);

            modelBuilder.Entity<Employee>()
                        .Property(employee => employee.Salary)
                        .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
