using System.Text;
using AIPlugins.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using OpenAI.Chat;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    //.AddJsonFile("appsettings.json")
    //.AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();



var kernelBuilder = Kernel.CreateBuilder();

kernelBuilder.AddAzureOpenAIChatCompletion(
                           deploymentName: configuration["AzureOpenAI:ModelId"],
                           endpoint: configuration["AzureOpenAI:Endpoint"],
                           apiKey: configuration["AzureOpenAI:ApiKey"],
                           modelId: configuration["AzureOpenAI:ModelId"]);

//kernelBuilder.AddOllamaChatCompletion(
//                            modelId: configuration["Ollama:ModelId"],
//                            endpoint: new Uri(configuration["Ollama:Endpoint"]));


kernelBuilder.Plugins.AddFromType<DateTimePlugin>();
kernelBuilder.Plugins.AddFromType<WeatherForecastPlugin>();

kernelBuilder.Services.AddHttpClient("WeatherForecast", (serviceProvider, client) =>
{
    client.BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast");
});

//kernelBuilder.Services.AddLogging(builder =>
//{
//    builder
//    .ClearProviders()
//    .AddConsole()
//    .SetMinimumLevel(LogLevel.Trace);
//});

//kernelBuilder.Services.AddHttpClient("Customers", (serviceProvider, client) =>
//{
//    client.BaseAddress = new Uri("https://localhost:7098");
//})
//.ConfigurePrimaryHttpMessageHandler(() =>
//{
//    return
//    new HttpClientHandler
//    {
//        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
//    };
//});




var kernel = kernelBuilder.Build();

//var clientFactory = kernel.GetRequiredService<IHttpClientFactory>();
//var httpClient = clientFactory.CreateClient("Customers");

//Disabled warning (treated as error) in order to be able to call the localhost (and maybe other APIs)
#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var executionParameters = new OpenApiFunctionExecutionParameters()
{
    EnablePayloadNamespacing = true,
    ServerUrlValidationOptions = new()
};
executionParameters.ServerUrlValidationOptions.AllowPrivateNetworkAccess = true;
//executionParameters.ServerUrlValidationOptions.AllowedBaseUrls = { https://dogapi.dog/ };

#pragma warning restore SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.


//This is not on the kernel buiilder, but on the kernel itself
//var customersPlugin = await kernel.ImportPluginFromOpenApiAsync(
//                                 pluginName: "get_customers_info",
//                                 uri: new Uri("https://localhost:7098/openapi/v1.json"),
//                                 executionParameters: executionParameters);

//new Microsoft.SemanticKernel.Plugins.OpenApi.OpenApiFunctionExecutionParameters() {IgnoreNonCompliantErrors =  true, HttpClient = httpClient }


//Adding a NoAsAService by file containing open api specs. File generated with chat gpt from an example response
var noAsAServicePlugin = await kernel.ImportPluginFromOpenApiAsync(
                    pluginName: "no_as_a_service",
                    filePath: "specs/no-as-a-service.yml");



// Invoke explicitly a plugin and a function inside it to test
//var result = await kernel.InvokeAsync(kernel.Plugins.GetFunction(
//    pluginName:"get_customers_info",
//    functionName: "GetAllCustomers"));

var result = await kernel.InvokeAsync(kernel.Plugins.GetFunction(
    pluginName: "no_as_a_service",
    functionName: "getNo"));

await StartPrompt(kernel);


public partial class Program
{

    public async static Task StartPrompt(Kernel kernel)
    {
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var chatHistory = new ChatHistory(systemMessage: "You are a helpful assistant that provides concise and accurate answers to user questions. Answer as a scientist.");
        var chatHistoryReducer = new ChatHistorySummarizationReducer(service: chatCompletionService, targetCount: 2, thresholdCount: 2);
        var promptExecutionSettings = new OpenAIPromptExecutionSettings()
        {
            //Required
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

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
            var chatResponseSb = new StringBuilder();
            ChatTokenUsage? chatTokenUsage = null;
            StreamingChatMessageContent? lastChatResponseChunk = null;

            //Send chat history, so there is no loss of context.
            await foreach (StreamingChatMessageContent chatResponseChunk in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, promptExecutionSettings, kernel))
            {

                Console.Write(chatResponseChunk.Content);
                chatResponseSb.Append(chatResponseChunk.Content);

                //Assign each time and capture the last update which will have the usage object not null
                lastChatResponseChunk = chatResponseChunk;
            }

            //System.Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();

            if (lastChatResponseChunk?.InnerContent is StreamingChatCompletionUpdate update)
            {
                var innerContent = lastChatResponseChunk.InnerContent as StreamingChatCompletionUpdate;
                chatTokenUsage = innerContent?.Usage;

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Detailed info:");
                Console.WriteLine($"ModelId: {innerContent?.Model}");
                Console.WriteLine($"Output token count: {chatTokenUsage?.OutputTokenCount}");
                Console.WriteLine($"Input token count: {chatTokenUsage?.InputTokenCount}");
                Console.WriteLine($"Total token count: {chatTokenUsage?.TotalTokenCount}");
            }

            chatHistory.AddAssistantMessage(chatResponseSb.ToString());
            Console.WriteLine();

            var reducedChatHistory = await chatHistoryReducer.ReduceAsync(chatHistory);
            //returns null if no reduction has happened
            if (reducedChatHistory is not null)
            {
                chatHistory = new ChatHistory(reducedChatHistory);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

}
