using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.CreateGenre
{
    [Collection(nameof(CreateGenreTestFixture))]
    public class CreateGenreTest
    {
        private readonly CreateGenreTestFixture _fixture;
        public CreateGenreTest(CreateGenreTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(Create))]
        [Trait("Application", "CreateGenre - UseCases")]
        public async Task Create()
        {
            var genreRepositoryMock = _fixture.GetGenreRepositoryMock();
            var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
            var useCase = new UseCase.CreateGenre(genreRepositoryMock.Object, unitOfWorkMock.Object);
            
            var input = _fixture.GetExampleInput();

            var output = await useCase.Handle(input, CancellationToken.None);

            genreRepositoryMock.Verify(x => x.Insert(It.IsAny<DomainEntity.Genre>(), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Categories.Should().HaveCount(0);
            output.IsActive.Should().Be(input.IsActive);
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }
    }
}
