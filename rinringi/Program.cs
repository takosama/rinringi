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

ringi.Circulate("きのこの山とたけのこの里を合体させた商品を作りたいです。会社員になったつもりで議論してください。最終的に賛成か反対かを具体的な理由を添えて結論を出してください。");
