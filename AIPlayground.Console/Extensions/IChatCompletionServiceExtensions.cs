using Microsoft.SemanticKernel.ChatCompletion;

namespace AIPlayground.Console.Extensions
{
    internal static class IChatCompletionServiceExtensions
    {
        extension(IChatCompletionService chatCompletionService)
        {
            internal void PrintAttributes()
            {
                System.Console.WriteLine($"Printing Attributes of {chatCompletionService.GetType().Name}");

                foreach (var item in chatCompletionService.Attributes)
                {
                    System.Console.WriteLine($"{item.Key}: {item.Value}");
                }

                System.Console.WriteLine();
            }
        }
    }
}
