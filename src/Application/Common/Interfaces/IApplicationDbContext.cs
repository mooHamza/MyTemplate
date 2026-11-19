using FullTemplate.Domain.Entities;

namespace FullTemplate.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<RefreshToken> RefreshTokens { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
