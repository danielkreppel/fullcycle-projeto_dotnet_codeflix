using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;


namespace FC.Codeflix.Category.Infra.Data.EF.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CodeflixCatalogDbContext _context;
        private DbSet<DomainEntity.Category> _categories => _context.Set<DomainEntity.Category>();

        public CategoryRepository(CodeflixCatalogDbContext context)
        {
            _context = context;
        }

        public async Task Insert(DomainEntity.Category aggregate, CancellationToken cancellationToken)
        {
            await _categories.AddAsync(aggregate, cancellationToken);
        }
        public async Task Delete(DomainEntity.Category aggregate, CancellationToken _)
        {
            await Task.FromResult(_categories.Remove(aggregate));
        }

        public async Task<DomainEntity.Category> Get(Guid id, CancellationToken cancellationToken)
        {
            //Set as no tracking so EF doesn't save changes for entities returned only for searching pusporses
            var category = await _categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            NotFoundException.ThrowIfNull(category, $"Category {id} not found");

            return category!;
        }

        public Task<SearchOutput<DomainEntity.Category>> Search(SearchInput input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Update(DomainEntity.Category aggregate, CancellationToken _)
        {
            await Task.FromResult(_categories.Update(aggregate));
        }
    }
}
