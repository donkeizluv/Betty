using System;
using System.Collections.Generic;
using Betty.DTO;
using Betty.EFModel;

namespace Betty.ViewModels
{
    public class BetVM
    {
        public DateTime Now { get; set; }
        public IEnumerable<GameOddsDto> Games { get; set; }
        public long MaxBet { get; set; }
        public long MinBet { get; set; }
        public long Step { get; set; }
    }
}
