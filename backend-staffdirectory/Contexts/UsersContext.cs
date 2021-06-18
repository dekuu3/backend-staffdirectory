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

                entity.HasIndex(e => e.Username)
                    .IsUnique();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'User'");

                entity.Property(e => e.Email)
                   .IsRequired()
                   .HasMaxLength(50);

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.Supervisor)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
