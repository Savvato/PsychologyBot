using System.Collections.Generic;

using PsychologyBot.Core.Models;

namespace PsychologyBot.Core.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers(CancellationToken cancellationToken = default);

        Task<User> GetUserById(string id, CancellationToken cancellationToken = default);

        Task SaveChanges(CancellationToken cancellationToken = default);
    }
}