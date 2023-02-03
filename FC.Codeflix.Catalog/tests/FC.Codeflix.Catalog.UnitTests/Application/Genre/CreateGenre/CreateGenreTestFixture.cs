using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.CreateGenre
{
    [CollectionDefinition(nameof(CreateGenreTestFixture))]
    public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture>
    {
    }
    public class CreateGenreTestFixture : GenreUseCasesBaseFixture
    {
        public CreateGenreInput GetExampleInput()
        {
            return new CreateGenreInput(
                GetValidGenreName(),
                GetRandomBoolean()
                );
        }

        public Mock<IGenreRepository> GetGenreRepositoryMock()
        {
            return new();
        }

        public Mock<IUnitOfWork> GetUnitOfWorkMock()
        {
            return new();
        }
    }
}
