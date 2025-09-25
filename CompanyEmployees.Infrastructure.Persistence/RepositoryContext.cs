using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Infrastructure.Persistence
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new AttendanceRecordConfiguration());
        }

        public DbSet<Company>? Companies { get; set; }
        public DbSet<Employee>? Employees { get; set; }
        public DbSet<AttendanceRecord>? AttendanceRecords { get; set; }
    }
}
