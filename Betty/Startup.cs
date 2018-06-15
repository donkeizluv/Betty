using Betty.EFModel;
using Betty.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text;
using Betty.Options;
using Betty.Service;
using Betty.Jobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using NLog.Extensions.Logging;
using Betty.Livefeeds;

namespace Betty
{
    public class Startup
    {
        public string SecretKey
        {
            get
            {
                return Configuration.GetSection(nameof(JwtIssuerOptions)).GetValue<string>("Secret");
            }
        }
        private readonly SymmetricSecurityKey _signingKey;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Jobs
            services.AddSingleton<IHostedService, ScrapeJob>();
            //Jwt
            services.AddSingleton<IJwtFactory, JwtFactory>();
            //Inject db context
            services.AddDbContext<BettyContext>(options =>
                       options.UseSqlServer(Configuration.GetConnectionString("Default")));
            services.AddScoped<IAuthService, AuthService>();
            //Http context
            services.AddHttpContextAccessor();
            //SignalR
            services.AddSignalR()
            .AddJsonProtocol(options => {
                options.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();
                options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            //Get setting section
            var jwtSection = Configuration.GetSection(nameof(JwtIssuerOptions));
            var authSection = Configuration.GetSection(nameof(AuthOptions));
            var betSection = Configuration.GetSection(nameof(BetOptions));
            //Inject option
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtSection[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtSection[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
            services.Configure<BetOptions>(options =>
            {
                options.LiveUpdate = betSection[nameof(BetOptions.ScrapeUrl)] == "1";
                options.ScrapeUrl = betSection[nameof(BetOptions.ScrapeUrl)];
                options.MaxBet = long.Parse(betSection[nameof(BetOptions.MaxBet)]);
                options.Step = long.Parse(betSection[nameof(BetOptions.Step)]);
                options.MinBet = long.Parse(betSection[nameof(BetOptions.MinBet)]);
                options.Delay = int.Parse(betSection[nameof(BetOptions.Delay)]);
            });
            services.Configure<AuthOptions>(options =>
            {
                options.Domain = authSection[nameof(AuthOptions.Domain)];
                options.NoPwdCheck = authSection[nameof(AuthOptions.NoPwdCheck)] == "1";
            });
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSection[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtSection[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtSection[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            //Compression
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                //Everything else is too small to compress
                options.MimeTypes = new[] { "text/css", "application/javascript" };
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Fastest;
            });
            services.AddMvc().AddJsonOptions(options =>
            {
                //solve auto camel case prop names
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //ignore loop ref of object contains each other
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            //Nlog
            env.ConfigureNLog("NLog.config");
            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();
            app.UseAuthentication();
            app.UseResponseCompression();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseSignalR(routes =>
            {
                routes.MapHub<FixturesFeed>("/Livefeed");
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
                routes.MapRoute(
                   name: "api",
                   template: "API/{controller}/{action}");
                //Use this to fallback route in case of using vue router heavily
                //Install - Package Microsoft.AspNetCore.SpaServices
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

    }
}
