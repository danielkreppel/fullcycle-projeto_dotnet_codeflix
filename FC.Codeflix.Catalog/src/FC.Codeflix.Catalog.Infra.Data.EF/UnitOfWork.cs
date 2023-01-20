using FC.Codeflix.Catalog.Application.Interfaces;

namespace FC.Codeflix.Catalog.Infra.Data.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CodeflixCatalogDbContext _catalogDbContext;

        public UnitOfWork(CodeflixCatalogDbContext catalogDbContext)
        {
            _catalogDbContext = catalogDbContext;
        }

        public async Task Commit(CancellationToken cancellationToken)
        {
            await _catalogDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task Rollback(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
