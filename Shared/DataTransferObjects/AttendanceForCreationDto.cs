namespace Shared.DataTransferObjects;

public record AttendanceForCreationDto(
    Guid EmployeeId,
    DateTime CheckInTime,
    string Status,
    string? Notes);
