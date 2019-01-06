namespace PsychologyBot.Core.Models
{
    public class Message
    {
        public string MessageString { get; }

        public bool IsUserMessage { get; }

        public Message(string messageString, bool isUserMessage)
        {
            MessageString = messageString;
            IsUserMessage = isUserMessage;
        }
    }
}