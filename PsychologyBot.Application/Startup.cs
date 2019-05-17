namespace PsychologyBot.Application
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using PsychologyBot.Application.Extensions;
    using PsychologyBot.Network.Hubs;

    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerFactory loggerFactory;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.loggerFactory = loggerFactory;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSignalR();
            services.AddPsychologyBot(this.configuration, this.loggerFactory);
            services.AddRepositories();
            services.AddDialogs();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework()
                .UseMvcWithDefaultRoute()
                .UseSignalR(routes =>
                {
                    routes.MapHub<ChatHub>("/chat");
                });
        }
    }
}