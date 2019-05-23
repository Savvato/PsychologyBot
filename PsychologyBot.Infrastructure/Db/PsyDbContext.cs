namespace PsychologyBot.Infrastructure.Db
{
    using Microsoft.Bot.Schema;
    using Microsoft.EntityFrameworkCore;

    using Npgsql;

    using PsychologyBot.Core.Models;

    public class PsyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Message> Messages { get; set; }

        static PsyDbContext()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Gender>();
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet(new[] { typeof(ConversationReference) });
        }

        public PsyDbContext(DbContextOptions<PsyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ForNpgsqlHasEnum<Gender>();

            modelBuilder.Entity<User>()
                .Property("_conversationReference")
                .HasColumnType("jsonb");
        }
    }
}
