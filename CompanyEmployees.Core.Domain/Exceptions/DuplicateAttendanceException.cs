namespace CompanyEmployees.Core.Domain.Exceptions;

public sealed class DuplicateAttendanceException : BadRequestException
{
    public DuplicateAttendanceException(Guid employeeId, DateOnly workDate)
        : base($"An attendance record for employee {employeeId} on {workDate:yyyy-MM-dd} already exists.")
    {
    }
}