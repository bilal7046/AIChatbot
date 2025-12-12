using AIChatbot.Components;
using AIChatbot.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register chatbot services
builder.Services.AddHttpClient<OpenAIService>();
builder.Services.AddHttpClient<DocumentService>();
builder.Services.AddSingleton<KnowledgeBaseService>();
builder.Services.AddScoped<ChatService>();

var app = builder.Build();

    // Initialize knowledge base
    using (var scope = app.Services.CreateScope())
    {
        var knowledgeBaseService = scope.ServiceProvider.GetRequiredService<KnowledgeBaseService>();
        await knowledgeBaseService.LoadKnowledgeBaseAsync();
    }

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
