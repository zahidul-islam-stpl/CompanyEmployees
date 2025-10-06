using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Infrastructure.Persistence.Repositories
{
    internal sealed class AttendanceRepository : RepositoryBase<AttendanceRecord>, IAttendanceRepository
    {
        public AttendanceRepository(RepositoryContext repositoryContext) 
            : base(repositoryContext)
        {
        }

        public async Task<AttendanceRecord?> GetAttendanceByIdAsync(Guid attendanceId, bool trackChanges, CancellationToken ct = default)
        {
            return await FindByCondition(a => a.Id.Equals(attendanceId), trackChanges)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<AttendanceRecord?> GetAttendanceByEmployeeAndDateAsync(Guid employeeId, DateOnly workDate, bool trackChanges, CancellationToken ct = default)
        {
            return await FindByCondition(a => a.EmployeeId.Equals(employeeId) && a.WorkDate.Equals(workDate), trackChanges)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<IEnumerable<AttendanceRecord>> GetEmployeeAttendanceAsync(Guid employeeId, DateTime? fromDate = null, DateTime? toDate = null, bool trackChanges = false, CancellationToken ct = default)
        {
            IQueryable<AttendanceRecord> query = FindByCondition(a => a.EmployeeId.Equals(employeeId), trackChanges)
                .Include(a => a.Employee);

            if (fromDate.HasValue)
            {
                var fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
                query = query.Where(a => a.WorkDate >= fromDateOnly);
            }

            if (toDate.HasValue)
            {
                var toDateOnly = DateOnly.FromDateTime(toDate.Value);
                query = query.Where(a => a.WorkDate <= toDateOnly);
            }

            return await query
                .OrderByDescending(a => a.WorkDate)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<AttendanceRecord>> GetCompanyAttendanceAsync(Guid companyId, DateTime? fromDate = null, DateTime? toDate = null, bool trackChanges = false, CancellationToken ct = default)
        {
            IQueryable<AttendanceRecord> query = FindByCondition(a => a.Employee != null && a.Employee.CompanyId.Equals(companyId), trackChanges)
                .Include(a => a.Employee);

            if (fromDate.HasValue)
            {
                var fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
                query = query.Where(a => a.WorkDate >= fromDateOnly);
            }

            if (toDate.HasValue)
            {
                var toDateOnly = DateOnly.FromDateTime(toDate.Value);
                query = query.Where(a => a.WorkDate <= toDateOnly);
            }

            return await query
                .OrderByDescending(a => a.WorkDate)
                .ThenBy(a => a.Employee?.Name ?? string.Empty)
                .ToListAsync(ct);
        }

        public void CreateAttendance(AttendanceRecord attendance) => Create(attendance);

        public void UpdateAttendance(AttendanceRecord attendance) => Update(attendance);

        public void DeleteAttendance(AttendanceRecord attendance) => Delete(attendance);
    }
}