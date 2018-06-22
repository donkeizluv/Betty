using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Betty.DTO
{
    public class CancelDto
    {
        [Required]
        public int Id { get; set; }
    }
}
