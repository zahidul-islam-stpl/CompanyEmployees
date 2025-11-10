using AutoMapper;
using CompanyEmployees.Application.Commands;
using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Exceptions;
using CompanyEmployees.Core.Domain.Repositories;
using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Handlers;

internal sealed class CheckInAttendanceHandler : IRequestHandler<CheckInAttendanceCommand, AttendanceDto>
{
    private readonly IRepositoryManager _repository;
    private readonly IMapper _mapper;

    public CheckInAttendanceHandler(IRepositoryManager repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AttendanceDto> Handle(CheckInAttendanceCommand request, CancellationToken cancellationToken)
    {
        // Check if employee already checked in today
        var existingAttendance = await _repository.Attendance
            .GetTodayAttendanceByEmployeeIdAsync(request.Attendance.EmployeeId, false, cancellationToken);

        if (existingAttendance != null)
        {
            throw new AttendanceAlreadyExistsException(request.Attendance.EmployeeId);
        }

        var attendanceEntity = _mapper.Map<Attendance>(request.Attendance);
        attendanceEntity.Id = Guid.NewGuid();

        _repository.Attendance.CreateAttendance(attendanceEntity);
        await _repository.SaveAsync(cancellationToken);

        var attendanceToReturn = _mapper.Map<AttendanceDto>(attendanceEntity);

        return attendanceToReturn;
    }
}
