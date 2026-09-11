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

        if (options.Approach != ExplorerType.Compare)
        {
            IArchitectureExplorer explorer =
            explorers.Single(
                explorer =>
                    explorer.Type == options.Approach);

            ExplorerResult result =
                await explorer.ExploreAsync(
                    new ArchitectureQuestion(questionText),
                    cancellationToken);

            System.Console.WriteLine();
            System.Console.WriteLine($"{explorer.Type} RESULT");
            System.Console.WriteLine("============");
            System.Console.WriteLine(result.Answer.Summary);
            System.Console.WriteLine();
            System.Console.WriteLine(
                $"Duration: {result.Metrics.Duration}");
        }
        else
        {
            foreach (IArchitectureExplorer explorer in
             explorers.OrderBy(item => item.Type))
                {
                    System.Console.WriteLine();
                    System.Console.WriteLine(
                        $"Running {explorer.Type}...");

                    ExplorerResult result =
                        await explorer.ExploreAsync(
                            new ArchitectureQuestion(questionText),
                            cancellationToken);

                System.Console.WriteLine();
                System.Console.WriteLine($"{explorer.Type} RESULT");
                System.Console.WriteLine("============");
                System.Console.WriteLine(result.Answer.Summary);
                System.Console.WriteLine();
                System.Console.WriteLine(
                    $"Duration: {result.Metrics.Duration}");
            }
        }
        
    }
}