using System;

namespace Betty.Options
{
    public class BetOptions
    {
        public bool LiveUpdate { get; set; }
        public string ScrapeUrl { get; set; }
        public int Delay { get; set; }
        public long MaxBet { get; set; }
        public long MinBet { get; set; }
        public long Step { get; set; }
    }
}
