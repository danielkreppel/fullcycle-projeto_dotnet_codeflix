﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UnitOfWorkInfra = FC.Codeflix.Catalog.Infra.Data.EF;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.UnitOfWork
{
    [Collection(nameof(UnitOfWorkTestFixture))]
    public class UnitOfWorkTest
    {
        private readonly UnitOfWorkTestFixture _fixture;

        public UnitOfWorkTest(UnitOfWorkTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(Commit))]
        [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
        public async Task Commit()
        {
            var dbContext = _fixture.CreateDbContextSample();
            var categoriesListSample = _fixture.GetValidCategorySampleList();

            await dbContext.AddRangeAsync(categoriesListSample);

            var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);

            await unitOfWork.Commit(CancellationToken.None);

            var assertDbContext = _fixture.CreateDbContextSample(true);

            var savedCategories = assertDbContext.Categories.AsNoTracking().ToList();

            savedCategories.Should().HaveCount(categoriesListSample.Count());
        }

        [Fact(DisplayName = nameof(Rollback))]
        [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
        public async Task Rollback()
        {
            var dbContext = _fixture.CreateDbContextSample();

            var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext);

            var task = async () => await unitOfWork.Rollback(CancellationToken.None);

            await task.Should().NotThrowAsync();
        }
    }
}
