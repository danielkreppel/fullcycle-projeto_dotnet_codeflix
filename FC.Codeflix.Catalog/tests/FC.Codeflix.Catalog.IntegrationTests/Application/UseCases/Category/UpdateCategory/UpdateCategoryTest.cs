using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory
{
    [Collection(nameof(UpdateCategoryTestFixture))]
    public class UpdateCategoryTest
    {
        private readonly UpdateCategoryTestFixture _fixture;

        public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = nameof(UpdateCategory))]
        [Trait("Integration/Application", "UpdateCategory - Use Cases")]
        [MemberData(
            nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
            parameters: 10,
            MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task UpdateCategory(DomainEntity.Category categorySample, UpdateCategoryInput input)
        {
            var dbContext = _fixture.CreateDbContextSample();
            await dbContext.AddRangeAsync(_fixture.GetValidCategorySampleList());
            var trackingInfo = await dbContext.AddAsync(categorySample);
            await dbContext.SaveChangesAsync();
            
            //Detach the entity from EF tracking
            trackingInfo.State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);

            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

            CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

            var dbCategory = await (_fixture.CreateDbContextSample(true)).Categories.FindAsync(output.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be((bool)input.IsActive!);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be((bool)input.IsActive!);          

        }

        [Theory(DisplayName = nameof(UpdateCategoryWithoutIsActive))]
        [Trait("Integration/Application", "UpdateCategory - Use Cases")]
        [MemberData(
            nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
            parameters: 10,
            MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task UpdateCategoryWithoutIsActive(DomainEntity.Category categorySample, UpdateCategoryInput inputSample)
        {
            var input = new UpdateCategoryInput(inputSample.Id, inputSample.Name, inputSample.Description);
            var dbContext = _fixture.CreateDbContextSample();
            await dbContext.AddRangeAsync(_fixture.GetValidCategorySampleList());
            var trackingInfo = await dbContext.AddAsync(categorySample);
            await dbContext.SaveChangesAsync();

            //Detach the entity from EF tracking
            trackingInfo.State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);

            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

            CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

            var dbCategory = await (_fixture.CreateDbContextSample(true)).Categories.FindAsync(output.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(categorySample.IsActive);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(categorySample.IsActive);

        }

        [Theory(DisplayName = nameof(UpdateCategoryWithOnlyName))]
        [Trait("Integration/Application", "UpdateCategory - Use Cases")]
        [MemberData(
            nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
            parameters: 10,
            MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task UpdateCategoryWithOnlyName(DomainEntity.Category categorySample, UpdateCategoryInput inputSample)
        {
            var input = new UpdateCategoryInput(inputSample.Id, inputSample.Name);
            var dbContext = _fixture.CreateDbContextSample();
            await dbContext.AddRangeAsync(_fixture.GetValidCategorySampleList());
            var trackingInfo = await dbContext.AddAsync(categorySample);
            await dbContext.SaveChangesAsync();

            //Detach the entity from EF tracking
            trackingInfo.State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);

            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

            CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

            var dbCategory = await (_fixture.CreateDbContextSample(true)).Categories.FindAsync(output.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(categorySample.Description);
            dbCategory.IsActive.Should().Be(categorySample.IsActive);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(categorySample.Description);
            output.IsActive.Should().Be(categorySample.IsActive);

        }

        [Fact(DisplayName = nameof(UpdateThrowsWhenNotFindCategory))]
        [Trait("Integration/Application", "UpdateCategory - Use Cases")]
        public async Task UpdateThrowsWhenNotFindCategory()
        {
            var input = _fixture.GetValidInput();
            var dbContext = _fixture.CreateDbContextSample();
            await dbContext.AddRangeAsync(_fixture.GetValidCategorySampleList());
            await dbContext.SaveChangesAsync();

            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);

            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

            var task = async () =>  await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>().WithMessage($"Category '{input.Id}' not found");
        }

        [Theory(DisplayName = nameof(UpdateThrowsWhenCantInstantiateCategory))]
        [Trait("Integration/Application", "UpdateCategory - Use Cases")]
        [MemberData(
            nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
            parameters: 6,
            MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task UpdateThrowsWhenCantInstantiateCategory(UpdateCategoryInput input, string expectedExceptionMessage)
        {
            var dbContext = _fixture.CreateDbContextSample();
            var sampleCategories = _fixture.GetValidCategorySampleList();
            await dbContext.AddRangeAsync(sampleCategories);
            await dbContext.SaveChangesAsync();

            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);

            var useCase = new UseCase.UpdateCategory(repository, unitOfWork);

            input.Id = sampleCategories[0].Id;

            var task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<EntityValidationException>().WithMessage(expectedExceptionMessage);
        }
    }
}
