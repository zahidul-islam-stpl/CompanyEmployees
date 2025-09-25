namespace CompanyEmployees.Core.Domain.Exceptions;

public sealed class WorkDateChangeNotAllowedException : BadRequestException
{
    public WorkDateChangeNotAllowedException()
        : base("Work date cannot be changed after attendance record creation.")
    {
    }
}