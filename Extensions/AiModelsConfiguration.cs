namespace AiChatWebApp.Extensions;

public static class AiModelsConfiguration
{
    public static IServiceCollection AddOllamaClients(this IServiceCollection services)
    {
        var ollamaUri = new Uri("http://localhost:11434");

        // 💡 mali, laki modeli — za laptop
        string[] modelName = ["all-minilm", "tinyllama", "qwen2.5:0.5b"];

        IChatClient chatClient = new OllamaApiClient(ollamaUri, modelName[2]);
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OllamaApiClient(ollamaUri, modelName[0]);

        services.AddChatClient(chatClient)
                .UseFunctionInvocation()
                .UseLogging();

        services.AddEmbeddingGenerator(embeddingGenerator);

        return services;
    }
}