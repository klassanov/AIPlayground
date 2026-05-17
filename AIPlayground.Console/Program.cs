using AIPlayground.Console.AIAdapters;
using AIPlayground.Console.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Onnx;

// Load configuration, without using Host (this is a lightweight alternative)
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();




// Bind settings
var settings = new SemanticKernelSettings();
configuration.GetSection("SemanticKernel").Bind(settings);

//Standalone instance without Kernel usage
await UseStandaloneInstance(configuration);


//Semantic Kernel builder, used for DI of Senamntic Kernel - specific services
var semanticKernelBuilder = Kernel.CreateBuilder();

//Custom config wrapper
semanticKernelBuilder.ConfigureModels(settings);
semanticKernelBuilder.Services.AddSingleton<IAIAdapter, GithubAdapter>();
semanticKernelBuilder.Services.AddSingleton<IAIAdapter, LocalPHI3Adapter>();
var kernel = semanticKernelBuilder.Build();

//This works regardless of the model
//var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

IAIAdapter adapter = kernel.GetRequiredService<IAIAdapter>();
await adapter.StartPrompt();



public partial class Program
{
    public static async Task UseStandaloneInstance(IConfiguration configuration)
    {
        //Standalone instance
        //Directly using chat completion service without using Kernel
        //Just one message for testing purposes => without interactive prompt and without chat history management
        Console.WriteLine("OnnxRuntimeGenAIChatCompletionService standalone demo");
        Console.WriteLine("Ask me something");

        var history = new ChatHistory(systemMessage: "You are a helpful assistant that provides concise and accurate answers to user questions. Answer as a scientist.");

        using (OnnxRuntimeGenAIChatCompletionService chatCompletionService =
            new OnnxRuntimeGenAIChatCompletionService(
                modelId: configuration["SemanticKernel:LocalPHI3:ModelId"],
                modelPath: configuration["SemanticKernel:LocalPHI3:ModelPath"]))
        {
            var userMessage = Console.ReadLine();
            history.AddUserMessage(userMessage??string.Empty);
            var response = await chatCompletionService.GetChatMessageContentAsync(history);
            Console.WriteLine(response.Content);
            Console.WriteLine();
        }
    }
}
