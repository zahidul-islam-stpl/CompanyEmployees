using CompanyEmployees.Core.Domain.ContextFactory;

namespace CompanyEmployees.Core.Domain.Repositories;

public interface IRepositoryManager
{
    ICompanyRepository Company { get; }
    IAttendanceRepository Attendance { get; }
    Task SaveAsync(CancellationToken ct = default);
}
