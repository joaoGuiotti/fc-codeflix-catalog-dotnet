using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
using FC.Codeflix.Catalog.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Models.Video;


public class UploadMediaApiInput
{

    private static class MediaType
    {
        public const string Banner = "banner";
        public const string Thumb = "thumbnail";
        public const string ThumbHalf = "thumbnail_half";
        public const string Media = "video";
        public const string Trailer = "trailer";
    }


    [FromForm(Name = "media_file")]
    public IFormFile Media { get; set; }

    public UploadMediasInput ToUploadMediasInput(Guid id, string type)
        => type?.ToLower() switch
        {
            MediaType.Banner => new UploadMediasInput(id, BannerFile: Media.ToFileInput()),
            MediaType.Thumb => new UploadMediasInput(id, ThumbFile: Media.ToFileInput()),
            MediaType.ThumbHalf => new UploadMediasInput(id, ThumbHalfFile: Media.ToFileInput()),
            MediaType.Media => new UploadMediasInput(id, VideoFile: Media.ToFileInput()),
            MediaType.Trailer => new UploadMediasInput(id, TrailerFile: Media.ToFileInput()),
            _ => throw new EntityValidationException(
                $"'{type}' is not a valid media type.")
        };

}

public static class IFormFileExtensions
{
    public static FileInput? ToFileInput(this IFormFile? formFile)
    {
        if (formFile == null) return null;
        var fileStream = new MemoryStream();
        formFile.CopyTo(fileStream);
        return new FileInput(
            Path.GetExtension(formFile.FileName),
            fileStream,
            formFile.ContentType);
    }
}
