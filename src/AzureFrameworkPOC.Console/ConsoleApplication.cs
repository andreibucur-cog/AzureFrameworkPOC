using AzureFrameworkPOC.Core.AI;
using AzureFrameworkPOC.Core.Repository;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using AzureFrameworkPOC.Core.Exploration;

namespace AzureFrameworkPOC.Console;

public sealed class ConsoleApplication(
    IChatClientProviderResolver providerResolver,
    IRepositoryContextAccessor repositoryContextAccessor,
    IEnumerable<IArchitectureExplorer> explorers,
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

        IArchitectureExplorer agentExplorer =
            explorers.Single(
                explorer =>
                    explorer.Type == ExplorerType.Agent);

        System.Console.WriteLine();
        System.Console.Write(
            "Architecture question: ");

        string? questionText =
            System.Console.ReadLine();

        if (string.IsNullOrWhiteSpace(questionText))
        {
            throw new ArgumentException(
                "An architecture question is required.");
        }

        ExplorerResult result =
            await agentExplorer.ExploreAsync(
                new ArchitectureQuestion(questionText),
                cancellationToken);

        System.Console.WriteLine();
        System.Console.WriteLine("AGENT RESULT");
        System.Console.WriteLine("============");
        System.Console.WriteLine(result.Answer.Summary);
        System.Console.WriteLine();
        System.Console.WriteLine(
            $"Duration: {result.Metrics.Duration}");
    }
}