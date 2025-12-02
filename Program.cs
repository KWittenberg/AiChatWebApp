using AiChatWebApp.Components;
using AiChatWebApp.Services;
using AiChatWebApp.Services.Ingestion;
using Microsoft.Extensions.AI;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();


// Initialize Ollama API client
string[] modelName = ["all-minilm", "tinyllama", "qwen3:1.7b"];
IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), modelName[2]);
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OllamaApiClient(new Uri("http://localhost:11434"), modelName[0]);


// Configure SQLite Vector Store
string dbName = "vector-store.db";
var vectorStorePath = Path.Combine(AppContext.BaseDirectory, dbName);
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";
builder.Services.AddSqliteVectorStore(_ => vectorStoreConnectionString);
builder.Services.AddSqliteCollection<string, IngestedChunk>(IngestedChunk.CollectionName, vectorStoreConnectionString);


// Register application services
builder.Services.AddSingleton<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();
builder.Services.AddKeyedSingleton("ingestion_directory", new DirectoryInfo(Path.Combine(builder.Environment.WebRootPath, "Data")));


builder.Services.AddChatClient(chatClient)
                .UseFunctionInvocation() // Important for PDF/Document Search!
                .UseLogging();


builder.Services.AddEmbeddingGenerator(embeddingGenerator);

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