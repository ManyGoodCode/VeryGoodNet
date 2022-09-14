namespace Blazor.Server.UI.Services
{
    public class UserPreferences
    {
        public bool RightToLeft { get; set; }
        public bool IsDarkMode { get; set; }
        public string PrimaryColor { get; set; } = "#2d4275";
        public string SecondaryColor { get; set; } = "#ff4081ff";
        public double BorderRadius { get; set; } = 4;
    }
}

