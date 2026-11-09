namespace FC.Codeflix.Catalog.Infra.Data.EF.Models;

public record VideosCastMembers(
    Guid VideoId,
    Guid CastMemberId
);
