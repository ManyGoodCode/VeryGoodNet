namespace Blazor.Server.UI.Models.Localization
{
    public record LanguageCode(string code, string displayName, bool isRTL = false);
    public static class LocalizationConstants
    {
        public static readonly LanguageCode[] SupportedLanguages =
        {
            new(code:"en-US",displayName:"English"),
            new(code:"fr-FR",displayName:"French"),
            new(code:"de-DE",displayName:"German"),
            new(code:"ja-JP",displayName:"Japanese"),
            new(code:"es-ES",displayName:"Spanish"),
            new(code:"ru-RU",displayName:"Russian"),
            new(code:"zh-CN",displayName:"Simplified Chinese")
        };
    }
}

