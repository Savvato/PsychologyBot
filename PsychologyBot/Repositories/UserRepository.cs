namespace PsychologyBot.Repositories
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Bot.Builder;

    using PsychologyBot.Models;

    public interface IUserBotRepository
    {
        User GetCurrentUser(ITurnContext turnContext);

        bool IsUserExists(ITurnContext turnContext);

        void AddUser(User user);
    }

    public interface IUserRepository
    {
        List<User> GetAllUsers();

        User GetUserById(string id);
    }

    public class UserRepository : IUserBotRepository, IUserRepository
    {
        private List<User> users;

        public UserRepository()
        {
            this.users = new List<User>();
        }

        public User GetCurrentUser(ITurnContext turnContext)
        {
            return this.users.FirstOrDefault(user => user.Id == turnContext.Activity.From.Id);
        }

        public List<User> GetAllUsers()
        {
            return this.users;
        }

        public User GetUserById(string id)
        {
            return this.users.FirstOrDefault(user => user.Id == id);
        }

        public bool IsUserExists(ITurnContext turnContext)
        {
            return this.users.Exists(user => user.Id == turnContext.Activity.From.Id);
        }

        public void AddUser(User user)
        {
            this.users.Add(user);
        }
    }
}