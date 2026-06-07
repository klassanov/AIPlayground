using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

var service = new AzureOpenAIChatCompletionService(
    deploymentName: configuration["AzureOpenAI:DeploymentName"],
    apiKey: configuration["AzureOpenAI:ApiKey"],
    endpoint: configuration["AzureOpenAI:Endpoint"],
    modelId: configuration["AzureOpenAI:ModelId"]
    );