using FC.Codeflix.Catalog.Api.ApiModels.Category;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
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
            var input = _fixture.GetInputSample();

            var (response, output) = await _fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>($"/Categories/{categorySample.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);

            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output.Data.Id.Should().Be(categorySample.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.Description.Should().Be(input.Description);
            output.Data.IsActive.Should().Be(input.IsActive!.Value);

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
            var input = new UpdateCategoryApiInput(_fixture.GetValidCategoryName());

            var (response, output) = await _fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>($"/Categories/{categorySample.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);

            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output.Data.Id.Should().Be(categorySample.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.Description.Should().Be(categorySample.Description);
            output.Data.IsActive.Should().Be(categorySample.IsActive);

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
            var input = new UpdateCategoryApiInput(_fixture.GetValidCategoryName(), _fixture.GetValidCategoryDescription());

            var (response, output) = await _fixture.ApiClient.Put<ApiResponse<CategoryModelOutput>>($"/Categories/{categorySample.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);

            output.Should().NotBeNull();
            output!.Data.Should().NotBeNull();
            output.Data.Id.Should().Be(categorySample.Id);
            output.Data.Name.Should().Be(input.Name);
            output.Data.Description.Should().Be(input.Description);
            output.Data.IsActive.Should().Be(categorySample.IsActive);

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
            var input = _fixture.GetInputSample();

            var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>($"/Categories/{randomGuid}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);

            output.Should().NotBeNull();
            output!.Title.Should().Be("Not Found");
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Category '{randomGuid}' not found");
            output.Status.Should().Be((int)StatusCodes.Status404NotFound);
        }

        [Theory(DisplayName = nameof(ErrorWhenCantInstantiateAggregate))]
        [Trait("EndToEnd/API", "Category/Update - Endpoints")]
        [MemberData(
            nameof(UpdateCategoryApiTestDataGenerator.GetInvalidInputs), 
            MemberType = typeof(UpdateCategoryApiTestDataGenerator))
        ]
        public async Task ErrorWhenCantInstantiateAggregate(UpdateCategoryApiInput input, string expectedDetail)
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(20);
            await _fixture.Persistence.InsertList(categoriesListSample);
            var exampleCategory = categoriesListSample[10];

            var (response, output) = await _fixture.ApiClient.Put<ProblemDetails>($"/Categories/{exampleCategory.Id}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);

            output.Should().NotBeNull();
            output!.Title.Should().Be("One or more validation errors occurred");
            output.Type.Should().Be("UnprocessableEntity");
            output.Detail.Should().Be(expectedDetail);
            output.Status.Should().Be((int)StatusCodes.Status422UnprocessableEntity);
        }
    }
}
