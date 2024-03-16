using Challengify.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Challengify.Entities.Database;

public interface IAppDbContext
{
    DbSet<Challenge> Challenges { get; set; }
    DbSet<Result> Results { get; set; }
    DbSet<User> Users { get; set; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}