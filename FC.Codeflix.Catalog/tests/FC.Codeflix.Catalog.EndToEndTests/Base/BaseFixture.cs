using Bogus;
using FC.Codeflix.Category.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FC.Codeflix.Catalog.EndToEndTests.Base
{
    public class BaseFixture
    {
        protected Faker Faker { get; set; }
        public CustomWebApplicationFactory<Program> WebAppFactory { get; set; }
        public HttpClient HttpClient { get; set; }
        public ApiClient ApiClient { get; set; }

        private readonly string _dbConnectionString;

        public BaseFixture()
        {
            Faker = new Faker("pt_BR");
            WebAppFactory = new CustomWebApplicationFactory<Program>();
            HttpClient = WebAppFactory.CreateClient();
            ApiClient = new ApiClient(HttpClient);
            var config = WebAppFactory.Services.GetService(typeof(IConfiguration));
            ArgumentNullException.ThrowIfNull(config);
            _dbConnectionString = ((IConfiguration)config).GetConnectionString("CatalogDB");
        }

        public CodeflixCatalogDbContext CreateDbContextSample()
        {
            var dbContext = new CodeflixCatalogDbContext(
                    new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                    .UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString))
                    //.UseInMemoryDatabase("end2end-tests-db")
                    .Options
                );           

            return dbContext;
        }

        public CodeflixCatalogDbContext CleanPersistence()
        {
            var dbContext = CreateDbContextSample();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            return dbContext;
        }
    }
}
