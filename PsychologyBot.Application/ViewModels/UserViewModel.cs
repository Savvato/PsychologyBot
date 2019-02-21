namespace PsychologyBot.Application.ViewModels
{
    using System.Collections.Generic;
    using PsychologyBot.Core.Models;

    public class UserViewModel
    {
        public List<User> AllUsers { get; set; }

        public User SelectedUser { get; set; }
    }
}