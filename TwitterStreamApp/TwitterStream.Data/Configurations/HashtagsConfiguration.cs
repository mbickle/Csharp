using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TwitterStream.Data.Models;

namespace TwitterStream.Data.Configurations
{
    public partial class HashtagsConfiguration : IEntityTypeConfiguration<Hashtag>
    {
        public void Configure(EntityTypeBuilder<Hashtag> entity)
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.TweetId).IsRequired();
            entity.Property(e => e.Content).HasMaxLength(int.MaxValue);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Hashtag> enity);
    }
}
