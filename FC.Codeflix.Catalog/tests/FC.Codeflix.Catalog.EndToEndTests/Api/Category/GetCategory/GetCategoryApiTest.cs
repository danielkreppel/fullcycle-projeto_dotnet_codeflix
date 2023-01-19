﻿using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTime;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategory
{
    [Collection(nameof(GetCategoryApiTestFixture))]
    public class GetCategoryApiTest : IDisposable
    {
        private readonly GetCategoryApiTestFixture _fixture;

        public GetCategoryApiTest(GetCategoryApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(GetCategory))]
        [Trait("EndToEnd/API", "Category/Get Endpoints")]
        public async Task GetCategory()
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(20);
            await _fixture.Persistence.InsertList(categoriesListSample);
            var categorySample = categoriesListSample[10];

            var (response, output) = await _fixture.ApiClient.Get<CategoryModelOutput>($"/Categories/{categorySample.Id}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output!.Id.Should().Be(categorySample.Id);
            output.Name.Should().Be(categorySample.Name);
            output.Description.Should().Be(categorySample.Description);
            output.IsActive.Should().Be(categorySample.IsActive);
            output.CreatedAt.TrimMilliseconds().Should().Be(categorySample.CreatedAt.TrimMilliseconds());
        }

        [Fact(DisplayName = nameof(ErrorWhenNotFound))]
        [Trait("EndToEnd/API", "Category/Get Endpoints")]
        public async Task ErrorWhenNotFound()
        {
            var categoriesListSample = _fixture.GetCategoriesListSample(20);
            await _fixture.Persistence.InsertList(categoriesListSample);
            var randomGuid = Guid.NewGuid();

            var (response, output) = await _fixture.ApiClient.Get<ProblemDetails>($"/Categories/{randomGuid}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
            output.Should().NotBeNull();
            output!.Status.Should().Be(StatusCodes.Status404NotFound);
            output.Title.Should().Be("Not Found");
            output.Detail.Should().Be($"Category '{randomGuid}' not found");
            output.Type.Should().Be("NotFound");
        }

        public void Dispose() => _fixture.CleanPersistence();
    }
}
