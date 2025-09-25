using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Queries
{
    public sealed record GetCompanyAttendanceQuery(
        Guid CompanyId,
        DateTime? FromDate,
        DateTime? ToDate,
        bool TrackChanges
    ) : IRequest<IEnumerable<AttendanceRecordDto>>;
}