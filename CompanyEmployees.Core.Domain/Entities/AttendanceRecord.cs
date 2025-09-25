using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyEmployees.Core.Domain.Entities
{
    public class AttendanceRecord
    {
        [Column("AttendanceRecordId")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Employee ID is a required field.")]
        [ForeignKey(nameof(Employee))]
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "Work date is a required field.")]
        public DateOnly WorkDate { get; set; }

        public DateTime? CheckInUtc { get; set; }

        public DateTime? CheckOutUtc { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        // Business rule: Auto-calculate status based on check-in/out presence
        public void UpdateStatus()
        {
            if (CheckInUtc.HasValue && CheckOutUtc.HasValue)
            {
                Status = AttendanceStatus.Present;
            }
            else if (CheckInUtc.HasValue && !CheckOutUtc.HasValue)
            {
                // Check if it's after normal work hours to determine MissingCheckOut vs Partial
                var currentTime = DateTime.UtcNow;
                var workDate = WorkDate.ToDateTime(TimeOnly.MinValue);
                var endOfWorkDay = workDate.AddHours(18); // Assume 6 PM end of work day

                Status = currentTime > endOfWorkDay ? AttendanceStatus.MissingCheckOut : AttendanceStatus.Partial;
            }
            else
            {
                Status = AttendanceStatus.Absent;
            }

            UpdatedAtUtc = DateTime.UtcNow;
        }

        // Business rule: CheckOut must be after CheckIn
        public bool IsValidTimeRange()
        {
            if (CheckInUtc.HasValue && CheckOutUtc.HasValue)
            {
                return CheckOutUtc.Value >= CheckInUtc.Value;
            }
            return true; // Valid if one or both are null
        }
    }

    public enum AttendanceStatus
    {
        Present = 1,        // Both CheckIn and CheckOut exist
        Partial = 2,        // Only CheckIn exists (missing checkout)
        Absent = 3,         // Neither CheckIn nor CheckOut (manually marked)
        MissingCheckOut = 4 // Only CheckIn exists after end of workday
    }
}