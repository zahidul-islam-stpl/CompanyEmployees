using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Commands;

/// <summary>
/// Command to check in attendance.
/// </summary>
/// <param name="Attendance">The attendance details to be recorded.</param>
public sealed record CheckInAttendanceCommand(AttendanceForCreationDto Attendance) : IRequest<AttendanceDto>;
