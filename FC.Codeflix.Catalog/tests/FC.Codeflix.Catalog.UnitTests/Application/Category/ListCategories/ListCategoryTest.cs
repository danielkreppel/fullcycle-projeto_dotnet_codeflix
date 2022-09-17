using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.ListCategories
{
    [Collection(nameof(ListCategoriesTestFixture))]
    public class ListCategoryTest
    {
        private readonly ListCategoriesTestFixture _fixture;
        public ListCategoryTest(ListCategoriesTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(List))]
        [Trait("Application", "ListCategory - UseCases")]
        public async Task List()
        {
            var categoriesListSample = _fixture.GetListGategoriesSample();
            var repositoryMock = _fixture.GetRepositoryMock();
            var input = _fixture.GetInputSample();
            var outputRepositorySearch = new SearchOutput<DomainEntity.Category>(
                    currentPage: input.Page,
                    perPage: input.PerPage,
                    items: (IReadOnlyList<DomainEntity.Category>)categoriesListSample,
                    total: new Random().Next(50, 200)
                );

            repositoryMock.Setup(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                        searchInput.Page == input.Page &&
                        searchInput.PerPage == input.PerPage &&
                        searchInput.Search == input.Search &&
                        searchInput.OrderBy == input.SortBy &&
                        searchInput.Order == input.SortDir
                    ),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(outputRepositorySearch);

            var useCase = new UseCase.ListCategories(repositoryMock.Object);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
            ((List<CategoryModelOutput>)output.Items).ForEach(outputItem =>
            {
                var repositoryCategory = outputRepositorySearch.Items.FirstOrDefault(x => x.Id == outputItem.Id);
                outputItem.Should().NotBeNull();
                outputItem.Name.Should().Be(repositoryCategory!.Name);
                outputItem.Description.Should().Be(repositoryCategory!.Description);
                outputItem.IsActive.Should().Be(repositoryCategory!.IsActive);
                outputItem.Id.Should().Be(repositoryCategory!.Id);
                outputItem.CreatedAt.Should().Be(repositoryCategory!.CreatedAt);
            });

            repositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                        searchInput.Page == input.Page &&
                        searchInput.PerPage == input.PerPage &&
                        searchInput.Search == input.Search &&
                        searchInput.OrderBy == input.SortBy &&
                        searchInput.Order == input.SortDir
                    ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Theory(DisplayName = nameof(ListInputWithoutAllParameters))]
        [Trait("Application", "ListCategory - UseCases")]
        [MemberData(
            nameof(ListCategoriesTestDataGenerator.GetInputsWithoutAllParameters),
            parameters: 14,
            MemberType = typeof(ListCategoriesTestDataGenerator)
         )]
        public async Task ListInputWithoutAllParameters(ListCategoriesInput input)
        {
            var categoriesListSample = _fixture.GetListGategoriesSample();
            var repositoryMock = _fixture.GetRepositoryMock();

            var outputRepositorySearch = new SearchOutput<DomainEntity.Category>(
                    currentPage: input.Page,
                    perPage: input.PerPage,
                    items: (IReadOnlyList<DomainEntity.Category>)categoriesListSample,
                    total: new Random().Next(50, 200)
                );

            repositoryMock.Setup(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                        searchInput.Page == input.Page &&
                        searchInput.PerPage == input.PerPage &&
                        searchInput.Search == input.Search &&
                        searchInput.OrderBy == input.SortBy &&
                        searchInput.Order == input.SortDir
                    ),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(outputRepositorySearch);

            var useCase = new UseCase.ListCategories(repositoryMock.Object);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(outputRepositorySearch.Total);
            output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
            ((List<CategoryModelOutput>)output.Items).ForEach(outputItem =>
            {
                var repositoryCategory = outputRepositorySearch.Items.FirstOrDefault(x => x.Id == outputItem.Id);
                outputItem.Should().NotBeNull();
                outputItem.Name.Should().Be(repositoryCategory!.Name);
                outputItem.Description.Should().Be(repositoryCategory!.Description);
                outputItem.IsActive.Should().Be(repositoryCategory!.IsActive);
                outputItem.Id.Should().Be(repositoryCategory!.Id);
                outputItem.CreatedAt.Should().Be(repositoryCategory!.CreatedAt);
            });

            repositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                        searchInput.Page == input.Page &&
                        searchInput.PerPage == input.PerPage &&
                        searchInput.Search == input.Search &&
                        searchInput.OrderBy == input.SortBy &&
                        searchInput.Order == input.SortDir
                    ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact(DisplayName = nameof(ListOkWhenEmpty))]
        [Trait("Application", "ListCategory - UseCases")]
        public async Task ListOkWhenEmpty()
        {
            var categoriesListSample = _fixture.GetListGategoriesSample();
            var repositoryMock = _fixture.GetRepositoryMock();
            var input = _fixture.GetInputSample();
            var outputRepositorySearch = new SearchOutput<DomainEntity.Category>(
                    currentPage: input.Page,
                    perPage: input.PerPage,
                    items: new List<DomainEntity.Category>().AsReadOnly(),
                    total: 0
                );

            repositoryMock.Setup(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                        searchInput.Page == input.Page &&
                        searchInput.PerPage == input.PerPage &&
                        searchInput.Search == input.Search &&
                        searchInput.OrderBy == input.SortBy &&
                        searchInput.Order == input.SortDir
                    ),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(outputRepositorySearch);

            var useCase = new UseCase.ListCategories(repositoryMock.Object);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(outputRepositorySearch.CurrentPage);
            output.PerPage.Should().Be(outputRepositorySearch.PerPage);
            output.Total.Should().Be(0);
            output.Items.Should().HaveCount(0);

            repositoryMock.Verify(x => x.Search(
                It.Is<SearchInput>(searchInput =>
                        searchInput.Page == input.Page &&
                        searchInput.PerPage == input.PerPage &&
                        searchInput.Search == input.Search &&
                        searchInput.OrderBy == input.SortBy &&
                        searchInput.Order == input.SortDir
                    ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }
    }
}
