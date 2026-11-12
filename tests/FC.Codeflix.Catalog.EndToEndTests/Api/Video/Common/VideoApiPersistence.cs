using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;

public class VideoApiPersistence
{
    public readonly CodeflixCatalogDbContext _context;

    public VideoApiPersistence(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entity.Video?> GetById(Guid id)
        => await _context.Videos
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id.Equals(id));

    public async Task<List<VideosCastMembers>> GetVideosCastMembersAsync(Guid videoId)
        => await _context.VideosCastMembers
            .AsNoTracking()
            .Where(x => x.VideoId.Equals(videoId))
            .ToListAsync();

    public async Task<List<VideosCategories>> GetVideosCategoriesAsync(Guid videoId)
        => await _context.VideosCategories
            .AsNoTracking()
            .Where(x => x.VideoId.Equals(videoId))
            .ToListAsync();

    public async Task<List<VideosGenres>> GetVideosGenresAsync(Guid videoId)
        => await _context.VideosGenres
        .AsNoTracking()
        .Where(x => x.VideoId.Equals(videoId))
        .ToListAsync();

}
