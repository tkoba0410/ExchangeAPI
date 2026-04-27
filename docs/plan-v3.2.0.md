# v3.2.0 Realtime Hardening / Venue Onboarding Preparation 実施指示

最終更新: 2026-04-27
位置づけ: v3.2.0 実施指示

状態: implementation started

## 1. 目的

v3.2.0 では、v3.1.0 で追加した bitFlyer public Realtime API read MVP を運用しやすくしつつ、v4.0.0 の new venue public read MVP に向けた準備を行う。

主題は次の 2 つである。

- Realtime hardening
- venue onboarding preparation

v3.2.0 は大きな public surface 拡張 release ではない。
論理性・合理性・可読性を優先し、v3.1.0 の Realtime API を壊さずに、次 release へ進むための足場を整える。

## 2. 採用 Scope

Realtime hardening:

- bitFlyer Realtime live verification runbook の明確化
- public realtime live test の短時間 opt-in 実行条件の明確化
- connection close / cancellation / invalid payload behavior の追加 test
- typed stream 終了時の unsubscribe behavior 固定
- unknown channel / non-channel JSON-RPC response handling の文書と test の再点検
- board snapshot / board delta の sample payload test 強化
- Realtime API usage guide の最小追加
- release / smoke script が Realtime surface を継続確認することの明文化
- private realtime design note の追加

venue onboarding preparation:

- venue onboarding guide
- venue project / folder / namespace checklist
- endpoint matrix template
- deterministic test template
- safe live read verification template
- package / smoke / docs checklist
- v4 venue candidate comparison のための評価項目整理

## 3. 非対象

v3.2.0 では次を扱わない。

- new venue の正式実装
- `Unified` 実装
- Binance realtime
- private realtime
- order / cancel / deposit / withdraw など state-changing operation
- auth / credentials を使う Realtime private channel
- full order book state builder の公開 API
- automatic reconnect / backoff の本格実装
- resubscribe policy の本格実装
- Rx dependency の core package 追加
- `IObservable<T>` public API
- HTTP endpoint contract の破壊的変更
- namespace 全面 rename
- factory API rename
- public DTO / response shape の広範囲変更

補足:
private realtime は v3.2.0 では実装しない。
ただし、v3.3.0 以降で実装判断できるように、auth / credentials / secret-free evidence / channel scope / non-scope を設計 note として文書化する。

## 4. v3.2.0 裁定

- Realtime hardening は、lifecycle / parsing / tests / runbook / guide の範囲に限定する
- reconnect / backoff は v3.2.0 では実装せず、`docs/realtime-bitflyer.md` と roadmap に見送り理由を残す
- board state builder は v3.2.0 では実装せず、snapshot / delta DTO と sample payload test に留める
- Rx optional integration は v3.2.0 では実装せず、optional integration 候補として roadmap に残す
- private realtime は v3.2.0 では実装せず、design note だけを追加する
- v4 venue candidate comparison は正式 venue 選定ではなく、評価項目整理に留める
- onboarding guide は `docs/venue-onboarding.md` に置く

## 5. 文書更新候補

追加候補:

- `docs/venue-onboarding.md`
- `docs/guides/realtime-bitflyer-getting-started.md`
- `verification/bitflyer-realtime-live.md`

更新候補:

- `docs/realtime-bitflyer.md`
- `docs/verification.md`
- `docs/local-nuget-consumer.md`
- `docs/guides/package-publish.md`
- `docs/roadmap-post-v2.md`
- `docs/document-inventory.md`

## 6. 実装 Scope

- Realtime deterministic tests の補強
- Realtime live test の timeout / cancellation behavior 改善
- smoke script の Realtime assertion 継続確認
- sample payload fixture の追加
- typed stream 終了時の unsubscribe 実装

実装しないもの:

- private realtime auth / subscription
- reconnect / backoff / resubscribe
- board state builder
- Rx integration

## 7. Verification

最低限:

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 3.2.0-local.check
bash scripts/smoke-local-nuget-consumer.sh 3.2.0-local.check
dotnet restore ExchangeApi.LiveTests.slnx
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

Realtime live verification を行う場合:

```bash
EXCHANGEAPI_RUN_LIVE_TESTS=1 dotnet test ExchangeApi.LiveTests.slnx --no-restore --filter Realtime
```

live verification は opt-in only とし、default では接続しない。

## 8. 完了条件候補

- v3.2.0 の scope / non-scope が本書に固定されている
- v3.1.0 の Realtime API public surface を壊していない
- Realtime hardening の採用 / 見送りが文書化されている
- v4 venue onboarding に必要な guide / checklist / template がある
- deterministic tests が通る
- package generation が通る
- local consumer smoke が通る
- live tests は opt-in なしで skip する
- secret-free rule が守られている
- new venue / Unified / private realtime / state-changing operation は含まれていない

## 9. 初回実装結果

実装済み:

- typed stream 終了時の best-effort unsubscribe
- unsubscribe lifecycle deterministic tests
- bitFlyer Realtime getting started guide
- bitFlyer Realtime live verification runbook
- private realtime design note
- verification 正本への Realtime live verification 方針追記
- venue onboarding guide
- endpoint matrix template
- deterministic test template
- safe live read verification template
- package / smoke / release checklist
- v4 candidate comparison template

検証結果:

```text
git diff --check passed
dotnet build ExchangeApi.slnx --no-restore passed
dotnet test tests/Exchanges/Bitflyer/Native.Tests/ExchangeApi.Exchanges.Bitflyer.Native.Tests.csproj --no-restore passed
dotnet test tests/Exchanges/Bitflyer/Composition.Tests/ExchangeApi.Exchanges.Bitflyer.Composition.Tests.csproj --no-restore passed
dotnet test ExchangeApi.slnx --no-restore passed
dotnet test ExchangeApi.LiveTests.slnx --no-restore passed; live tests skipped safely without opt-in
bash scripts/pack-local-nuget.sh 3.2.0-local.check passed
bash scripts/smoke-local-nuget-consumer.sh 3.2.0-local.check passed
```

## 10. Venue Onboarding Preparation 結果

追加済み:

- `docs/venue-onboarding.md`
  - venue onboarding scope / non-scope
  - v4 candidate evaluation table
  - endpoint matrix template
  - project / folder / namespace checklist
  - implementation checklist
  - deterministic test template
  - safe live read verification template
  - package / smoke / release checklist
  - v4 candidate comparison template

検証結果:

```text
git diff --check passed
dotnet test ExchangeApi.slnx --no-restore passed
```
