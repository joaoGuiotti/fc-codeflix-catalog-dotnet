using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FC.Codeflix.Catalog.Api.Filters;

public class ApiGlobalExceptionFilter : IExceptionFilter
{
    private readonly IHostEnvironment _env;
    public ApiGlobalExceptionFilter(IHostEnvironment env)
    {
        _env = env;
    }

    public void OnException(ExceptionContext context)
    {
        var details = new ProblemDetails();
        var execption = context.Exception;

        if (_env.IsDevelopment())
            details.Extensions.Add("StackTrace", execption.StackTrace);

        switch (execption)
        {
            case EntityValidationException:
                details.Title = "One or more validation errors occurred.";
                details.Type = "UnprocessableEntity";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Detail = execption.Message;
                break;
            case NotFoundException:
                details.Title = "Not Found";
                details.Type = "NotFound";
                details.Status = StatusCodes.Status404NotFound;
                details.Detail = execption.Message;
                break;
            default:
                details.Title = "An unexpected error ocurred";
                details.Type = "UnexpectedError";
                details.Status = StatusCodes.Status422UnprocessableEntity;
                details.Detail = execption.Message;
                break;

        }

        context.HttpContext.Response.StatusCode = (int)details.Status;
        context.Result = new ObjectResult(details);
        context.ExceptionHandled = true;
    }
}
