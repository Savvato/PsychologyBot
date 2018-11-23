namespace PsychologyBot.Bot.Accessors
{
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;

    using PsychologyBot.Bot.States;

    public class ConversationStateAccessors
    {
        public ConversationStateAccessors(IPropertyManager conversationState)
        {
            this.DialogStateAccessor = conversationState.CreateProperty<DialogState>(nameof(DialogState));
            this.RegistrationStateAccessor = conversationState.CreateProperty<RegistrationState>(nameof(RegistrationState));
            this.MessageStateAccessor = conversationState.CreateProperty<MessageState>(nameof(MessageState));
        }

        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; }

        public IStatePropertyAccessor<RegistrationState> RegistrationStateAccessor { get; }

        public IStatePropertyAccessor<MessageState> MessageStateAccessor { get; }
    }
}