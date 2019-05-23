namespace PsychologyBot.Application
{
    using Microsoft.AspNetCore.Builder;
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
            services.AddDbContext<PsyDbContext>(options =>
            {
                options.UseNpgsql(
                    connectionString: this.configuration.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.MigrationsAssembly(assemblyName: typeof(PsyDbContext).Assembly.GetName().Name));
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                PsyDbContext psyDbContext = serviceScope.ServiceProvider.GetRequiredService<PsyDbContext>();
                psyDbContext.Database.Migrate();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseSignalR(routes =>
                {
                    routes.MapHub<ChatHub>("/chat");
                })
                .UseBotFramework()
                .UseMvcWithDefaultRoute();
        }
    }
}