using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SeleniumDotNet
{
    public abstract class EntityTypeConfigurationBase<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : class
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // This configuration applies to ALL entities.
            builder.Property<DateTime>("_Created")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();
            
            this.FurtherConfiguration(builder);
        }

        public abstract void FurtherConfiguration(EntityTypeBuilder<TEntity> builder);
    }
}
