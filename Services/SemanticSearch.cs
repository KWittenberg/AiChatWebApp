using AiChatWebApp.Services.Ingestion;
using Microsoft.Extensions.VectorData;

namespace AiChatWebApp.Services;

public class SemanticSearch(VectorStoreCollection<string, IngestedChunk> vectorCollection,
                            [FromKeyedServices("ingestion_directory")] DirectoryInfo ingestionDirectory,
                            DataIngestor dataIngestor)
{
    Task? _ingestionTask;

    // public async Task LoadDocumentsAsync() => await (_ingestionTask ??= dataIngestor.IngestDataAsync(ingestionDirectory, searchPattern: "*.*"));

    public async Task<IReadOnlyList<IngestedChunk>> SearchAsync(string text, string? documentIdFilter, int maxResults)
    {
        // Ensure documents have been loaded before searching
        await LoadDocumentsAsync();

        var nearest = vectorCollection.SearchAsync(text, maxResults, new VectorSearchOptions<IngestedChunk>
        {
            Filter = documentIdFilter is { Length: > 0 } ? record => record.DocumentId == documentIdFilter : null,
        });

        return await nearest.Select(result => result.Record).ToListAsync();
    }

    public async Task LoadDocumentsAsync()
    {
        // Ako vector-store.db postoji i nije prazan, preskoči ingestion
        if (File.Exists(Path.Combine(AppContext.BaseDirectory, "vector-store.db")))
        {
            var info = new FileInfo(Path.Combine(AppContext.BaseDirectory, "vector-store.db"));
            if (info.Length > 1024 * 10) // veći od 10KB = vjerovatno sadrži embeddinge
            {
                Console.WriteLine("📂 Dokumenti već probavljeni, preskačem ingestion...");
                return;
            }
        }

        Console.WriteLine("🚀 Pokrećem ingestion novih dokumenata...");
        await (_ingestionTask ??= dataIngestor.IngestDataAsync(ingestionDirectory, searchPattern: "*.*"));
    }
}