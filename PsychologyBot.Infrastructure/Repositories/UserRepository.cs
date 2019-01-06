using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder;
using PsychologyBot.Core.Interfaces;
using PsychologyBot.Core.Models;

namespace PsychologyBot.Infrastructure.Repositories
{
    public class UserRepository : IUserBotRepository, IUserRepository
    {
        private readonly List<User> users;

        public UserRepository()
        {
            users = new List<User>();
        }

        public User GetCurrentUser(ITurnContext turnContext)
        {
            return users.FirstOrDefault(user => user.Id == turnContext.Activity.From.Id);
        }

        public bool IsUserExists(ITurnContext turnContext)
        {
            return users.Exists(user => user.Id == turnContext.Activity.From.Id);
        }

        public void AddUser(User user)
        {
            users.Add(user);
        }

        public List<User> GetAllUsers()
        {
            return users;
        }

        public User GetUserById(string id)
        {
            return users.FirstOrDefault(user => user.Id == id);
        }
    }
}