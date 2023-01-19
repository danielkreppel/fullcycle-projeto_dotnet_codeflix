using FC.Codeflix.Category.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Api.Configurations
{
    public static class ConnectionsConfiguration
    {
        public static IServiceCollection AddAppConnections(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbConnection(config);

            return services;
        }

        private static IServiceCollection AddDbConnection(this IServiceCollection services, IConfiguration config)
        {
            var connString = config.GetConnectionString("CatalogDB");
            services.AddDbContext<CodeflixCatalogDbContext>(
                options => options.UseMySql(connString, ServerVersion.AutoDetect(connString))
                //To use an in memory database instead
                //options => options.UseInMemoryDatabase("InMemory-DSV-Database")
            );

            return services;
        }
    }
}
