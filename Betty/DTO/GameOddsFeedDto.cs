using System;
using System.Collections.Generic;

namespace Betty.DTO
{
    public class GameOddsFeedDto
    {
        public int Id { get; set; }
        public decimal Odds1 { get; set; }
        public decimal Odds2 { get; set; }
        public decimal Win1 { get; set; }
        public decimal Win2 { get; set; }
    }
}
