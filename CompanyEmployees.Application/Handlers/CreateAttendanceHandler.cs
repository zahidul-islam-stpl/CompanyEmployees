using AutoMapper;
using CompanyEmployees.Application.Commands;
using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Exceptions;
using CompanyEmployees.Core.Domain.Repositories;
using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Handlers
{
    internal sealed class CreateAttendanceHandler : IRequestHandler<CreateAttendanceCommand, AttendanceRecordDto>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public CreateAttendanceHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AttendanceRecordDto> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
        {
            // Check if attendance record already exists for this date
            var existingAttendance = await _repository.Attendance.GetAttendanceByEmployeeAndDateAsync(
                request.EmployeeId, 
                request.Attendance.WorkDate, 
                false, 
                cancellationToken);

            if (existingAttendance != null)
            {
                throw new DuplicateAttendanceException(request.EmployeeId, request.Attendance.WorkDate);
            }

            // Create new attendance record
            var attendanceEntity = _mapper.Map<AttendanceRecord>(request.Attendance);
            attendanceEntity.EmployeeId = request.EmployeeId;
            
            // Auto-set check-in time if not provided
            if (!attendanceEntity.CheckInUtc.HasValue)
            {
                attendanceEntity.CheckInUtc = DateTime.UtcNow;
            }
            
            // Update status based on check-in/out presence
            attendanceEntity.UpdateStatus();

            _repository.Attendance.CreateAttendance(attendanceEntity);
            await _repository.SaveAsync(cancellationToken);

            var attendanceToReturn = _mapper.Map<AttendanceRecordDto>(attendanceEntity);
            return attendanceToReturn;
        }
    }
}