using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

var service = new AzureOpenAIChatCompletionService(
    deploymentName: configuration["AzureOpenAI:ModelId"],
    apiKey: configuration["AzureOpenAI:ApiKey"],
    endpoint: configuration["AzureOpenAI:Endpoint"],
    modelId: configuration["AzureOpenAI:ModelId"]
    );

var imageFileNames = Directory.GetFiles("images");

foreach (var imageFileName in imageFileNames)
{
    Console.WriteLine($"Image {imageFileName}");

    var imageBytes = File.ReadAllBytes(imageFileName);


    var chatHistory = new ChatHistory(systemMessage:
@"You are a traffic analyzer AI that monitors traffic congestion images and congestion level.
Heavy congestion level is when there is very little room between cars and vehicles are breaking.
Medium congestion is when there is a lot of cars but they are not braking.
Low traffic is when there are few cars on the road.
In addition, attempt to determine if the image was taken with a malfunctioning camera by looking for distorted image or missing content");



    chatHistory.AddUserMessage(contentItems: [
        new ImageContent(imageBytes, "image/jpeg"),
        new TextContent("Analyze the image and determine the traffic congestion level. Also determine if camera is malfunctioning.")
    ]);


    var response = await service.GetChatMessageContentAsync(chatHistory);
    Console.WriteLine($"Response: {response.Content}");
    Console.WriteLine(new string('-', 50));

    await Task.Delay(1000);

}