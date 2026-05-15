namespace Rinringi;

class Ringi
{
    private readonly List<Person> allMembers;
    private readonly Moderator moderator;
    private readonly int roundsPerStage;

    public Ringi(IEnumerable<Person> allMembers, int roundsPerStage, Moderator moderator)
    {
        this.allMembers = allMembers.ToList();
        this.moderator = moderator;
        this.roundsPerStage = roundsPerStage;
    }

    public void Circulate(string topic)
    {
        Console.WriteLine("========== 稟議開始 ==========");
        Console.WriteLine("議題: " + topic);
        Console.WriteLine();

        Verdict RunStage(int tier, string priorConclusion)
        {
            var stageMembers = allMembers.Where(p => p.Tier == tier).ToList();
            string label = tier switch
            {
                1 => "第1段　担当者・係長レベル",
                2 => "第2段　課長レベル",
                3 => "第3段　役員・経営レベル",
                _ => $"第{tier}段"
            };
            Console.WriteLine($"========== {label} ==========");
            Console.WriteLine();
            return new RingiStage(stageMembers, roundsPerStage, moderator).Deliberate(topic, priorConclusion);
        }

        const int maxAttempts = 3;

        Verdict tier1 = RunStage(1, "");

        Verdict tier2 = RunStage(2, tier1.Summary);
        for (int attempt = 1; !tier2.IsApproved && attempt < maxAttempts; attempt++)
        {
            Console.WriteLine($"========== 第2段が却下 → 第1段を再審議（{attempt}回目）==========");
            Console.WriteLine();
            tier1 = RunStage(1, $"【第2段より却下】\n{tier2.Summary}");
            tier2 = RunStage(2, tier1.Summary);
        }

        Verdict tier3 = RunStage(3, tier2.Summary);
        for (int attempt = 1; !tier3.IsApproved && attempt < maxAttempts; attempt++)
        {
            Console.WriteLine($"========== 第3段が却下 → 第2段を再審議（{attempt}回目）==========");
            Console.WriteLine();
            tier2 = RunStage(2, $"【第3段より却下】\n{tier3.Summary}");
            tier3 = RunStage(3, tier2.Summary);
        }

        Console.WriteLine("========== 最終決裁 ==========");
        Console.WriteLine(tier3.IsApproved ? "【承認】" : "【却下】");
        Console.WriteLine(tier3.Summary);
    }
}
