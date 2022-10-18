using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory
{
    [Collection(nameof(UpdateCategoryApiTestFixture))]
    public class UpdateCategoryApiTest
    {
        private readonly UpdateCategoryApiTestFixture _fixture;

        public UpdateCategoryApiTest(UpdateCategoryApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(UpdateCategory))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task UpdateCategory()
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(20);
            await _fixture.Persistence.InsertList(categoriesListSample);
            var categorySample = categoriesListSample[10];
            var input = _fixture.GetInputSample(categorySample.Id);

            var (response, output) = await _fixture.ApiClient.Put<CategoryModelOutput>($"/Categories/{categorySample.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);

            output.Should().NotBeNull();
            output!.Id.Should().Be(categorySample.Id);
            output!.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(input.IsActive!.Value);

            var dbCategory = await _fixture.Persistence.GetById(categorySample.Id);
            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(input.IsActive!.Value);
        }

        [Fact(DisplayName = nameof(UpdateCategoryOnlyName))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task UpdateCategoryOnlyName()
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(20);
            await _fixture.Persistence.InsertList(categoriesListSample);
            var categorySample = categoriesListSample[10];
            var input = new UpdateCategoryInput(categorySample.Id, _fixture.GetValidCategoryName());

            var (response, output) = await _fixture.ApiClient.Put<CategoryModelOutput>($"/Categories/{categorySample.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);

            output.Should().NotBeNull();
            output!.Id.Should().Be(categorySample.Id);
            output!.Name.Should().Be(input.Name);
            output.Description.Should().Be(categorySample.Description);
            output.IsActive.Should().Be(categorySample.IsActive);

            var dbCategory = await _fixture.Persistence.GetById(categorySample.Id);
            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(categorySample.Description);
            dbCategory.IsActive.Should().Be(categorySample.IsActive);
        }

        [Fact(DisplayName = nameof(UpdateCategoryNameAndDescription))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task UpdateCategoryNameAndDescription()
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(20);
            await _fixture.Persistence.InsertList(categoriesListSample);
            var categorySample = categoriesListSample[10];
            var input = new UpdateCategoryInput(categorySample.Id, _fixture.GetValidCategoryName(), _fixture.GetValidCategoryDescription());

            var (response, output) = await _fixture.ApiClient.Put<CategoryModelOutput>($"/Categories/{categorySample.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);

            output.Should().NotBeNull();
            output!.Id.Should().Be(categorySample.Id);
            output!.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(categorySample.IsActive);

            var dbCategory = await _fixture.Persistence.GetById(categorySample.Id);
            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(categorySample.IsActive);
        }

        [Fact(DisplayName = nameof(ErrorWhenNotFound))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        public async Task ErrorWhenNotFound()
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(20);
            await _fixture.Persistence.InsertList(categoriesListSample);
            var randomGuid = Guid.NewGuid();
            var input = _fixture.GetInputSample(randomGuid);

            var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>($"/Categories/{randomGuid}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);

            output.Should().NotBeNull();
            output!.Title.Should().Be("Not Found");
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Category '{randomGuid}' not found");
            output.Status.Should().Be((int)StatusCodes.Status404NotFound);
        }
    }
}
