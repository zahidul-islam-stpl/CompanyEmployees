namespace CompanyEmployees.Core.Domain.Exceptions;

public sealed class AlreadyCheckedInException : BadRequestException
{
    public AlreadyCheckedInException(Guid employeeId, DateOnly workDate)
        : base($"Employee {employeeId} has already checked in for {workDate:yyyy-MM-dd}.")
    {
    }
}