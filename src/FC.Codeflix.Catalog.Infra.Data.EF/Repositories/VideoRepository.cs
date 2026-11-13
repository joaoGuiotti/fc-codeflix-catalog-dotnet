using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Domain.ValueObject;
using FC.Codeflix.Catalog.Infra.Data.EF.Extensions;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public class VideoRepository(CodeflixCatalogDbContext context) : IVideoRepository
{
    private CodeflixCatalogDbContext _context => context;
    private DbSet<Video> _videos => context.Set<Video>();
    private DbSet<Media> _medias => context.Set<Media>();
    private DbSet<VideosCategories> _videosCategories => context.Set<VideosCategories>();
    private DbSet<VideosGenres> _videosGenres => context.Set<VideosGenres>();
    private DbSet<VideosCastMembers> _videosCastMembers => context.Set<VideosCastMembers>();

    public async Task Insert(Video video, CancellationToken cancellation)
    {
        await _videos.AddAsync(video, cancellation);
        await AddRelationsAsync(video, cancellation);
    }

    public async Task<Video> Get(Guid id, CancellationToken cancellation)
    {
        var video = await _videos.FirstOrDefaultAsync(x => x.Id == id, cancellation);
        NotFoundException.ThrowIfNull(video, id);

        await LoadRelationsAsync([video], cancellation);
        return video;
    }

    public async Task Update(Video video, CancellationToken cancellationToken)
    {
        _videos.Update(video);

        RemoveRelations(video.Id);

        await AddRelationsAsync(video, cancellationToken);

        DeleteOrphanMedias(video);
    }

    public Task Delete(Video video, CancellationToken cancellationToken)
    {
        RemoveRelations(video.Id);

        if (video.Trailer is not null)
            _medias.Remove(video.Trailer);

        if (video.Media is not null)
            _medias.Remove(video.Media);

        _videos.Remove(video);
        return Task.CompletedTask;
    }

    public async Task<SearchOutput<Video>> Search(
        SearchInput input,
        CancellationToken cancellationToken
    )
    {
        var query = _videos
            .AsNoTracking()
            .ApplyFilter(
                search: input.Search,
                x => x.Title
            )
            .ApplyOrder(
                sort: input.Sort,
                sortDir: input.SortDir,
                defaultOrder: x => x.CreatedAt,
                x => x.Id,
                x => x.Title,
                x => x.CreatedAt
            );

        var output = await query.ToSearchOutputAsync(input, cancellationToken);

        if (output.Items.Count > 0)
            await LoadRelationsAsync(output.Items, cancellationToken);

        return output;
    }

    private async Task AddRelationsAsync(Video video, CancellationToken cancellationToken)
    {
        if (video.Categories.Count > 0)
        {
            var categoriesRelations = video.Categories
                .Select(categoryId => new VideosCategories(video.Id, categoryId));
            await _videosCategories.AddRangeAsync(categoriesRelations, cancellationToken);
        }

        if (video.Genres.Count > 0)
        {
            var genresRelations = video.Genres
                .Select(genreId => new VideosGenres(video.Id, genreId));
            await _videosGenres.AddRangeAsync(genresRelations, cancellationToken);
        }

        if (video.CastMembers.Count > 0)
        {
            var castMembersRelations = video.CastMembers
                .Select(castMemberId => new VideosCastMembers(video.Id, castMemberId));
            await _videosCastMembers.AddRangeAsync(castMembersRelations, cancellationToken);
        }
    }

    private void RemoveRelations(Guid videoId)
    {
        _videosCategories.RemoveRange(_videosCategories.AsNoTracking().Where(x => x.VideoId == videoId));
        _videosGenres.RemoveRange(_videosGenres.AsNoTracking().Where(x => x.VideoId == videoId));
        _videosCastMembers.RemoveRange(_videosCastMembers.AsNoTracking().Where(x => x.VideoId == videoId));
    }

    private async Task LoadRelationsAsync(
        IReadOnlyList<Video> videos,
        CancellationToken cancellationToken
    )
    {
        var videosById = videos.ToDictionary(video => video.Id);
        var videosIds = videosById.Keys.ToList();

        var categoriesRelations = await _videosCategories
            .AsNoTracking()
            .Where(x => videosIds.Contains(x.VideoId))
            .ToListAsync(cancellationToken);
        foreach (var relation in categoriesRelations)
            videosById[relation.VideoId].AddCategory(relation.CategoryId);

        var genresRelations = await _videosGenres
            .AsNoTracking()
            .Where(x => videosIds.Contains(x.VideoId))
            .ToListAsync(cancellationToken);
        foreach (var relation in genresRelations)
            videosById[relation.VideoId].AddGenre(relation.GenreId);

        var castMembersRelations = await _videosCastMembers
            .AsNoTracking()
            .Where(x => videosIds.Contains(x.VideoId))
            .ToListAsync(cancellationToken);
        foreach (var relation in castMembersRelations)
            videosById[relation.VideoId].AddCastMember(relation.CastMemberId);
    }

    private void DeleteOrphanMedias(Video video)
    {
        if (_context.Entry(video).Reference(v => v.Trailer).IsModified)
        {
            var oldTrailerId = _context.Entry(video)
                .OriginalValues.GetValue<Guid?>($"{nameof(Video.Trailer)}Id");
            if (oldTrailerId != null && oldTrailerId != video.Trailer?.Id)
            {
                var oldTrailer = _medias.Find(oldTrailerId);
                _medias.Remove(oldTrailer!);
            }
        }

        if (_context.Entry(video).Reference(v => v.Media).IsModified)
        {
            var oldMediaId = _context.Entry(video)
                .OriginalValues.GetValue<Guid?>($"{nameof(Video.Media)}Id");
            if (oldMediaId != null && oldMediaId != video.Media?.Id)
            {
                var oldMedia = _medias.Find(oldMediaId);
                _medias.Remove(oldMedia!);
            }
        }
    }
}
