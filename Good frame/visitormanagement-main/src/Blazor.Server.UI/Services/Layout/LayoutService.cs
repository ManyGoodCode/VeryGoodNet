using MudBlazor;

namespace Blazor.Server.UI.Services
{
    public class LayoutService
    {
        private readonly IUserPreferencesService userPreferencesService;
        private UserPreferences userPreferences = new();

        public bool IsRTL { get; private set; } = false;
        public bool IsDarkMode { get; private set; } = false;
        public string PrimaryColor { get; set; } = "#2d4275";
        public string SecondaryColor { get; set; } = "#ff4081ff";
        public double BorderRadius { get; set; } = 4;

        public MudTheme CurrentTheme { get; private set; }

        public event EventHandler MajorUpdateOccured;

        public LayoutService(IUserPreferencesService userPreferencesService)
        {
            this.userPreferencesService = userPreferencesService;
        }

        public void SetDarkMode(bool value)
        {
            IsDarkMode = value;
        }

        public async Task<UserPreferences> ApplyUserPreferences(bool isDarkModeDefaultTheme)
        {
            userPreferences = await userPreferencesService.LoadUserPreferences();
            if (userPreferences != null)
            {
                IsDarkMode = userPreferences.IsDarkMode;
                IsRTL = userPreferences.RightToLeft;
                PrimaryColor = userPreferences.PrimaryColor;
                SecondaryColor = userPreferences.SecondaryColor;
                BorderRadius = userPreferences.BorderRadius;
                CurrentTheme.Palette.Primary = PrimaryColor;
                CurrentTheme.PaletteDark.Primary = PrimaryColor;
                CurrentTheme.Palette.Secondary = SecondaryColor;
                CurrentTheme.PaletteDark.Secondary = SecondaryColor;
                CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
            }
            else
            {
                IsDarkMode = isDarkModeDefaultTheme;
                userPreferences = new UserPreferences { IsDarkMode = IsDarkMode };
                await userPreferencesService.SaveUserPreferences(userPreferences);
            }

            return userPreferences;
        }

        private void OnMajorUpdateOccured() => MajorUpdateOccured?.Invoke(this, EventArgs.Empty);

        public async Task ToggleDarkMode()
        {
            IsDarkMode = !IsDarkMode;
            userPreferences.IsDarkMode = IsDarkMode;
            await userPreferencesService.SaveUserPreferences(userPreferences);
            OnMajorUpdateOccured();
        }

        public async Task ToggleRightToLeft()
        {
            IsRTL = !IsRTL;
            userPreferences.RightToLeft = IsRTL;
            await userPreferencesService.SaveUserPreferences(userPreferences);
            OnMajorUpdateOccured();
        }

        public void SetBaseTheme(MudTheme theme)
        {
            CurrentTheme = theme;
            CurrentTheme.Palette.Primary = PrimaryColor;
            CurrentTheme.PaletteDark.Primary = PrimaryColor;
            CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
            OnMajorUpdateOccured();
        }

        public async Task SetPrimaryColor(string color)
        {
            PrimaryColor = color;
            CurrentTheme.Palette.Primary = PrimaryColor;
            userPreferences.PrimaryColor = PrimaryColor;
            await userPreferencesService.SaveUserPreferences(userPreferences);
            OnMajorUpdateOccured();
        }

        public async Task SetSecondaryColor(string color)
        {
            SecondaryColor = color;
            CurrentTheme.Palette.Secondary = SecondaryColor;
            userPreferences.SecondaryColor = SecondaryColor;
            await userPreferencesService.SaveUserPreferences(userPreferences);
            OnMajorUpdateOccured();
        }

        public async Task SetBorderRadius(double size)
        {
            BorderRadius = size;
            CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
            userPreferences.BorderRadius = BorderRadius;
            await userPreferencesService.SaveUserPreferences(userPreferences);
            OnMajorUpdateOccured();
        }

        public async Task UpdateUserPreferences(UserPreferences preferences)
        {
            userPreferences = preferences;
            IsDarkMode = userPreferences.IsDarkMode;
            IsRTL = userPreferences.RightToLeft;
            PrimaryColor = userPreferences.PrimaryColor;
            SecondaryColor = userPreferences.SecondaryColor;
            BorderRadius = userPreferences.BorderRadius;
            CurrentTheme.Palette.Primary = PrimaryColor;
            CurrentTheme.PaletteDark.Primary = PrimaryColor;
            CurrentTheme.Palette.Secondary = SecondaryColor;
            CurrentTheme.PaletteDark.Secondary = SecondaryColor;
            CurrentTheme.LayoutProperties.DefaultBorderRadius = BorderRadius + "px";
            await userPreferencesService.SaveUserPreferences(userPreferences);
            OnMajorUpdateOccured();
        }
    }
}
