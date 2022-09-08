using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Entities.Audit;
using CleanArchitecture.Blazor.Domain.Entities.Log;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{

    public interface IApplicationDbContext
    {
        DbSet<Logger> Loggers { get; set; }
        DbSet<AuditTrail> AuditTrails { get; set; }
        DbSet<DocumentType> DocumentTypes { get; set; }
        DbSet<Document> Documents { get; set; }
        DbSet<KeyValue> KeyValues { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<Department> Departments { get; set; }
        DbSet<Designation> Designations { get; set; }
        DbSet<Employee> Employees { get; set; }
        DbSet<Visitor> Visitors { get; set; }
        DbSet<VisitorHistory> VisitorHistories { get; set; }
        DbSet<Site> Sites { get; set; }
        DbSet<CheckinPoint> CheckinPoints { get; set; }
        DbSet<Device> Devices { get; set; }
        DbSet<Companion> Companions { get; set; }
        DbSet<ApprovalHistory> ApprovalHistories { get; set; }
        DbSet<SiteConfiguration> SiteConfigurations { get; set; }
        DbSet<MessageTemplate> MessageTemplates { get; set; }
        Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker ChangeTracker { get; }
        Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken);
    }
}
