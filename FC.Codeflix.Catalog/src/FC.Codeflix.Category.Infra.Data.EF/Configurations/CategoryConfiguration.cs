using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Category.Infra.Data.EF.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<DomainEntity.Category>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<DomainEntity.Category> builder)
        {
            builder.HasKey(category => category.Id);
            builder.Property(category => category.Name).HasMaxLength(255);
            builder.Property(category => category.Description).HasMaxLength(10_000);
        }
    }
}
