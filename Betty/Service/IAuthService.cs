namespace Betty.Service
{
    public interface IAuthService
    {
        bool Authenticate(string userName, string pwd);
    }
}