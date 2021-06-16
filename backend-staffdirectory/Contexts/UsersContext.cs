using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using backend_staffdirectory.Entities;

#nullable disable

namespace backend_staffdirectory.Contexts {
    public partial class UsersContext : DbContext {
        private readonly IConfiguration _config;

        public UsersContext() {

        } 

        public UsersContext(DbContextOptions<UsersContext> options, IConfiguration config) : base(options) {
            _config = config;
        }

        public virtual DbSet<User> Users { get; set; } 
        public virtual DbSet<UserInfo> UsersInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                var connectionString = _config["ConnectionString"];
                optionsBuilder.UseMySql(connectionString, ServerVersion.Parse("8.0.25-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            modelBuilder.Entity<User>(entity => {
                entity.ToTable("users");

                entity.HasKey(k => k.Id).HasName("pk_user_id");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseMySqlIdentityColumn();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'0'");

                entity.HasOne(f => f.UserInfo)
                    .WithOne(f => f.User)
                    .HasForeignKey<UserInfo>(fd => fd.UserId);
            });

            modelBuilder.Entity<UserInfo>(entity => {
                entity.ToTable("usersinfo");

                entity.HasKey(f => f.Id).HasName("pk_userinfo_userid");

                entity.Property(e => e.Id)
                    .IsRequired()
                    .UseMySqlIdentityColumn();

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Supervisor)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'0'");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
