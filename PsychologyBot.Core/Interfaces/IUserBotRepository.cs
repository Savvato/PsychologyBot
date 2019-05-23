using Microsoft.Bot.Builder;

using PsychologyBot.Core.Models;

namespace PsychologyBot.Core.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUserBotRepository
    {
        Task<User> GetCurrentUser(ITurnContext turnContext, CancellationToken cancellationToken = default);

        Task<bool> IsUserExists(ITurnContext turnContext, CancellationToken cancellationToken = default);

        Task SaveChanges(CancellationToken cancellationToken = default);

        Task AddUser(User user, CancellationToken cancellationToken = default);
    }
}