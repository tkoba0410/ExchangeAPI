# Public Surface for Bot Consumers

この文書は、Bot 別リポジトリから参照してよい公開面を定義する参考文書です。
基本方針は「取引所別 Normalized を主利用面、Contracts を最小横断面」とする。  
（安定保証の契約条文の正本は `docs/normative/contracts/contracts.md` / `docs/normative/contracts/resilience.md`。
`docs/normative/contracts/overview.md` は概要導線）

## 対象読者

- 取引所機能網羅を必要とする Bot / 高度利用者
- 一般利用者向けの安定契約は `docs/normative/contracts/contracts.md` / `docs/normative/contracts/resilience.md` を正本参照する

## 利用レベル

### Level A（主利用面 / 実用）

- `src/Exchanges/<Exchange>/Normalized/Api`（取引所別 Normalized API）
- `src/Exchanges/<Exchange>/Composition`（当該取引所の構成エントリ）
  - `CreateClient(...)` は Normalized API を返す
  - `CreateContractPublicClient(...)` は最小横断（Contracts, Public）利用向け
  - `CreateContractPrivateClient(...)` は最小横断（Contracts, Private）利用向け

### Level B（最小横断面 / 補助）

- `src/Contracts/Common`（`ExchangeApi.Contracts.Common`）
- `src/Contracts/Facade`（`ExchangeApi.Contracts.Facade`）

## 禁止する参照先

- `src/Exchanges/*/Wire` / `Raw` / `Adapter` / `Internal`
- `src/Transport` / `src/Utilities` / `src/Application` の内部実装層

## 運用ルール

- Bot の基本実装は取引所別 Normalized API を利用する。
- Contracts は「複数取引所で共通に成立する最小機能」のみを横断利用する。
- 取引所差で有無が揺れる機能は、nullable ではなく capability I/F（例: `IContractCandlesticksClient`）で判定する。
- Normalized は公開安定契約ではない（互換保証外）ため、更新時は追従前提で運用する。
- 公開安定面は Contracts 層のみ。契約条文の正本は `docs/normative/contracts/contracts.md` と `docs/normative/contracts/resilience.md` を参照する。

## 非目標

- Contracts で全取引所 API を無理に共通化しない。
- Normalized を公開安定 API として扱わない。
- Raw / Wire / Internal 実装層を外部利用導線に含めない。
