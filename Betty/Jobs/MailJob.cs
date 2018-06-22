using System;
using System.Threading;
using System.Threading.Tasks;
using Betty.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Betty.Helper;
using System.Net.Mail;
using System.Net;
using Betty.Service;

namespace Betty.Jobs
{
    public class MailJob : BackgroundService
    {
        private readonly MailerOptions _options;
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;
        private readonly IMailQueue _queue;
        public MailJob(IOptions<MailerOptions> options,
                            IServiceProvider provider,
                            ILogger<MailJob> logger,
                            IMailQueue queue)
        {
            _options = options.Value;
            _provider = provider;
            _logger = logger;
            _queue = queue;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(_options.Disabled)
            {
                _logger.LogDebug("MailJob is disabled");
                return;
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using(var smtp = new SmtpClient(_options.Server, _options.Port))
                    {
                        smtp.Credentials = new NetworkCredential(_options.Username, _options.Pwd);
                        while(_queue.TryDequeue(out var mail))
                        {
                            await smtp.SendMailAsync(mail);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utility.LogException(ex, _logger);
                    // throw;
                }
                await Task.Delay(1000, stoppingToken);
            }
            _logger.LogInformation("ScrapeJob stopping...");
        }
    }
}
