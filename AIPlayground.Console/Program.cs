using AIPlayground.Console.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
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


//Specific to Open AI. Each model has a different class for settings. WIll not work with other models.
var promptExecutionSettings = new OpenAIPromptExecutionSettings()
{
    //System prompt
    ChatSystemPrompt = "You are a helpful assistant that provides concise and accurate answers to user questions. Always answer as a pirate.",
    
    //Pretty different response eeach time
    Temperature = 0.9,

    //Max token in the response
    MaxTokens = 100
};


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
    var chatResponse = await chatCompletionService.GetChatMessageContentAsync(userPrompt, promptExecutionSettings);
    Console.WriteLine(chatResponse.Content);

    if(chatResponse.InnerContent is ChatCompletion)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("Detailed info:");
        var details = chatResponse.InnerContent as ChatCompletion;
        Console.WriteLine($"ModelId: {chatResponse.ModelId}");
        Console.WriteLine($"Output token count: {details!.Usage.OutputTokenCount}");
        Console.WriteLine($"Input token count: {details!.Usage.InputTokenCount}");
        Console.WriteLine($"Total token count: {details!.Usage.TotalTokenCount}");
    }

    Console.WriteLine();
}

Console.ForegroundColor = ConsoleColor.White;

