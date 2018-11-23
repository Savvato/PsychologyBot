namespace PsychologyBot.Models
{
    using System.Collections.Generic;

    using Microsoft.Bot.Schema;

    public class User
    {
        public string Id { get; }

        public string Name { get; set; }

        public Gender Gender { get; set; }

        public bool HasFamily { get; set; }

        public bool HasConversationTroubles { get; set; }

        public ConversationReference ConversationReference { get; }

        public List<Message> Messages { get; }

        public User(string id, ConversationReference conversationReference)
        {
            this.Id = id;
            this.ConversationReference = conversationReference;
            this.Messages = new List<Message>();
        }
    }
}