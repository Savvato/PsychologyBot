namespace PsychologyBot.Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Newtonsoft.Json;

    public class Note
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, Column(TypeName = "text")]
        public string NoteString { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [JsonIgnore, ForeignKey(nameof(Note.UserId))]
        public User User { get; set; }
    }
}