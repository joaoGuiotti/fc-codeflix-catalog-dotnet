namespace FC.Codeflix.Catalog.Infra.Data.EF.Models;

public record VideosCategories(
    Guid VideoId,
    Guid CategoryId
);
