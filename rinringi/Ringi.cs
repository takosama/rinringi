namespace Rinringi;

class Ringi
{
    private readonly List<Person> allMembers;
    private readonly Moderator moderator;
    private readonly int roundsPerStage;
    private readonly int maxAttempts;

    public Ringi(IEnumerable<Person> allMembers, int roundsPerStage, Moderator moderator, int maxAttempts = 3)
    {
        this.allMembers = allMembers.ToList();
        this.moderator = moderator;
        this.roundsPerStage = roundsPerStage;
        this.maxAttempts = maxAttempts;
    }

    public void Circulate(string topic)
    {
        Console.WriteLine("========== 稟議開始 ==========");
        Console.WriteLine("議題: " + topic);
        Console.WriteLine();

        string enrichedTopic =
            topic + "\n\n" +
            "自分の専門・立場から見えるリスク・コスト・課題・条件を具体的な数値や事実で示しながら議論し、採否を決めること。" +
            "賛成・反対どちらでも構わないが、自分の判断根拠を明確にすること。";

        int[] tiers = allMembers.Select(p => p.Tier).Distinct().OrderBy(t => t).ToArray();

        Verdict RunStage(int tier, string priorConclusion)
        {
            var stageMembers = allMembers.Where(p => p.Tier == tier).ToList();
            Console.WriteLine($"========== 第{tier}段 ==========");
            Console.WriteLine();
            return new RingiStage(stageMembers, roundsPerStage, moderator).Deliberate(enrichedTopic, priorConclusion);
        }

        var verdicts = new Verdict[tiers.Length];
        verdicts[0] = RunStage(tiers[0], "");

        for (int i = 1; i < tiers.Length; i++)
        {
            verdicts[i] = RunStage(tiers[i], verdicts[i - 1].Summary);

            for (int attempt = 1; !verdicts[i].IsApproved && attempt < maxAttempts; attempt++)
            {
                Console.WriteLine($"========== 第{tiers[i]}段が却下 → 第{tiers[i - 1]}段を再審議（{attempt}回目）==========");
                Console.WriteLine();
                verdicts[i - 1] = RunStage(tiers[i - 1], $"【第{tiers[i]}段より却下】\n{verdicts[i].Summary}");
                verdicts[i] = RunStage(tiers[i], verdicts[i - 1].Summary);
            }

            if (!verdicts[i].IsApproved)
            {
                Console.WriteLine("========== 最終決裁 ==========");
                Console.WriteLine($"【却下】（第{tiers[i]}段で{maxAttempts}回否決されたため審議終了）");
                Console.WriteLine(verdicts[i].Summary);
                return;
            }
        }

        var final = verdicts[tiers.Length - 1];
        Console.WriteLine("========== 最終決裁 ==========");
        Console.WriteLine(final.IsApproved ? "【承認】" : "【却下】");
        Console.WriteLine(final.Summary);
    }
}
