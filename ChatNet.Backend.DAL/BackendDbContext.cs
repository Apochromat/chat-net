using ChatNet.Backend.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatNet.Backend.DAL; 

public class BackendDbContext: DbContext {
    public DbSet<Chat> Chats { get; set; }
    public DbSet<FriendShipRequest>FriendShipRequests { get; set; }
    public DbSet<UserBackend> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<PrivateChat> PrivateChats { get; set; }
    public DbSet<GroupChat> GroupChats { get; set; }
    public DbSet<NotificationPreferences> NotificationPreferences { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupChat>()
            .HasMany(g => g.Administrators)
            .WithMany()
            .UsingEntity(j => j.ToTable("GroupChatAdministrators"));

        base.OnModelCreating(modelBuilder);
    }
    
    public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options) {
    }
}