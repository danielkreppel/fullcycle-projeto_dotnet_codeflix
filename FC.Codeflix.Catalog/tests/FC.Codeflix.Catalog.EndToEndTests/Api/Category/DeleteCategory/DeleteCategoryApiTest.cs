using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.DeleteCategory
{
    [Collection(nameof(DeleteCategoryApiTestFixture))]
    public class DeleteCategoryApiTest
    {
        private readonly DeleteCategoryApiTestFixture _fixture;

        public DeleteCategoryApiTest(DeleteCategoryApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
        public async void DeleteCategory()
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(20);
            await _fixture.Persistence.InsertList(categoriesListSample);
            var categorySample = categoriesListSample[10];

            var (response, output) = await _fixture.ApiClient.Delete<object>($"/Categories/{categorySample.Id}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
            output.Should().BeNull();

            var persistenceCategory = await _fixture.Persistence.GetById(categorySample.Id);

            persistenceCategory.Should().BeNull();
        }

        [Fact(DisplayName = nameof(ErrorWhenNotFound))]
        [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
        public async void ErrorWhenNotFound()
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(20);
            await _fixture.Persistence.InsertList(categoriesListSample);
            var randomGuid = Guid.NewGuid();

            var (response, output) = await _fixture.ApiClient.Delete<ProblemDetails>($"/Categories/{randomGuid}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output!.Title.Should().Be("Not Found");
            output!.Type.Should().Be("NotFound");
            output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
            output!.Detail.Should().Be($"Category '{randomGuid}' not found");
        }
    }
}
