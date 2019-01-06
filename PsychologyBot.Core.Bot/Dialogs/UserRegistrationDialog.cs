using PsychologyBot.Core.Bot.Accessors;
using PsychologyBot.Core.Bot.States;
using PsychologyBot.Core.Interfaces;
using PsychologyBot.Core.Models;

namespace PsychologyBot.Bot.Dialogs
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Bogus;
    using Bogus.DataSets;

    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;

    public class UserRegistrationDialog : ComponentDialog
    {
        public const string DialogId = nameof(UserRegistrationDialog);

        private const string WaterfallDialogId = "UserRegistrationDialogId";
        private const string ChoicePromptId = "ChoicePromptId";
        private const string ConfirmPromptId = "ConfirmPromptId";

        private readonly ConversationStateAccessors conversationStateAccessors;
        private readonly IUserBotRepository userRepository;

        private readonly Faker userFaker;

        public UserRegistrationDialog(
            ConversationStateAccessors conversationStateAccessors,
            IUserBotRepository userRepository)
            : base(UserRegistrationDialog.DialogId)
        {
            this.conversationStateAccessors = conversationStateAccessors;
            this.userRepository = userRepository;

            this.userFaker = new Faker();

            WaterfallStep[] steps =
            {
                this.AskGender,
                this.ApplyGender,
                this.SuggestName,
                this.AskAboutFamily,
                this.AskAboutConversationTroubles,
                this.Register,
            };

            this.AddDialog(new WaterfallDialog(WaterfallDialogId, steps));

            this.AddDialog(new ChoicePrompt(ChoicePromptId));

            this.AddDialog(new ConfirmPrompt(ConfirmPromptId));
        }

        private async Task<DialogTurnResult> AskGender(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                ChoicePromptId,
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text("Пожалуйста, укажите ваш пол"),
                    RetryPrompt = MessageFactory.Text("Извините, не могу распознать Ваш ответ"),
                    Choices = ChoiceFactory.ToChoices(new List<string> {"Мужчина", "Женщина"}),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ApplyGender(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RegistrationState registrationState = await this.conversationStateAccessors
                .RegistrationStateAccessor
                .GetAsync(
                    stepContext.Context,
                    () => new RegistrationState(),
                    cancellationToken);
            registrationState.Gender = ((FoundChoice)stepContext.Result).Value == "Мужчина" ? Gender.Male : Gender.Female;

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> SuggestName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RegistrationState registrationState = await this.conversationStateAccessors
                .RegistrationStateAccessor
                .GetAsync(
                    stepContext.Context,
                    () => new RegistrationState(),
                    cancellationToken);

            registrationState.Name = this.userFaker.Name.FullName(registrationState.Gender == Gender.Male ? Name.Gender.Male : Name.Gender.Female);

            await stepContext.Context.SendActivityAsync(
                $"Для общения с психологом Вам присвоен псевдоним: {registrationState.Name}",
                cancellationToken: cancellationToken);

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> AskAboutFamily(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(
                ConfirmPromptId,
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text("У вас есть семья?"),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> AskAboutConversationTroubles(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RegistrationState registrationState = await this.conversationStateAccessors
                .RegistrationStateAccessor
                .GetAsync(
                    stepContext.Context,
                    () => new RegistrationState(),
                    cancellationToken);

            registrationState.HasFamily = (bool)stepContext.Result;

            return await stepContext.PromptAsync(
                ConfirmPromptId,
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text("У вас есть проблемы в общении?"),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> Register(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RegistrationState registrationState = await this.conversationStateAccessors
                .RegistrationStateAccessor
                .GetAsync(
                    stepContext.Context,
                    () => new RegistrationState(),
                    cancellationToken);

            registrationState.HasConversationTroubles = (bool)stepContext.Result;

            User user = new User(stepContext.Context.Activity.From.Id, stepContext.Context.Activity.GetConversationReference())
            {
                Name = registrationState.Name,
                Gender = registrationState.Gender,
                HasFamily = registrationState.HasFamily,
                HasConversationTroubles = registrationState.HasConversationTroubles,
            };
            this.userRepository.AddUser(user);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}