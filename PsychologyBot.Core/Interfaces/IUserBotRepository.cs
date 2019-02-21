using Microsoft.Bot.Builder;

using PsychologyBot.Core.Models;

namespace PsychologyBot.Core.Interfaces
{
    public interface IUserBotRepository
    {
        User GetCurrentUser(ITurnContext turnContext);

        bool IsUserExists(ITurnContext turnContext);

        void AddUser(User user);
    }
}