using FC.Codeflix.Catalog.Application.Exceptions;
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
        private readonly ICategoryRepository _categoryRepository;

        public CreateGenre(IGenreRepository genreRepository, IUnitOfWork uniteOfWork, ICategoryRepository categoryRepository)
        {
            _genreRepository = genreRepository;
            _uniteOfWork = uniteOfWork;
            _categoryRepository = categoryRepository;
        }

        public async Task<GenreModelOutput> Handle(CreateGenreInput request, CancellationToken cancellationToken)
        {
            var genre = new DomainEntity.Genre(request.Name, request.IsActive);

            if (request.CategoriesIds != null && request.CategoriesIds.Count > 0)
            {
                var persistedCategoryIds = await _categoryRepository.GetIdsListByIds(request.CategoriesIds, cancellationToken);
                if (persistedCategoryIds.Count < request.CategoriesIds.Count)
                {
                    var notFoundIds = request.CategoriesIds.FindAll(id => !persistedCategoryIds.Contains(id));
                    throw new RelatedAggregateException($"Related category id (or ids) not found: '{string.Join(", ", notFoundIds)}'");
                }
                    
                request.CategoriesIds.ForEach(genre.AddCategory);
            }
                

            await _genreRepository.Insert(genre, CancellationToken.None);
            await _uniteOfWork.Commit(CancellationToken.None);

            return GenreModelOutput.FromGenre(genre);
        }
    }
}
