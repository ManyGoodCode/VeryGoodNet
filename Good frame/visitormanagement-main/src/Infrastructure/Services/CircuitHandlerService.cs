using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleanArchitecture.Blazor.Infrastructure.Services
{
    public class CircuitHandlerService : CircuitHandler
    {

        private readonly IUsersStateContainer usersStateContainer;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private string circuitId = string.Empty;

        public CircuitHandlerService(
            IUsersStateContainer usersStateContainer,
            AuthenticationStateProvider authenticationStateProvider)
        {
            this.usersStateContainer = usersStateContainer;
            this.authenticationStateProvider = authenticationStateProvider;
            this.authenticationStateProvider.AuthenticationStateChanged += AuthenticationStateProvider_AuthenticationStateChanged;
        }
        ~CircuitHandlerService()
        {
            authenticationStateProvider.AuthenticationStateChanged -= AuthenticationStateProvider_AuthenticationStateChanged;
        }
        public override async Task OnConnectionUpAsync(Circuit circuit,
            CancellationToken cancellationToken)
        {
            this.circuitId = circuit.Id;
            AuthenticationState state = await authenticationStateProvider.GetAuthenticationStateAsync();
            usersStateContainer.Update(circuit.Id, state.User.Identity?.Name);
        }

        public override Task OnConnectionDownAsync(Circuit circuit,
            CancellationToken cancellationToken)
        {
            usersStateContainer.Remove(circuit.Id);
            return Task.CompletedTask;
        }
        private async void AuthenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> state)
        {
            usersStateContainer.Update(circuitId, (await state).User.Identity?.Name);
        }
    }
}