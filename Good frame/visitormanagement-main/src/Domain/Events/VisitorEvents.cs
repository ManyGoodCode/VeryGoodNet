using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Domain.Events
{
    public class VisitorCreatedEvent : DomainEvent
    {
        public VisitorCreatedEvent(Visitor item)
        {
            Item = item;
        }
        public Visitor Item { get; }
    }
}

public class VisitorUpdatedEvent : DomainEvent
{
    public VisitorUpdatedEvent(Visitor item)
    {
        Item = item;
    }
    public Visitor Item { get; }
}
public class VisitorDeletedEvent : DomainEvent
{
    public VisitorDeletedEvent(Visitor item)
    {
        Item = item;
    }
    public Visitor Item { get; }
}