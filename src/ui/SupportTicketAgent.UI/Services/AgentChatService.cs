using Microsoft.Agents.AI;
using Microsoft.Agents.AI.AGUI;
using Microsoft.Extensions.AI;

namespace SupportTicketAgent.UI.Services;

public class AgentChatService
{
    private readonly HttpClient _httpClient;
    private AIAgent? _agent;
    private AgentSession? _session;

    public AgentChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private AIAgent GetAgent()
    {
        if (_agent is null)
        {
            var chatClient = new AGUIChatClient(_httpClient, _httpClient.BaseAddress!.ToString());
            _agent = chatClient.AsAIAgent(
                name: "agui-blazor-client",
                description: "Blazor AG-UI Client Agent");
        }
        return _agent;
    }

    private async Task<AgentSession> GetSessionAsync()
    {
        _session ??= await GetAgent().CreateSessionAsync();
        return _session;
    }

    public async IAsyncEnumerable<string> StreamResponseAsync(List<ChatMessage> messages)
    {
        var agent = GetAgent();
        var session = await GetSessionAsync();

        await foreach (var update in agent.RunStreamingAsync(messages, session))
        {
            foreach (var content in update.Contents)
            {
                if (content is TextContent textContent && !string.IsNullOrEmpty(textContent.Text))
                {
                    yield return textContent.Text;
                }
            }
        }
    }

    public void ResetSession()
    {
        _session = null;
    }
}
