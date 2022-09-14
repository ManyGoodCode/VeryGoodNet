
using Blazored.LocalStorage;

namespace Blazor.Server.UI.Services
{

    public interface IUserPreferencesService
    {
        public Task SaveUserPreferences(UserPreferences userPreferences);
        public Task<UserPreferences> LoadUserPreferences();
    }

    public class UserPreferencesService : IUserPreferencesService
    {
        private readonly ILocalStorageService localStorage;
        private const string Key = "userPreferences";

        public UserPreferencesService(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public async Task SaveUserPreferences(UserPreferences userPreferences)
        {
            await localStorage.SetItemAsync(Key, userPreferences);
        }

        public async Task<UserPreferences> LoadUserPreferences()
        {
            return await localStorage.GetItemAsync<UserPreferences>(Key);
        }
    }
}

