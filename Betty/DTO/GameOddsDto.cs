using System;
using System.Collections.Generic;

namespace Betty.DTO
{
    public class GameOddsDto
    {
        public int Id { get; set; }
        public string Player1 { get; set; }
        public string CountryCode1 { get; set; }
        public string Player2 { get; set; }
        public string CountryCode2 { get; set; }
        public string MatchType { get; set; }
        public decimal Odds1 { get; set; }
        public decimal Odds2 { get; set; }
        public decimal Win1 { get; set; }
        public decimal Win2 { get; set; }
        public DateTime Start { get; set; }
        public bool Registered { get; set; }
    }
}
