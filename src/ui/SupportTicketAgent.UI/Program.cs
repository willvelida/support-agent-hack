using SupportTicketAgent.UI.Components;
using SupportTicketAgent.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register the AG-UI chat service
var agentServerUrl = builder.Configuration["AGENT_SERVER_URL"] ?? "http://localhost:5100";
builder.Services.AddHttpClient<AgentChatService>(client =>
{
    client.BaseAddress = new Uri(agentServerUrl);
    client.Timeout = TimeSpan.FromSeconds(120);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
