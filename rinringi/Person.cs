namespace Rinringi;

class Person
{
    private readonly OpenAIClient api;

    public string Name { get; set; }
    public int Age { get; set; }
    public string Background { get; set; }
    public string SpeakingStyle { get; set; }
    public int Tier { get; set; }
    public bool IsApprover { get; set; }

    public Person(OpenAIClient api, string name, int age, string background, string speakingStyle, int tier, bool isApprover)
    {
        this.api = api;
        Name = name;
        Age = age;
        Background = background;
        SpeakingStyle = speakingStyle;
        Tier = tier;
        IsApprover = isApprover;
    }

    public string Introduce() => $"私は{Name}、{Age}歳です。{Background}";

    public string Speak(string topic, IReadOnlyList<Speech> transcript)
    {
        string transcriptText = transcript.Count == 0
            ? "まだ発言はありません。"
            : string.Join("\n", transcript.Select(s => $"{s.Speaker}: {s.Content}"));

        string roleInstruction = IsApprover
            ? "あなたはこの会議の決裁者として発言する。部下の意見を踏まえて方針を示し、議論をまとめる方向で話す。"
            : "すでに出た意見を少し受けて、自分の観点を足す。";

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

        return string.IsNullOrWhiteSpace(answer)
            ? "発言を生成できませんでした。"
            : answer.Trim();
    }
}
