namespace Blazor.Server.UI.Models.Localization
{
    public record LanguageCode(string code, string displayName, bool isRTL = false);
    public static class LocalizationConstants
    {
        public static readonly LanguageCode[] SupportedLanguages =
        {
            new("en-US", "English"),
            new("fr-FR", "French"),
            new("de-DE", "German"),
            new("ja-JP", "Japanese"),
            new("es-ES", "Spanish"),
            new("ru-RU", "Russian"),
            new("zh-CN", "Simplified Chinese")
    };
    }
}

