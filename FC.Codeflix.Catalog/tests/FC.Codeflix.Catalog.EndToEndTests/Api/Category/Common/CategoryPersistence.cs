﻿using FC.Codeflix.Category.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common
{
    public class CategoryPersistence
    {
        private readonly CodeflixCatalogDbContext _context;

        public CategoryPersistence(CodeflixCatalogDbContext context)
        {
            _context = context;
        }

        public async Task<DomainEntity.Category?> GetById(Guid id)
        {
            return await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task InsertList(List<DomainEntity.Category> categories)
        {
            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
        }
    }
}