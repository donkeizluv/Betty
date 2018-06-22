using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Betty.DTO
{
    public class BetDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public long Amt { get; set; }
        [Required]
        [Range(1,2)]
        public short Player { get; set; }
    }
}
