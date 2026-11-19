using FullTemplate.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace FullTemplate.Web.Infrastructure;

public static class ResultExtensions
{
    public static ActionResult ToFailure(this Result result)
    {
        var body = new { errors = result.Errors };

        return result.ErrorKind switch
        {
            ErrorKind.NotFound     => new NotFoundObjectResult(body),
            ErrorKind.Unauthorized => new UnauthorizedObjectResult(body),
            ErrorKind.Forbidden    => new ForbidResult(),
            ErrorKind.Conflict     => new ConflictObjectResult(body),
            
            _                      => new BadRequestObjectResult(body)
        };
    }


}