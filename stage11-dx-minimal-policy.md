# Stage11 DX改善 方針（最小スコープ）

## 目的

Stage11 の完了条件を逸脱せずに、Library / CLI / MCP Server を**初見の利用者が実際に使い始められる状態**にするための最小限の DX 改善だけを行う。

本方針の主眼は、使いやすさの磨き込みではなく、利用者が次を自力で達成できる文書を揃えることにある。

- Library を導入し、最小コードで 1 回成功させる
- CLI を起動し、最小コマンドで 1 回成功させる
- MCP Server を起動し、最小 tool call で 1 回成功させる

## 前提

- library 正本は [`docs/spec.md`](docs/spec.md)
- CLI 正本は [`docs/cli.md`](docs/cli.md)
- MCP Server 正本は [`docs/mcp-server.md`](docs/mcp-server.md)
- Stage11 の成果物整理は [`stage11.md`](stage11.md) と [`docs/distribution.md`](docs/distribution.md) を正本とする

本方針は、上記 SSOT を置き換えない。DX 文書は**利用開始に必要な導線と最小例**に限定し、詳細仕様を再記述しない。

## ユーザー中心の原則

DX 文書は「説明文書」ではなく、「利用開始文書」として書く。

各文書は、読者が次の問いに 1 回で答えられることを目標にする。

- これは何ができるのか
- 自分は何を用意すればよいのか
- 何を実行すれば最初の成功になるのか
- 成功したかどうかを何で判断するのか
- 詰まったらどこを見るのか

そのため、各文書は次を守る。

- 1 文書で 1 成果物に集中する
- 最小例はそのまま試せる形で載せる
- 成功時の状態は具体的に書く
- 詳細仕様は正本へリンクし、本文で重複説明しない

## 含める

- Getting Started 3 本
- 他 repo からの利用導線
- public API の最小 XML ドキュメント
- CLI の help / error の最小改善
- MCP tool description の最小改善
- 初期導入の最小トラブルシュート

## 含めない

- 網羅的 XML ドキュメント整備
- 大規模サンプル集
- recipe / FAQ の大量追加
- CLI 機能拡張
- shell completion
- analyzer 導入
- Stage12 の先食い

## 採用する補正方針

元の DX 指示を、そのままではなく現行 repo 方針と整合する形に補正して採用する。

### 1. Library 外部利用の推奨導線

- 外部 consumer 向けの推奨導線は local NuGet feed とする
- [`docs/local-nuget-consumer.md`](docs/local-nuget-consumer.md) を正本とする
- `ProjectReference` は禁止例としては扱わない
- ただし `ProjectReference` は repo 内開発または近接開発向けであり、外部 consumer の推奨導線ではないと明記する

理由:
- [`README.md`](README.md) と [`docs/distribution.md`](docs/distribution.md) は `ProjectReference` を依然として有効な導線として認めている
- ここを一律禁止にすると、Stage11 で確定済みの distribution 方針と衝突する

### 2. Getting Started 文書は利用開始導線に限定する

新規作成する文書:
- `docs/guides/library-getting-started.md`
- `docs/guides/cli-getting-started.md`
- `docs/guides/mcp-getting-started.md`

各文書は次だけを持つ:
- 目的
- 前提
- 導入
- 最小例
- 動作確認

各文書は詳細仕様を再記述せず、必要なら既存正本へリンクする。

各文書が到達させる状態:
- `library-getting-started.md`
  - consumer が依存を追加し、最小コードを実行して 1 回成功できる
- `cli-getting-started.md`
  - 利用者が CLI を build または publish し、1 コマンド成功できる
- `mcp-getting-started.md`
  - 利用者が MCP Server を起動し、client から 1 tool call 成功できる

特に次は必須とする:
- Library は consumer 側 install 手順を含む
- CLI は executable の起動方法を含む
- MCP は server の起動方法と client 設定例を含む
- 動作確認には抽象説明ではなく、1 行の期待結果例を入れる

### 3. XML ドキュメントは allowlist 方式で入れる

Task 3 は diff が膨らみやすいため、対象を明示 allowlist に限定する。

対象:
- client 生成入口
  - `src/Exchanges/Bitflyer/Composition/Factory/BitflyerClientFactory.cs`
  - `src/Exchanges/Binance/Composition/Factory/BinanceClientFactory.cs`
- getting started で使う主要 request / response
  - bitFlyer `GetTickerRequest` / `GetTickerResponse`
  - Binance `GetKlinesRequest` / `GetKlinesResponse`
- getting started から直接触る option / bundle 型があれば、その最小 public entry のみ

原則:
- `<summary>` は必須
- メソッドには `<param>` / `<returns>` を付ける
- 制約がある場合だけ `<remarks>` を使う
- それ以外の DTO へ波及させない

### 4. CLI UX 改善は parser / help / top-level に限定する

CLI help はすでに相当量あるため、最小改善に留める。

改善対象:
- root help
- venue / surface / scope / command help
- parser / binder / top-level の代表的な input error

改善方針:
- 何が問題か
- どう直すか
- 必要なら 1 つの例

ただし、全 command の個別 validation 文言を全面的に磨かない。

### 5. MCP tool description は誤解防止に限定する

全 tool description には次を揃える。
- 何をするか
- 何をしないか
- read / evaluate 用途であること
- 実発注しないこと

ただし、tool behavior の追加変更は行わない。

### 6. トラブルシュートは最小 3 件に限定する

`docs/guides/troubleshooting.md` には次だけを入れる。
- credential が見つからない
- nuget 解決失敗
- venue 指定ミス

網羅的 FAQ にはしない。

## 実施順

1. `docs/guides/*.md` の Getting Started 3 本を追加
2. `docs/local-nuget-consumer.md` を必要最小限で整理
3. allowlist 対象だけに XML ドキュメントを追加
4. CLI help / error を最小改善
5. MCP tool description を統一
6. `docs/guides/troubleshooting.md` を追加
7. 必要な README 導線だけを足す

## 完了条件

以下を満たした時点で、Stage11 の最小 DX 改善は完了とする。

- Getting Started 3 本が存在し、それぞれが最初の成功まで到達できる
- 他 repo 利用ガイドが存在し、consumer が依存導入まで迷わない
- allowlist 対象の public API に XML ドキュメントが付き、IDE 上で最小理解ができる
- CLI が help と代表的 error だけで最低限使える
- MCP tool description が read / evaluate-only であり、実発注しないことを明確に伝える
- 初期導入トラブルが 1 文書で自己解決できる

利用者視点の判定としては、次を満たすことを最終目標とする。

- Library: install -> minimal code -> success を再現できる
- CLI: build/publish -> minimal command -> success を再現できる
- MCP: build/publish -> launch -> minimal tool call -> success を再現できる

## 非目標

この改善で目指すのは Stage11 の導入 DX の底上げであり、以下は目標に含めない。

- 完全な API リファレンス化
- CLI の高度な UX 改善
- bot 向け orchestration recipe
- package 配布戦略の拡張
- 統一された public website / docs portal 化
