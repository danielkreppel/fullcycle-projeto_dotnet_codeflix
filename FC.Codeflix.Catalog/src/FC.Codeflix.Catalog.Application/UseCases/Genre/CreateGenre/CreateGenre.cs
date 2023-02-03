using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre
{
    public class CreateGenre : ICreateGenre
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IUnitOfWork _uniteOfWork;

        public CreateGenre(IGenreRepository genreRepository, IUnitOfWork uniteOfWork)
        {
            _genreRepository = genreRepository;
            _uniteOfWork = uniteOfWork;
        }

        public async Task<GenreModelOutput> Handle(CreateGenreInput request, CancellationToken cancellationToken)
        {
            var genre = new DomainEntity.Genre(request.Name, request.IsActive);
            await _genreRepository.Insert(genre, CancellationToken.None);
            await _uniteOfWork.Commit(CancellationToken.None);

            return new GenreModelOutput(genre.Id, genre.Name, genre.CreatedAt, genre.Categories, genre.IsActive);
        }
    }
}
