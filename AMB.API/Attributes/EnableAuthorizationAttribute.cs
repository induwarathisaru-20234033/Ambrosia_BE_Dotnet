using Microsoft.AspNetCore.Authorization;

namespace AMB.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class EnableAuthorizationAttribute : AuthorizeAttribute
    {
    }
}
