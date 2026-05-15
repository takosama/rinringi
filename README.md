# rinringi

AI が複数の人格を演じ、日本の稟議プロセスをシミュレートする .NET コンソールアプリ。

## 概要

議題を入力すると、CSV で定義した参加者が 3 段階の階層で議論し、司会 AI が採否を判定する。

```
第1段（担当者・係長）→ 第2段（課長）→ 第3段（役員）→ 最終決裁
```

上位段が却下した場合、下位段が再審議する（最大 3 回）。3 回否決されると審議終了。

## 実行方法

```bash
# 環境変数を設定
echo "OPENAI_API_KEY=<your-deepseek-api-key>" > rinringi/.env

# 議題を引数で渡す
dotnet run --project rinringi -- "新商品の企画について"

# または対話入力
dotnet run --project rinringi
```

## 参加者の定義

`rinringi/participants.csv` を編集して参加者を追加・変更できる。

| 列 | 説明 |
|----|------|
| Name | 氏名 |
| Age | 年齢 |
| Background | 経歴・立場（AIへのペルソナ指示） |
| SpeakingStyle | 話し方の特徴 |
| Tier | 審議段（1=担当者、2=課長、3=役員） |
| IsBoss | 各段の決裁者かどうか（true/false） |

Tier の数は自由。CSV に定義された Tier の種類が自動的に審議段数になる。

## アーキテクチャ

```
Program.cs          エントリポイント、APIクライアント初期化、議題受付
Ringi.cs            稟議フロー全体（N段ループ、却下リトライ制御）
RingiStage.cs       1段分の審議（ラウンド制並列発言 → 司会まとめ）
Person.cs           参加者ペルソナ、発言生成
Moderator.cs        議事録を読んで論点整理・採否判定
ParticipantLoader.cs  CSVパーサー
OpenAIClient.cs     OpenAI互換APIクライアント（指数バックオフリトライ付き）
```

## 依存

- .NET 10
- [DotNetEnv](https://github.com/tonerdo/dotnet-env) — `.env` ファイル読み込み
- DeepSeek API（`deepseek-v4-flash`）または OpenAI 互換エンドポイント
