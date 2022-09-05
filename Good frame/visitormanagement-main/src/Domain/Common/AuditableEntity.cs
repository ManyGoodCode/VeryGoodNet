using System;

namespace CleanArchitecture.Blazor.Domain.Common
{
    public interface IEntity
    {
    }

    /// <summary>
    /// 可审计的 
    /// </summary>
    public abstract class AuditableEntity : IEntity
    {
        public DateTime? Created { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string? LastModifiedBy { get; set; }
    }

    public interface ISoftDelete
    {
        DateTime? Deleted { get; set; }
        string? DeletedBy { get; set; }
    }

    public abstract class AuditableSoftDeleteEntity : AuditableEntity, ISoftDelete
    {
        public DateTime? Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
