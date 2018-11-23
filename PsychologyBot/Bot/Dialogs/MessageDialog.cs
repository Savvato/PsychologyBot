﻿namespace PsychologyBot.Bot.Dialogs
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;

    using PsychologyBot.Bot.Accessors;
    using PsychologyBot.Bot.States;
    using PsychologyBot.Models;
    using PsychologyBot.Repositories;

    public class MessageDialog : ComponentDialog
    {
        public const string DialogId = nameof(MessageDialog);

        private const string WaterfallDialogId = "MessageWaterfallDialogId";
        private const string TextPromptId = "TextPromptId";
        private const string ConfirmPromptId = "ConfirmPromptId";

        private readonly ConversationStateAccessors conversationStateAccessors;
        private readonly IUserBotRepository userRepository;

        public MessageDialog(
            ConversationStateAccessors conversationStateAccessors,
            IUserBotRepository userRepository)
            : base(DialogId)
        {
            this.conversationStateAccessors = conversationStateAccessors;
            this.userRepository = userRepository;

            WaterfallStep[] steps =
            {
                this.AskMessage,
                this.ApplyMessage,
                this.ConfirmSend,
                this.Send,
            };

            this.AddDialog(new WaterfallDialog(WaterfallDialogId, steps));

            this.AddDialog(new TextPrompt(TextPromptId));
            this.AddDialog(new ConfirmPrompt(ConfirmPromptId));
        }

        private async Task<DialogTurnResult> AskMessage(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                TextPromptId,
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text("Введите сообщение:"),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ApplyMessage(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MessageState messageState = await this.conversationStateAccessors
                .MessageStateAccessor
                .GetAsync(
                    stepContext.Context,
                    () => new MessageState(),
                    cancellationToken);

            messageState.MessageString = stepContext.Result.ToString();

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmSend(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MessageState messageState = await this.conversationStateAccessors
                .MessageStateAccessor
                .GetAsync(
                    stepContext.Context,
                    () => new MessageState(),
                    cancellationToken);
            await stepContext.Context.SendActivityAsync($"Вы написали: {messageState.MessageString}", cancellationToken: cancellationToken);

            return await stepContext.PromptAsync(
                ConfirmPromptId,
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text("Отправить это сообщение психологу?"),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> Send(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!((bool) stepContext.Result))
            {
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

            MessageState messageState = await this.conversationStateAccessors
                .MessageStateAccessor
                .GetAsync(
                    stepContext.Context,
                    () => new MessageState(),
                    cancellationToken);

            User user = this.userRepository.GetCurrentUser(stepContext.Context);
            user.Messages.Add(new Message(messageState.MessageString, true));

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}