namespace FC.Codeflix.Catalog.Infra.Messaging.DTOs;

public class VideoEncodedMessageDTO
{
    public VideoEncodedMetadaDTO? Video { get; set; }
    public VideoEncodedMetadaDTO? Message { get; set; }

    public string? Error { get; set; }
}

public class VideoEncodedMetadaDTO
{
    public string? EncodedVideoFolder { get; set; }
    public string? FilePath { get; set; }
    public Guid ResourceId { get; set; }
    public string FullEncodedVideoFilePath => $"{EncodedVideoFolder}/{FilePath}";
}
