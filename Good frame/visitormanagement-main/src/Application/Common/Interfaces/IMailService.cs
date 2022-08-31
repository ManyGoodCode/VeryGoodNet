// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Settings;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{

    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}
