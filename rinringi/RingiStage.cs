namespace Rinringi;

class RingiStage
{
    private readonly List<Person> members;
    private readonly int maxRounds;
    private readonly Moderator moderator;

    public RingiStage(IEnumerable<Person> members, int maxRounds, Moderator moderator)
    {
        this.members = members.ToList();
        this.maxRounds = maxRounds;
        this.moderator = moderator;
    }

    public Verdict Deliberate(string topic, string priorConclusion = "")
    {
        string fullTopic = string.IsNullOrWhiteSpace(priorConclusion)
            ? topic
            : $"{topic}\n\n{priorConclusion}";

        foreach (var member in members)
            Console.WriteLine(member.Introduce());

        Console.WriteLine();

        var transcript = new List<Speech>();

        for (int round = 1; round <= maxRounds; round++)
        {
            Console.WriteLine($"--- Round {round} ---");
            Console.WriteLine();

            var speeches = members
                .AsParallel()
                .AsOrdered()
                .Select(p => new Speech(round, p.Name, p.Speak(fullTopic, transcript)))
                .ToList();

            foreach (var speech in speeches)
            {
                transcript.Add(speech);
                Console.WriteLine($"[{speech.Speaker}]");
                Console.WriteLine(speech.Content);
                Console.WriteLine();
            }
        }

        Console.WriteLine("--- まとめ ---");
        Verdict verdict = moderator.Evaluate(fullTopic, transcript);
        Console.WriteLine(verdict.Summary);
        Console.WriteLine();

        return verdict;
    }
}
