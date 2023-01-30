namespace FC.Codeflix.Catalog.Application.Common
{
    public abstract class PaginatedListOutput<TOuputItem>
    {
        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
        public IReadOnlyList<TOuputItem> Items { get; set; }

        protected PaginatedListOutput(int currentPage, int perPage, int total, IReadOnlyList<TOuputItem> items)
        {
            CurrentPage = currentPage;
            PerPage = perPage;
            Total = total;
            Items = items;
        }
    }
}
