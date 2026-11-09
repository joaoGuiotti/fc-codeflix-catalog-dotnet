using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

public class VideosCategoriesConfiguration : IEntityTypeConfiguration<VideosCategories>
{
    public void Configure(EntityTypeBuilder<VideosCategories> builder)
    {
        builder.HasKey(relation => new
        {
            relation.VideoId,
            relation.CategoryId
        });
    }
}
