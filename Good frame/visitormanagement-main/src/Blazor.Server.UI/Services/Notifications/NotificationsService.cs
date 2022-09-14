using Blazored.LocalStorage;

namespace Blazor.Server.UI.Services.Notifications;

public class InMemoryNotificationService : INotificationService
{
    private const string LocalStorageKey = "__notficationTimestamp";
    private readonly ILocalStorageService localStorageService;
    private readonly ILogger<InMemoryNotificationService> logger;

    private readonly List<NotificationMessage> messages;

    public InMemoryNotificationService(
        ILocalStorageService localStorageService,
        ILogger<InMemoryNotificationService> logger)
    {
        this.localStorageService = localStorageService;
        this.logger = logger;
        this.messages = new List<NotificationMessage>();
    }

    private async Task<DateTime> GetLastReadTimestamp()
    {
        if (await localStorageService.ContainKeyAsync(LocalStorageKey) == false)
        {
            return DateTime.MinValue;
        }
        else
        {
            DateTime timestamp = await localStorageService.GetItemAsync<DateTime>(LocalStorageKey);
            return timestamp;
        }
    }

    public async Task<bool> AreNewNotificationsAvailable()
    {
        DateTime timestamp = await GetLastReadTimestamp();
        bool entriesFound = messages.Any(x => x.PublishDate > timestamp);
        return entriesFound;
    }

    public async Task MarkNotificationsAsRead()
    {
        await localStorageService.SetItemAsync(LocalStorageKey, DateTime.UtcNow.Date);
    }

    public async Task MarkNotificationsAsRead(string id)
    {
        NotificationMessage message = await GetMessageById(id);
        if (message == null)
            return;
        DateTime timestamp = await localStorageService.GetItemAsync<DateTime>(LocalStorageKey);
        if (message.PublishDate > timestamp)
        {
            await localStorageService.SetItemAsync(LocalStorageKey, message.PublishDate);
        }

    }

    public Task<NotificationMessage> GetMessageById(string id) =>
        Task.FromResult(messages.FirstOrDefault((x => x.Id == id)));

    public async Task<IDictionary<NotificationMessage, bool>> GetNotifications()
    {
        DateTime lastReadTimestamp = await GetLastReadTimestamp();
        Dictionary<NotificationMessage,bool> items = messages.ToDictionary(x => x, x => lastReadTimestamp > x.PublishDate);
        return items;
    }

    public Task AddNotification(NotificationMessage message)
    {
        messages.Add(message);
        return Task.CompletedTask;
    }

    public void Preload()
    {
        messages.Add(new NotificationMessage(
            "mudblazor-here-to-stay",
            "MudBlazor is here to stay",
            "We are paving the way for the future of Blazor",
            "Announcement",
            new DateTime(2022, 01, 13),
            "_content/MudBlazor.Docs/images/announcements/mudblazor_heretostay.png",
            new[]
            {
                new NotificationAuthor("Jonny Larsson",
                    "https://avatars.githubusercontent.com/u/10367109?v=4")
            }, typeof(NotificationMessage)));
    }
}