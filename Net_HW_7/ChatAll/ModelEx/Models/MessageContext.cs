using Microsoft.EntityFrameworkCore;

namespace ModelsEx.Models
{
    public class MessageContext : DbContext
    {
        public MessageContext() { }
        public MessageContext(DbContextOptions options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = true;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("message_pkey");
                entity.ToTable("Messages");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Text).HasColumnName("text");
                entity.Property(e => e.FromUserId).HasColumnName("from_user_id");
                entity.Property(e => e.ToUserId).HasColumnName("to_user_id");

                entity.HasOne(d => d.FromUser).WithMany(p => p.FromMessages)
                    .HasForeignKey(d => d.FromUserId)
                    .HasConstraintName("messages_from_user_id_fkey");

                entity.HasOne(d => d.ToUser).WithMany(p => p.ToMessage)
                    .HasForeignKey(d => d.ToUserId)
                    .HasConstraintName("messages_to_user_id_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("users_pkey");

                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

            });

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine).UseLazyLoadingProxies().UseSqlServer
            ("Server=(localdb)\\MSSQLLocalDB;Database=Chat;Trusted_Connection=True");

            //base.OnConfiguring(optionsBuilder);
        }
    }
}
