namespace Management_System_Api.Exceptions
{
    public abstract class ApiException : Exception
    {
        protected ApiException(string message, int statusCode) : base(message) { StatusCode = statusCode; }
        public int StatusCode { get; }
    }
    public class NotFoundException : ApiException { public NotFoundException(string m) : base(m, 404) { } }
    public class BadRequestException : ApiException { public BadRequestException(string m) : base(m, 400) { } }
    public class ForbiddenException : ApiException { public ForbiddenException(string m) : base(m, 403) { } }


}
