using Microsoft.EntityFrameworkCore;
using TwitterStream.Data.Models;

namespace TwitterStream.Data
{
    public partial class TwitterStreamDbContext: DbContext
    {
        public TwitterStreamDbContext()
        {
        }

        public TwitterStreamDbContext(DbContextOptions<TwitterStreamDbContext> options) : base(options) 
        { 
        }

        public virtual DbSet<Tweet> Tweets { get; set; }
        public virtual DbSet<Hashtag> Hashtags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.TweetsConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.HashtagsConfiguration());
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
