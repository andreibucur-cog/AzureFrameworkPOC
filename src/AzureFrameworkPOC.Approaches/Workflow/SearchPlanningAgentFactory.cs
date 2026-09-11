using AzureFrameworkPOC.Core.AI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AzureFrameworkPOC.Approaches.Workflow;

public sealed class SearchPlanningAgentFactory(
    IChatClientProviderResolver providerResolver)
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

        var chatOptions = new ChatClientAgentOptions
        {
            ChatOptions = new ChatOptions
            {
                Instructions =
                """
                You create focused source-code search plans for
                .NET repositories.

                Given an architecture question and a repository
                overview, return a concise JSON search plan.

                Search terms must be literal terms likely to exist
                in source code, configuration files or project files.

                Include:
                - framework and package names;
                - likely interfaces;
                - registration methods;
                - configuration names;
                - implementation types;
                - consumer abstractions.

                Avoid broad terms such as:
                - service;
                - manager;
                - application;
                - project;
                - code.

                Return only valid JSON in this shape:

                {
                  "operations": [
                    {
                      "searchTerm": "IDistributedCache",
                      "filePattern": "*.cs",
                      "purpose": "Find consumers of distributed caching"
                    }
                  ]
                }

                Generate no more than eight operations.
                Do not include Markdown fences.
                """,
                Temperature = 0,
                MaxOutputTokens = 1_200
            }
        };

        return new ChatClientAgent(
            chatClient,
            chatOptions);
    }
}