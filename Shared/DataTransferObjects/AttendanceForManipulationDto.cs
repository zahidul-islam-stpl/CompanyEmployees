using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public abstract record AttendanceForManipulationDto(
    [Required(ErrorMessage = "Work date is required.")]
    DateOnly WorkDate,
    
    DateTime? CheckInUtc,
    
    DateTime? CheckOutUtc,
    
    string? Notes
);