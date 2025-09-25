namespace CompanyEmployees.Core.Domain.Exceptions;

public sealed class AttendanceNotFoundException : NotFoundException
{
    public AttendanceNotFoundException(Guid attendanceId)
        : base($"The attendance record with id: {attendanceId} doesn't exist in the database.")
    {
    }
}