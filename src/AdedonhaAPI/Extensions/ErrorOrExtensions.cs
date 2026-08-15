using AdedonhaAPI.Application.Common.Context;
using ErrorOr;

namespace AdedonhaAPI.Extensions
{
    public static class ErrorOrExtensions
    {
        public static IResult MatchResponse<T>(this ErrorOr<T> result, Func<T, IResult> onValue) =>
            result.Match(onValue, HandleErrors);

        private static IResult HandleErrors(List<Error> errors)
        {
            if (errors.Count == 0)
                return Results.Problem();

            var firstError = errors.First();

            var statusCode = firstError.Type switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            return Results.Problem(
                statusCode: statusCode,
                title: firstError.Description,
                extensions: new Dictionary<string, object?>
                {
                    { "errorCode", firstError.Code },
                    { "errors", errors.Select(e => new { e.Code, e.Description }) },
                    { "operationId", OperationContext.Current ?? "unknown" }
                });
        }
    }
}
