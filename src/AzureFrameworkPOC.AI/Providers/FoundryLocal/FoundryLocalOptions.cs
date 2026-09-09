namespace AzureFrameworkPOC.AI.Providers.FoundryLocal;

public sealed class FoundryLocalOptions
{
    public const string SectionName = "AI:FoundryLocal";

    public string ModelAlias { get; init; } = "qwen2.5-0.5b";

    public string Endpoint { get; init; } =
        "http://127.0.0.1:55588";

    public string ApiKey { get; init; } = "not-needed";

    public bool DownloadExecutionProviders { get; init; } = true;

    public bool UnloadModelOnShutdown { get; init; } = true;
}