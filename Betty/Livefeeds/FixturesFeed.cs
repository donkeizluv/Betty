using Betty.EFModel;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betty.Livefeeds
{
    public class FixturesFeed : Hub
    {
        public async Task Broadcast(List<GameOdds> games)
        {
            await Clients.All.SendAsync("Fixtures", games);
        }
    }
}