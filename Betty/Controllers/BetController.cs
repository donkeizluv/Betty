using Betty.Const;
using Betty.Filter;
using Betty.Auth;
using Betty.Models;
using Betty.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Betty.Options;
using Betty.EFModel;
using Betty.DTO;
using System.Linq;
using Betty.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Logging;
using Betty.Helper;

namespace Betty.Controllers
{
    [Authorize]
    [CustomExceptionFilterAttribute]
    public class BettyController : Controller
    {
        private readonly ILogger _logger;
        private readonly IBetService _service;
        // IHubContext<FixturesFeed> _hubcontext;
        public BettyController(ILogger<BettyController> logger, IBetService service)
        {   
            _logger = logger;
            _service = service;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await _service.GetVM());
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] BetDto bet)
        { 
            if(!ModelState.IsValid) return BadRequest();
            if(bet.Player != 1 && bet.Player != 2) return BadRequest();
            try
            {
                await _service.Create(bet);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Cancel([FromBody] CancelDto cancel)
        {
            if(!ModelState.IsValid) return BadRequest();
            try
            {
                await _service.Cancel(cancel);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
