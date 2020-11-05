using Adsbility.Appilication.Interfaces;
using Adsbility.Domain.Entities;
using Adsbility.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Adsbility.Infrastructure.presistence
{
    public class AppilicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>, IAppilicatioDbContext
    {
        public AppilicationDbContext( DbContextOptions options) : base(options)
        {
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            int result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        public DbSet<TestEnt> Test { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
    }
}
