using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using PsychologyBot.Bot.Dialogs;
using PsychologyBot.Core.Bot.Accessors;
using PsychologyBot.Core.Bot.Dialogs;
using PsychologyBot.Core.Interfaces;

namespace PsychologyBot.Core.Bot
{
    public class Bot : IBot
    {
        private readonly ConversationStateAccessors conversationStateAccessors;

        private readonly DialogSet dialogs;
        private readonly ILogger<Bot> logger;
        private readonly IUserBotRepository userRepository;

        public Bot(
            IUserBotRepository userRepository,
            ConversationStateAccessors conversationStateAccessors,
            UserRegistrationDialog userRegistrationDialog,
            MessageDialog messageDialog,
            ILogger<Bot> logger)
        {
            this.userRepository = userRepository;
            this.conversationStateAccessors = conversationStateAccessors;
            this.logger = logger;

            dialogs = new DialogSet(this.conversationStateAccessors.DialogStateAccessor);
            dialogs.Add(userRegistrationDialog);
            dialogs.Add(messageDialog);
        }

        public async Task OnTurnAsync(ITurnContext turnContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            DialogContext dialogContext = await dialogs.CreateContextAsync(turnContext, cancellationToken);

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    await HandleMessage(turnContext, dialogContext, cancellationToken);
                    break;
                case ActivityTypes.ConversationUpdate:
                    string botId = turnContext.Activity.Recipient.Id;
                    bool isBotAdded = turnContext.Activity.MembersAdded.ToList().Exists(member => member.Id == botId);

                    if (isBotAdded) return;

                    if (!userRepository.IsUserExists(turnContext))
                    {
                        await turnContext.SendActivityAsync(
                            "Пожалуйста, пройдите регистрацию, ответив на несколько вопросов",
                            cancellationToken: cancellationToken);

                        await dialogContext.BeginDialogAsync(UserRegistrationDialog.DialogId,
                            cancellationToken: cancellationToken);
                        return;
                    }

                    await SendActionsCard(turnContext, cancellationToken);
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
                    switch (turnContext.Activity.Text)
                    {
                        case "Отправить сообщение":
                            await dialogContext.BeginDialogAsync(MessageDialog.DialogId,
                                cancellationToken: cancellationToken);
                            break;
                    }

                    break;
                case DialogTurnStatus.Complete:
                case DialogTurnStatus.Cancelled:
                    await SendActionsCard(turnContext, cancellationToken);
                    break;
            }
        }

        private async Task SendActionsCard(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            List<CardAction> actions = new List<CardAction>();

            CardAction cardAction = new CardAction
            {
                Value = "Отправить сообщение",
                Title = "Отправить сообщение",
                Type = ActionTypes.ImBack
            };

            actions.Add(cardAction);

            HeroCard actionsCard = new HeroCard
            {
                Buttons = actions,
                Text = "Что бы вы хотели бы сделать?"
            };

            Activity reply = turnContext.Activity.CreateReply();
            reply.Attachments.Add(actionsCard.ToAttachment());
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}