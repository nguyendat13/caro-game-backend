using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    public class CaroDbContext : DbContext
    {
        public CaroDbContext(DbContextOptions<CaroDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameMove> GameMoves { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<ProfileUpdateOtp> ProfileUpdateOtps { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<CommunityEvent> CommunityEvents { get; set; }
        public DbSet<ClanRecruit> ClanRecruits { get; set; }
        public DbSet<ChatChannel> ChatChannels { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<ChannelMember> ChannelMembers { get; set; }
        public DbSet<VoiceChannel> VoiceChannels { get; set; }
        public DbSet<VoiceParticipant> VoiceParticipants { get; set; }
        public DbSet<VoiceSettings> VoiceSettings { get; set; }
        public DbSet<MessageReaction> MessageReactions { get; set; }
        public DbSet<EventFeature> EventFeatures { get; set; }
        public DbSet<DeleteAccountOtp> DeleteAccountOtps { get; set; }
        public DbSet<ChannelInvite> ChannelInvites { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Sử dụng TPT (Table-per-Type) mapping
            modelBuilder.Entity<Tournament>().ToTable("Tournaments");
            modelBuilder.Entity<CommunityEvent>().ToTable("CommunityEvents");
            modelBuilder.Entity<ClanRecruit>().ToTable("ClanRecruits");
            modelBuilder.Entity<ChatChannel>().ToTable("ChatChannels");
            modelBuilder.Entity<Article>().ToTable("Articles");
            modelBuilder.Entity<Announcement>().ToTable("Announcements");

            // Lưu enum dưới dạng string
            modelBuilder.Entity<Event>()
                .Property(e => e.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Article>()
                .Property(a => a.Category)
                .HasConversion<string>();


            modelBuilder.Entity<Role>().HasData(
        new Role { RoleId = 1, RoleName = "superadmin" },
        new Role { RoleId = 2, RoleName = "admin" },
        new Role { RoleId = 3, RoleName = "user" }
    );
            modelBuilder.Entity<Event>().HasData(
    new Event { EventId = 1, Title = "Giải Đấu", Description = "Tổ chức giải đấu chuyên nghiệp với bảng xếp hạng tự động", Type = EventType.Tournament, CreatedAt = DateTime.Parse("2025-11-23T07:06:21.4173744") },
    new Event { EventId = 2, Title = "Kênh Chat Mới", Description = "Tạo phòng chat riêng cho nhóm, clan hoặc sự kiện", Type = EventType.ChatChannel, CreatedAt = DateTime.Parse("2025-11-23T07:09:27.7487092") },
    new Event { EventId = 3, Title = "Sự Kiện Cộng Đồng", Description = "Tổ chức buổi livestream, workshop, AMA...", Type = EventType.CommunityEvent, CreatedAt = DateTime.Parse("2025-11-23T07:10:05.3026004") },
    new Event { EventId = 4, Title = "Tuyển Thành Viên Clan", Description = "Tìm đồng đội, thành lập team thi đấu", Type = EventType.ClanRecruit, CreatedAt = DateTime.Parse("2025-11-23T07:10:42.5803229") },
    new Event { EventId = 5, Title = "Thông Báo Lớn", Description = "Gửi thông báo quan trọng đến toàn server", Type = EventType.Announcement, CreatedAt = DateTime.Parse("2025-11-23T07:11:22.2680185") },
    new Event { EventId = 6, Title = "Đăng Bài Viết", Description = "Viết bài hướng dẫn, chia sẻ chiến lược, câu chuyện cá nhân", Type = EventType.Article, CreatedAt = DateTime.Parse("2025-11-23T07:12:03.836081") }
);
            modelBuilder.Entity<ChannelInvite>()
             .HasOne(ci => ci.User)          // Người được mời
             .WithMany(u => u.ChannelInvites)
             .HasForeignKey(ci => ci.UserId)
             .OnDelete(DeleteBehavior.Restrict); // Không cascade

            modelBuilder.Entity<ChannelInvite>()
                .HasOne(ci => ci.InvitedBy)     // Người gửi lời mời
                .WithMany(u => u.SentChannelInvites)
                .HasForeignKey(ci => ci.InvitedById)
                .OnDelete(DeleteBehavior.Restrict); // Không cascade


            // Map từng bảng con với Event (1-1)
            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Event)
                .WithOne()
                .HasForeignKey<Tournament>(t => t.EventRefId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommunityEvent>()
                .HasOne(c => c.Event)
                .WithOne()
                .HasForeignKey<CommunityEvent>(c => c.EventRefId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClanRecruit>()
                .HasOne(c => c.Event)
                .WithOne()
                .HasForeignKey<ClanRecruit>(c => c.EventRefId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatChannel>()
                .HasOne(c => c.Event)
                .WithOne()
                .HasForeignKey<ChatChannel>(c => c.EventRefId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Event)
                .WithOne()
                .HasForeignKey<Article>(a => a.EventRefId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Announcement>()
                .HasOne(a => a.Event)
                .WithOne()
                .HasForeignKey<Announcement>(a => a.EventRefId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VoiceSettings>(entity =>
            {
                entity.HasKey(vs => vs.UserId);

                entity.HasOne(vs => vs.User)
                    .WithOne(u => u.VoiceSettings)
                    .HasForeignKey<VoiceSettings>(vs => vs.UserId);

                entity.Property(vs => vs.InputVolume)
                    .HasPrecision(5, 2); // vd: 100.00

                entity.Property(vs => vs.OutputVolume)
                    .HasPrecision(5, 2);

                entity.Property(vs => vs.VoiceActivationThreshold)
                    .HasPrecision(6, 2); // ví dụ: -40.00
            });


            modelBuilder.Entity<VoiceParticipant>()
            .HasKey(vp => new { vp.UserId, vp.VoiceChannelId });

            modelBuilder.Entity<VoiceParticipant>()
                .HasOne(vp => vp.User)
                .WithMany(u => u.VoiceParticipants)
                .HasForeignKey(vp => vp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VoiceParticipant>()
                .HasOne(vp => vp.VoiceChannel)
                .WithMany(vc => vc.Participants)
                .HasForeignKey(vp => vp.VoiceChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageReaction>()
                .HasKey(mr => new { mr.MessageId, mr.UserId, mr.Emoji });

            modelBuilder.Entity<MessageReaction>()
                .HasOne(mr => mr.Message)
                .WithMany(m => m.Reactions)
                .HasForeignKey(mr => mr.MessageId);

            modelBuilder.Entity<MessageReaction>()
                .HasOne(mr => mr.User)
                .WithMany(u => u.MessageReactions)
                .HasForeignKey(mr => mr.UserId);

            modelBuilder.Entity<ChannelMember>()
            .HasKey(cm => new { cm.UserId, cm.ChannelId });

            modelBuilder.Entity<ChannelMember>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ChannelMembers)
                .HasForeignKey(cm => cm.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChannelMember>()
                .HasOne(cm => cm.Channel)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.ChannelId);


            modelBuilder.Entity<ChatChannel>()
              .HasOne(c => c.Creator)
                .WithMany(u => u.CreatedChannels)
              .HasForeignKey(c => c.CreatorId)
              .OnDelete(DeleteBehavior.Restrict); // tránh multiple cascade


            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Game>()
                .HasOne(g => g.PlayerX)
                .WithMany(u => u.GamesAsX)
                .HasForeignKey(g => g.PlayerXId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.PlayerO)
                .WithMany(u => u.GamesAsO)
                .HasForeignKey(g => g.PlayerOId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GameMove>()
                .HasOne(m => m.Game)
                    .WithMany(g => g.Moves)
                .HasForeignKey(m => m.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameMove>()
                .HasOne(m => m.Player)
                .WithMany(u => u.Moves)
                .HasForeignKey(m => m.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(c => c.Game)
                .WithMany(g => g.Messages)
                .HasForeignKey(c => c.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(c => c.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Connection>()
                .HasOne(c => c.User)
                .WithMany(u => u.Connections)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
