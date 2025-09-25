using AutoMapper;
using CompanyEmployees.Application.Commands;
using CompanyEmployees.Core.Domain.Exceptions;
using CompanyEmployees.Core.Domain.Repositories;
using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Handlers
{
    internal sealed class UpdateCheckOutHandler : IRequestHandler<UpdateCheckOutCommand, AttendanceRecordDto>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public UpdateCheckOutHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AttendanceRecordDto> Handle(UpdateCheckOutCommand request, CancellationToken cancellationToken)
        {
            var attendance = await _repository.Attendance.GetAttendanceByIdAsync(request.AttendanceId, true, cancellationToken);
            if (attendance == null)
            {
                throw new AttendanceNotFoundException(request.AttendanceId);
            }

            // Check if already checked out
            if (attendance.CheckOutUtc.HasValue)
            {
                throw new AlreadyCheckedOutException();
            }

            // Set check-out time
            attendance.CheckOutUtc = request.CheckOut.CheckOutUtc;
            if (!string.IsNullOrWhiteSpace(request.CheckOut.Notes))
            {
                attendance.Notes = request.CheckOut.Notes;
            }

            // Validate time range
            if (!attendance.IsValidTimeRange())
            {
                throw new InvalidAttendanceTimeException();
            }

            // Update status
            attendance.UpdateStatus();

            _repository.Attendance.UpdateAttendance(attendance);
            await _repository.SaveAsync(cancellationToken);

            var attendanceToReturn = _mapper.Map<AttendanceRecordDto>(attendance);
            return attendanceToReturn;
        }
    }
}