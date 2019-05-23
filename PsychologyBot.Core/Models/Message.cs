namespace PsychologyBot.Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Newtonsoft.Json;

    public class Message
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, Column(TypeName = "text")]
        public string MessageString { get; set; }

        [Required]
        public bool IsUserMessage { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [JsonIgnore, ForeignKey(nameof(Message.UserId))]
        public User User { get; set; }
    }
}