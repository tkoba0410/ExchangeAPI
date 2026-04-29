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

v3.8.0 では、human convenience ではなく machine-verifiable contract を優先する。
後続開発者や自動化エージェントが、追加の暗黙知なしに仕様確認・差分判断・検証再現できることを重視する。

優先する:

- 明示性
- 再現性
- 機械検証性
- event sequence
- fixture / replay input
- deterministic tests

優先しない:

- human-friendly convenience helper
- UI / CLI の親切機能
- 直感的だが曖昧な挙動
- silent recovery
- v3.8.0 の目的に直接関係しない public API sugar

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

採用:

- 案Cを採用する。
- bitFlyer stream contract は `docs/realtime-bitflyer.md` に置く。
- diagnostic vocabulary / secret-free observability は `docs/realtime-diagnostics.md` に置く。
- 将来 venue を見越すのは、diagnostic vocabulary と責務境界までに限定する。
- 未実装 venue の channel / DTO / lifecycle quirks は先取りしない。

理由:

- 現行 Realtime 実装は bitFlyer のみであり、実装・正本は現行 bitFlyer contract を中心にする方が正確である。
- 一方で `MessageRejected` / `ContinuityLost` / severity / secret-free rule などの診断語彙は venue-neutral に扱いやすく、将来 venue にも流用しやすい。
- 未実装 venue の仕様を仮定して stream contract を一般化すると、存在しない要件で現行 contract が歪む。

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

採用:

- envelope stream を lifecycle / diagnostics / continuity の正本 API とする。
- DTO-only stream は data-only convenience API として維持する。
- 後続開発者、自動化エージェント、上位 consumer が検証可能性を必要とする場合は envelope stream を使う。
- DTO-only stream を continuity-aware API として扱わない。
- DTO-only stream で `MessageRejected` / `ContinuityLost` 相当を silent recovery として隠さない。

理由:

- v3.8.0 は human convenience より machine-verifiable contract を優先する。
- envelope stream は `Data` / `MessageRejected` / `ContinuityLost` / `Reconnecting` / `Reconnected` / `Resubscribed` などを event sequence として検証できる。
- DTO-only stream は簡単な read には有用だが、lifecycle / diagnostics / continuity 判断を明示できない。
- Codex を含む自動化エージェントにとって、暗黙の回復や欠落は差分判断・検証再現を難しくする。

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

採用:

- envelope stream では malformed JSON / DTO decode failure を `MessageRejected` event として流す。
- envelope stream は原則継続する。
- `MessageRejected` は continuity-impacting event として扱う。
- `MessageRejected` は raw payload、API key、API secret、signature、Authorization 相当値、private auth payload を持たない。
- DTO-only stream は `MessageRejected` を表現できないため、continuity-aware API として扱わない。
- DTO-only stream で malformed payload を silent recovery として隠さない。
- DTO-only stream の実装上の細部は現状を棚卸しし、必要なら最小 contract と deterministic tests で固定する。

理由:

- malformed payload は connection failure ではないため、envelope stream では stream 全体を直ちに fault させるより、`MessageRejected` として明示して継続する方が運用・検証に適する。
- ただし data 欠落は起きているため、`MessageRejected` を continuity-impacting event として扱う。
- silent recovery は v3.8.0 の machine-verifiable contract 方針に反する。
- DTO-only stream は lifecycle / diagnostics / continuity 判断を表現できないため、運用判断には envelope stream を使う。

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

採用:

- non-target / unknown channel は target data として流さない。
- non-target / unknown channel は stream fault にしない。
- silent recovery は避けるため、envelope stream では diagnostic として観測可能にする。
- DTO-only stream は data-only convenience API なので、non-target / unknown channel を表現しない。
- `MessageRejected` は malformed / decode failure に限定する。
- non-target / unknown channel 用の event type 追加は、既存実装と diagnostic vocabulary を棚卸しして最小判断する。

理由:

- 対象外 channel message は target data ではないため、typed data stream に混ぜない。
- 一方で、想定外 message が来た事実を完全に隠すと、後続開発者や自動化エージェントが transport / subscription の異常を検証しにくい。
- `MessageRejected` は target message の malformed / decode failure に限定する方が、event semantics が明確になる。
- v3.8.0 では event type 追加を前提にせず、既存 vocabulary と実装の棚卸し後に最小判断する。

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

採用:

- caller cancellation は normal completion とする。
- client dispose は normal completion とする。
- normal completion は completion とする。
- remote close は reconnect target とする。
- transport exception は reconnect target とする。
- idle timeout は configured only とし、発生時は reconnect target とする。
- reconnect exhausted は controlled exception とする。
- auth failure は controlled exception とする。
- resubscribe failure は controlled exception とする。
- failure reason / exception message / diagnostic は secret-free とする。

envelope stream:

