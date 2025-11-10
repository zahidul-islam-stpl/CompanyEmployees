using CompanyEmployees.Core.Domain.Entities;

namespace CompanyEmployees.Core.Domain.Repositories;

public interface IAttendanceRepository
{
    Task<IEnumerable<Attendance>> GetAllAttendancesAsync(bool trackChanges, CancellationToken ct = default);
    Task<Attendance> GetAttendanceAsync(Guid attendanceId, bool trackChanges, CancellationToken ct = default);
    Task<IEnumerable<Attendance>> GetAttendancesByEmployeeIdAsync(Guid employeeId, bool trackChanges, CancellationToken ct = default);
    Task<Attendance?> GetTodayAttendanceByEmployeeIdAsync(Guid employeeId, bool trackChanges, CancellationToken ct = default);
    void CreateAttendance(Attendance attendance);
    void DeleteAttendance(Attendance attendance);
}
