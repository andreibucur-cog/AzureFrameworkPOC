using AzureFrameworkPOC.Core.AI;

namespace AzureFrameworkPOC.AI.Providers;

public sealed class ChatClientProviderResolver(
    IEnumerable<IChatClientProvider> providers)
    : IChatClientProviderResolver
{
    private readonly IReadOnlyDictionary<
        ChatProviderType,
        IChatClientProvider> _providers =
        providers.ToDictionary(provider => provider.Type);

    public IChatClientProvider Resolve(
        ChatProviderType providerType)
    {
        if (_providers.TryGetValue(
                providerType,
                out IChatClientProvider? provider))
        {
            return provider;
        }

        throw new InvalidOperationException(
            $"No chat-client provider is registered for " +
            $"'{providerType}'.");
    }
}