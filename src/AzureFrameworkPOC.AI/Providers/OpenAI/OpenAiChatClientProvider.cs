using AzureFrameworkPOC.Core.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;

namespace AzureFrameworkPOC.AI.Providers.OpenAI;

public sealed class OpenAiChatClientProvider(
    IOptions<OpenAiOptions> options)
    : IChatClientProvider
{
    private readonly OpenAiOptions _options =
        options.Value;

    private IChatClient? _client;

    public ChatProviderType Type =>
        ChatProviderType.OpenAI;

    public Task<IChatClient> GetChatClientAsync(
        CancellationToken cancellationToken = default)
    {
        _client ??= new OpenAIClient(
                _options.ApiKey)
            .GetChatClient(_options.Model)
            .AsIChatClient();

        return Task.FromResult(_client);
    }
}