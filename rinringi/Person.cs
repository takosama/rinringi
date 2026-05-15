namespace Rinringi;

class Person(OpenAIClient api, string name, int age, string background, string speakingStyle, int tier, bool isApprover)
{
    public string Name { get; } = name;
    public int Age { get; } = age;
    public string Background { get; } = background;
    public string SpeakingStyle { get; } = speakingStyle;
    public int Tier { get; } = tier;
    public bool IsApprover { get; } = isApprover;

    public string Introduce() => $"私は{Name}、{Age}歳です。{Background}";

    public string Speak(string topic, IReadOnlyList<Speech> transcript)
    {
        string transcriptText = transcript.Count == 0
            ? "まだ発言はありません。"
            : string.Join("\n", transcript.Select(s => $"{s.Speaker}: {s.Content}"));

        string roleInstruction = IsApprover
            ? "あなたはこの会議の決裁者だ。自分の立場から見えるリスク・課題を数値で問い返し、まだ合意が取れていない重要な論点を明示して判断基準を示す。"
            : "自分の専門・立場から、この議題で本当に問題になる点を自ら見つけ、具体的な数値や事実を挙げて主張する。他の意見に疑問や反論があれば積極的に述べる。全員が同じ意見にまとまる必要はない。";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                "system",
                $"あなたは{Name}（{Age}歳）として会議に参加しています。\n" +
                $"背景: {Background}\n" +
                $"話し方: {SpeakingStyle}\n" +
                $"{roleInstruction}\n" +
                "ルール:\n" +
                "1. 必ずこの人物として発言する。\n" +
                "2. 他の参加者を演じない。\n" +
                "3. 180字以内で話す。\n" +
                "4. 日本語で自然に話す。"
            ),
            new ChatMessage(
                "user",
                $"議題:\n{topic}\n\nこれまでの議事録:\n{transcriptText}\n\nあなたの発言をしてください。"
            )
        };

        string? answer = api.Complete(messages);
        return string.IsNullOrWhiteSpace(answer) ? "発言を生成できませんでした。" : answer.Trim();
    }
}