- cancellation / dispose は normal completion とする。
- remote close / transport exception / idle timeout は reconnect target とする。
- reconnect が成功した場合は `ContinuityLost` を出す。
- reconnect が尽きた場合は controlled exception として終了する。

DTO-only stream:

- cancellation / dispose は normal completion とする。
- lifecycle / reconnect / continuity は表現しない。
- reconnect / remote close の細部は現状を棚卸しし、必要なら最小 contract と deterministic tests で固定する。
- continuity-aware API として扱わない。

理由:

- caller cancellation / dispose は利用者の意思による停止であり、error として扱わない。
- remote close / transport exception は利用者の意思ではないため、normal completion と同一視しない。
- envelope stream は reconnect / continuity event を表現できるため、remote close / transport exception を reconnect target として扱える。
- reconnect 後の gap-free continuity は保証しないため、`ContinuityLost` が必要である。
- reconnect exhausted / auth failure / resubscribe failure は継続不能なので controlled exception とする。

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

採用:

- 案Aを採用する。
- public reconnect sequence は次の順序で固定する。

```text
Reconnecting
Reconnected
Resubscribed
ContinuityLost
Data...
```

- private reconnect sequence は次の順序で固定する。

```text
Reconnecting
Reconnected
AuthenticationReplayed
Resubscribed
ContinuityLost
Data...
```

補助ルール:

- `ContinuityLost` は recovery success 後、最初の `Data` 前に出す。
- public recovery success は reconnect success + resubscribe success とする。
- private recovery success は reconnect success + auth replay success + resubscribe success とする。
- recovery success 前に reconnect exhausted / auth failure / resubscribe failure が発生した場合は controlled exception で終了し、`ContinuityLost` は必須ではない。
- resubscribe success 後、最初の `Data` 前に再度 transport interruption が発生した場合は、`ContinuityLost` を出した上で次の reconnect sequence に入ってよい。
- idle timeout は transport interruption と同じ reconnect target として扱う。
- remote close は caller cancellation ではないため reconnect target として扱う。
- event sequence は deterministic tests で固定する。
- DTO-only stream ではこの sequence を表現しない。

理由:

- transport recovery -> subscription recovery -> continuity warning -> data resumes の順序が最も機械検証しやすい。
- `ContinuityLost` を data 再開前に出すことで、後続開発者、自動化エージェント、上位 consumer が resync / invalidation 要否を判断しやすい。
- private stream では auth replay が resubscribe の前提であり、順序を明確にすることで secret-free auth replay contract も検証しやすくなる。
- recovery success 前の failure に `ContinuityLost` を必須化しないことで、復帰していない stream と復帰したが連続性を失った stream を区別できる。

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

採用:

- 既存 `BitflyerRealtimeErrorKind` を最小補強する。
- venue-neutral `RealtimeErrorKind` は作らない。
- error taxonomy の大規模再設計はしない。
- HTTP と統一した error taxonomy は v3.8.0 では扱わない。
- Unified 前提の分類は行わない。
- Gateway / Platform 向け error model は扱わない。
- exception / diagnostic / result は secret-free とする。
- known category に入らないものは `Unknown` / `Unclassified` 相当の fallback を許容する。
- 既存 enum に fallback がない場合は、棚卸し後に追加要否を最小判断する。

v3.8.0 で十分な分類範囲:

- transport: remote close / socket failure / network failure / idle timeout
- reconnect: reconnect attempts exhausted
- subscription: subscribe failed / resubscribe failed
- authentication: private auth failed / auth replay failed
- decode: malformed JSON / DTO decode failure
- protocol: invalid JSON-RPC shape / unexpected response shape
- fallback: unknown / unclassified

固定する性質:

- error kind は machine-readable とする。
- exception message は human-readable でもよいが secret-free とする。
- diagnostic attributes は narrow / structured / secret-free とする。
- raw payload は exception / diagnostic に含めない。
- deterministic tests で期待 kind と secret-free rule を確認する。

理由:

- v3.8.0 で必要なのは、controlled exception を機械的に判定できる最低限の分類である。
- 現行 Realtime 実装は bitFlyer のみであり、venue-neutral taxonomy を作ると未実装 venue の仕様を先取りする。
- Codex を含む自動化エージェントにとっては、exception message より error kind の方が差分判断・検証再現に向いている。
- 分類不能を分類不能として扱える fallback がある方が、過剰分類より安全である。

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

採用:

- sample payload catalog は contract fixture として扱う。
- 用途は decode / replay / diagnostic classification / envelope event sequence / secret-free verification に限定する。
- scenario catalog にはしない。
- simulation には使わない。
- state reconstruction には使わない。
- Gateway / Platform / Strategy testing には拡張しない。
- CTradeBot 固有 fixture は作らない。
- fixture 本体はまず repo 内 deterministic tests 用に置く。
- `ExchangeApi.Optional.Testing` は helper を提供し、fixture 本体を大量に package へ含めることは避ける。

