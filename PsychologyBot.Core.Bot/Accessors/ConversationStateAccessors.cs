using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

using PsychologyBot.Core.Bot.States;

namespace PsychologyBot.Core.Bot.Accessors
{
    public class ConversationStateAccessors
    {
        public ConversationStateAccessors(IPropertyManager conversationState)
        {
            this.DialogStateAccessor = conversationState.CreateProperty<DialogState>(nameof(DialogState));
            this.RegistrationStateAccessor =
                conversationState.CreateProperty<RegistrationState>(nameof(RegistrationState));
        }

        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; }

        public IStatePropertyAccessor<RegistrationState> RegistrationStateAccessor { get; }
    }
}