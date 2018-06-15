using Betty.Auth;
using Betty.DTO;
using Betty.Options;
using Microsoft.Extensions.Options;

namespace Betty.Service
{
    public class MailerService
    {
        private readonly MailerOptions _options;
        public MailerService(IOptions<MailerOptions> options)
        {
            _options = options.Value;
        }
        public void Mail(BetDto bet)
        {
            
        }
    }
}