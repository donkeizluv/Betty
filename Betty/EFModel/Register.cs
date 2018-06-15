using System;
using System.Collections.Generic;

namespace Betty.EFModel
{
    public partial class Register
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int GameId { get; set; }
        public DateTime SubmitTime { get; set; }
        public long BetAmt { get; set; }
        public short BetPlayer { get; set; }
        public decimal RefOdds1 { get; set; }
        public decimal RefOdds2 { get; set; }
        public decimal RefWin1 { get; set; }
        public decimal RefWin2 { get; set; }
    }
}
