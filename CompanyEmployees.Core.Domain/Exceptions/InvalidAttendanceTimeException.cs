namespace CompanyEmployees.Core.Domain.Exceptions;

public sealed class InvalidAttendanceTimeException : BadRequestException
{
    public InvalidAttendanceTimeException(string message)
        : base(message)
    {
    }

    public InvalidAttendanceTimeException()
        : base("Check-out time must be after check-in time.")
    {
    }
}