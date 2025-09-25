using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record AttendanceForCreationDto(
    [Required(ErrorMessage = "Work date is required.")]
    DateOnly WorkDate,
    
    DateTime? CheckInUtc,
    
    string? Notes
) : AttendanceForManipulationDto(WorkDate, CheckInUtc, null, Notes);