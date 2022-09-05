using System;
using CleanArchitecture.Blazor.Application.Common.Interfaces;

namespace CleanArchitecture.Blazor.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
