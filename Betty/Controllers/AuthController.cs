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

namespace Betty.Controllers
{
    [Authorize]
    [CustomExceptionFilterAttribute]
    public class AuthController : Controller
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IAuthService _service;
        public AuthController(IJwtFactory jwtFactory, IAuthService service, IOptions<JwtIssuerOptions> jwtOptions)
        {   
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _service = service;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm]string username = "", [FromForm]string pwd = "")
        {
            var name = username.ToLower();
            if(!_service.Authenticate(name, pwd)) return Forbid();
            var userIdentity = new ClaimsIdentity();
            userIdentity.AddClaim(new Claim(CustomClaims.Username, name));
            var jwt = await Token.GenerateJwt(userIdentity,
                _jwtFactory,
                name,
                _jwtOptions,
                new JsonSerializerSettings { Formatting = Formatting.Indented });
            return Ok(jwt);
        }
        [HttpGet]
        [Authorize]
        //Use this to check if token still valid
        public IActionResult Ping()
        {
            return Ok();
        }
    }
}
