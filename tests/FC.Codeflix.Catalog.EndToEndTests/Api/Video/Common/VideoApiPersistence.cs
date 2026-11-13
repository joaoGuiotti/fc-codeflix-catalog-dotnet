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

    public async Task<int> GetMediaCountAsync()
        => await _context.Set<Domain.Entity.Media>().CountAsync();

    public async Task BulkInsert(List<Domain.Entity.Video> videos)
    {
        await _context.Videos.AddRangeAsync(videos);

        foreach (var video in videos)
        {
            var videosCategories = video.Categories?
                .Select(categoryId => new VideosCategories(video.Id, categoryId));
            if (videosCategories != null && videosCategories.Any())
                await _context.VideosCategories.AddRangeAsync(videosCategories);

            var videosGenres = video.Genres?
                .Select(genreId => new VideosGenres(video.Id, genreId));
            if (videosGenres != null && videosGenres.Any())
                await _context.VideosGenres.AddRangeAsync(videosGenres);

            var videosCastMembers = video.CastMembers
                ?.Select(CastMemberId => new VideosCastMembers(video.Id, CastMemberId));
            if (videosCastMembers != null && videosCastMembers.Any())
                await _context.VideosCastMembers.AddRangeAsync(videosCastMembers);
        }

        await _context.SaveChangesAsync();
    }

}
