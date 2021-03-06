﻿namespace PsychologyBot.Core.Bot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Bogus;
    using Bogus.DataSets;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;

    using PsychologyBot.Core.Bot.Accessors;
    using PsychologyBot.Core.Bot.States;
    using PsychologyBot.Core.Interfaces;
    using PsychologyBot.Core.Models;

    using Microsoft.AspNetCore.SignalR;

    using PsychologyBot.Network.Hubs;

    public class UserRegistrationDialog : ComponentDialog
    {
        public const string DialogId = nameof(UserRegistrationDialog);

        private const string WaterfallDialogId = "UserRegistrationDialogId";
        private const string ChoicePromptId = "ChoicePromptId";
        private const string ConfirmPromptId = "ConfirmPromptId";

        private readonly ConversationStateAccessors conversationStateAccessors;

        private readonly Faker userFaker;
        private readonly IUserBotRepository userRepository;
        private readonly IHubContext<ChatHub> chatHub;

        public UserRegistrationDialog(
            ConversationStateAccessors conversationStateAccessors,
            IUserBotRepository userRepository,
            IHubContext<ChatHub> chatHub)
            : base(DialogId)
        {
            this.conversationStateAccessors = conversationStateAccessors;
            this.userRepository = userRepository;
            this.chatHub = chatHub;

            this.userFaker = new Faker();

            WaterfallStep[] steps =
            {
                this.AskForRegistration,
                this.AskGender,
                this.ApplyGender,
                this.SuggestName,
                this.Register
            };

            this.AddDialog(new WaterfallDialog(WaterfallDialogId, steps));

            this.AddDialog(new ChoicePrompt(ChoicePromptId));

            this.AddDialog(new ConfirmPrompt(ConfirmPromptId));
        }

        private async Task<DialogTurnResult> AskForRegistration(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(
                "Пожалуйста, пройдите регистрацию",
                cancellationToken: cancellationToken);

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> AskGender(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                ChoicePromptId,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Пожалуйста, укажите ваш пол"),
                    RetryPrompt = MessageFactory.Text("Извините, не могу распознать Ваш ответ"),
                    Choices = ChoiceFactory.ToChoices(new List<string> {"Мужчина", "Женщина"})
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ApplyGender(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            RegistrationState registrationState = await this.conversationStateAccessors
                .RegistrationStateAccessor
                .GetAsync(
                    stepContext.Context,
                    () => new RegistrationState(),
                    cancellationToken);
            registrationState.Gender =
                ((FoundChoice) stepContext.Result).Value == "Мужчина" ? Gender.Male : Gender.Female;

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> SuggestName(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            RegistrationState registrationState = await this.conversationStateAccessors
                .RegistrationStateAccessor
                .GetAsync(
                    stepContext.Context,
                    () => new RegistrationState(),
                    cancellationToken);

            registrationState.Name = this.userFaker.Name.FullName(registrationState.Gender == Gender.Male
                ? Name.Gender.Male
                : Name.Gender.Female);

            await stepContext.Context.SendActivityAsync(
                $"Для общения с психологом Вам присвоен псевдоним: {registrationState.Name}",
                cancellationToken: cancellationToken);

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }


        private async Task<DialogTurnResult> Register(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            RegistrationState registrationState = await this.conversationStateAccessors
                .RegistrationStateAccessor
                .GetAsync(
                    stepContext.Context,
                    () => new RegistrationState(),
                    cancellationToken);

            User user = new User
            {
                ChannelId = stepContext.Context.Activity.From.Id,
                Name = registrationState.Name,
                Gender = registrationState.Gender,
                ConversationReference = stepContext.Context.Activity.GetConversationReference()
            };

            await this.userRepository.AddUser(user, cancellationToken);

            await this.chatHub.Clients.All.SendAsync(method: "userAdded", arg1: user, cancellationToken: cancellationToken);

            await stepContext.Context.SendActivityAsync(
                "Регистрация завершена, теперь все ваши сообщения будут отправляться психологу",
                cancellationToken: cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}