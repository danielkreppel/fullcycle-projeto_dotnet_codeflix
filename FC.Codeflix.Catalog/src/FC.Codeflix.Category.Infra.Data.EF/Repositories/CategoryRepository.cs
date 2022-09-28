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

        public async Task<SearchOutput<DomainEntity.Category>> Search(SearchInput input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = _categories.AsNoTracking();
            query = AddOrderToQuery(query, input.OrderBy, input.Order);

            if (!string.IsNullOrWhiteSpace(input.Search))
            {
                query = query.Where(x => x.Name.Contains(input.Search));
            }

            var total = await query.CountAsync();
            var items = await query.Skip(toSkip).Take(input.PerPage).ToListAsync();

            return new (input.Page, input.PerPage, total, items);
        }

        private IQueryable<DomainEntity.Category> AddOrderToQuery(IQueryable<DomainEntity.Category> query, string orderProperty, SearchOrder orderDir)
        {
            var ordered = (orderProperty.ToLower(), orderDir) switch
            {
                ("name", SearchOrder.ASC) => query.OrderBy(x => x.Name),
                ("name", SearchOrder.DESC) => query.OrderByDescending(x => x.Name),
                ("id", SearchOrder.ASC) => query.OrderBy(x => x.Id),
                ("id", SearchOrder.DESC) => query.OrderByDescending(x => x.Id),
                ("createdat", SearchOrder.ASC) => query.OrderBy(x => x.CreatedAt),
                ("createdat", SearchOrder.DESC) => query.OrderByDescending(x => x.CreatedAt),
                _ => query.OrderBy(x => x.Name),
            };

            return ordered;
        }

        public async Task Update(DomainEntity.Category aggregate, CancellationToken _)
        {
            await Task.FromResult(_categories.Update(aggregate));
        }
    }
}
