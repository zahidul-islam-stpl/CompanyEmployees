using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Commands;

public sealed record CheckInAttendanceCommand(AttendanceForCreationDto Attendance) : IRequest<AttendanceDto>;
