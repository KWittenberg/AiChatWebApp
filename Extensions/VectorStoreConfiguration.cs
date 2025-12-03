namespace AiChatWebApp.Extensions;

public static class VectorStoreConfiguration
{
    public static IServiceCollection AddVectorStore(this IServiceCollection services)
    {
        string dbName = "vector-store.db";
        var vectorStorePath = Path.Combine(AppContext.BaseDirectory, dbName);
        var vectorStoreConnectionString = $"Data Source={vectorStorePath}";

        services.AddSqliteVectorStore(_ => vectorStoreConnectionString);
        services.AddSqliteCollection<string, IngestedChunk>(IngestedChunk.CollectionName, vectorStoreConnectionString);

        return services;
    }
}