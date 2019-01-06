using PsychologyBot.Core.Models;

namespace PsychologyBot.ViewModels
{
    using System.Collections.Generic;

    public class UserViewModel
    {
        public List<User> AllUsers { get; set; }

        public User SelectedUser { get; set; }
    }
}