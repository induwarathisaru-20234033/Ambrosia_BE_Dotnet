namespace AMB.Domain.Exceptions
{
    public class BaseException: Exception
    {
        public string ErrorDescription { get; }

        protected BaseException(string message, string errorDescription = "")
            : base(message)
        {
            ErrorDescription = errorDescription;
        }
    }
}
