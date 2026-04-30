using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;

namespace AIPlayground.Console.AIAdapters;

internal class SimpleAIAdapter : AIAdapter
{
    private readonly IChatCompletionService chatCompletionService;
    private readonly OpenAIPromptExecutionSettings promptExecutionSettings;
    private readonly IChatHistoryReducer chatHistoryReducer;
    private ChatHistory chatHistory;

    public SimpleAIAdapter(IChatCompletionService chatCompletionService)
    {
        this.chatCompletionService = chatCompletionService;
        this.chatHistoryReducer = new ChatHistoryTruncationReducer(targetCount: 2);
        this.chatHistory = new ChatHistory();

        this.promptExecutionSettings = new OpenAIPromptExecutionSettings()
        {
            //System prompt
            ChatSystemPrompt = "You are a helpful assistant that provides concise and accurate answers to user questions. Answer as a scientist.",

            //Pretty different response eeach time
            Temperature = 0.9,

            //Max token in the response => limit the cost
            MaxTokens = 200
        };
    }

    public async Task StartPrompt()
    {
        while (true)
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("Enter your prompt here. Press 'q' or 'Enter' to quit");
            System.Console.ForegroundColor = ConsoleColor.DarkYellow;
            var userPrompt = System.Console.ReadLine();

            if (string.IsNullOrEmpty(userPrompt) || userPrompt.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            chatHistory.AddUserMessage(userPrompt);

            System.Console.ForegroundColor = ConsoleColor.Cyan;

            //Stateliess
            //Send chat history, so there is no loss of context.
            var chatResponse = await chatCompletionService.GetChatMessageContentAsync(chatHistory, promptExecutionSettings);

            if (chatResponse != null)
            {
                chatHistory.AddAssistantMessage(chatResponse.Content ?? string.Empty);
                System.Console.WriteLine(chatResponse.Content);

                if (chatResponse.InnerContent is ChatCompletion)
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkCyan;
                    System.Console.WriteLine("Detailed info:");
                    var details = chatResponse.InnerContent as ChatCompletion;
                    System.Console.WriteLine($"ModelId: {chatResponse.ModelId}");
                    System.Console.WriteLine($"Output token count: {details!.Usage.OutputTokenCount}");
                    System.Console.WriteLine($"Input token count: {details!.Usage.InputTokenCount}");
                    System.Console.WriteLine($"Total token count: {details!.Usage.TotalTokenCount}");
                }
            }
            else
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("No response from the model.");
                System.Console.ForegroundColor = ConsoleColor.White;
            }

            System.Console.WriteLine();

            var reducedChatHistory = await chatHistoryReducer.ReduceAsync(chatHistory);
            //returns null if no reduction has happened
            if (reducedChatHistory is not null)
            {
                chatHistory = new ChatHistory(reducedChatHistory);
            }
        }

        System.Console.ForegroundColor = ConsoleColor.White;
    }
}
