using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Commands
{
    public sealed record UpdateAttendanceCommand(
        Guid AttendanceId,
        AttendanceForUpdateDto Attendance,
        bool TrackChanges
    ) : IRequest;
}