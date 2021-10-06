using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SeleniumDotNet
{
    public class ReviewUrlEntityTypeConfiguration : EntityTypeConfigurationBase<ReviewUrlDto>
    {
        public override void FurtherConfiguration(EntityTypeBuilder<ReviewUrlDto> builder)
        {
            builder.ToTable("ReviewUrl");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Value).IsRequired().HasMaxLength(1000);
        }
    }
}