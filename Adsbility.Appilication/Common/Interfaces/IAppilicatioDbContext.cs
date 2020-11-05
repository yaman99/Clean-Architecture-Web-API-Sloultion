using Adsbility.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Adsbility.Appilication.Interfaces
{
    public interface IAppilicatioDbContext
    {
        DbSet<TestEnt> Test {get;set;}
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
