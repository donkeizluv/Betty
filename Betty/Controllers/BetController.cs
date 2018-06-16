using Betty.Const;
using Betty.Filter;
using Betty.Auth;
using Betty.Models;
using Betty.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Betty.Options;
using Betty.EFModel;
using Betty.DTO;
using System.Linq;
using Betty.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Logging;
using Betty.Helper;

namespace Betty.Controllers
{
    [Authorize]
    [CustomExceptionFilterAttribute]
    public class BettyController : Controller
    {
        private readonly BettyContext _context;
        private readonly HttpContext _httpContext;
        private readonly BetOptions _options;
        private readonly IMailerService _mail;
        private ILogger _logger;
        // IHubContext<FixturesFeed> _hubcontext;
        public BettyController(BettyContext context,
                                IHttpContextAccessor httpContext,
                                IOptions<BetOptions> options,
                                IMailerService mail,
                                ILogger<BettyController> logger)
        {   
            _context = context;
            _httpContext = httpContext.HttpContext;
            _options = options.Value;
            _mail = mail;
            _logger = logger;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var vm = new BetVM();
            var now = DateTime.Now;
            vm.MaxBet = _options.MaxBet;
            vm.MinBet = _options.MinBet;
            vm.Step = _options.Step;
            //Add not expired games
            var gameList = await BaseQuery().Where(g => g.Start >= now).OrderBy(g => g.Start).ToListAsync();
            //Add expired games
            gameList.AddRange(await BaseQuery().Where(g => g.Start < now).OrderBy(g => g.Start).ToListAsync());
            vm.Games = gameList;
            return Ok(vm);
        }
        private IQueryable<GameOddsDto> BaseQuery()
        {
            return _context.GameOdds.Select(g => new GameOddsDto(){
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
                Registered = _context.Register.Any(r => r.GameId == g.Id && r.Username == ContextUsername())});
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] BetDto bet)
        { 
            if(!ModelState.IsValid) return BadRequest();
            var now = DateTime.Now;
            var userName = ContextUsername();
            //Checj already reg
            if(await _context.Register.AnyAsync(r => r.GameId == bet.Id && r.Username == userName))
                return BadRequest(); 
            //Check timed out
            var game = await _context.GameOdds.SingleOrDefaultAsync(g => g.Id == bet.Id);
            if(game == null) return BadRequest();
            if(now >= game.Start) return BadRequest();
            if(bet.Player != 1 && bet.Player != 2) return BadRequest();
            //Check amt
            if(bet.Amt < _options.MinBet || bet.Amt > _options.MaxBet || bet.Amt % _options.Step != 0) return BadRequest();
            //Save
            var newBet = new Register(){
                GameId = bet.Id,
                BetAmt = bet.Amt,
                BetPlayer = bet.Player,
                Username = userName,
                SubmitTime = now,
                RefOdds1 = game.Odds1,
                RefOdds2 = game.Odds2,
                RefWin1 = game.Win1,
                RefWin2 = game.Win2
            };
            await _context.Register.AddAsync(newBet);
            await _context.SaveChangesAsync();
            try
            {
                await _mail.MailNewBet(game, newBet);
            }
            catch (Exception ex)
            {
                Utility.LogException(ex, _logger);
            }
            return Ok();
        }
        private string ContextUsername()
        {
            return _httpContext.User.Claims.First(c => c.Type == CustomClaims.Username).Value.ToLower();
        }
    }
}
