namespace Rinringi;

class Moderator
{
    private readonly OpenAIClient api;

    public Moderator(OpenAIClient api)
    {
        this.api = api;
    }

    public Verdict Evaluate(string topic, IReadOnlyList<Speech> transcript)
    {
        string transcriptText = string.Join("\n", transcript.Select(s =>
            $"[Round {s.Round} / {s.Speaker}] {s.Content}"));

        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                "system",
                "あなたは会議の司会者です。\n" +
                "ルール:\n" +
                "1. 全参加者の意見を公平にまとめる。\n" +
                "2. 各参加者が最終的に賛成か反対か、その理由を箇条書きで整理する。\n" +
                "3. 全体の結論を簡潔に述べる。\n" +
                "4. 日本語で話す。\n" +
                "5. まとめの最後の行には必ず「【承認】」または「【却下】」のどちらか一方だけを単独で記載する。"
            ),
            new ChatMessage(
                "user",
                $"議題:\n{topic}\n\n議事録:\n{transcriptText}\n\n全員の意見をまとめ、結論を出してください。"
            )
        };

        string? raw = api.Complete(messages);
        string summary = string.IsNullOrWhiteSpace(raw) ? "まとめを生成できませんでした。" : raw.Trim();
        bool isApproved = !summary.Contains("【却下】");

        return new Verdict(summary, isApproved);
    }
}
