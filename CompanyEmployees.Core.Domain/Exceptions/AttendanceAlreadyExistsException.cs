namespace CompanyEmployees.Core.Domain.Exceptions;

public sealed class AttendanceAlreadyExistsException : BadRequestException
{
    public AttendanceAlreadyExistsException(Guid employeeId)
        : base($"Employee with ID {employeeId} has already checked in today.")
    {
    }
}
