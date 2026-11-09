using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF;

public class CodeflixCatalogDbContext : DbContext
{
    public DbSet<Category> Categories =>
        Set<Category>();
    public DbSet<Genre> Genres =>
        Set<Genre>();
    public DbSet<CastMember> CastMembers
        => Set<CastMember>();
    public DbSet<Video> Videos
        => Set<Video>();
    public DbSet<GenresCategories> GenresCategories =>
        Set<GenresCategories>();
    public DbSet<VideosCategories> VideosCategories
        => Set<VideosCategories>();
    public DbSet<VideosGenres> VideosGenres
        => Set<VideosGenres>();
    public DbSet<VideosCastMembers> VideosCastMembers
        => Set<VideosCastMembers>();
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
