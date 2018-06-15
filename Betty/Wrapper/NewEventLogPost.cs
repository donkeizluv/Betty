using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Betty.Wrapper
{
    public class NewEventLogPost
    {
        private string _name;
        private string _note;

        public int ContractId { get; set; }
        public string Name { get => _name?.Trim()?? string.Empty; set => _name = value; }
        public DateTime DateOfEvent { get; set; }
        public IFormFile File { get; set; }
        public string Note { get => _note?.Trim()?? string.Empty; set => _note = value; }
    }
    public class UpdateEventLogPost
    {
        private string _name;
        private string _note;
        public int EventId { get; set; }
        public string Name { get => _name?.Trim()?? string.Empty; set => _name = value; }
        public DateTime DateOfEvent { get; set; }
        public string Note { get => _note?.Trim()?? string.Empty; set => _note = value; }
    }
}
