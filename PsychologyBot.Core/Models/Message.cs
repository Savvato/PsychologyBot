namespace PsychologyBot.Core.Models
{
    using System;

    public class Message
    {
        public Message(string messageString, bool isUserMessage)
        {
            this.MessageString = messageString;
            this.IsUserMessage = isUserMessage;
            this.Date = DateTime.Now;
        }

        public string MessageString { get; }

        public bool IsUserMessage { get; }

        public DateTime Date { get; }
    }
}