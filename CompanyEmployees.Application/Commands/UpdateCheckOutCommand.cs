using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Commands
{
    public sealed record UpdateCheckOutCommand(
        Guid AttendanceId,
        CheckOutDto CheckOut
    ) : IRequest<AttendanceRecordDto>;
}