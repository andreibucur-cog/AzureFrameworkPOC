using AzureFrameworkPOC.Core.AI;
using AzureFrameworkPOC.Core.Repository;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace AzureFrameworkPOC.Console;

public sealed class ConsoleApplication(
    IChatClientProviderResolver providerResolver,
    IRepositoryContextAccessor repositoryContextAccessor,
    ILogger<ConsoleApplication> logger)
{
    public async Task RunAsync(
        string[] args,
        CancellationToken cancellationToken)
    {
        CommandLineOptions options =
            CommandLineOptions.Parse(args);

        repositoryContextAccessor.SetRepository(
            options.RepositoryPath);

        RepositoryContext repository =
            repositoryContextAccessor.Current;

        logger.LogInformation(
            "Selected repository: {RepositoryPath}",
            repository.RootPath);

        IChatClientProvider provider =
            providerResolver.Resolve(options.Provider);

        logger.LogInformation(
            "Starting AI provider {Provider}.",
            provider.Type);

        IChatClient chatClient =
            await provider.GetChatClientAsync(
                cancellationToken);

        ChatResponse response =
            await chatClient.GetResponseAsync(
                [
                    new ChatMessage(
                        ChatRole.System,
                        """
                        You are a concise assistant used to verify
                        an AI provider connection.
                        """),

                    new ChatMessage(
                        ChatRole.User,
                        """
                        Respond with exactly one short sentence confirming
                        that the local model connection is working.
                        """)
                ],
                new ChatOptions
                {
                    MaxOutputTokens = 100,
                    Temperature = 0
                },
                cancellationToken);

        System.Console.WriteLine();
        System.Console.WriteLine("Model response:");
        System.Console.WriteLine(response.Text);
        System.Console.WriteLine();
        System.Console.WriteLine(
            $"Repository selected: {repository.RootPath}");
    }
}