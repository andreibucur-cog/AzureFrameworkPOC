namespace AzureFrameworkPOC.AI.Providers.FoundryLocal;

public interface IFoundryLocalRuntime
{
    string ModelId { get; }

    Uri OpenAiEndpoint { get; }

    Task EnsureStartedAsync(
        CancellationToken cancellationToken = default);
}