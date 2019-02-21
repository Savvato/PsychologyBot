using System.Collections.Generic;

using PsychologyBot.Core.Models;

namespace PsychologyBot.Core.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAllUsers();

        User GetUserById(string id);
    }
}