namespace PsychologyBot.Core.Models
{
    public class Message
    {
        public Message(string messageString, bool isUserMessage)
        {
            this.MessageString = messageString;
            this.IsUserMessage = isUserMessage;
        }

        public string MessageString { get; }

        public bool IsUserMessage { get; }
    }
}