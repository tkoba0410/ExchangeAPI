# v3.8.0 Realtime Foundation Inventory / Minimal Contract Hardening 実施指示

最終更新: 2026-04-29
位置づけ: v3.8.0 Realtime Foundation Inventory / Minimal Contract Hardening preparation 指示

状態: preparation / scope decision pending

## 1. 目的

v3.8.0 は、v3.1.0 から v3.7.0 までに実装した bitFlyer Realtime API foundation を棚卸しし、v3 realtime track を閉じるために必要な最小 contract hardening を行う release とする。

目的は、新しい realtime feature を広げることではなく、利用者が次を予測できる状態にすることである。

- stream がいつ継続するか
- stream がいつ終了するか
- malformed payload / unknown channel / transport interruption をどう扱うか
- reconnect / resubscribe 後に何が保証され、何が保証されないか
- diagnostic event / stream envelope / DTO-only stream の関係
- sample payload / replay / deterministic tests が何を固定するか

併せて、後続の開発者や自動化エージェントが自律的に仕様確認・差分判断・検証再現を行えるように、decision / rationale、test gap、fixture rule、verification path を明文化する。

これは Codex 自律性向上を含むが、Codex 固有の仕組みにはしない。
一般化した development autonomy / 開発自律性として扱う。

v3.8.0 では、棚卸しは v3 realtime foundation 全体に対して行うが、修正は v3.7.0 までに入った realtime surface の契約確認に必要な最小範囲へ限定する。

## 2. 採用予定範囲

採用候補:

- lifecycle event contract の再確認と正本化
- stream status event contract の再確認と正本化
- malformed payload handling の固定
- unknown / non-target channel handling の固定
- cancellation / dispose / completion / fault behavior の固定
- reconnect / resubscribe event order の deterministic tests 強化
- private realtime auth replay event order の deterministic tests 強化
- realtime error taxonomy の最小整理
- sample payload catalog rule の固定
- `docs/realtime-bitflyer.md` / `docs/realtime-diagnostics.md` / `docs/spec.md` の整合
- release checklist / release notes の準備
- decision / rationale ledger
- realtime test gap list
- fixture / replay rule の明文化
- v4.0 / v4.1 / v5.0 へ送る項目の分類

## 3. 非対象

v3.8.0 では次を扱わない。

- 新しい realtime channel 追加
- Binance realtime
- venue 横断 realtime abstraction
- Unified realtime abstraction
- board / account / position / order state reconstruction
- HTTP + realtime state coordination
- Gateway / Platform behavior
- simulation
- order / cancel / deposit / withdraw などの state-changing operation
- core / venue package への Rx dependency 追加
- `ExchangeApi.Optional.Reactive` の public API 拡張
- `ExchangeApi.Optional.Testing` の simulation / Gateway / Platform / Strategy testing 拡張
- v4 の Exchange I/O semantics foundation
- SymbolSpec / SizeStep / PriceStep / Capability
- stateless order validation
- error taxonomy の大規模再設計
- HTTP contract / consumer verification catch-up
- CTradeBot 固有導線
- 自動化エージェント専用 tool / config
- broader consumer verification framework

## 4. 作業方針

- v3.8.0 は docs-first で開始する。
- 既存実装と docs の差分を先に棚卸しする。
- 裁定が必要な項目を決めてから implementation scope を固定する。
- public API 追加は原則避け、必要な場合は理由を plan と topic doc に残す。
- exact contract は `docs/realtime-bitflyer.md` / `docs/realtime-diagnostics.md` に置く。
- release scope、非対象、完了条件、裁定理由は本書に置く。
- `docs/roadmap-post-v2.md` は version placement のみを更新する。
- development autonomy は scope 拡大の理由にしない。
- v3.8.0 では Realtime foundation に直接関係しない consumer verification は扱わない。

v3.8.0 に入れてよいものは、次をすべて満たすものに限定する。

1. Realtime foundation に直接関係する
2. v3.7.0 までに入れた surface の棚卸し・契約固定・検証再現である
3. docs / tests / scripts / verification の範囲に収まる
4. public API を原則増やさない
5. v4 / v5 の設計判断を先取りしない

