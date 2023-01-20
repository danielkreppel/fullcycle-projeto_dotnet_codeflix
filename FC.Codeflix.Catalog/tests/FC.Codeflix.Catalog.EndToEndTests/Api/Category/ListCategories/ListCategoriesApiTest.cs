using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTime;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using Xunit.Abstractions;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories
{
    [Collection(nameof(ListCategoriesApiTestFixture))]
    public class ListCategoriesApiTest : IDisposable
    {
        private readonly ListCategoriesApiTestFixture _fixture;
        private readonly ITestOutputHelper _output;

        public ListCategoriesApiTest(ListCategoriesApiTestFixture fixture, ITestOutputHelper output)
        {
            (_fixture, _output) = (fixture, output);
        }

        [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        public async Task ListCategoriesAndTotalByDefault()
        {
            var defaultPerPage = 15;
            var categoriesListSample = _fixture.GetCategoriesListSample(20);
            await _fixture.Persistence.InsertList(categoriesListSample);

            var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>($"/Categories");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Items.Should().HaveCount(defaultPerPage);
            output.Total.Should().Be(categoriesListSample.Count);
            output.Page.Should().Be(1);
            output.PerPage.Should().Be(defaultPerPage);

            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var expectedItem = categoriesListSample.FirstOrDefault(c => c.Id == outputItem.Id);

                expectedItem.Should().NotBeNull();
                outputItem.Name.Should().Be(expectedItem!.Name);
                outputItem.Description.Should().Be(expectedItem.Description);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
            }
        }

        [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        public async Task ItemsEmptyWhenPersistenceEmpty()
        {
            var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>($"/Categories");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Items.Should().HaveCount(0);
            output.Total.Should().Be(0);
        }

        [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        public async Task ListCategoriesAndTotal()
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(20);
            await _fixture.Persistence.InsertList(categoriesListSample);
            var input = new ListCategoriesInput(page: 1, perPage: 5);

            var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>($"/Categories", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Items.Should().HaveCount(5);
            output.Total.Should().Be(categoriesListSample.Count);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);

            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var expectedItem = categoriesListSample.FirstOrDefault(c => c.Id == outputItem.Id);

                expectedItem.Should().NotBeNull();
                outputItem.Name.Should().Be(expectedItem!.Name);
                outputItem.Description.Should().Be(expectedItem.Description);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
            }
        }

        [Theory(DisplayName = nameof(ListPaginated))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task ListPaginated(int qttyCategoryToGenerate, int page, int perPage, int expectedQttyItems)
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(qttyCategoryToGenerate);
            await _fixture.Persistence.InsertList(categoriesListSample);
            var input = new ListCategoriesInput(page, perPage);

            var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>($"/Categories", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Items.Should().HaveCount(expectedQttyItems);
            output.Total.Should().Be(categoriesListSample.Count);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);

            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var expectedItem = categoriesListSample.FirstOrDefault(c => c.Id == outputItem.Id);

                expectedItem.Should().NotBeNull();
                outputItem.Name.Should().Be(expectedItem!.Name);
                outputItem.Description.Should().Be(expectedItem.Description);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
            }
        }

        [Theory(DisplayName = nameof(SearchByText))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
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
            var categoryNamesList = new List<string>()
            {
                "Action", "Horror", "Horror - Robots", "Horror - Based on Real Facts", "Drama", "Sci-fi IA", "Sci-fi Space", "Sci-fi Robots", "Sci-fi Future"
            };

            var categoriesListSample = _fixture.GetValidCategorySampleListWithNames(categoryNamesList);
            await _fixture.Persistence.InsertList(categoriesListSample);
            var input = new ListCategoriesInput(page, perPage, search);

            var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>($"/Categories", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Items.Should().HaveCount(expectedQttyItemsReturned);
            output.Total.Should().Be(expectedQttyTotalItems);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);

            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var expectedItem = categoriesListSample.FirstOrDefault(c => c.Id == outputItem.Id);

                expectedItem.Should().NotBeNull();
                outputItem.Name.Should().Be(expectedItem!.Name);
                outputItem.Description.Should().Be(expectedItem.Description);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
            }
        }

        [Theory(DisplayName = nameof(ListOrdered))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        [InlineData("name", "asc")]
        [InlineData("name", "desc")]
        [InlineData("id", "asc")]
        [InlineData("id", "desc")]
        [InlineData("", "asc")]
        public async Task ListOrdered(string orderBy, string orderDir)
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(10);
            await _fixture.Persistence.InsertList(categoriesListSample);

            var searchOrder = orderDir.ToLower() == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
            var input = new ListCategoriesInput(page: 1, perPage: 20, sortBy: orderBy, sortDir: searchOrder);

            var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>($"/Categories", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Items.Should().HaveCount(categoriesListSample.Count);
            output.Total.Should().Be(categoriesListSample.Count);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);

            var expectedOrderedList = _fixture.CloneCategoryListOrdered(categoriesListSample, orderBy, searchOrder);

            var count = 0;
            var expectedArray = expectedOrderedList.Select(x => $"{count++} {x.Name} {x.CreatedAt} {JsonConvert.SerializeObject(x)}");
            count = 0;
            var outputdArray = output.Items.Select(x => $"{count++} {x.Name} {x.CreatedAt} {JsonConvert.SerializeObject(x)}");

            _output.WriteLine("Expected...");
            _output.WriteLine(String.Join('\n', expectedArray));
            _output.WriteLine("Output...");
            _output.WriteLine(String.Join('\n', outputdArray));

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
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());
            }
        }

        [Theory(DisplayName = nameof(ListOrderedDates))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        [InlineData("createdAt", "asc")]
        [InlineData("createdAt", "desc")]
        public async Task ListOrderedDates(string orderBy, string orderDir)
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(10);
            await _fixture.Persistence.InsertList(categoriesListSample);

            var searchOrder = orderDir.ToLower() == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
            var input = new ListCategoriesInput(page: 1, perPage: 20, sortBy: orderBy, sortDir: searchOrder);

            var (response, output) = await _fixture.ApiClient.Get<ListCategoriesOutput>($"/Categories", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Items.Should().HaveCount(categoriesListSample.Count);
            output.Total.Should().Be(categoriesListSample.Count);
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);

            var count = 0;
            var outputdArray = output.Items.Select(x => $"{count++} {x.Name} {x.CreatedAt} {JsonConvert.SerializeObject(x)}");

            _output.WriteLine("Output...");
            _output.WriteLine(String.Join('\n', outputdArray));

            DateTime? lastItemDate = null;

            foreach (CategoryModelOutput outputItem in output.Items)
            {
                var expectedItem = categoriesListSample.FirstOrDefault(c => c.Id == outputItem.Id);

                expectedItem.Should().NotBeNull();
                outputItem.Name.Should().Be(expectedItem!.Name);
                outputItem.Description.Should().Be(expectedItem.Description);
                outputItem.IsActive.Should().Be(expectedItem.IsActive);
                outputItem.CreatedAt.TrimMilliseconds().Should().Be(expectedItem.CreatedAt.TrimMilliseconds());

                if (lastItemDate != null)
                {
                    if (orderDir == "asc")
                        Assert.True(outputItem.CreatedAt >= lastItemDate);
                    else
                        Assert.True(outputItem.CreatedAt <= lastItemDate);
                }

                lastItemDate = outputItem.CreatedAt;
            }
        }

        public void Dispose() => _fixture.CleanPersistence();
    }
}
