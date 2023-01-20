using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FluentAssertions;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

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
            var dbCategory = await (_fixture.CreateDbContextSample(true)).Categories.FindAsync(categorySample.Id);
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

            var categoryRepositoy = new Repository.CategoryRepository(_fixture.CreateDbContextSample(true));

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
                .WithMessage($"Category '{exampleId}' not found");
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
            var dbCategory = await (_fixture.CreateDbContextSample(true)).Categories.FindAsync(categorySample.Id);

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
            var categorySampleList = _fixture.GetValidCategorySampleList(15);
            categorySampleList.Add(categorySample);

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var categoryRepositoy = new Repository.CategoryRepository(dbContext);

            await categoryRepositoy.Delete(categorySample, CancellationToken.None);
            await dbContext.SaveChangesAsync();

            //Using new context to crate a valid test not using the in memory tracked objects
            var dbCategory = await (_fixture.CreateDbContextSample(true)).Categories.FindAsync(categorySample.Id);

            dbCategory.Should().BeNull();
        }

        [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task SearchReturnsListAndTotal()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var categorySampleList = _fixture.GetValidCategorySampleList(15);
            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.ASC);

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var categoryRepositoy = new Repository.CategoryRepository(dbContext);

            

            var output = await categoryRepositoy.Search(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(categorySampleList.Count);
            output.Items.Should().HaveCount(categorySampleList.Count);

            foreach(DomainEntity.Category outputItem in output.Items)
            {
                var item = categorySampleList.Find(category => category.Id == outputItem.Id);
                
                item.Should().NotBeNull();
                outputItem.Name.Should().Be(item!.Name);
                outputItem.Id.Should().Be(item.Id);
                outputItem.Description.Should().Be(item.Description);
                outputItem.IsActive.Should().Be(item.IsActive);
                outputItem.CreatedAt.Should().Be(item.CreatedAt);
            }           
        }

        [Fact(DisplayName = nameof(SearchReturnsEmptyWhenPersistenceIsEmpty))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        public async Task SearchReturnsEmptyWhenPersistenceIsEmpty()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.ASC);

            var categoryRepositoy = new Repository.CategoryRepository(dbContext);

            var output = await categoryRepositoy.Search(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(0);
            output.Items.Should().HaveCount(0);
        }

        [Theory(DisplayName = nameof(SearchReturnsPaginated))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task SearchReturnsPaginated(int qttyCategoryToGenerate, int page, int perPage, int expectedQttyItems)
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var categorySampleList = _fixture.GetValidCategorySampleList(qttyCategoryToGenerate);
            var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.ASC);

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var categoryRepositoy = new Repository.CategoryRepository(dbContext);

            var output = await categoryRepositoy.Search(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(qttyCategoryToGenerate);
            output.Items.Should().HaveCount(expectedQttyItems);

            foreach (DomainEntity.Category outputItem in output.Items)
            {
                var item = categorySampleList.Find(category => category.Id == outputItem.Id);

                item.Should().NotBeNull();
                outputItem.Name.Should().Be(item!.Name);
                outputItem.Id.Should().Be(item.Id);
                outputItem.Description.Should().Be(item.Description);
                outputItem.IsActive.Should().Be(item.IsActive);
                outputItem.CreatedAt.Should().Be(item.CreatedAt);
            }
        }

        [Theory(DisplayName = nameof(SearchByText))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        [InlineData("Action", 1, 5, 1, 1)]
        [InlineData("Horror", 1, 5, 3, 3)]
        [InlineData("Horror", 2, 5, 0, 3)]
        [InlineData("Sci-fi", 1, 5, 4, 4)]
        [InlineData("Sci-fi", 1, 2, 2, 4)]
        [InlineData("Sci-fi", 2, 3, 1, 4)]
        [InlineData("Sci-fi Other", 1, 3, 0, 0)]
        [InlineData("Robots", 1, 5, 2, 2)]
        public async Task SearchByText(string search, int page, int perPage, int expectedQttyItemsReturned, int expectedQttyTotalItems )
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var categorySampleList = _fixture.GetValidCategorySampleListWithNames(new List<string>()
            {
                "Action", "Horror", "Horror - Robots", "Horror - Based on Real Facts", "Drama", "Sci-fi IA", "Sci-fi Space", "Sci-fi Robots", "Sci-fi Future"
            });

            var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.ASC);

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var categoryRepositoy = new Repository.CategoryRepository(dbContext);

            var output = await categoryRepositoy.Search(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(expectedQttyTotalItems);
            output.Items.Should().HaveCount(expectedQttyItemsReturned);

            foreach (DomainEntity.Category outputItem in output.Items)
            {
                var item = categorySampleList.Find(category => category.Id == outputItem.Id);

                item.Should().NotBeNull();
                outputItem.Name.Should().Be(item!.Name);
                outputItem.Id.Should().Be(item.Id);
                outputItem.Description.Should().Be(item.Description);
                outputItem.IsActive.Should().Be(item.IsActive);
                outputItem.CreatedAt.Should().Be(item.CreatedAt);
            }
        }

        [Theory(DisplayName = nameof(SearchOrdered))]
        [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
        [InlineData("name", "asc")]
        [InlineData("name", "desc")]
        [InlineData("id", "asc")]
        [InlineData("id", "desc")]
        [InlineData("createdAt", "asc")]
        [InlineData("createdAt", "desc")]
        [InlineData("", "asc")]
        public async Task SearchOrdered(string orderBy, string orderDir)
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var categorySampleList = _fixture.GetValidCategorySampleList(10);

            var searchOrder = orderDir.ToLower() == "asc" ? SearchOrder.ASC : SearchOrder.DESC;

            var searchInput = new SearchInput(1, 20, "", orderBy, searchOrder);

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var categoryRepositoy = new Repository.CategoryRepository(dbContext);

            var output = await categoryRepositoy.Search(searchInput, CancellationToken.None);

            var expectedOrderedList = _fixture.CloneCategoryListOrdered(categorySampleList, orderBy, searchOrder);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(categorySampleList.Count());
            output.Items.Should().HaveCount(categorySampleList.Count());

            for (int i=0; i< expectedOrderedList.Count(); i++)
            {
                var expectedItem = expectedOrderedList[i];
                var outputItem = output.Items[i];

                expectedItem.Should().NotBeNull();
                outputItem.Should().NotBeNull();

                outputItem.Name.Should().Be(expectedItem.Name);
                outputItem.Id.Should().Be(expectedItem.Id);
                outputItem.Description.Should().Be(expectedItem.Description);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
            }
        }
    }
}
