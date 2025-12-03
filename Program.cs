var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add Identity, Repositories, Services, Localization, Validators, Seed 
services.AddConfigureServices(configuration);





// Initialize Ollama API client
string[] modelName = ["all-minilm", "tinyllama", "qwen2.5:0.5b"];
IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), modelName[2]);
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OllamaApiClient(new Uri("http://localhost:11434"), modelName[0]);


// Configure SQLite Vector Store
string dbName = "vector-store.db";
var vectorStorePath = Path.Combine(AppContext.BaseDirectory, dbName);
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";


services.AddSqliteVectorStore(_ => vectorStoreConnectionString);
services.AddSqliteCollection<string, IngestedChunk>(IngestedChunk.CollectionName, vectorStoreConnectionString);


// Register application services
services.AddSingleton<DataIngestor>();
services.AddSingleton<SemanticSearch>();
services.AddKeyedSingleton("ingestion_directory", new DirectoryInfo(Path.Combine(builder.Environment.WebRootPath, "Data")));


services.AddChatClient(chatClient)
        .UseFunctionInvocation() // Important for PDF/Document Search!
        .UseLogging();


services.AddEmbeddingGenerator(embeddingGenerator);






var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();