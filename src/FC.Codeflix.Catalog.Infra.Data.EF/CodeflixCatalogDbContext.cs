using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF;

public class CodeflixCatalogDbContext : DbContext
{
    public DbSet<Domain.Entity.Category> Categories { get; set; } = null!;

    public DbSet<Domain.Entity.Genre> Genres { get; set; } = null!;

    public CodeflixCatalogDbContext(
        DbContextOptions<CodeflixCatalogDbContext> options
    )
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CategoryConfiguration());
    }
}
