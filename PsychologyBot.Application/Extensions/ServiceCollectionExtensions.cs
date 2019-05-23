namespace PsychologyBot.Application.Extensions
{
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.BotFramework;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using PsychologyBot.Core.Bot;
    using PsychologyBot.Core.Bot.Accessors;
    using PsychologyBot.Core.Bot.Dialogs;
    using PsychologyBot.Core.Interfaces;
    using PsychologyBot.Infrastructure.Repositories;

    public static class ServiceCollectionExtensions
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<UserRepository>();
            services.AddScoped<IUserBotRepository>(
                serviceProvider => serviceProvider.GetRequiredService<UserRepository>());
            services.AddScoped<IUserRepository>(
                serviceProvider => serviceProvider.GetRequiredService<UserRepository>());
        }

        public static void AddDialogs(this IServiceCollection services)
        {
            services.AddTransient<UserRegistrationDialog>();
        }

        public static void AddPsychologyBot(this IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            ILogger<Startup> logger = loggerFactory.CreateLogger<Startup>();

            IStorage dataStore = new MemoryStorage();
            ConversationState conversationState = new ConversationState(dataStore);
            UserState userState = new UserState(dataStore);
            ConfigurationCredentialProvider credentialProvider = new ConfigurationCredentialProvider(configuration);

            ConversationStateAccessors conversationStateAccessors = new ConversationStateAccessors(conversationState);

            services.AddSingleton(dataStore);
            services.AddSingleton(conversationState);
            services.AddSingleton(userState);
            services.AddSingleton(credentialProvider);
            services.AddSingleton(conversationStateAccessors);

            services.AddBot<Bot>(options =>
            {
                options.CredentialProvider = credentialProvider;

                options.OnTurnError = async (context, exception) =>
                {
                    logger.LogError(exception.Message, exception);
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                    await context.SendActivityAsync($"EXCEPTION: {exception.Message}");
                };

                options.Middleware.Add(new ShowTypingMiddleware());
                options.Middleware.Add(new AutoSaveStateMiddleware(userState, conversationState));
            });
        }
    }
}