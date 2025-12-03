namespace AiChatWebApp.Extensions;

public static class ConfigureServices
{
    public static IServiceCollection AddConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddServerSideBlazor().AddCircuitOptions(o => o.DetailedErrors = true);

        // ✅ Ostalo iz template-a
        services.Configure<OpenWeatherOptions>(configuration.GetSection(nameof(OpenWeatherOptions)));
        services.AddHttpClient<OpenWeatherClient>();

        // ✅ AI integracije
        services.AddOllamaClients();
        services.AddVectorStore();
        services.AddAppServices(env);

        return services;
    }
}