配置方針:

```text
tests/Exchanges/Bitflyer/Fixtures/Realtime/
```

fixture に入れないもの:

- API key
- API secret
- signature
- Authorization 相当値
- private auth request payload
- raw credential profile
- credential file path

private event payload は、実 credentials 由来でない sanitized synthetic payload だけ許容する。

v3.8.0 に fixture を追加する判断基準:

1. Realtime contract を検証する
2. deterministic test で使う
3. secret-free である
4. state reconstruction / simulation に使わない

理由:

- sample payload / fixture は、後続開発者や自動化エージェントが live API に依存せず同じ入力で再現検証するために有効である。
- 一方で scenario catalog や simulation に広げると、`ExchangeApi.Optional.Testing` の責務を超え、Gateway / Platform / Strategy testing に近づく。
- v3.8.0 では machine-verifiable contract に必要な fixture だけを扱う。

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

採用:

- public API / `src` 変更は evidence-gated とする。
- 根拠ある変更は許容する。
- 安易な変更は許容しない。

public API / `src` を変更してよい根拠:

- 採用済み裁定を実装・検証するために必要
- docs と実装に明確な矛盾がある
- deterministic test で contract violation が露出した
- secret-free rule 違反を防ぐ必要がある
- machine-verifiable contract に必要な event type / error kind が不足している
- 既存実装が v3.9 close の blocker になる

許容しない変更理由:

- 便利そう
- 将来使いそう
- 人間が読みやすそう
- CTradeBot で使うかもしれない
- API をきれいにしたい
- ついでの整理
- v4 / v5 の先取り
- 未実装 venue を想像した抽象化
- human convenience

public API / `src` を触る場合は、本書に次を残す。

```text
Change:
  何を変えるか

Evidence:
  なぜ必要か
  どの裁定 / test / docs mismatch に基づくか

Scope:
  なぜ最小変更か
  何を含めないか

Verification:
  どの test / script で確認するか
```

理由:

- v3.8.0 は改善余地を閉じないが、scope を Realtime foundation の minimal hardening に制限する必要がある。
- evidence-gated にすることで、根拠ある変更を許容しつつ、便利さ・将来予測・ついでの整理による scope expansion を止められる。
- 後続開発者や自動化エージェントが変更理由と検証方法を追跡できる。

### 7.10 v3.9.0 への送り出し条件

決めること:

- v3.8.0 で何が固まれば v3.9.0 の verification / release close に進めるか。

論点:

- v3.9 は v3 realtime foundation を閉じる release として予定されている。
- v3.8 で lifecycle / contract が曖昧なままだと v3.9 が release checklist 整理だけで閉じられない。

初期推奨:

- lifecycle / continuation / reconnect / error kind / sample payload rule が docs と tests で固定されていることを v3.9 への入口条件にする。

採用:

- 案Aを採用する。
- v3.9 に進める条件は、v3 realtime foundation の contract / verification / roadmap gap が分類済みであることとする。
- v3.8 の完了状態は、すべての gap を潰すことではなく、すべての gap を `v3.8で直す` / `v3.9で閉じる` / `v4+へ送る` / `やらない` に分類することとする。
- 分類されていない gap を残して v3.9 に進まない。

v3.9 に進める具体条件:

- Realtime lifecycle / continuation rule の正本位置が固定されている
- DTO-only stream と envelope stream の関係が固定されている
- malformed payload の扱いが固定されている
- unknown / non-target channel の扱いが固定されている
- cancellation / dispose / remote close / transport failure の扱いが固定されている
- reconnect / resubscribe / auth replay / continuity loss の event order が固定されている
- realtime error taxonomy の v3.8 範囲が固定されている
- sample payload catalog rule が固定されている
- public API / `src` change gate が固定されている
- test gap list が作成されている
- v3.8 で直すものが明確である
- v3.9 close へ送るものが明確である
- v4.0 / v4.1 / v5.0 へ送るものが明確である
- deterministic tests が通る
- local package / consumer smoke が通る
- live tests が opt-in なしで skip する

v3.9 に送るもの:

- release checklist
- release notes
- release preflight
- GitHub Packages smoke
- live verification runbook final check
- evidence / secret-free close check
- v3 realtime docs consolidation

v4+ に送るもの:

- HTTP contract / consumer verification catch-up -> v4.0
- Exchange I/O semantics foundation -> v4.1
- new venue onboarding -> v5.0
- Unified -> v6.0+

理由:

- v3.8.0 は hardening + classification の release であり、すべての gap を潰す release ではない。
- 分類されていない gap を残さないことで、v3.9.0 の release close を機械的に判断しやすくなる。
- v4.0 / v4.1 / v5.0 へ送る項目を明確にすることで、v3.8.0 の scope expansion を防げる。

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
