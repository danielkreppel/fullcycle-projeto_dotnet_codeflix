using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Category.Infra.Data.EF;
using FluentAssertions;
using Repository = FC.Codeflix.Category.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository
{
    [Collection(nameof(CategoryRepositoryTestFixture))]
    public class CategoryRepositoryTest
    {
        private readonly CategoryRepositoryTestFixture _fixture;

        public CategoryRepositoryTest(CategoryRepositoryTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(Insert))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task Insert()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var categorySample = _fixture.GetValidCategorySample();
            var categoryRepositoy = new Repository.CategoryRepository(dbContext);
            
            await categoryRepositoy.Insert(categorySample, CancellationToken.None);
            await dbContext.SaveChangesAsync();

            //Using new context to crate a valid test not using the in memory tracked objects
            var dbCategory = await (_fixture.CreateDbContextSample()).Categories.FindAsync(categorySample.Id);
            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(categorySample.Name);
            dbCategory.Description.Should().Be(categorySample.Description);
            dbCategory.IsActive.Should().Be(categorySample.IsActive);
            dbCategory.CreatedAt.Should().Be(categorySample.CreatedAt);
        }

        [Fact(DisplayName = nameof(Get))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task Get()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var categorySample = _fixture.GetValidCategorySample();
            var categorySampleList = _fixture.GetValidCategorySampleList(15);
            categorySampleList.Add(categorySample);

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var categoryRepositoy = new Repository.CategoryRepository(_fixture.CreateDbContextSample());

            var dbCategory = await categoryRepositoy.Get(categorySample.Id, CancellationToken.None);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(categorySample.Name);
            dbCategory.Id.Should().Be(categorySample.Id);
            dbCategory.Description.Should().Be(categorySample.Description);
            dbCategory.IsActive.Should().Be(categorySample.IsActive);
            dbCategory.CreatedAt.Should().Be(categorySample.CreatedAt);
        }

        [Fact(DisplayName = nameof(GetThrowIfNotFound))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task GetThrowIfNotFound()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var categorySampleList = _fixture.GetValidCategorySampleList(15);
            var exampleId = Guid.NewGuid();

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var categoryRepositoy = new Repository.CategoryRepository(dbContext);

            var task = async () => await categoryRepositoy.Get(exampleId, CancellationToken.None);

            await task.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Category {exampleId} not found");
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task Update()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var categorySample = _fixture.GetValidCategorySample();
            var newCategoryValues = _fixture.GetValidCategorySample();
            var categorySampleList = _fixture.GetValidCategorySampleList(15);
            categorySampleList.Add(categorySample);

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var categoryRepositoy = new Repository.CategoryRepository(dbContext);

            categorySample.Update(newCategoryValues.Name, newCategoryValues.Description);

            await categoryRepositoy.Update(categorySample, CancellationToken.None);
            await dbContext.SaveChangesAsync();

            //Using new context to crate a valid test not using the in memory tracked objects
            var dbCategory = await (_fixture.CreateDbContextSample()).Categories.FindAsync(categorySample.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(newCategoryValues.Name);
            dbCategory.Id.Should().Be(categorySample.Id);
            dbCategory.Description.Should().Be(newCategoryValues.Description);
            dbCategory.IsActive.Should().Be(categorySample.IsActive);
            dbCategory.CreatedAt.Should().Be(categorySample.CreatedAt);
        }

        [Fact(DisplayName = nameof(Delete))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task Delete()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var categorySample = _fixture.GetValidCategorySample();
            var newCategoryValues = _fixture.GetValidCategorySample();
            var categorySampleList = _fixture.GetValidCategorySampleList(15);
            categorySampleList.Add(categorySample);

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var categoryRepositoy = new Repository.CategoryRepository(dbContext);

            await categoryRepositoy.Delete(categorySample, CancellationToken.None);
            await dbContext.SaveChangesAsync();

            //Using new context to crate a valid test not using the in memory tracked objects
            var dbCategory = await (_fixture.CreateDbContextSample()).Categories.FindAsync(categorySample.Id);

            dbCategory.Should().BeNull();
        }
    }
}
