using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories
{
    public class ListCategories : IListCategories
    {
        private readonly ICategoryRepository _categoryRepository;

        public ListCategories(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ListCategoriesOutput> Handle(ListCategoriesInput request, CancellationToken cancellationToken)
        {
            var searchOuput = await _categoryRepository.Search(
                    new(
                        request.Page, 
                        request.PerPage, 
                        request.Search, 
                        request.SortBy, 
                        request.SortDir
                        ),
                    cancellationToken
                );

            var output = new ListCategoriesOutput(
                    searchOuput.CurrentPage,
                    searchOuput.PerPage,
                    searchOuput.Total,
                    searchOuput.Items.Select(CategoryModelOutput.FromCategory).ToList()
                );

            return output;
        }
    }
}
