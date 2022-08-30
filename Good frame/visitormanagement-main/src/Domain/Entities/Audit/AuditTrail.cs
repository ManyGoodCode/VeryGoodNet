using System;
using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Blazor.Domain.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CleanArchitecture.Blazor.Domain.Entities.Audit
{
    public class AuditTrail : IEntity
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public AuditType AuditType { get; set; }
        public string? TableName { get; set; }
        public DateTime DateTime { get; set; }
        public Dictionary<string, object>? OldValues { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object>? NewValues { get; set; } = new Dictionary<string, object> ();
        public ICollection<string>? AffectedColumns { get; set; }
        public Dictionary<string, object> PrimaryKey { get; set; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();
        public bool HasTemporaryProperties => TemporaryProperties.Any();
    }
}
