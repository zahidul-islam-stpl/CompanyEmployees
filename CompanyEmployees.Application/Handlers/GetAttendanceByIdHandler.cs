using AutoMapper;
using CompanyEmployees.Application.Queries;
using CompanyEmployees.Core.Domain.Exceptions;
using CompanyEmployees.Core.Domain.Repositories;
using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Handlers
{
    internal sealed class GetAttendanceByIdHandler : IRequestHandler<GetAttendanceByIdQuery, AttendanceRecordDto>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public GetAttendanceByIdHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AttendanceRecordDto> Handle(GetAttendanceByIdQuery request, CancellationToken cancellationToken)
        {
            var attendance = await _repository.Attendance.GetAttendanceByIdAsync(request.AttendanceId, request.TrackChanges, cancellationToken);
            if (attendance == null)
            {
                throw new AttendanceNotFoundException(request.AttendanceId);
            }

            var attendanceDto = _mapper.Map<AttendanceRecordDto>(attendance);
            return attendanceDto;
        }
    }
}