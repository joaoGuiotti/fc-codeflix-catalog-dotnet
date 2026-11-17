using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
using FC.Codeflix.Catalog.Infra.Messaging.DTOs;
using FluentAssertions;

namespace FC.Codeflix.Catalog.EndToEndTests.Worker;

[Collection(nameof(VideoApiBaseFixture))]
public class VideoEncodedEventConsumerTest : IDisposable
{

    private readonly VideoApiBaseFixture _fixture;

    public VideoEncodedEventConsumerTest(VideoApiBaseFixture fixture) => _fixture = fixture;

    public void Dispose()
    {
        _fixture.DisposePersistence();
        _fixture.PurgeRabbitMQQueuesAsync()
            .GetAwaiter().GetResult();
    }

    [Fact(DisplayName = nameof(EncodingSuccededEventReceived))]
    [Trait("e2e/Worker", "Video Encoded - Event Handler")]
    public async Task EncodingSuccededEventReceived()
    {
        var exampleVideos = _fixture.GetVideoColletion(5);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var video = exampleVideos[2];

        var encodedFilePath = _fixture.GetValidMediaPath();
        var exampleEvent = new VideoEncodedMessageDTO
        {
            Video = new VideoEncodedMetadaDTO
            {
                EncodedVideoFolder = encodedFilePath,
                FilePath = video.Media!.FilePath,
                ResourceId = video.Id
            }
        };

        await _fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);

        var videoFromDB = await _fixture.VideoPersistence.GetById(video.Id);

        videoFromDB.Should().NotBeNull();
        videoFromDB.Media!.Status.Should().Be(EMediaStatus.Completed);
        videoFromDB.Media!.EncodedPath.Should().Be(exampleEvent.Video.FullEncodedVideoFilePath);
        videoFromDB.Media!.FilePath.Should().Be(video.Media!.FilePath);

        (object? @event, uint count) = await _fixture.ReadMessageFromRabbitMQ<object>(
            _fixture.WebAppFactory.RabbitMQConfiguration!.VideoEncodedQueue
        );

        @event.Should().BeNull();
        count.Should().Be(0);
    }

    [Fact(DisplayName = nameof(InvalidMessageEventReceived))]
    [Trait("e2e/Worker", "Video Encoded - Event Handler")]
    public async Task InvalidMessageEventReceived()
    {
        var exampleVideos = _fixture.GetVideoColletion(5);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);

        var encodedFilePath = _fixture.GetValidMediaPath();
        var exampleEvent = new VideoEncodedMessageDTO
        {
            Message = new VideoEncodedMetadaDTO
            {
                FilePath = _fixture.GetValidMediaPath(),
                ResourceId = Guid.NewGuid()
            },
            Error = "There was an error on processing the video."
        };

        await _fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);

        (object? @event, uint count) = await _fixture.ReadMessageFromRabbitMQ<object>(
            _fixture.WebAppFactory.RabbitMQConfiguration!.VideoEncodedQueue
        );

        @event.Should().BeNull();
        count.Should().Be(0);
    }

    [Fact(DisplayName = nameof(EncodingFailedEventReceived))]
    [Trait("e2e/Worker", "Video Encoded - Event Handler")]
    public async Task EncodingFailedEventReceived()
    {
        var exampleVideos = _fixture.GetVideoColletion(5);
        await _fixture.VideoPersistence.BulkInsert(exampleVideos);
        var video = exampleVideos[2];
        var encodedFilePath = _fixture.GetValidMediaPath();
        var exampleEvent = new VideoEncodedMessageDTO
        {
            Message = new VideoEncodedMetadaDTO
            {
                FilePath = video.Media!.FilePath,
                ResourceId = video.Id
            },
            Error = "There was an error on processing the video."
        };

        await _fixture.PublishMessageToRabbitMQ(exampleEvent);

        await Task.Delay(800);
        var videoFromDB = await _fixture.VideoPersistence.GetById(video.Id);
        videoFromDB.Should().NotBeNull();
        videoFromDB!.Media!.Status.Should().Be(EMediaStatus.Error);
        videoFromDB!.Media!.EncodedPath.Should().BeNull();
        (object? @event, uint count) = await _fixture.ReadMessageFromRabbitMQ<object>(
            _fixture.WebAppFactory.RabbitMQConfiguration!.VideoEncodedQueue
        );
        @event.Should().BeNull();
        count.Should().Be(0);
    }
}
