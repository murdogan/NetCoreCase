namespace NetCoreCase.Domain.Constants;

public static class LanguageConstants
{
    public const string Turkish = "tr";
    public const string English = "en";
    
    public static readonly string[] SupportedLanguages = { Turkish, English };
    
    public static bool IsSupported(string language)
    {
        return SupportedLanguages.Contains(language?.ToLower());
    }
} 