using FC.Codeflix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title)
            .HasMaxLength(255).IsRequired();
        builder.Property(x => x.Description)
            .HasMaxLength(4_000);

        builder.OwnsOne(video => video.Thumb, thumb
            => thumb.Property(x => x.Path).HasColumnName("ThumbPath")
        );
        builder.OwnsOne(video => video.ThumbHalf, thumbHalf
            => thumbHalf.Property(x => x.Path).HasColumnName("ThumbHalfPath")
        );
        builder.OwnsOne(video => video.Banner, banner
            => banner.Property(x => x.Path).HasColumnName("BannerPath")
        );

        builder.HasOne(video => video.Media);
        builder.Navigation(x => x.Media).AutoInclude();

        builder.HasOne(video => video.Trailer);
        builder.Navigation(x => x.Trailer).AutoInclude();

        builder.Property<DateTime>("LastUpdated")
           .ValueGeneratedOnAdd();
    }
}
