using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Infrastructure.Persistence.Repositories;

internal sealed class AttendanceRepository : RepositoryBase<Attendance>, IAttendanceRepository
{
    public AttendanceRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync(bool trackChanges, CancellationToken ct = default) =>
        await FindAll(trackChanges)
            .OrderByDescending(a => a.CheckInTime)
            .ToListAsync(ct);

    public async Task<Attendance> GetAttendanceAsync(Guid attendanceId, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(a => a.Id.Equals(attendanceId), trackChanges)
            .SingleOrDefaultAsync(ct);

    public async Task<IEnumerable<Attendance>> GetAttendancesByEmployeeIdAsync(Guid employeeId, bool trackChanges, CancellationToken ct = default) =>
        await FindByCondition(a => a.EmployeeId.Equals(employeeId), trackChanges)
            .OrderByDescending(a => a.CheckInTime)
            .ToListAsync(ct);

    public async Task<Attendance?> GetTodayAttendanceByEmployeeIdAsync(Guid employeeId, bool trackChanges, CancellationToken ct = default)
    {
        // BUG: Using DateTime.Now instead of DateTime.UtcNow - timezone issue
        var today = DateTime.Now.Date;
        
        // BUG: Using Date comparison can fail with timezone differences
        return await FindByCondition(
                a => a.EmployeeId.Equals(employeeId) && 
                     a.CheckInTime.Date == today, 
                trackChanges)
            .FirstOrDefaultAsync(ct);
    }

    public void CreateAttendance(Attendance attendance) => Create(attendance);

    public void DeleteAttendance(Attendance attendance) => Delete(attendance);
}
