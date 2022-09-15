using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using System.Runtime.CompilerServices;
using System.Threading;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory
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
        [Trait("Application", "UpdateCategory - Use Cases")]
        [MemberData(
            nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
            parameters: 10, 
            MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task UpdateCategory(Category categorySample, UpdateCategoryInput input)
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

            repositoryMock.Setup(x => x.Get(categorySample.Id, It.IsAny<CancellationToken>())).ReturnsAsync(categorySample);

            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be((bool)input.IsActive!);
            repositoryMock.Verify(x => x.Get(categorySample.Id, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(x => x.Update(categorySample, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        public async Task ThrowWhenCategoryNotFound()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

            var input = _fixture.GetValidInput();

            repositoryMock.Setup(x => x.Get(input.Id, It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException($"Category {input.Id} not found."));

            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            var task = async () =>  await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>();

            repositoryMock.Verify(x => x.Get(input.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory(DisplayName = nameof(UpdateCategoryWithoutProvidingIsActive))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        [MemberData(
            nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
            parameters: 10,
            MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task UpdateCategoryWithoutProvidingIsActive(Category categorySample, UpdateCategoryInput inputSample)
        {
            var input = new UpdateCategoryInput(
                    inputSample.Id,
                    inputSample.Name,
                    inputSample.Description
                );

            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

            repositoryMock.Setup(x => x.Get(categorySample.Id, It.IsAny<CancellationToken>())).ReturnsAsync(categorySample);

            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(input.Description);
            output.IsActive.Should().Be(categorySample.IsActive);
            repositoryMock.Verify(x => x.Get(categorySample.Id, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(x => x.Update(categorySample, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

        }

        [Theory(DisplayName = nameof(UpdateCategoryOnlyName))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        [MemberData(
            nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
            parameters: 10,
            MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task UpdateCategoryOnlyName(Category categorySample, UpdateCategoryInput inputSample)
        {
            var input = new UpdateCategoryInput(
                    inputSample.Id,
                    inputSample.Name
                );

            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

            repositoryMock.Setup(x => x.Get(categorySample.Id, It.IsAny<CancellationToken>())).ReturnsAsync(categorySample);

            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Description.Should().Be(categorySample.Description);
            output.IsActive.Should().Be(categorySample.IsActive);
            repositoryMock.Verify(x => x.Get(categorySample.Id, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(x => x.Update(categorySample, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory(DisplayName = nameof(ThrowWhenCantUpdateCategory))]
        [Trait("Application", "UpdateCategory - Use Cases")]
        [MemberData(
            nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs), 
            parameters: 12, 
            MemberType = typeof(UpdateCategoryTestDataGenerator))]
        public async Task ThrowWhenCantUpdateCategory(UpdateCategoryInput input, string expectedExceptionMessage)
        {
            var categoySample = _fixture.GetCategorySample();
            input.Id = categoySample.Id;
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

            repositoryMock.Setup(x => x.Get(categoySample.Id, It.IsAny<CancellationToken>())).ReturnsAsync(categoySample);

            var useCase = new UseCase.UpdateCategory(repositoryMock.Object, unitOfWorkMock.Object);

            var task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should()
                .ThrowAsync<EntityValidationException>()
                .WithMessage(expectedExceptionMessage);

            repositoryMock.Verify(x => x.Get(categoySample.Id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
