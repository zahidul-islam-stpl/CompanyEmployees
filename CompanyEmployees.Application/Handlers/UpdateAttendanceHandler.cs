using AutoMapper;
using CompanyEmployees.Application.Commands;
using CompanyEmployees.Core.Domain.Exceptions;
using CompanyEmployees.Core.Domain.Repositories;
using MediatR;

namespace CompanyEmployees.Application.Handlers
{
    internal sealed class UpdateAttendanceHandler : IRequestHandler<UpdateAttendanceCommand>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public UpdateAttendanceHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
        {
            var attendance = await _repository.Attendance.GetAttendanceByIdAsync(request.AttendanceId, request.TrackChanges, cancellationToken);
            if (attendance == null)
            {
                throw new AttendanceNotFoundException(request.AttendanceId);
            }

            // Store original work date for validation
            var originalWorkDate = attendance.WorkDate;
            
            // Map the update data
            _mapper.Map(request.Attendance, attendance);
            
            // Business rule: Cannot change WorkDate after creation
            if (attendance.WorkDate != originalWorkDate)
            {
                throw new WorkDateChangeNotAllowedException();
            }

            // Validate time range if both times are provided
            if (!attendance.IsValidTimeRange())
            {
                throw new InvalidAttendanceTimeException();
            }

            // Update status based on check-in/out presence
            attendance.UpdateStatus();

            _repository.Attendance.UpdateAttendance(attendance);
            await _repository.SaveAsync(cancellationToken);
        }
    }
}