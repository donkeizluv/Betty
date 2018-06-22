using System;
using System.Linq;
using System.Threading.Tasks;
using Betty.Auth;
using Betty.Const;
using Betty.DTO;
using Betty.EFModel;
using Betty.Helper;
using Betty.Options;
using Betty.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Betty.Service
{
    public class BetService : IBetService
    {
        private readonly HttpContext _httpContext;
        private readonly BetOptions _options;
        private readonly BettyContext _context;
        private readonly IMailerService _mail;
        private ILogger _logger;
        public BetService(IOptions<BetOptions> options,
            BettyContext context,
            IHttpContextAccessor httpContext,
            IMailerService mail,
            ILogger<BetService> logger)
        {
            _options = options.Value;
            _context = context;
            _httpContext = httpContext.HttpContext;
            _mail = mail;
            _logger = logger;
        }
        public async Task<BetVM> GetVM()
        {
            var vm = new BetVM();
            var now = DateTime.Now;
            vm.Now = now;
            vm.MaxBet = _options.MaxBet;
            vm.MinBet = _options.MinBet;
            vm.Step = _options.Step;
            //Add not expired games
            var gameList = await BaseQuery(now)
                            .Where(g => g.Start >= now)
                            .OrderBy(g => g.Start)
                            .Concat(BaseQuery(now) //Expired games
                                .Where(g => g.Start < now)
                                .OrderByDescending(g => g.Start))
                            .ToListAsync();
            vm.Games = gameList;
            return vm;
        }
        public async Task Cancel(CancelDto cancel)
        {
            var now = DateTime.Now;
            var reg = await _context.Register
                .Where(r => r.GameId == cancel.Id && r.Username == Username)
                .Include(r => r.Game)
                .SingleOrDefaultAsync();
            //Valid reg from context user
            if (reg == null)
                throw new ArgumentException("Bet not valid");
            //Check expired
            if (reg.Game.Start <= now)
                throw new ArgumentException("Game has started");
            //OK, proceed to remove
            _context.Register.Remove(reg);
            await _context.SaveChangesAsync();
            _mail.MailCancelBet(reg);
        }
        public async Task Create(BetDto bet)
        {
            var now = DateTime.Now;
            if (await _context.Register.AnyAsync(r => r.GameId == bet.Id && r.Username == Username))
                throw new ArgumentException("This bet has already been made");
            //Get game
            var game = await _context.GameOdds.SingleOrDefaultAsync(g => g.Id == bet.Id && now < g.Start);
            if (game == null)
                throw new ArgumentException("Invalid game id");
            //Check amt
            if (bet.Amt < _options.MinBet ||
                bet.Amt > _options.MaxBet ||
                bet.Amt % _options.Step != 0)
                throw new ArgumentException("Bet amount is not valid");
            //Save
            var newBet = new Register()
            {
                GameId = bet.Id,
                BetAmt = bet.Amt,
                BetPlayer = bet.Player,
                Username = Username,
                SubmitTime = now,
                RefOdds1 = game.Odds1,
                RefOdds2 = game.Odds2,
                RefWin1 = game.Win1,
                RefWin2 = game.Win2
            };
            await _context.Register.AddAsync(newBet);
            await _context.SaveChangesAsync();
            _mail.MailNewBet(game, newBet);
        }
        private IQueryable<GameOddsDto> BaseQuery(DateTime baseTime)
        {
            return _context.GameOdds.Select(g => new GameOddsDto()
            {
                Id = g.Id,
                Player1 = g.Player1,
                CountryCode1 = g.CountryCode1,
                Player2 = g.Player2,
                CountryCode2 = g.CountryCode2,
                Odds1 = g.Odds1,
                Odds2 = g.Odds2,
                Win1 = g.Win1,
                Win2 = g.Win2,
                MatchType = g.MatchType,
                Start = g.Start,
                Expired = baseTime > g.Start,
                TotalReg = g.Register.Count,
                Player1Reg = g.Register.Where(r => r.BetPlayer == 1).Count(),
                Player2Reg = g.Register.Where(r => r.BetPlayer == 2).Count(),
                Registered = g.Register.Any(r => r.Username == Username)
            });
        }
        private string Username => _httpContext.User.Claims.First(c => c.Type == CustomClaims.Username).Value.ToLower();
    }
}