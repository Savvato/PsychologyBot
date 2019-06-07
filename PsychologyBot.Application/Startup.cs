namespace PsychologyBot.Application
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    using PsychologyBot.Application.Extensions;
    using PsychologyBot.Infrastructure.Db;
    using PsychologyBot.Network.Hubs;

    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerFactory loggerFactory;
        private readonly IHostingEnvironment hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
            this.loggerFactory = loggerFactory;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSignalR();
            services.AddPsychologyBot(this.configuration, this.loggerFactory);
            services.AddRepositories();
            services.AddDialogs();
            services.AddDbContext<PsyDbContext>(options =>
            {
                options.UseNpgsql(
                    connectionString: this.configuration.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.MigrationsAssembly(assemblyName: typeof(PsyDbContext).Assembly.GetName().Name));
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = this.hostingEnvironment.IsProduction();
                    options.Audience = this.configuration["Identity:Audience"];
                    options.Authority = this.configuration["Identity:Issuer"];
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = this.configuration["Identity:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = this.configuration["Identity:Audience"],
                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = false,
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    name: "OnlyPsychologists",
                    policyBuilder =>
                    {
                        string[] userIds = this.configuration.GetSection("Identity:Psychologists")
                            .GetChildren()
                            .ToArray()
                            .Select(c => c.Value)
                            .ToArray();

                        policyBuilder.RequireClaim(
                            claimType: "sub",
                            requiredValues: userIds);
                    });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                PsyDbContext psyDbContext = serviceScope.ServiceProvider.GetRequiredService<PsyDbContext>();
                psyDbContext.Database.Migrate();
            }

            if (this.hostingEnvironment.IsProduction())
            {
                app.UseHttpsRedirection();
            }

            app
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseAuthentication()
                .UseSignalR(routes =>
                {
                    routes.MapHub<ChatHub>("/chat");
                })
                .UseBotFramework()
                .UseMvcWithDefaultRoute();
        }
    }
}