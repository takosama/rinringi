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
                "1. 議論の中で実際に争点になった論点を自ら抽出し、\n" +
                "   各論点について「決定: ○○」または「未決: 対立した理由」を箇条書きで示す。\n" +
                "2. 主要な論点に具体的な合意が得られていれば【承認】、\n" +
                "   重要な論点が未解決のまま残っていれば【却下】とする。\n" +
                "3. 各参加者の最終的な立場と理由を簡潔にまとめる。\n" +
                "4. 日本語で話す。\n" +
                "5. まとめの最後の行には必ず「【承認】」または「【却下】」のどちらか一方だけを単独で記載する。"
            ),
            new ChatMessage(
                "user",
                $"議題:\n{topic}\n\n議事録:\n{transcriptText}\n\n全員の意見をまとめ、結論を出してください。"
            )
        };

        string? raw = api.Complete(messages);
        if (string.IsNullOrWhiteSpace(raw))
            return new Verdict("まとめを生成できませんでした。", false);

        string summary = raw.Trim();
        bool isApproved = summary.Contains("【承認】") && !summary.Contains("【却下】");

        return new Verdict(summary, isApproved);
    }
}
