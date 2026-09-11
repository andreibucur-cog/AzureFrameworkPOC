using AzureFrameworkPOC.Core.AI;
using AzureFrameworkPOC.Core.Exploration;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed class SynthesisAgentFactory(
    IChatClientProviderResolver providerResolver,
    WorkflowLimits limits)
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

        var options = new ChatClientAgentOptions
        {
            ChatOptions = new ChatOptions
            {

                Instructions =
                """
                You are a .NET project architecture reviewer.

                Answer only from the source evidence provided in the
                request. Do not use assumed framework conventions as
                proof of repository behavior.

                Every finding must identify:

                - repository-relative file path;
                - exact source line range;
                - what the code proves;
                - whether the finding is direct evidence or an inference.

                Do not invent missing configuration values, runtime
                behavior, deployment topology or external services.

                If evidence is incomplete, state that clearly.

                Organize the answer under:

                1. Summary
                2. Verified findings
                3. Architectural interpretation
                4. Uncertainties
                5. Suggested follow-up questions
                """,
                Temperature = 0,
                MaxOutputTokens =
                limits.MaximumOutputTokens
            }
        };

        return new ChatClientAgent(
            chatClient,
            options);
    }
}