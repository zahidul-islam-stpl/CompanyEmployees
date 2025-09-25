namespace CompanyEmployees.Core.Domain.Exceptions;

public sealed class AlreadyCheckedOutException : BadRequestException
{
    public AlreadyCheckedOutException()
        : base("Employee has already checked out for this date.")
    {
    }
}