using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF;

public class CodeflixCatalogDbContext : DbContext
{
    public DbSet<Domain.Entity.Category> Categories =>
        Set<Domain.Entity.Category>();

    public DbSet<Domain.Entity.Genre> Genres =>
        Set<Domain.Entity.Genre>();
    public DbSet<GenresCategories> GenresCategories =>
        Set<GenresCategories>();

    public CodeflixCatalogDbContext(
        DbContextOptions<CodeflixCatalogDbContext> options
    )
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CategoryConfiguration());
        builder.ApplyConfiguration(new GenreConfiguration());

        // Relations
        builder.ApplyConfiguration(new GenresCategoriesConfiguration());

    }
}
