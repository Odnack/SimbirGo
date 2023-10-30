using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Simbir.Go.DataAccess.Context;
using Simbir.GO.Entities.Options;

namespace Simbir.Go.DataAccess.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PostgresContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(PostgresContext).Assembly.FullName)
                ));

        return services;
    }

    public static IServiceProvider MigrateDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<PostgresContext>();
        context.Database.Migrate();
        return services;
    }
    
    public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtOptions>(config.GetSection(JwtOptions.SectionOption));
        return services;
    }
}
