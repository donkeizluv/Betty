using System;
using System.Threading;
using System.Threading.Tasks;
using Betty.EFModel;
using Betty.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Betty.Helper;
using Microsoft.AspNetCore.SignalR;
using Betty.Livefeeds;
using Microsoft.EntityFrameworkCore;
using Betty.DTO;

namespace Betty.Jobs
{
    public class ScrapeJob : BackgroundService
    {
        private readonly BetOptions _options;
        private readonly IServiceProvider _provider;
        private readonly ILogger<ScrapeJob> _logger;
        private readonly IHubContext<FixturesFeed> _hubcontext;
        public ScrapeJob(IOptions<BetOptions> options,
                            IServiceProvider provider,
                            ILogger<ScrapeJob> logger,
                            IHubContext<FixturesFeed> hubcontext)
        {
            _hubcontext = hubcontext;
            _options = options.Value;
            _provider = provider;
            _logger = logger;
        }
        private const string TablePath = @"//div[@id='masterdiv']/div/table[1]/tr";
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _provider.CreateScope())
                    {
                        var now = DateTime.Now;
                        var context = scope.ServiceProvider.GetService<BettyContext>();
                        var web = new HtmlWeb();
                        var doc = await web.LoadFromWebAsync(_options.ScrapeUrl);
                        var allTr = doc.DocumentNode.SelectNodes(TablePath);
                        if(allTr == null)
                        {
                            _logger.LogDebug("Can not get table rows");
                            continue;
                        }
                        foreach (var game in context.GameOdds.Where(g=> g.Start > now).Select(g => g))
                        {
                            var foundRow = false;
                            var error = false;
                            foreach (var tr in allTr)
                            {
                                //Find tr that contains players' names
                                var matchName = StringHelper.CleanInnerText(tr.SelectSingleNode("./td[2]")?.InnerText);
                                if(matchName.Contains(game.Player1.Trim(), StringComparison.OrdinalIgnoreCase) &&
                                    matchName.Contains(game.Player2.Trim(), StringComparison.OrdinalIgnoreCase))
                                {
                                    _logger.LogTrace($"Row for game {game.Player1}-{game.Player2} found");
                                    //Update odds
                                    foundRow = true;
                                    var handicapString = StringHelper.CleanInnerText(tr.SelectSingleNode("./td[4]")?.InnerText);
                                    var win1String = StringHelper.CleanInnerText(tr.SelectSingleNode("./td[3]")?.InnerText);
                                    var win2String = StringHelper.CleanInnerText(tr.SelectSingleNode("./td[5]")?.InnerText);
                                    var splitHandicap = handicapString.Split(':');
                                    if(splitHandicap.Count() != 2)
                                    {
                                        error = true;
                                        _logger.LogError($"Invalid handicap: '{handicapString}'");
                                        continue;
                                    }
                                    if(!StringHelper.TryParseDivideOperator(splitHandicap.First(), out var handicap1))
                                    {
                                        error = true;
                                        _logger.LogError($"Cant parse handicap1: '{splitHandicap.First()}'");
                                        continue;
                                    }
                                    if(!StringHelper.TryParseDivideOperator(splitHandicap.Last(), out var handicap2))
                                    {
                                        error = true;
                                        _logger.LogError($"Cant parse handicap2: '{splitHandicap.Last()}'");
                                        continue;
                                    }
                                    if(!decimal.TryParse(win1String, out var win1))
                                    {
                                        error = true;
                                        _logger.LogError($"Cant parse win1: '{win1String}'");
                                        continue;
                                    }
                                    if(!decimal.TryParse(win2String, out var win2))
                                    {
                                        error = true;
                                        _logger.LogError($"Cant parse win2: '{win2String}'");
                                        continue;
                                    }
                                    game.Odds1 = handicap1;
                                    game.Odds2 = handicap2;
                                    game.Win1 = win1;
                                    game.Win2 = win2;
                                    // games.Add(game);
                                }
                            }
                            if(!foundRow)
                                _logger.LogInformation($"No row found for {game.Player1}-{game.Player2}");
                        }
                        //Make sure the only type to change here is GameOdds
                        var changed = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified)
                                        .Select(e => new GameOddsFeedDto(){
                                             Id = ((GameOdds)e.Entity).Id,
                                             Odds1 = ((GameOdds)e.Entity).Odds1,
                                             Odds2 = ((GameOdds)e.Entity).Odds2,
                                             Win1 = ((GameOdds)e.Entity).Win1,
                                             Win2 = ((GameOdds)e.Entity).Win2
                                        }).ToList();
                         _logger.LogInformation($"Changes {changed.Count()}");
                        //  await _hubcontext.Clients.All.SendAsync("Fixtures", games);
                        if(changed.Count() > 0)
                        {
                            //Save & broadcast changes
                            await context.SaveChangesAsync();
                            await _hubcontext.Clients.All.SendAsync("Fixtures", changed);
                        }
                    }         
                }
                catch (Exception ex)
                {
                    Utility.LogException(ex, _logger);
                    // throw;
                }
                await Task.Delay(_options.Delay, stoppingToken);
            }
            _logger.LogInformation("ScrapeJob stopping...");
        }
    }
}
