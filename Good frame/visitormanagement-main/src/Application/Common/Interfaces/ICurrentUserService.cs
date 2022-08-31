// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{

    public interface ICurrentUserService
    {
        Task<string> UserId();
        Task<string> UserName();
        Task<int?> SiteId();
        Task<string> SiteName();
    }
}
