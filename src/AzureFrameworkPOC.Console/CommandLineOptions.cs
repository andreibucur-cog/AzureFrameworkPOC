using AzureFrameworkPOC.Core.AI;
using AzureFrameworkPOC.Core.Exploration;

namespace AzureFrameworkPOC.Console;

public sealed record CommandLineOptions(
    string RepositoryPath,
    ChatProviderType Provider,
    ExplorerType Approach)
{
    public static CommandLineOptions Parse(
        IReadOnlyList<string> args)
    {
        string? repositoryPath = null;

        var provider =
            ChatProviderType.FoundryLocal;

        var approach =
            ExplorerType.Agent;

        for (var index = 0; index < args.Count; index++)
        {
            string argument = args[index];

            switch (argument)
            {
                case "--repository":
                case "-r":
                    repositoryPath =
                        ReadValue(args, ref index, argument);
                    break;

                case "--provider":
                case "-p":
                    string providerValue =
                        ReadValue(args, ref index, argument);

                    if (!Enum.TryParse(
                            providerValue,
                            ignoreCase: true,
                            out provider))
                    {
                        throw new ArgumentException(
                            $"Unknown provider: {providerValue}");
                    }

                    break;

                case "--approach":
                case "-a":
                    string approachValue =
                        ReadValue(args, ref index, argument);

                    if (!Enum.TryParse(
                            approachValue,
                            ignoreCase: true,
                            out approach))
                    {
                        throw new ArgumentException(
                            $"Unknown approach: {approachValue}");
                    }

                    break;

                case "--help":
                case "-h":
                    PrintHelp();
                    Environment.Exit(0);
                    break;

                default:
                    throw new ArgumentException(
                        $"Unknown argument: {argument}");
            }
        }

        repositoryPath ??= PromptForRepositoryPath();

        return new CommandLineOptions(
            repositoryPath,
            provider,
            approach);
    }

    private static string ReadValue(
        IReadOnlyList<string> args,
        ref int index,
        string argument)
    {
        if (index + 1 >= args.Count)
        {
            throw new ArgumentException(
                $"Missing value after {argument}.");
        }

        index++;
        return args[index];
    }

    private static string PromptForRepositoryPath()
    {
        System.Console.Write(
            "Path of the repository to explore: ");

        string? value = System.Console.ReadLine();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "A repository path is required.");
        }

        return value.Trim().Trim('"');
    }

    private static void PrintHelp()
    {
        System.Console.WriteLine(
            """
            AzureFrameworkPOC

            Options:

              --repository, -r <path>
                  Path to the repository that will be explored.

              --provider, -p <provider>
                  FoundryLocal, OpenAI or AzureOpenAI.
                  Only FoundryLocal is implemented initially.

              --approach, -a <approach>
                  Agent, Workflow or Harness.

            Example:

              dotnet run --repository "C:\Projects\MyProject" \
                  --provider FoundryLocal \
                  --approach Agent
            """);
    }
}