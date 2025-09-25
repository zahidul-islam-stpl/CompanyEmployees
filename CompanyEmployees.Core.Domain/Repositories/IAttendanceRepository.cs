using CompanyEmployees.Core.Domain.Entities;

namespace CompanyEmployees.Core.Domain.Repositories
{
    public interface IAttendanceRepository
    {
        Task<AttendanceRecord?> GetAttendanceByIdAsync(Guid attendanceId, bool trackChanges, CancellationToken ct = default);
        Task<AttendanceRecord?> GetAttendanceByEmployeeAndDateAsync(Guid employeeId, DateOnly workDate, bool trackChanges, CancellationToken ct = default);
        Task<IEnumerable<AttendanceRecord>> GetEmployeeAttendanceAsync(Guid employeeId, DateTime? fromDate = null, DateTime? toDate = null, bool trackChanges = false, CancellationToken ct = default);
        Task<IEnumerable<AttendanceRecord>> GetCompanyAttendanceAsync(Guid companyId, DateTime? fromDate = null, DateTime? toDate = null, bool trackChanges = false, CancellationToken ct = default);
        void CreateAttendance(AttendanceRecord attendance);
        void UpdateAttendance(AttendanceRecord attendance);
        void DeleteAttendance(AttendanceRecord attendance);
    }
}