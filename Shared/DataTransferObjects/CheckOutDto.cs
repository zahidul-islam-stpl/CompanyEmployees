using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record CheckOutDto(
    [Required(ErrorMessage = "Check-out time is required.")]
    DateTime CheckOutUtc,
    
    string? Notes
);