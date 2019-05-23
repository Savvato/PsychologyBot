using System.Collections.Generic;

using Microsoft.Bot.Builder;

using PsychologyBot.Core.Interfaces;
using PsychologyBot.Core.Models;

namespace PsychologyBot.Infrastructure.Repositories
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PsychologyBot.Infrastructure.Db;

    public class UserRepository : IUserBotRepository, IUserRepository
    {
        private readonly PsyDbContext dbContext;

        public UserRepository(PsyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<User> GetCurrentUser(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            return this.dbContext
                .Users
                .Include(u => u.Messages)
                .FirstOrDefaultAsync(
                    user => user.ChannelId == turnContext.Activity.From.Id, 
                    cancellationToken);
        }

        public async Task<bool> IsUserExists(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            return await this.dbContext.Users.AnyAsync(user => user.ChannelId == turnContext.Activity.From.Id, cancellationToken);
        }

        public async Task AddUser(User user, CancellationToken cancellationToken = default)
        {
            await this.dbContext.Users.AddAsync(user, cancellationToken);
            await this.dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<User>> GetAllUsers(CancellationToken cancellationToken = default)
        {
            return await this.dbContext
                .Users
                .Include(u => u.Messages)
                .ToListAsync(cancellationToken);
        }

        public async Task<User> GetUserById(string id, CancellationToken cancellationToken = default)
        {
            return await this.dbContext
                .Users
                .Include(u => u.Messages)
                .FirstOrDefaultAsync(
                    user => user.ChannelId == id, 
                    cancellationToken);
        }

        public async Task SaveChanges(CancellationToken cancellationToken = default)
        {
            await this.dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}