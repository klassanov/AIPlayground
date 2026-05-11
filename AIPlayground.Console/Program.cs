using AIPlayground.Console.AIAdapters;
using AIPlayground.Console.AIHostingAdapters;
using AIPlayground.Console.Config;
using AIPlayground.Console.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

var semanticKernelBuilder = Kernel.CreateBuilder();

//Custom config wrapper
semanticKernelBuilder.ConfigureModels(settings);


//semanticKernelBuilder.Services.AddSingleton<IAIAdapter, GithubAdapter>();


var kernel = semanticKernelBuilder.Build();

//This works regardless of the model
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>(ModelHost.GITHUB.ToString());
chatCompletionService.PrintAttributes();



//TODO: implement visitor streaming vs simple and all different kind of adaptors per each provider

IAIAdapter adapter;
//adapter = new SimpleAIAdapter(chatCompletionService);
adapter = new ResponseStreamingIAAdapter(chatCompletionService);

await adapter.StartPrompt();

