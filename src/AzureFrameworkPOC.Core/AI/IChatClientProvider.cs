using Microsoft.Extensions.AI;

namespace AzureFrameworkPOC.Core.AI;

public interface IChatClientProvider
{
    ChatProviderType Type { get; }

    Task<IChatClient> GetChatClientAsync(
        CancellationToken cancellationToken = default);
}