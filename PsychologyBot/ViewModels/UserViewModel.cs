using System.Collections.Generic;
using PsychologyBot.Core.Models;

namespace PsychologyBot.ViewModels
{
    public class UserViewModel
    {
        public List<User> AllUsers { get; set; }

        public User SelectedUser { get; set; }
    }
}