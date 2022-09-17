using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Application.Common
{
    public abstract class PaginatedListInput
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public string Search { get; set; }
        public string SortBy { get; set; }
        public SearchOrder SortDir { get; set; }

        public PaginatedListInput(int page, int perPage, string search, string sortBy, SearchOrder sortDir)
        {
            Page = page;
            PerPage = perPage;
            Search = search;
            SortBy = sortBy;
            SortDir = sortDir;
        }

    }
}
