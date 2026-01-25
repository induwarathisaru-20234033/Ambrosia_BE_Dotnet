using AMB.Application.Interfaces.Services;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Infra.Identity
{
    public class SupabaseAuthService : IAuthService
    {
        private readonly Supabase.Client _client;

        public SupabaseAuthService(Supabase.Client client)
        {
            _client = client;
        }

        public async Task<string> CreateUserAsync(string email, string password, Dictionary<string, object>? claims)
        {
            var session = await _client.Auth.SignUp(email, password);

            if (session?.User == null)
            {
                throw new Exception("Failed to create user in Supabase");
            }

            return session.User.Id;
        }
    }
}
