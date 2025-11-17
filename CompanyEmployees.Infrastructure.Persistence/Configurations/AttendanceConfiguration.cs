using CompanyEmployees.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyEmployees.Infrastructure.Persistence.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.HasKey(a => a.Id);

        // BUG: Navigation property name is wrong - entity has EmployeeDetails not Employee
        builder.HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(a => a.Notes)
            .HasMaxLength(200);

        builder.Property(a => a.CheckInTime)
            .IsRequired();
        
        // BUG: Missing index on EmployeeId and CheckInTime for performance
        // BUG: Missing unique constraint on (EmployeeId, CheckInTime.Date) to prevent duplicates at DB level
    }
}
