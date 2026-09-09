namespace AzureFrameworkPOC.AI.Providers.OpenAI;

public sealed class OpenAiOptions
{
    public const string SectionName = "AI:OpenAI";

    public string Model { get; init; } = string.Empty;

    public string ApiKey { get; init; } = string.Empty;
}