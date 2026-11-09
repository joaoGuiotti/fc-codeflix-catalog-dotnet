using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Domain.Entity;

public class Media(string filePath, string? encodedPath = null) : AggregateRoot()
{
    public string FilePath { get; private set; } = filePath;
    public string? EncodedPath { get; private set; } = encodedPath;
    public EMediaStatus Status { get; private set; } = EMediaStatus.Pending;

    public void UpdateAsSentToEncode()
    {
        Status = EMediaStatus.Processing;
    }

    public void UpdateAsEnconded(string encodedExamplePath)
    {
        Status = EMediaStatus.Completed;
        EncodedPath = encodedExamplePath;
    }

}
