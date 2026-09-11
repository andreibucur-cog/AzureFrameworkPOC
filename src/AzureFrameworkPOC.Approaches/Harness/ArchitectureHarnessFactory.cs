using AzureFrameworkPOC.AI.Tools;
using AzureFrameworkPOC.Core.AI;
using AzureFrameworkPOC.Core.Exploration;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AzureFrameworkPOC.Approaches.Harness;

public sealed class ArchitectureHarnessFactory(
    IChatClientProviderResolver providerResolver,
    ArchitectureToolFactory toolFactory,
    HarnessLimits limits)
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

        IReadOnlyList<AITool> repositoryTools =
            toolFactory.CreateReadOnlyTools();

        var options =
            new HarnessAgentOptions
            {
                Name =
                    "ProjectArchitectureHarness",

                HarnessInstructions =
                    HarnessInstructions.HarnessGuidance,

                ChatOptions =
                    new ChatOptions
                    {
                        Instructions =
                            HarnessInstructions
                                .AgentInstructions,

                        Tools =
                            repositoryTools.ToList(),

                        Temperature = 0,

                        MaxOutputTokens =
                            limits.MaximumOutputTokens
                    },

#pragma warning disable MAAI001
                MaxContextWindowTokens =
                    limits.MaximumContextWindowTokens,

                MaxOutputTokens =
                    limits.MaximumOutputTokens
#pragma warning restore MAAI001
            };

        return chatClient.AsHarnessAgent(options);
    }
}