namespace FC.Codeflix.Catalog.Application.Common
{
    public abstract class PaginatedListOutput<TOuputItem>
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
        public IReadOnlyList<TOuputItem> Items { get; set; }

        protected PaginatedListOutput(int page, int perPage, int total, IReadOnlyList<TOuputItem> items)
        {
            Page = page;
            PerPage = perPage;
            Total = total;
            Items = items;
        }
    }
}
