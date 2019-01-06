using System.Collections.Generic;
using Microsoft.Bot.Schema;

namespace PsychologyBot.Core.Models
{
    public class User
    {
        public User(string id, ConversationReference conversationReference)
        {
            Id = id;
            ConversationReference = conversationReference;
            Messages = new List<Message>();
        }

        public string Id { get; }

        public string Name { get; set; }

        public Gender Gender { get; set; }

        public bool HasFamily { get; set; }

        public bool HasConversationTroubles { get; set; }

        public ConversationReference ConversationReference { get; }

        public List<Message> Messages { get; }
    }
}