## 5. 関連正本

- [`docs/spec.md`](./spec.md)
- [`docs/realtime-bitflyer.md`](./realtime-bitflyer.md)
- [`docs/realtime-diagnostics.md`](./realtime-diagnostics.md)
- [`docs/verification.md`](./verification.md)
- [`docs/roadmap-post-v2.md`](./roadmap-post-v2.md)
- [`docs/execution-boundary-policy.md`](./execution-boundary-policy.md)
- [`verification/bitflyer-realtime-live.md`](../verification/bitflyer-realtime-live.md)
- [`verification/bitflyer-private-realtime-live.md`](../verification/bitflyer-private-realtime-live.md)
- [`verification/bitflyer-realtime-resilience.md`](../verification/bitflyer-realtime-resilience.md)

## 6. 初期実装順

1. `docs/realtime-bitflyer.md` と `docs/realtime-diagnostics.md` の lifecycle / error / continuation rule を棚卸しする
2. 裁定項目を本書に追記し、採用案を固定する
3. topic doc に exact contract を反映する
4. deterministic tests の不足を洗い出す
5. 必要最小限の implementation / tests を追加または修正する
6. local package / consumer smoke への影響を確認する
7. release checklist / release notes を追加する
8. release preflight を実行する

## 7. 裁定が必要な項目

### 7.1 Lifecycle 正本の置き場所

決めること:

- lifecycle / continuation rule の正本を `docs/realtime-bitflyer.md` に寄せるか、`docs/realtime-diagnostics.md` に寄せるか。

論点:

- `docs/realtime-bitflyer.md` は bitFlyer-specific channel / client / stream contract に近い。
- `docs/realtime-diagnostics.md` は diagnostic event / secret-free observability に近い。
- 重複すると contract drift が起きやすい。

初期推奨:

- bitFlyer-specific stream contract は `docs/realtime-bitflyer.md` に置く。
- diagnostic event vocabulary / logging / evidence は `docs/realtime-diagnostics.md` に置く。
- 両文書で同じ表を重複保持しない。

### 7.2 DTO-only stream と envelope stream の関係

決めること:

- DTO-only stream の error / continuation behavior を envelope stream とどこまで揃えるか。

論点:

- DTO-only stream は簡単に使える typed data stream である。
- envelope stream は lifecycle event を混ぜられるため、recoverable event を表現できる。
- DTO-only stream に lifecycle event を混ぜると API の単純さが壊れる。

初期推奨:

- DTO-only stream は data only を維持する。
- recoverable diagnostic / lifecycle を見たい利用者は envelope stream を使う。
- DTO-only stream の unrecoverable failure は controlled exception として終了する。

### 7.3 malformed payload の扱い

決めること:

- malformed JSON / DTO decode failure を継続可能な `MessageRejected` とするか、stream fault とするか。

論点:

- WebSocket 接続が切れたわけではない壊れた message は、stream 全体を終了させない方が実運用に強い。
- ただし DTO-only stream は `MessageRejected` を流せない。
- 連続性が壊れた可能性を利用者が把握できる必要がある。

初期推奨:

- envelope stream では `MessageRejected` を出して原則継続する。
- `MessageRejected` は continuity-impacting event として扱う。
- DTO-only stream の扱いは別途裁定する。

### 7.4 unknown / non-target channel の扱い

決めること:

- subscribed target 以外の channel message を ignore するか、diagnostic event として流すか。

論点:

- multi-subscription / reconnect では想定外 message が混ざる可能性がある。
- 全てを fault にすると長時間運用に弱い。
- diagnostic が多すぎると利用者にとって noisy になる。

初期推奨:

- non-target channel は ignore を基本にする。
- envelope stream で diagnostic に出すかは、実装コストと有用性を見て裁定する。

### 7.5 cancellation / dispose / normal completion / remote close

決めること:

- caller cancellation、client dispose、normal completion、remote close、transport exception の terminal behavior を固定する。

論点:

- `IAsyncEnumerable<T>` の利用者が `await foreach` の終了理由を予測できる必要がある。
- cancellation を error 扱いにすると通常の停止処理が扱いづらい。
- remote close は正常終了ではなく、再接続対象か fault として明示した方がよい。

