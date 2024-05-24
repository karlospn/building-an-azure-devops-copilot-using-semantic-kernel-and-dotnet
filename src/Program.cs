using CustomCopilot.AzureDevOpsPlugin;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;

namespace CustomCopilot
{
    internal class Program
    {

        static async Task Main(string[] args)

        {
            // Create a kernel with the Azure OpenAI chat completion service
            var builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(Environment.GetEnvironmentVariable("OAI_MODEL_NAME")!,
                Environment.GetEnvironmentVariable("OAI_ENDPOINT")!,
                Environment.GetEnvironmentVariable("OAI_APIKEY")!);

            // Load the plugins
            #pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            builder.Plugins.AddFromType<TimePlugin>();
            builder.Plugins.AddFromObject(new AzureDevOpsProjectsPlugin(), nameof(AzureDevOpsProjectsPlugin));
            builder.Plugins.AddFromObject(new AzureDevOpsRepositoriesPlugin(), nameof(AzureDevOpsRepositoriesPlugin));
            builder.Plugins.AddFromObject(new AzureDevOpsBranchesPlugin(), nameof(AzureDevOpsBranchesPlugin));
            builder.Plugins.AddFromObject(new AzureDevOpsUsersPlugin(), nameof(AzureDevOpsUsersPlugin));
            builder.Plugins.AddFromObject(new AzureDevOpsCodeSearchPlugin(), nameof(AzureDevOpsCodeSearchPlugin));
            builder.Plugins.AddFromObject(new AzureDevOpsBuildsPlugin(), nameof(AzureDevOpsBuildsPlugin));


            // Build the kernel
            var kernel = builder.Build();

            // Create chat history
            ChatHistory history = [];
            history.AddSystemMessage(@"You are a virtual assistant specifically designed to manage an Azure DevOps instance. Your scope of conversation is strictly limited to this domain. Your responses should be concise, accurate, and directly related to the query at hand.
In order to provide the most accurate responses, you require precise inputs. 
If a function calling involves parameters that you do not have sufficient information about it, it is crucial that you do not attempt to guess or infer their values. Instead, your primary action should always be to ask the user to provide more detailed information about these parameters. This is a non-negotiable aspect of your function. Guessing or inferring values is not an acceptable course of action. Your goal is to avoid any potential misunderstandings and to provide the most accurate and helpful response possible.
Remember, when in doubt, always ask for more information. Never guess the values of a function parameter.
If a function call fails to produce any valid data, the response must always be: 'I'm sorry, but I wasn't able to retrieve any data.If the problem persists, you may want to contact your Azure DevOps administrator or support for further assistance'. 
Never fabricate a response if the function calling fails or returns invalid data.");

            // Get chat completion service
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            // Start the conversation
            while (true)
            {
                // Trim chat history
                if (history.Count > 20)
                {
                    history.RemoveRange(0, 4);
                }

                // Get user input
                Console.Title = "Azure DevOps Copilot";
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("\nUser > ");
                history.AddUserMessage(Console.ReadLine()!);

                // Enable auto function calling
                OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new()
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                };


                // Get the response from the AI
                var response = chatCompletionService.GetStreamingChatMessageContentsAsync(
                               history,
                               executionSettings: openAiPromptExecutionSettings,
                               kernel: kernel);


                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\nAssistant > ");

                string combinedResponse = string.Empty;
                await foreach (var message in response)
                {
                    //Write the response to the console
                    Console.Write(message);
                    combinedResponse += message;
                }

                Console.WriteLine();

                // Add the message from the agent to the chat history
                history.AddAssistantMessage(combinedResponse);
            }
        }
    }
}