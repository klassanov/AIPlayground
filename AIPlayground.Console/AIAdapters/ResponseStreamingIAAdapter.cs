using System.Text;
using AIPlayground.Console.Extensions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;

namespace AIPlayground.Console.AIAdapters
{
    internal class ResponseStreamingIAAdapter : IAIAdapter
    {
        private readonly IChatCompletionService chatCompletionService;
        private ChatHistory chatHistory;
        protected PromptExecutionSettings? promptExecutionSettings;
        protected IChatHistoryReducer chatHistoryReducer;

        public ResponseStreamingIAAdapter(IChatCompletionService chatCompletionService)
        {
            chatCompletionService.PrintAttributes();
            this.chatCompletionService = chatCompletionService;
            this.chatHistoryReducer = new ChatHistorySummarizationReducer(service: chatCompletionService, targetCount: 2, thresholdCount: 2);
            this.chatHistory = new ChatHistory();
            InitializePromptExecutionSettings();            
        }

        protected virtual void InitializePromptExecutionSettings()
        {
            //TODO: Specific to each model that is used => needs to be changed
            this.promptExecutionSettings = new OpenAIPromptExecutionSettings()
            {
                //System prompt
                ChatSystemPrompt = "You are a helpful assistant that provides concise and accurate answers to user questions. Answer as a scientist.",

                //Pretty different response eeach time
                Temperature = 0.9,

                //Max token in the response => limit the cost
                MaxTokens = 2000
            };
        }

        protected virtual void InitializeChatHistoryReducer()
        {
            //Default reducer that works for most cases, but can be overridden for specific scenarios.
            this.chatHistoryReducer = new ChatHistorySummarizationReducer(service: chatCompletionService, targetCount: 2, thresholdCount: 2);
        }

        public virtual async Task StartPrompt()
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
                var chatResponseSb = new StringBuilder();
                ChatTokenUsage? chatTokenUsage = null;
                StreamingChatMessageContent? lastChatResponseChunk = null;

                //Send chat history, so there is no loss of context.
                await foreach (StreamingChatMessageContent chatResponseChunk in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, promptExecutionSettings))
                {

                    System.Console.Write(chatResponseChunk.Content);
                    chatResponseSb.Append(chatResponseChunk.Content);

                    //Assign each time and capture the last update which will have the usage object not null
                    lastChatResponseChunk = chatResponseChunk;
                }

                //System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.WriteLine();

                if (lastChatResponseChunk?.InnerContent is StreamingChatCompletionUpdate update)
                {
                    var innerContent = lastChatResponseChunk.InnerContent as StreamingChatCompletionUpdate;
                    chatTokenUsage = innerContent?.Usage;

                    System.Console.ForegroundColor = ConsoleColor.DarkCyan;
                    System.Console.WriteLine("Detailed info:");
                    System.Console.WriteLine($"ModelId: {innerContent?.Model}");
                    System.Console.WriteLine($"Output token count: {chatTokenUsage?.OutputTokenCount}");
                    System.Console.WriteLine($"Input token count: {chatTokenUsage?.InputTokenCount}");
                    System.Console.WriteLine($"Total token count: {chatTokenUsage?.TotalTokenCount}");
                }

                chatHistory.AddAssistantMessage(chatResponseSb.ToString());
                System.Console.WriteLine();

                var reducedChatHistory = await chatHistoryReducer.ReduceAsync(chatHistory);
                //returns null if no reduction has happened
                if (reducedChatHistory is not null)
                {
                    chatHistory = new ChatHistory(reducedChatHistory);
                }
                System.Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
