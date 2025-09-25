using CompanyEmployees.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyEmployees.Infrastructure.Persistence.Configurations
{
    public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
    {
        public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
        {
            // Primary key
            builder.HasKey(x => x.Id);

            // Unique constraint on (EmployeeId, WorkDate) - business rule: one record per employee per day
            builder.HasIndex(x => new { x.EmployeeId, x.WorkDate })
                   .IsUnique()
                   .HasDatabaseName("IX_AttendanceRecord_EmployeeId_WorkDate");

            // Indexes for commonly queried fields
            builder.HasIndex(x => x.WorkDate)
                   .HasDatabaseName("IX_AttendanceRecord_WorkDate");

            builder.HasIndex(x => x.EmployeeId)
                   .HasDatabaseName("IX_AttendanceRecord_EmployeeId");

            // Properties
            builder.Property(x => x.WorkDate)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasConversion<int>(); // Store enum as int

            builder.Property(x => x.CheckInUtc)
                   .HasColumnType("datetime2");

            builder.Property(x => x.CheckOutUtc)
                   .HasColumnType("datetime2");

            builder.Property(x => x.CreatedAtUtc)
                   .IsRequired()
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.UpdatedAtUtc)
                   .IsRequired()
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.Notes)
                   .HasMaxLength(500);

            // Foreign key relationship to Employee
            builder.HasOne(x => x.Employee)
                   .WithMany()
                   .HasForeignKey(x => x.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}