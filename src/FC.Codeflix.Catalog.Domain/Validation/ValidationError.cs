namespace FC.Codeflix.Catalog.Domain.Validation;

public record ValidationError(string ErrorMessage, string? PropertyName = null);
