using Microsoft.EntityFrameworkCore;
using MinimalApis.Domain.Accounts;

namespace MinimalApis.Domain;

public interface IDbContext
{
    DbSet<Account> Accounts { get; set; }
    DbSet<TwoFactor> TwoFactors { get; set; }
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
