using AutoMapper;
using CompanyEmployees.Application.Queries;
using CompanyEmployees.Core.Domain.Repositories;
using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Handlers
{
    internal sealed class GetEmployeeAttendanceHandler : IRequestHandler<GetEmployeeAttendanceQuery, IEnumerable<AttendanceRecordDto>>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public GetEmployeeAttendanceHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AttendanceRecordDto>> Handle(GetEmployeeAttendanceQuery request, CancellationToken cancellationToken)
        {
            var attendanceRecords = await _repository.Attendance.GetEmployeeAttendanceAsync(
                request.EmployeeId, 
                request.FromDate, 
                request.ToDate, 
                request.TrackChanges, 
                cancellationToken);

            var attendanceDtos = _mapper.Map<IEnumerable<AttendanceRecordDto>>(attendanceRecords);
            return attendanceDtos;
        }
    }
}