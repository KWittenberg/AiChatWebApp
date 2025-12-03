namespace AiChatWebApp.Extensions;

public static class AppServiceConfiguration
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.AddSingleton<DataIngestor>();
        services.AddSingleton<SemanticSearch>();

        // ⬇️ fizički folder s dokumentima za ingestion
        var ingestionDir = new DirectoryInfo(Path.Combine(env.WebRootPath, "Data"));
        services.AddKeyedSingleton("ingestion_directory", ingestionDir);

        return services;
    }
}