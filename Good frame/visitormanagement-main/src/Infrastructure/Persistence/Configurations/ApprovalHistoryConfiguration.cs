using CleanArchitecture.Blazor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public class ApprovalHistoryConfiguration : IEntityTypeConfiguration<ApprovalHistory>
    {
        public void Configure(EntityTypeBuilder<ApprovalHistory> builder)
        {
            builder.Ignore(e => e.DomainEvents);
        }
    }
}
