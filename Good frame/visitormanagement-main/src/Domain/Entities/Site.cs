using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Entities
{
    /// <summary>
    /// 场地。名称 / 公司名称 / 地址 / 等级点 / 事件集合
    /// </summary>
    public class Site : AuditableEntity, IHasDomainEvent, IAuditTrial
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Address { get; set; }
        public virtual ICollection<CheckinPoint> CheckinPoints { get; set; } = new HashSet<CheckinPoint>();

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}
