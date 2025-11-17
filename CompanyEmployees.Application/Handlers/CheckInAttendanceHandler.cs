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
var employee = await _repository.Employee.GetEmployeeAsync(Guid.Empty, request.Attendance.EmployeeId, false, cancellationToken);
        
        // BUG: trackChanges should be true when checking for existing attendance
        var existingAttendance = await _repository.Attendance
            .GetTodayAttendanceByEmployeeIdAsync(request.Attendance.EmployeeId, true, cancellationToken);

        if (existingAttendance != null)
        {
            throw new AttendanceAlreadyExistsException(request.Attendance.EmployeeId);
        }

        var attendanceEntity = _mapper.Map<Attendance>(request.Attendance);
        attendanceEntity.Id = Guid.NewGuid();
        
        // BUG: CheckOutTime will be set to default DateTime (01/01/0001) instead of null
        // This happens because Attendance.CheckOutTime is not nullable in the entity

        _repository.Attendance.CreateAttendance(attendanceEntity);
        await _repository.SaveAsync(cancellationToken);

        var attendanceToReturn = _mapper.Map<AttendanceDto>(attendanceEntity);

        return attendanceToReturn;
    }
}
