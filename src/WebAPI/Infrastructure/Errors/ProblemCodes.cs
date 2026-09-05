namespace WebAPI.Infrastructure.Errors;

internal static class ProblemCodes
{
    public const string Unexpected       = "Error.Unexpected";
    public const string ValidationFailed = "Request.ValidationFailed";
    public const string Unauthorized     = "Request.Unauthorized";
    public const string Forbidden        = "Request.Forbidden";
    public const string NotFound         = "Resource.NotFound";
    public const string MethodNotAllowed = "Request.MethodNotAllowed";
    public const string UnsupportedMedia = "Request.UnsupportedMediaType";
}