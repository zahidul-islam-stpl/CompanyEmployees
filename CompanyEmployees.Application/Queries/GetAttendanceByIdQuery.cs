using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Queries
{
    public sealed record GetAttendanceByIdQuery(Guid AttendanceId, bool TrackChanges) : IRequest<AttendanceRecordDto>;
}