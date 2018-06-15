using Betty.Auth;
using Betty.Options;
using Microsoft.Extensions.Options;

namespace Betty.Service
{
    public class AuthService : IAuthService
    {
        private readonly AuthOptions _authOptions;
        public AuthService(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions.Value;
        }
        public bool Authenticate(string userName, string pwd)
        {
            if(_authOptions.NoPwdCheck) return true;
            return WindowsAuth.Authenticate(userName, pwd, _authOptions.Domain);
        }
    }
}