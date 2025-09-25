using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Queries
{
    public sealed record GetEmployeeAttendanceQuery(
        Guid EmployeeId,
        DateTime? FromDate,
        DateTime? ToDate,
        bool TrackChanges
    ) : IRequest<IEnumerable<AttendanceRecordDto>>;
}