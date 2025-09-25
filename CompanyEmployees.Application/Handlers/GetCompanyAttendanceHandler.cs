using AutoMapper;
using CompanyEmployees.Application.Queries;
using CompanyEmployees.Core.Domain.Repositories;
using MediatR;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Application.Handlers
{
    internal sealed class GetCompanyAttendanceHandler : IRequestHandler<GetCompanyAttendanceQuery, IEnumerable<AttendanceRecordDto>>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public GetCompanyAttendanceHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AttendanceRecordDto>> Handle(GetCompanyAttendanceQuery request, CancellationToken cancellationToken)
        {
            var attendanceRecords = await _repository.Attendance.GetCompanyAttendanceAsync(
                request.CompanyId, 
                request.FromDate, 
                request.ToDate, 
                request.TrackChanges, 
                cancellationToken);

            var attendanceDtos = _mapper.Map<IEnumerable<AttendanceRecordDto>>(attendanceRecords);
            return attendanceDtos;
        }
    }
}