初期推奨:

- caller cancellation / dispose は normal completion。
- remote close / transport exception は envelope stream では reconnect target。
- reconnect exhausted は controlled exception。

### 7.6 reconnect / resubscribe event order

決めること:

- public / private reconnect sequence の event order を固定する。

論点:

- private realtime は reconnect 後に auth replay が必要。
- reconnect 後は gap-free continuity を保証できない。
- 利用者は `ContinuityLost` を見て resync / invalidation を判断する。

初期推奨:

```text
public:
  Reconnecting
  Reconnected
  Resubscribed
  ContinuityLost
  Data...

private:
  Reconnecting
  Reconnected
  AuthenticationReplayed
  Resubscribed
  ContinuityLost
  Data...
```

### 7.7 realtime error taxonomy

決めること:

- `BitflyerRealtimeErrorKind` を v3.8.0 でどこまで整理するか。

論点:

- 利用者は exception message 文字列ではなく error kind で判断したい。
- 大きな taxonomy 再設計は v3.8 の scope を超えやすい。
- secret-free exception rule を守る必要がある。

初期推奨:

- 既存 `BitflyerRealtimeErrorKind` を最小補強する。
- venue 横断 error taxonomy にはしない。
- secret-free exception / result rule を deterministic tests で確認する。

### 7.8 sample payload catalog rule

決めること:

- sample payload catalog を contract fixture として扱う範囲を固定する。

論点:

- sample payload は decode / replay / diagnostic の再現性に効く。
- scenario catalog や simulation input に広げると `Optional.Testing` の責務を超える。
- secret / private auth payload を fixture に含めない規則が必要。

初期推奨:

- sample payload catalog は decode / replay / diagnostic fixture に限定する。
- simulation / Gateway / Platform / Strategy testing へ拡張しない。
- private auth payload、API key、signature、credential path は含めない。

### 7.9 public API 変更の許容範囲

決めること:

- v3.8.0 で public API を追加・変更してよい条件を固定する。

論点:

- v3.8 は hardening release であり、新 feature 拡張ではない。
- 既存 behavior を test で固定するだけで足りる可能性がある。
- public API を増やすと release scope が大きくなる。

初期推奨:

- 原則として public API 追加はしない。
- docs と実装が矛盾している場合、既存 contract を明確化する最小変更だけ許容する。

### 7.10 v3.9.0 への送り出し条件

決めること:

- v3.8.0 で何が固まれば v3.9.0 の verification / release close に進めるか。

論点:

- v3.9 は v3 realtime foundation を閉じる release として予定されている。
- v3.8 で lifecycle / contract が曖昧なままだと v3.9 が release checklist 整理だけで閉じられない。

初期推奨:

- lifecycle / continuation / reconnect / error kind / sample payload rule が docs と tests で固定されていることを v3.9 への入口条件にする。

## 8. Verification 候補

最低限:

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 3.8.0-local.lifecycle
bash scripts/smoke-local-nuget-consumer.sh 3.8.0-local.lifecycle
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check
```

追加候補:

- realtime envelope event order tests
- malformed payload tests
- unknown / non-target channel tests
- cancellation / dispose terminal behavior tests
- reconnect exhausted error kind tests
- private auth replay event order tests
- sample payload catalog validation tests
- secret-free exception / diagnostic tests

## 9. 完了条件候補

- `docs/plan-v3.8.0.md` が scope / non-scope / 裁定 / verification を固定している
- lifecycle / continuation rule の正本が一箇所に整理されている
- DTO-only stream と envelope stream の関係が明確である
- malformed payload / unknown channel / remote close / cancellation の挙動が明確である
- reconnect / resubscribe / auth replay / continuity loss の event order が明確である
- realtime error taxonomy が v3.8.0 の範囲で最小整理されている
- sample payload catalog rule が固定されている
- deterministic tests が通る
- local consumer smoke が通る
- live tests が opt-in なしで skip する
- v3.8.0 に新 channel / Binance realtime / Unified / state reconstruction / Gateway / Platform behavior が含まれていない
