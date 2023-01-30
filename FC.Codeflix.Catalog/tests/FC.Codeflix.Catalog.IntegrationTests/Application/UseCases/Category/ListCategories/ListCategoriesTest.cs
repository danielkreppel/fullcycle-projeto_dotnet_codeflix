using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategories
{
    [Collection(nameof(ListCategoriesTestFixture))]
    public class ListCategoriesTest
    {
        private readonly ListCategoriesTestFixture _fixture;

        public ListCategoriesTest(ListCategoriesTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(SearchReturnsListAndTotal))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        public async Task SearchReturnsListAndTotal()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var categorySampleList = _fixture.GetValidCategorySampleList(10);

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var categoryRepositoy = new CategoryRepository(dbContext);

            var input = new ListCategoriesInput(1, 20);

            var useCase = new UseCase.ListCategories(categoryRepositoy);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(categorySampleList.Count);
            output.Items.Should().HaveCount(categorySampleList.Count);

            foreach (CategoryModelOutput outputItem in output.Items)
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

        [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        public async Task SearchReturnsEmptyWhenEmpty()
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var input = new ListCategoriesInput(1, 20);

            var categoryRepositoy = new CategoryRepository(dbContext);

            var useCase = new UseCase.ListCategories(categoryRepositoy);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(0);
            output.Items.Should().HaveCount(0);
        }

        [Theory(DisplayName = nameof(SearchReturnsPaginated))]
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task SearchReturnsPaginated(int qttyCategoryToGenerate, int page, int perPage, int expectedQttyItems)
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var categorySampleList = _fixture.GetValidCategorySampleList(qttyCategoryToGenerate);

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var input = new ListCategoriesInput(page, perPage, "", "", SearchOrder.ASC);

            var categoryRepositoy = new CategoryRepository(dbContext);

            var useCase = new UseCase.ListCategories(categoryRepositoy);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(qttyCategoryToGenerate);
            output.Items.Should().HaveCount(expectedQttyItems);

            foreach (var outputItem in output.Items)
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
        [Trait("Integration/Application", "ListCategories - Use Cases")]
        [InlineData("Action", 1, 5, 1, 1)]
        [InlineData("Horror", 1, 5, 3, 3)]
        [InlineData("Horror", 2, 5, 0, 3)]
        [InlineData("Sci-fi", 1, 5, 4, 4)]
        [InlineData("Sci-fi", 1, 2, 2, 4)]
        [InlineData("Sci-fi", 2, 3, 1, 4)]
        [InlineData("Sci-fi Other", 1, 3, 0, 0)]
        [InlineData("Robots", 1, 5, 2, 2)]
        public async Task SearchByText(string search, int page, int perPage, int expectedQttyItemsReturned, int expectedQttyTotalItems)
        {
            CodeflixCatalogDbContext dbContext = _fixture.CreateDbContextSample();
            var categorySampleList = _fixture.GetValidCategorySampleListWithNames(new List<string>()
            {
                "Action", "Horror", "Horror - Robots", "Horror - Based on Real Facts", "Drama", "Sci-fi IA", "Sci-fi Space", "Sci-fi Robots", "Sci-fi Future"
            });

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var input = new ListCategoriesInput(page, perPage, search, "", SearchOrder.ASC);

            var categoryRepositoy = new CategoryRepository(dbContext);

            var useCase = new UseCase.ListCategories(categoryRepositoy);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(expectedQttyTotalItems);
            output.Items.Should().HaveCount(expectedQttyItemsReturned);

            foreach (var outputItem in output.Items)
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
        [Trait("Integration/Application", "ListCategories - Use Cases")]
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

            await dbContext.AddRangeAsync(categorySampleList);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var searchOrder = orderDir.ToLower() == "asc" ? SearchOrder.ASC : SearchOrder.DESC;

            var input = new ListCategoriesInput(1, 20, "", orderBy, searchOrder);

            var categoryRepositoy = new CategoryRepository(dbContext);

            var useCase = new UseCase.ListCategories(categoryRepositoy);

            var output = await useCase.Handle(input, CancellationToken.None);

            var expectedOrderedList = _fixture.CloneCategoryListOrdered(categorySampleList, orderBy, searchOrder);

            output.Should().NotBeNull();
            output.Items.Should().NotBeNull();
            output.CurrentPage.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(categorySampleList.Count());
            output.Items.Should().HaveCount(categorySampleList.Count());

            for (int i = 0; i < expectedOrderedList.Count(); i++)
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
