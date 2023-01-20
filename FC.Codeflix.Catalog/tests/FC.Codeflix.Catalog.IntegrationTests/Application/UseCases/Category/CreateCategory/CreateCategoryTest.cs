using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory
{
    [Collection(nameof(CreateCategoryTestFixture))]
    public class CreateCategoryTest
    {
        private readonly CreateCategoryTestFixture _fixture;

        public CreateCategoryTest(CreateCategoryTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(CreateCategory))]
        [Trait("Integration/Application", "CreateCategory - Use Cases")]
        public async Task CreateCategory()
        {
            //Arrange
            var dbContext = _fixture.CreateDbContextSample();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.CreateCategory(repository, unitOfWork);

            var input = _fixture.GetInput();

            //Act
            var output = await useCase.Handle(input, CancellationToken.None);

            var dbCategory = await (_fixture.CreateDbContextSample(true)).Categories.FindAsync(output.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(input.IsActive);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(input.IsActive);
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = nameof(CreateCategoryOnlyWithName))]
        [Trait("Integration/Application", "CreateCategory - Use Cases")]
        public async Task CreateCategoryOnlyWithName()
        {
            //Arrange
            var dbContext = _fixture.CreateDbContextSample();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.CreateCategory(repository, unitOfWork);

            var input = new CreateCategoryInput(_fixture.GetInput().Name);

            //Act
            var output = await useCase.Handle(input, CancellationToken.None);

            var dbCategory = await (_fixture.CreateDbContextSample(true)).Categories.FindAsync(output.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be("");
            dbCategory.IsActive.Should().Be(true);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be("");
            output.IsActive.Should().Be(true);
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Fact(DisplayName = nameof(CreateCategoryOnlyWithNameAndDescription))]
        [Trait("Integration/Application", "CreateCategory - Use Cases")]
        public async Task CreateCategoryOnlyWithNameAndDescription()
        {
            //Arrange
            var dbContext = _fixture.CreateDbContextSample();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.CreateCategory(repository, unitOfWork);

            var inputSample = _fixture.GetInput();

            var input = new CreateCategoryInput(inputSample.Name, inputSample.Description);

            //Act
            var output = await useCase.Handle(input, CancellationToken.None);

            var dbCategory = await (_fixture.CreateDbContextSample(true)).Categories.FindAsync(output.Id);

            dbCategory.Should().NotBeNull();
            dbCategory!.Name.Should().Be(input.Name);
            dbCategory.Description.Should().Be(input.Description);
            dbCategory.IsActive.Should().Be(true);
            dbCategory.CreatedAt.Should().Be(output.CreatedAt);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(true);
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Theory(DisplayName = nameof(ThrownWhenCantInstantiateCategory))]
        [Trait("Integration/Application", "CreateCategory - Use Cases")]
        [MemberData(nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
            parameters: 4,
            MemberType = typeof(CreateCategoryTestDataGenerator))]
        public async Task ThrownWhenCantInstantiateCategory(CreateCategoryInput input, string exceptionMessage)
        {
            //Arrange

            var dbContext = _fixture.CreateDbContextSample();
            var repository = new CategoryRepository(dbContext);
            var unitOfWork = new UnitOfWork(dbContext);
            var useCase = new UseCase.CreateCategory(repository, unitOfWork);

            //Act
            Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should()
                .ThrowAsync<EntityValidationException>()
                .WithMessage(exceptionMessage);

            var dbCategoriesList = _fixture.CreateDbContextSample(true).Categories.AsNoTracking().ToList();

            dbCategoriesList.Should().HaveCount(0);
        }
    }
}
