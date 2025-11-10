namespace Shared.DataTransferObjects;

public record AttendanceDto(
    Guid Id, 
    Guid EmployeeId, 
    DateTime CheckInTime, 
    DateTime? CheckOutTime, 
    string Status, 
    string? Notes);
