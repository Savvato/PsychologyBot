namespace PsychologyBot.Application
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

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
            services.AddDbStorage(this.configuration);
            services.AddHubAuthentication(this.configuration, this.hostingEnvironment);
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