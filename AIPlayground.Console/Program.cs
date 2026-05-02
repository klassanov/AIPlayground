using AIPlayground.Console.AIAdapters;
using AIPlayground.Console.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

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

//Custom config wrapper
builder.ConfigureModel(ModelType.OPENAI, settings);

//builder.AddAzureOpenAIChatCompletion(
//    deploymentName: settings.AzureOpenAI.DeploymentName,
//    endpoint: settings.AzureOpenAI.Endpoint,
//    apiKey: settings.AzureOpenAI.ApiKey);

var kernel = builder.Build();

//This works regardless of the model
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

AIAdapter adapter;

//adapter = new SimpleAIAdapter(chatCompletionService);
adapter = new ResponseStreamingIAAdapter(chatCompletionService);

await adapter.StartPrompt();

/*
//Specific to Open AI. Each model has a different class for settings. WIll not work with other models.
var promptExecutionSettings = new OpenAIPromptExecutionSettings()
{
    //System prompt
    ChatSystemPrompt = "You are a helpful assistant that provides concise and accurate answers to user questions. Answer as a scientist.",

    //Pretty different response eeach time
    Temperature = 0.9,

    //Max token in the response => limit the cost
    MaxTokens = 200
};

var chatHistory = new ChatHistory();


//Try different reducer strategies
IChatHistoryReducer reducer;
//targetCount: the count of messages that will remain in chatHistory after reduction of chat history is applied
//manual trigger
//var reducer = new ChatHistoryTruncationReducer(targetCount: 2);

//We are using the same model for reduction, but any model can be used here
//manual trigger
//tresholdCount: an optional number of messages beyond the 'targetCount' that must be present in order to trigger reduction
//after reducing we have targetCount + 1 summary message which is the last one in the reducedChat history collection
reducer = new ChatHistorySummarizationReducer(service: chatCompletionService, targetCount: 2, thresholdCount: 2);

//custom implementation of IChatHistoryReducer is also possible


while (true)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("Enter your prompt here. Press 'q' or 'Enter' to quit");
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    var userPrompt = Console.ReadLine();

    if (string.IsNullOrEmpty(userPrompt) || userPrompt.Equals("q", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    chatHistory.AddUserMessage(userPrompt);

    Console.ForegroundColor = ConsoleColor.Cyan;

    //Stateliess
    //var chatResponse = await chatCompletionService.GetChatMessageContentAsync(userPrompt, promptExecutionSettings);

    //Send chat history, so there is no loss of context.
    var chatResponse = await chatCompletionService.GetChatMessageContentAsync(chatHistory, promptExecutionSettings);

    if (chatResponse != null)
    {
        chatHistory.AddAssistantMessage(chatResponse.Content ?? string.Empty);
        Console.WriteLine(chatResponse.Content);

        if (chatResponse.InnerContent is ChatCompletion)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Detailed info:");
            var details = chatResponse.InnerContent as ChatCompletion;
            Console.WriteLine($"ModelId: {chatResponse.ModelId}");
            Console.WriteLine($"Output token count: {details!.Usage.OutputTokenCount}");
            Console.WriteLine($"Input token count: {details!.Usage.InputTokenCount}");
            Console.WriteLine($"Total token count: {details!.Usage.TotalTokenCount}");
        }
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("No response from the model.");
        Console.ForegroundColor = ConsoleColor.White;
    }

    Console.WriteLine();

    var reducedChatHistory = await reducer.ReduceAsync(chatHistory);
    //returns null if no reduction has happened
    if (reducedChatHistory is not null)
    {
        chatHistory = new ChatHistory(reducedChatHistory);
    }
}

Console.ForegroundColor = ConsoleColor.White;
*/

