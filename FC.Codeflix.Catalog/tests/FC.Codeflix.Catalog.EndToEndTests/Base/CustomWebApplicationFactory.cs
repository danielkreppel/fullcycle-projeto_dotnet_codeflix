using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Codeflix.Catalog.EndToEndTests.Base
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("EndToEndTest");
            builder.ConfigureServices(services =>
            {
                var serviceProvider = services.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<CodeflixCatalogDbContext>();

                    ArgumentNullException.ThrowIfNull(context);

                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }

                //To use an in memory database instead
                //var dbOptions = services.FirstOrDefault(x => x.ServiceType == typeof(DbContextOptions<CodeflixCatalogDbContext>));
                
                //if (dbOptions is not null)
                //    services.Remove(dbOptions);

                //services.AddDbContext<CodeflixCatalogDbContext>(options =>
                //{
                //    options.UseInMemoryDatabase("end2end-tests-db");
                //});
            });

            base.ConfigureWebHost(builder);
        }
    }
}
