using AzureFrameworkPOC.AI.Tools;
using AzureFrameworkPOC.Core.AI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace AzureFrameworkPOC.Approaches.Agent;

public sealed class ArchitectureAgentFactory(
    IChatClientProviderResolver providerResolver,
    ArchitectureToolFactory toolFactory,
    ILoggerFactory loggerFactory)
{
    public async Task<AIAgent> CreateAsync(
        ChatProviderType providerType,
        CancellationToken cancellationToken = default)
    {
        IChatClientProvider provider =
            providerResolver.Resolve(providerType);

        IChatClient chatClient =
            await provider.GetChatClientAsync(
                cancellationToken);

        IReadOnlyList<AITool> tools =
            toolFactory.CreateReadOnlyTools();

        return new ChatClientAgent(
            chatClient,
            instructions: AgentInstructions.Text,
            name: "ProjectArchitectureAgent",
            description:
                "Answers focused questions about a selected " +
                ".NET repository using source-code evidence.",
            tools: (IList<AITool>?)tools);
    }
}