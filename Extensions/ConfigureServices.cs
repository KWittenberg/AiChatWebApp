namespace N10.Extensions;

public static class ConfigureServices
{
    public static IServiceCollection AddConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRazorComponents().AddInteractiveServerComponents();
        services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });








        services.Configure<OpenWeatherOptions>(configuration.GetSection(nameof(OpenWeatherOptions)));
        services.AddHttpClient<OpenWeatherClient>();


        return services;
    }
}