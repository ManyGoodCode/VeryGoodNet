using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Infrastructure.Constants;
using Microsoft.AspNetCore.SignalR;


namespace CleanArchitecture.Blazor.Infrastructure.Hubs
{

    public class SignalRHub : Hub
    {

        private static readonly ConcurrentDictionary<string, string> onlineUsers = new();

        public SignalRHub()
        {

        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string id = Context.ConnectionId;
            if (onlineUsers.TryRemove(id, out string? userId))
            {
                await Clients.All.SendAsync(SignalR.DisconnectUser, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task OnConnectUser(string userId)
        {
            string id = Context.ConnectionId;
            if (!onlineUsers.ContainsKey(id))
            {
                if (onlineUsers.TryAdd(id, userId))
                {
                    // re-use existing message for now
                    await Clients.All.SendAsync(SignalR.ConnectUser, userId);
                }
            }
        }
        public async Task SendMessage(string userId, string message)
        {
            await Clients.All.SendAsync(SignalR.SendMessage, userId, message);
        }
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync(SignalR.SendNotification, message);
        }
    }
}
