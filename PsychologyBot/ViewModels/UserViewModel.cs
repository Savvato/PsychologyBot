namespace PsychologyBot.ViewModels
{
    using System.Collections.Generic;

    using PsychologyBot.Models;

    public class UserViewModel
    {
        public List<User> AllUsers { get; set; }
        public User SelectedUser { get; set; }
    }
}