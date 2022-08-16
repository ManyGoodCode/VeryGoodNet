using CleanArchitecture.Blazor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public class CheckinPointConfiguration : IEntityTypeConfiguration<CheckinPoint>
    {
        public void Configure(EntityTypeBuilder<CheckinPoint> builder)
        {
            builder.Ignore(e => e.DomainEvents);
            builder.HasMany(a => a.Devices)
                   .WithOne(b => b.CheckinPoint)
                   .HasForeignKey(b => b.CheckinPointId);
        }
    }
}
