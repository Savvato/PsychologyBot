using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

using PsychologyBot.Core.Bot.Accessors;
using PsychologyBot.Core.Bot.Dialogs;
using PsychologyBot.Core.Interfaces;

namespace PsychologyBot.Core.Bot
{
    using System;

    using Microsoft.AspNetCore.SignalR;

    using PsychologyBot.Core.Models;
    using PsychologyBot.Network.Hubs;

    public class Bot : IBot
    {
        private readonly ConversationStateAccessors conversationStateAccessors;

        private readonly DialogSet dialogs;
        private readonly IUserBotRepository userRepository;
        private readonly IHubContext<ChatHub> chatHub;

        public Bot(
            IUserBotRepository userRepository,
            ConversationStateAccessors conversationStateAccessors,
            UserRegistrationDialog userRegistrationDialog,
            IHubContext<ChatHub> chatHub,
            ILogger<Bot> logger)
        {
            this.userRepository = userRepository;
            this.conversationStateAccessors = conversationStateAccessors;
            this.chatHub = chatHub;

            this.dialogs = new DialogSet(this.conversationStateAccessors.DialogStateAccessor);
            this.dialogs.Add(userRegistrationDialog);
        }

        public async Task OnTurnAsync(ITurnContext turnContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            DialogContext dialogContext = await this.dialogs.CreateContextAsync(turnContext, cancellationToken);

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    await this.HandleMessage(turnContext, dialogContext, cancellationToken);
                    break;
                case ActivityTypes.ConversationUpdate:
                    string botId = turnContext.Activity.Recipient.Id;
                    bool isBotAdded = turnContext.Activity.MembersAdded.ToList().Exists(member => member.Id == botId);

                    if (isBotAdded)
                    {
                        return;
                    }

                    if (!this.userRepository.IsUserExists(turnContext))
                    {
                        await turnContext.SendActivityAsync(
                            "Пожалуйста, пройдите регистрацию, ответив на несколько вопросов",
                            cancellationToken: cancellationToken);

                        await dialogContext.BeginDialogAsync(UserRegistrationDialog.DialogId,
                            cancellationToken: cancellationToken);
                    }

                    break;
            }
        }

        private async Task HandleMessage(ITurnContext turnContext, DialogContext dialogContext,
            CancellationToken cancellationToken)
        {
            DialogTurnResult results = await dialogContext.ContinueDialogAsync(cancellationToken);

            switch (results.Status)
            {
                // There wasn't any active dialog
                case DialogTurnStatus.Empty:
                    User user = this.userRepository.GetCurrentUser(turnContext);
                    Message message = new Message
                    {
                        MessageString = turnContext.Activity.Text,
                        IsUserMessage = true,
                        Date = DateTime.Now
                    };
                    user.Messages.Add(message);

                    await this.chatHub.Clients.All.SendAsync(method: "chatUpdate", arg1: user.ChannelId, arg2: message);

                    await turnContext.SendActivityAsync(
                        "Ваше сообщение отправлено психологу, пожалуйста, ожидайте ответа",
                        cancellationToken: cancellationToken);
                    break;
            }
        }
    }
}