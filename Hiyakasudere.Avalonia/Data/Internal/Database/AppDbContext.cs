using Microsoft.EntityFrameworkCore;

namespace Hiyakasudere.Data.Internal.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<FavoritePost> Favorites { get; set; }
        public DbSet<SearchHistoryEntry> SearchHistory { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FavoritePost>(entity =>
            {
                entity.HasIndex(e => new { e.SourcePostId, e.SourceId }).IsUnique();
            });

            modelBuilder.Entity<SearchHistoryEntry>(entity =>
            {
                entity.HasIndex(e => e.SearchedAt);
            });
        }
    }
}
