namespace PsychologyBot.Network.Hubs
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.BotFramework;

    using PsychologyBot.Core.Interfaces;
    using PsychologyBot.Core.Models;

    public class ChatHub : Hub
    {
        private readonly IUserRepository userRepository;
        private readonly BotFrameworkAdapter adapter;
        private readonly ConfigurationCredentialProvider credentialProvider;

        public ChatHub(IUserRepository userRepository, BotFrameworkAdapter adapter, ConfigurationCredentialProvider credentialProvider)
        {
            this.userRepository = userRepository;
            this.adapter = adapter;
            this.credentialProvider = credentialProvider;
        }

        public async Task SendMessageToUser(string userId, string text)
        {
            User user = this.userRepository.GetUserById(userId);
            Message message = new Message(
                messageString: text,
                isUserMessage: false);
            user.Messages.Add(message);

            await this.adapter.ContinueConversationAsync(
                botAppId: this.credentialProvider.AppId,
                reference: user.ConversationReference,
                callback: async (turnContext, cancellationToken) => await turnContext.SendActivityAsync(
                    message.MessageString,
                    cancellationToken: cancellationToken),
                cancellationToken: default);

            await this.Clients.All.SendAsync(method: "chatUpdate", arg1: userId, arg2: message);
        }

        public async Task SendMessageToPsychologyst(string userId, string text)
        {
            User user = this.userRepository.GetUserById(userId);
            Message message = new Message(
                messageString: text,
                isUserMessage: true);
            user.Messages.Add(message);

            await this.Clients.All.SendAsync(method: "chatUpdate", arg1: userId, arg2: message);
        }

        public async Task UserAdded(User user)
        {
            await this.Clients.All.SendAsync(method: "userAdded", arg1: user);
        }
    }
}
