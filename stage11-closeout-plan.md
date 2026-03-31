# Stage11 Closeout Plan

最終更新: 2026-03-31  
対象ブランチ: `stage11`

## 1. 目的

本書は、Stage11 を完了宣言に耐える状態で締めるための最小修正計画を示す。  
主眼は新機能追加ではなく、完了条件、実装、文書、CI 導線の整合を閉じることにある。

前提:

- library 正本は [`docs/spec.md`](docs/spec.md)
- CLI 正本は [`docs/cli.md`](docs/cli.md)
- MCP 正本は [`docs/mcp-server.md`](docs/mcp-server.md)
- [`stage11.md`](stage11.md) は goal document であり、詳細仕様の再記述はしない

## 2. 残項目

Stage11 を締めるうえで、現時点の残項目は次の 5 点に限る。

1. CI workflow が repo 内に存在しない script を参照している
2. [`stage11.md`](stage11.md) の MCP 完了条件が現行実装より広い
3. [`README.md`](README.md) が CLI / MCP を未完了に見せる
4. MCP live test の公式導線が main solution と live-only solution で分かれている
5. 入口文言に軽い未完成感が残っている

## 3. 論理性最優先の解決案

### 3.1 CI

結論:

- **存在しない script を参照する step は削除する**
- CI の責務は、repo 単体で再現可能な build/test gate に限定する

理由:

- 存在しない script を呼ぶ CI は、品質ゲートではなく偽のゲートである
- ダミースクリプトを後付けするより、実際に repo 内で成立する手順に縮める方が論理的である
- Stage11 締め作業の目的は「動く CI」を残すことであり、「将来の lint 構想」を残すことではない

採用形:

- [`ExchangeApi.slnx`](ExchangeApi.slnx) に対して
  - `dotnet restore`
  - `dotnet build --no-restore`
  - `dotnet test --no-build`
- live test は CI の必須 gate に含めない
  - opt-in 前提であり、通常 CI では `SKIP` を前提とする

### 3.2 `stage11.md`

結論:

- **MCP の Stage11 完了条件から `Protocol` raw/debug 経路 requirement を外す**
- CLI の `Protocol` requirement は維持する

理由:

- 現行 MCP は `Composition` 上の tool adapter として成立しており、MCP に protocol raw/debug surface を持たせる必然がない
- MCP に protocol surface を足すのは Stage12 的な拡張であり、Stage11 締め作業の目的に反する
- 現在の [`docs/mcp-server.md`](docs/mcp-server.md) と [`src/Adapters/McpServer`](src/Adapters/McpServer) は、`Native`/`Composition` 主体の read/evaluate tool server として整合している

採用形:

- [`stage11.md`](stage11.md) の goal / completion 条件を次の意味に揃える
  - CLI は `Native` 主経路 + `Protocol` 明示 opt-in
  - MCP は `Composition` 上の tool adapter として成立し、[`docs/mcp-server.md`](docs/mcp-server.md) の tool surface と契約を満たす

### 3.3 README

結論:

- **README の「整備中」表現をやめる**
- README は導線文書に徹し、詳細仕様は各正本へ委譲する

理由:

- 実装済み slice を未完了のように見せるのは、完了宣言と矛盾する
- README が詳細仕様を背負う必要はない
- 入口文書の役割は「何があるか」「どこを見るか」の提示である

採用形:

- [`README.md`](README.md) では
  - CLI と MCP は Stage11 の実装対象であり、現行 branch で利用可能
  - exact contract は [`docs/cli.md`](docs/cli.md) と [`docs/mcp-server.md`](docs/mcp-server.md) を参照
  とだけ述べる

### 3.4 MCP live test 導線

結論:

- **live test の公式 solution は [`ExchangeApi.LiveTests.slnx`](ExchangeApi.LiveTests.slnx) に統一する**
- MCP live test project も live-only solution に追加する
- main solution に live test project が残っていても、説明上の正本は live-only solution とする

理由:

- live test project が存在する以上、live-only solution に収載されない方が不自然である
- main solution に入っていて skip すること自体は問題ないが、live test の実行導線の説明は 1 つに寄せた方が論理的である
- docs ではすでに `tests/Adapters/McpServer.LiveTests` を正式に書いているため、solution 側を合わせるのが最小である

採用形:

- [`ExchangeApi.LiveTests.slnx`](ExchangeApi.LiveTests.slnx) に
  - `tests/Adapters/McpServer.LiveTests/ExchangeApi.Adapters.McpServer.LiveTests.csproj`
  を追加する
- [`docs/mcp-server.md`](docs/mcp-server.md) には
  - adapter live test は `tests/Adapters/McpServer.LiveTests`
  - opt-in 実行時の公式 solution として `ExchangeApi.LiveTests.slnx` を使う
  を短く追記する

### 3.5 wording cleanup

結論:

- **未完成感を示す wording だけを小さく修正する**

理由:

- 実体が完成に近いのに `scaffold` などの語が残ると、文書や help の信頼性を落とす
- ただし大規模な rename や責務変更は不要である

採用形:

- [`src/Adapters/McpServer/Infrastructure/McpApplication.cs`](src/Adapters/McpServer/Infrastructure/McpApplication.cs)
  - `current scaffold` を `current MCP tool surface` などへ変更する
- 必要なら CLI / MCP の help 文言で、scope を誤認させる表現だけを掃除する

## 4. 実施順

1. [`stage11.md`](stage11.md) を更新する  
   MCP 完了条件から `Protocol` requirement を外し、現行実装 slice と goal document を一致させる

2. [`README.md`](README.md) を更新する  
   `整備中` をやめ、CLI / MCP の導線だけを簡潔に残す

3. [`.github/workflows/ci.yml`](.github/workflows/ci.yml) を修正する  
   欠損 script を参照する step を削除し、restore/build/test の最小 gate にする

4. [`ExchangeApi.LiveTests.slnx`](ExchangeApi.LiveTests.slnx) を更新する  
   MCP live test project を追加し、live-only solution を official path にする

5. [`docs/mcp-server.md`](docs/mcp-server.md) を最小更新する  
   live test の official execution path を追記し、solution 記述と矛盾しないようにする

6. [`src/Adapters/McpServer/Infrastructure/McpApplication.cs`](src/Adapters/McpServer/Infrastructure/McpApplication.cs) の wording を掃除する

## 5. 検証手順

最低限の検証:

```bash
dotnet restore ExchangeApi.slnx
dotnet build ExchangeApi.slnx --no-restore
dotnet test ExchangeApi.slnx --no-build
```

live test 導線の確認:

```bash
dotnet restore ExchangeApi.LiveTests.slnx
dotnet build ExchangeApi.LiveTests.slnx --no-restore
dotnet test ExchangeApi.LiveTests.slnx --no-build
```

補足:

- live test は opt-in 未設定なら `SKIP` を許容する
- private live test は read-only に限定し、明示指示なしに実行しない

## 6. 完了判定

Stage11 は、少なくとも次を満たしたときに「締められる」と判断する。

1. CI が repo 単体で成立する
2. [`stage11.md`](stage11.md) と現行 MCP 実装が矛盾しない
3. [`README.md`](README.md) が現況を正しく示す
4. MCP live test の位置づけが文書・project・solution で一致し、official path が一意に説明できる
5. 未完成感を示す wording が締め作業の妨げにならない

## 7. 非ゴール

今回やらないこと:

- MCP に新しい実発注 tool を足すこと
- Stage12 相当の新規拡張を先食いすること
- `Unified` 実装を Stage11 完了条件へ引き上げること
- CLI / MCP / library の責務境界を作り直すこと
