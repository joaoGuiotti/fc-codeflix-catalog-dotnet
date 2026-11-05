using FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;
using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.DeleteVideo;

[CollectionDefinition(nameof(DeleteVideoTestFixture))]
public record DeleteVideoTestFixtureCollection : ICollectionFixture<DeleteVideoTestFixture>;

public class DeleteVideoTestFixture : VideoTestFixtureBase
{
    public DeleteVideoInput GetValidInput(Guid? id = null)
        => new(id ?? Guid.NewGuid());
}
