using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Infrastructure.Constants;
using CleanArchitecture.Blazor.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;

namespace CleanArchitecture.Blazor.Infrastructure.Hubs
{
    public class HubClient : IAsyncDisposable
    {
        private HubConnection hubConnection;
        private string hubUrl;
        private string userId;
        private readonly NavigationManager navigationManager;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private bool started = false;

        public HubClient(
            NavigationManager navigationManager,
            AuthenticationStateProvider authenticationStateProvider)
        {
            this.navigationManager = navigationManager;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task StartAsync()
        {
            this.hubUrl = navigationManager.BaseUri.TrimEnd('/') + SignalR.HubUrl;
            AuthenticationState? state = await authenticationStateProvider.GetAuthenticationStateAsync();
            userId = state.User.GetUserId();
            if (!started)
            {
                hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl)
                    .Build();

                hubConnection.On<string>(SignalR.ConnectUser,
                (userId) =>
                {
                    LoggedIn?.Invoke(this, userId);
                });

                hubConnection.On<string>(SignalR.DisconnectUser, (userId) =>
                {
                    LoggedOut?.Invoke(this, userId);
                });

                hubConnection.On<string>(SignalR.SendNotification, (message) =>
                {
                    NotificationReceived?.Invoke(this, message);
                });

                hubConnection.On<string, string>(SignalR.SendMessage, (userId, message) =>
                {
                    HandleReceiveMessage(userId, message);
                });

                // 开启
                await hubConnection.StartAsync();

                await hubConnection.SendAsync(SignalR.ConnectUser, userId);
                started = true;
            }

        }

        private void HandleReceiveMessage(string userId, string message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(userId, message));
        }
        public async Task StopAsync()
        {
            if (started)
            {
                await hubConnection.StopAsync();
                await hubConnection.DisposeAsync();
                hubConnection = null;
                started = false;
            }
        }
        public async Task SendAsync(string message)
        {
            if (!started)
                throw new InvalidOperationException("Client not started");
            await hubConnection.SendAsync(SignalR.SendMessage, userId, message);
        }
        public async Task NotifyAsync(string message)
        {
            if (!started)
                throw new InvalidOperationException("Client not started");
            await hubConnection.SendAsync(SignalR.SendNotification, message);
        }
        public async ValueTask DisposeAsync()
        {
            await StopAsync();
        }

        public event EventHandler<string>? LoggedIn;
        public event EventHandler<string>? LoggedOut;
        public event EventHandler<string>? NotificationReceived;
        public event MessageReceivedEventHandler? MessageReceived;
        public delegate Task MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

        public class MessageReceivedEventArgs : EventArgs
        {
            public MessageReceivedEventArgs(string userId, string message)
            {
                UserId = userId;
                Message = message;
            }

            public string UserId { get; set; }
            public string Message { get; set; }
        }
    }
}
