namespace AMB.Domain.Exceptions
{
    public class Auth0Exception : BaseException
    {
        public string? Auth0Error { get; }
        public string? Auth0ErrorDescription { get; }
        public int? StatusCode { get; }

        public Auth0Exception(string message, string? error = null, string? errorDescription = null, int? statusCode = null)
            : base(message, errorDescription ?? error ?? string.Empty)
        {
            Auth0Error = error;
            Auth0ErrorDescription = errorDescription;
            StatusCode = statusCode;
        }
    }
}
