using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.GetVideo;

[CollectionDefinition(nameof(GetVideoTestFixture))]
public class GetVideoTestFixtureCollection : ICollectionFixture<GetVideoTestFixture>;

public class GetVideoTestFixture : VideoTestFixtureBase;
