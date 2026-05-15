using System.Text;
using DotNetEnv;
using Rinringi;

Console.OutputEncoding = Encoding.UTF8;
Env.Load();

var api = new OpenAIClient(
    "https://api.openai.com/v1/chat/completions",
    "gpt-5.5"
);

var members = ParticipantLoader.Load("participants.csv", api);
var moderator = new Moderator(api);
var ringi = new Ringi(members, roundsPerStage: 2, moderator);

string topic = args.Length > 0 ? string.Join(" ", args) : null!;
if (string.IsNullOrWhiteSpace(topic))
{
    Console.Write("議題: ");
    topic = Console.ReadLine() ?? "";
}
if (string.IsNullOrWhiteSpace(topic))
{
    Console.Error.WriteLine("議題が入力されていません。");
    return;
}
ringi.Circulate(topic);
