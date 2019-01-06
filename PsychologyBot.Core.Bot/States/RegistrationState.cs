using PsychologyBot.Core.Models;

namespace PsychologyBot.Core.Bot.States
{
    public class RegistrationState
    {
        public string Name { get; set; }

        public Gender Gender { get; set; }

        public bool HasFamily { get; set; }

        public bool HasConversationTroubles { get; set; }
    }
}