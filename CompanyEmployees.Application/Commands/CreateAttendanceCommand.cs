using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Commands
{
    public sealed record CreateAttendanceCommand(
        Guid EmployeeId, 
        AttendanceForCreationDto Attendance
    ) : IRequest<AttendanceRecordDto>;
}