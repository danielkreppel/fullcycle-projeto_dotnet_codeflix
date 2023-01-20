using Bogus;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.IntegrationTests.Base
{
    public class BaseFixture
    {
        public BaseFixture()
        {
            Faker = new Faker("pt_BR");
        }

        protected Faker Faker { get; set; }

        public CodeflixCatalogDbContext CreateDbContextSample(bool preserveData = false, string dbId = "")
        {
            var dbContext = new CodeflixCatalogDbContext(
                    new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
                    .UseInMemoryDatabase($"Integration-tests-db{dbId}")
                    .Options
                );

            if (!preserveData)
                dbContext.Database.EnsureDeleted();

            return dbContext;
        }
    }
}
