using System.Collections.Generic;

using Microsoft.Bot.Schema;

namespace PsychologyBot.Core.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Newtonsoft.Json;

    public class User
    {
        private string _conversationReference;

        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, Column(TypeName = "varchar(40)")]
        public string ChannelId { get; set; }

        [Required, Column(TypeName = "varchar(255)")]
        public string Name { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [JsonIgnore, NotMapped]
        public ConversationReference ConversationReference
        {
            get => JsonConvert.DeserializeObject<ConversationReference>(this._conversationReference);
            set => this._conversationReference = JsonConvert.SerializeObject(value);
        }

        public List<Message> Messages { get; set; }
    }
}