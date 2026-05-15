using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Rinringi;

class OpenAIClient(string url, string model)
{
    public string Url { get; } = url;
    public string Model { get; } = model;

    private static readonly HttpClient httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(120)
    };

    public string? Complete(IEnumerable<ChatMessage> messages)
    {
        string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
            return "OPENAI_API_KEY がありません。環境変数にAPIキーを設定してください。";

        string json = JsonSerializer.Serialize(new { model = Model, messages });

        const int maxRetries = 4;
        int delay = 1000;

        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, Url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using HttpResponseMessage response = httpClient.SendAsync(request).GetAwaiter().GetResult();
                string responseText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    string? content = ExtractContent(responseText);
                    if (!string.IsNullOrWhiteSpace(content) || attempt == maxRetries)
                        return content;
                }
                else
                {
                    int status = (int)response.StatusCode;
                    if ((status != 429 && status < 500) || attempt == maxRetries)
                        return $"APIエラー {status}:\n{responseText}";
                }
            }
            catch (Exception ex)
            {
                if (attempt == maxRetries)
                    return "通信または処理中の例外:\n" + ex.Message;
            }

            Thread.Sleep(delay + Random.Shared.Next(0, 500));
            delay *= 2;
        }

        return null;
    }

    private static string? ExtractContent(string json)
    {
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        if (!root.TryGetProperty("choices", out JsonElement choices) || choices.GetArrayLength() == 0)
            return null;

        JsonElement first = choices[0];
        if (!first.TryGetProperty("message", out JsonElement message))
            return null;
        if (!message.TryGetProperty("content", out JsonElement content))
            return null;

        return content.GetString();
    }
}
