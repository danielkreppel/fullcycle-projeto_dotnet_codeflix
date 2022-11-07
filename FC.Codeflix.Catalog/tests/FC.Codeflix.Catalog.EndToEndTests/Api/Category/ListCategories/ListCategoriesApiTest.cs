using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories
{
    [Collection(nameof(ListCategoriesApiTestFixture))]
    public class ListCategoriesApiTest : IDisposable
    {
        private readonly ListCategoriesApiTestFixture _fixture;

        public ListCategoriesApiTest(ListCategoriesApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        public async void ListCategoriesAndTotalByDefault()
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
                outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
            }
        }

        [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
        [Trait("EndToEnd/API", "Category/List - Endpoints")]
        public async void ItemsEmptyWhenPersistenceEmpty()
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
        public async void ListCategoriesAndTotal()
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
                outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
            }
        }

        public void Dispose() => _fixture.CleanPersistence();
    }
}
