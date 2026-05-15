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

ringi.Circulate(
    "「きのこの山」と「たけのこの里」を合わせた新商品を作りたい。\n" +
    "以下の各点について、具体的な数値・根拠を出しながら議論し、採否を決めること。\n" +
    "①商品仕様: 合体形状か混合パックか（形・内容量）\n" +
    "②価格: いくらが妥当か（根拠を示す）\n" +
    "③販売形態: 期間限定か通年か、販売チャネルはどこか\n" +
    "④製造・品質リスク: 具体的にどんな問題が起きうるか、許容できる基準は何か\n" +
    "賛成・反対どちらでも構わないが、各項目に具体的な立場を示すこと。"
);
