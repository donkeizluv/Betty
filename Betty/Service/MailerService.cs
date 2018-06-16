using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Betty.Auth;
using Betty.DTO;
using Betty.EFModel;
using Betty.Options;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;

namespace Betty.Service
{
    public class MailerService : IMailerService
    {
        private readonly MailerOptions _options;
        private readonly IHtmlComposer _composer;
        public MailerService(IOptions<MailerOptions> options, IHtmlComposer composer)
        {
            _options = options.Value;
            _composer = composer;
        }
        public async Task MailNewBet(GameOdds game, Register bet)
        {
            _composer.AppendText("p", $"New bet from: <b>{bet.Username}</b>");
            _composer.AppendText("p", $"<b>{game.Player1} vs {game.Player2}</b> - {game.Start.ToString("dd-MM-yyyy hh:mm")}");
            if(bet.BetPlayer == 1)
            {
                _composer.AppendText("p", $"Bet on: <b>{game.Player1}</b>");
            }
            else
            {
                _composer.AppendText("p", $"Bet on: <b>{game.Player2}</b>");
            }
            _composer.AppendText("p", $"Bet amount: <b>{bet.BetAmt.ToString("#,##0")}</b>");
            _composer.AppendText("p", "With:");
            _composer.AppendText("p", $"Handicap: <b>{Math.Round(bet.RefOdds1, 3)} | {Math.Round(bet.RefOdds2, 3)}</b>");
            _composer.AppendText("p", $"O/U: <b>{Math.Round(bet.RefWin1, 3)} | {Math.Round(bet.RefWin2, 3)}</b>");
            _composer.AppendText("br", string.Empty);
            _composer.AppendText("p", $"Bet more at: {_options.Ad}");
            _composer.AppendText("p", "Thanks for playing :D");

            var mail = new MailMessage()
            {
                From = CreateMailAddress(_options.Username),
                IsBodyHtml = true,
                Body = _composer.ToString(),
                Subject = "Bet confirm"
            };
            //Add receivers
            foreach (var address in _options.Receivers.Select(r => CreateMailAddress(r)))
            {
                mail.To.Add(address);
            }
            //CC bet owner
            mail.CC.Add(CreateMailAddress(bet.Username));
            var smtp = new SmtpClient(_options.Server, _options.Port);
            smtp.Credentials = new NetworkCredential(_options.Username, _options.Pwd);
            await smtp.SendMailAsync(mail);
        }
        private MailAddress CreateMailAddress(string s)
        {
            if(!s.Contains("@"))
                return new MailAddress(s + _options.Suffix);
            return new MailAddress(s);
        }
    }
}