using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Category.Infra.Data.EF;
using FC.Codeflix.Category.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.DeleteCategory
{
    [Collection(nameof(DeleteCategoryTestFixture))]
    public class DeleteCategoryTest
    {
        private readonly DeleteCategoryTestFixture _fixture;

        public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(DeleteCategory))]
        [Trait("Integration/Application", "DeleteCategory - UseCases")]
        public async Task DeleteCategory()
        {
            var dbContext = _fixture.CreateDbContextSample();

            var sampleList = _fixture.GetValidCategorySampleList(10);
            var validCategorySample = _fixture.GetValidCategorySample();
            await dbContext.AddRangeAsync(sampleList);
            var tracking = await dbContext.AddAsync(validCategorySample);
            await dbContext.SaveChangesAsync();
            tracking.State = EntityState.Detached;

            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);

            var input = new UseCase.DeleteCategoryInput(validCategorySample.Id);
            var useCase = new UseCase.DeleteCategory(repository, unitOfWork);

            await useCase.Handle(input, CancellationToken.None);

            var assertDbContext = _fixture.CreateDbContextSample(true);

            var dbCategoryDeleted = await assertDbContext.Categories.FindAsync(validCategorySample.Id);

            dbCategoryDeleted.Should().BeNull();

            var dbCategories = await assertDbContext.Categories.ToListAsync();
            dbCategories.Should().HaveCount(sampleList.Count());
        }

        [Fact(DisplayName = nameof(DeleteCategoryThrowsWhenNotFound))]
        [Trait("Integration/Application", "DeleteCategory - UseCases")]
        public async Task DeleteCategoryThrowsWhenNotFound()
        {
            var dbContext = _fixture.CreateDbContextSample();
            var sampleList = _fixture.GetValidCategorySampleList(10);
            
            await dbContext.AddRangeAsync(sampleList);
            await dbContext.SaveChangesAsync();            

            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);

            var input = new UseCase.DeleteCategoryInput(Guid.NewGuid());
            var useCase = new UseCase.DeleteCategory(repository, unitOfWork);

            var task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>().WithMessage($"Category '{input.Id}' not found");
        }
    }
}
