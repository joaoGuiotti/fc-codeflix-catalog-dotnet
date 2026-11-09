namespace FC.Codeflix.Catalog.Infra.Data.EF.Models;

public record VideosGenres(
    Guid VideoId,
    Guid GenreId
);
