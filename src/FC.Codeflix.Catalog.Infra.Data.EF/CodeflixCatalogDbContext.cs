using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
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
    public DbSet<Domain.Entity.CastMember> CastMembers
        => Set<CastMember>();

    public CodeflixCatalogDbContext(
        DbContextOptions<CodeflixCatalogDbContext> options
    )
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(CodeflixCatalogDbContext).Assembly);
    }
}
