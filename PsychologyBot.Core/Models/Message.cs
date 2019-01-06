namespace PsychologyBot.Core.Models
{
    public class Message
    {
        public Message(string messageString, bool isUserMessage)
        {
            MessageString = messageString;
            IsUserMessage = isUserMessage;
        }

        public string MessageString { get; }

        public bool IsUserMessage { get; }
    }
}