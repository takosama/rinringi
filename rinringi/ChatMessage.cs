using System.Text.Json.Serialization;

namespace Rinringi;

record ChatMessage(
    [property: JsonPropertyName("role")]    string Role,
    [property: JsonPropertyName("content")] string Content
);
