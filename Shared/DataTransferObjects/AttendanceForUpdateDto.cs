using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record AttendanceForUpdateDto(
    [Required(ErrorMessage = "Work date is required.")]
    DateOnly WorkDate,
    
    DateTime? CheckInUtc,
    
    DateTime? CheckOutUtc,
    
    string? Notes
) : AttendanceForManipulationDto(WorkDate, CheckInUtc, CheckOutUtc, Notes);