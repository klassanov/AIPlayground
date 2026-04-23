using AIPlayground.Console.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Chat;

// Load configuration
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


var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    deploymentName: settings.AzureOpenAI.DeploymentName,
    endpoint: settings.AzureOpenAI.Endpoint,
    apiKey: settings.AzureOpenAI.ApiKey);

var kernel = builder.Build();



//This works regardless of the model
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

while (true)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("Enter your prompt here. Press 'q' or 'Enter' to quit");

    Console.ForegroundColor = ConsoleColor.DarkYellow;
    var userPrompt = Console.ReadLine();
    if(string.IsNullOrEmpty(userPrompt) || userPrompt.Equals("q", StringComparison.OrdinalIgnoreCase)) 
    {
        break;
    }

    Console.ForegroundColor = ConsoleColor.Cyan;
    var chatResponse = await chatCompletionService.GetChatMessageContentAsync(userPrompt);
    Console.WriteLine(chatResponse.Content);

    if(chatResponse.InnerContent is ChatCompletion)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("Detailed info:");
        var details = chatResponse.InnerContent as ChatCompletion;
        Console.WriteLine($"ModelId: {chatResponse.ModelId}");
        Console.WriteLine($"Output token count: {details!.Usage.OutputTokenCount}");
        Console.WriteLine($"Input token count: {details!.Usage.InputTokenCount}");
    }

    Console.WriteLine();
}

