# Public Surface for Bot Consumers

この文書は、Bot 別リポジトリから参照してよい公開面を定義する参考文書です。
基本方針は「取引所別 Normalized を主利用面、Contracts を最小横断面」とする。

## 利用レベル

### Level A（主利用面 / 実用）

- `src/Exchanges/<Exchange>/Normalized/Api`（取引所別 Normalized API）
- `src/Exchanges/<Exchange>/Composition`（当該取引所の構成エントリ）

### Level B（最小横断面 / 補助）

- `src/Contracts/Common`（`ExchangeApi.Contracts.Common`）
- `src/Contracts/Facade`（`ExchangeApi.Contracts.Facade`）

## 禁止する参照先

- `src/Exchanges/*/Wire` / `Raw` / `Adapter` / `Internal`
- `src/Transport` / `src/Utilities` / `src/Application` の内部実装層

## 運用ルール

- Bot の基本実装は取引所別 Normalized API を利用する。
- Contracts は「複数取引所で共通に成立する最小機能」のみを横断利用する。
- Normalized は公開安定契約ではない（互換保証は限定的）ため、更新時は追従前提で運用する。
- 公開安定面の正本は `docs/normative/contracts/contracts.md` と `docs/normative/contracts/overview.md` を参照する。
