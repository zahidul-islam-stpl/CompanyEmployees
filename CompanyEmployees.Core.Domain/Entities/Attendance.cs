using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyEmployees.Core.Domain.Entities
{
    public class Attendance
    {
        [Column("AttendanceId")]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(Employee))]
        public Guid EmployeeId { get; set; }

        [Required]
        public DateTime CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Maximum length for the Status is 10 characters.")]
        public string? Status { get; set; } // Present, Absent, Late, etc.

        [MaxLength(200, ErrorMessage = "Maximum length for the Notes is 200 characters.")]
        public string? Notes { get; set; }

        public Employee? Employee { get; set; }
    }
}
