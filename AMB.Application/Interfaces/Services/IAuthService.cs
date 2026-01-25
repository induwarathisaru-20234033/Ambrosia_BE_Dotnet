using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> CreateUserAsync(string email, string password, Dictionary<string, object>? claims);
    }
}
