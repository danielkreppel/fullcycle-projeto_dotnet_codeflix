﻿using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using Moq;
using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.DeleteCategory
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
        [Trait("Application", "DeleteCategory - UseCases")]
        public async Task DeleteCategory()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var validCategorySample = _fixture.GetValidCategorySample();

            repositoryMock.Setup(x => x.Get(validCategorySample.Id, It.IsAny<CancellationToken>())).ReturnsAsync(validCategorySample);

            var input = new UseCase.DeleteCategoryInput(validCategorySample.Id);
            var useCase = new UseCase.DeleteCategory(repositoryMock.Object, unitOfWorkMock.Object);

            await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Get(validCategorySample.Id, It.IsAny<CancellationToken>()), Times.Once);
            repositoryMock.Verify(x => x.Delete(validCategorySample, It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
        [Trait("Application", "DeleteCategory - UseCases")]
        public async Task ThrowWhenCategoryNotFound()
        {
            var repositoryMock = _fixture.GetRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var sampleGuid = Guid.NewGuid();

            repositoryMock.Setup(x => x.Get(sampleGuid, It.IsAny<CancellationToken>())).ThrowsAsync(new NotFoundException($"Category '{sampleGuid}' not found."));

            var input = new UseCase.DeleteCategoryInput(sampleGuid);
            var useCase = new UseCase.DeleteCategory(repositoryMock.Object, unitOfWorkMock.Object);

            var task = async () => await useCase.Handle(input, CancellationToken.None);

            await task.Should().ThrowAsync<NotFoundException>();

            repositoryMock.Verify(x => x.Get(sampleGuid, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
