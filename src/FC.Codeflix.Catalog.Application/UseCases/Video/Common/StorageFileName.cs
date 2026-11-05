namespace FC.Codeflix.Catalog.Application.UseCases.Video.Common;

public static class StorageFileName
{
    public static string Create(Guid id, string propertyName, string extension)
        => $"{id}-{propertyName.ToLower().Trim()}.{extension.Replace(".", "").Trim()}";
}
