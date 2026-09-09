using AzureFrameworkPOC.Core.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;

namespace AzureFrameworkPOC.AI.Providers.FoundryLocal;

public sealed class FoundryLocalChatClientProvider(
    IFoundryLocalRuntime runtime)
    : IChatClientProvider
{
    private readonly SemaphoreSlim _clientLock = new(1, 1);

    private IChatClient? _chatClient;

    public ChatProviderType Type =>
        ChatProviderType.FoundryLocal;

    public async Task<IChatClient> GetChatClientAsync(
        CancellationToken cancellationToken = default)
    {
        if (_chatClient is not null)
        {
            return _chatClient;
        }

        await _clientLock.WaitAsync(cancellationToken);

        try
        {
            if (_chatClient is not null)
            {
                return _chatClient;
            }

            await runtime.EnsureStartedAsync(
                cancellationToken);

            var credential = new ApiKeyCredential(
                "not-needed");

            var openAiOptions = new OpenAIClientOptions
            {
                Endpoint = runtime.OpenAiEndpoint
            };

            var openAiClient = new OpenAIClient(
                credential,
                openAiOptions);

            global::OpenAI.Chat.ChatClient officialChatClient =
            openAiClient.GetChatClient(runtime.ModelId);

            _chatClient =
                officialChatClient.AsIChatClient();

            return _chatClient;
        }
        finally
        {
            _clientLock.Release();
        }
    }
}