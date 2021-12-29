using System.Reflection;
using Duende.IdentityServer.EntityFramework.Options;
using IdentityServer.Identity;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IdentityServer;

public class IdentityAppDbContext : ApiAuthorizationDbContext<ApplicationUser>
{

    public IdentityAppDbContext(
        DbContextOptions<IdentityAppDbContext> options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
