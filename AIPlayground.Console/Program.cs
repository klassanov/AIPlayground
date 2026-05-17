using AIPlayground.Console.AIAdapters;
using AIPlayground.Console.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

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

//Semantic Kernel builder, used for DI of Senamntic Kernel - specific services
var semanticKernelBuilder = Kernel.CreateBuilder();

//Custom config wrapper
semanticKernelBuilder.ConfigureModels(settings);
semanticKernelBuilder.Services.AddSingleton<IAIAdapter, GithubAdapter>();
var kernel = semanticKernelBuilder.Build();

//This works regardless of the model
//var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

IAIAdapter adapter = kernel.GetRequiredService<IAIAdapter>();
await adapter.StartPrompt();

