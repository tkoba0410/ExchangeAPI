# ユーザー視点（DX）使い勝手レビュー

## 1. TL;DR
- 現状は「設計規範の明確さ」が非常に強く、長期運用での一貫性は高い。
- 一方で初見利用者には、最初の成功体験（5分で1回叩く）への導線が不足している。
- README は設計文書への案内としては優秀だが、利用開始手順としては抽象度が高い。
- Facade API の命名・戻り値形状は統一され、複数取引所対応の学習コストは抑えられている。
- `Call<TReq,TRes>` による成功/失敗の表現は、Bot/分析用途での自動判定に向いている。
- ただしエラー種別は `CallErrorKind` が粗く、認証・レート制限・契約違反の判別を呼び出し側で補う必要がある。
- 型設計は ValueObject 化が進み、安全性は高いが、簡単な用途では記述量が増えやすい。
- レート制限・リトライのポリシーは実装上は明確で、運用パラメータも調整可能。
- バージョニング面では「安定保証は Contract 層のみ」の境界は明示されている。
- 総評: 中〜上級開発者には堅牢、初見導入のハードルはやや高い。

## 2. 初見利用者の詰まりポイント Top5（重要度順）

### 1) 「最初に何を実行すればよいか」が README だけでは分からない
README が docs 導線中心で、利用開始用の最小コード・実行例・インストール手順が存在しないため、初回成功体験までに探索コストが発生する。
- Evidence: README.md
- Evidence: docs/index.md

### 2) Public/Private クライアント生成と資格情報設定の関係がドキュメントから辿りづらい
実装上は `BitflyerFactory` / `BittradeFactory` と CredentialProvider 群があるが、利用者向け手順としてまとまっていない。
- Evidence: src/Exchanges/Bitflyer/Composition/BitflyerFactory.cs
- Evidence: src/Exchanges/Bittrade/Composition/BittradeFactory.cs
- Evidence: src/Composition/Providers/Credentials/EnvironmentVariableApiCredentialProvider.cs

### 3) 「Docs をどこまで読めば使い始められるか」が判断しづらい
Normative/Reference/Inventory の整理は厳密だが、利用者目線の最短読了パス（例: 3ファイルで開始）が定義されていない。
- Evidence: README.md
- Evidence: docs/index.md
- Evidence: docs/reference/navigation.md

### 4) エラー分類が呼び出し判断に対して粒度不足
`CallErrorKind` が Transport/Http 等の技術分類中心で、認証・レート制限・一時障害の直接判定には補助情報の解釈が必要。
- Evidence: src/Primitives/CallCommon/CallErrorKind.cs
- Evidence: src/Primitives/CallCommon/CallError.cs
- Evidence: docs/normative/contracts/resilience.md

### 5) 文字列入口を閉じる型設計は安全だが、簡易利用での負担が増える
`Symbol`/`Period` などの専用型化は有効だが、スクリプト的利用では都度型化が必要になり、書き始めの負担がある。
- Evidence: src/Primitives/DomainCommon/Types/Symbol.cs
- Evidence: src/Primitives/DomainCommon/Types/Period.cs
- Evidence: src/Contracts/Facade/Requests/TickerRequest.cs

## 3. 良い点（具体例つき）

### 3.1 API 一貫性が高い（学習の再利用が効く）
`IPublicApi` / `IPrivateApi` のメソッド命名、`Task<Call<Req,Res>>` という戻り値形状、`CancellationToken` の位置が揃っており、利用者が横展開しやすい。
- Evidence: src/Contracts/Facade/Interfaces/IPublicApi.cs
- Evidence: src/Contracts/Facade/Interfaces/IPrivateApi.cs

### 3.2 省略呼び出し用の Extension があり、記述量を減らせる
Facade Request DTO を毎回 new しなくても、`Symbol` / `Period` から直接呼べる拡張メソッドが提供されている。
- Evidence: src/Contracts/Facade/Extensions/PublicApiExtensions.cs
- Evidence: src/Contracts/Facade/Extensions/PrivateApiExtensions.cs

### 3.3 レジリエンス方針が契約と実装の両面で明確
429 の `Retry-After` 優先、指数バックオフ+ジッター、最大再試行時間などが文書化され、`RetryHttpPolicy` 実装にも反映されている。
- Evidence: docs/normative/contracts/resilience.md
- Evidence: src/Transport/Policy/RetryHttpPolicy.cs
- Evidence: src/Transport/Policy/HttpPolicyOptions.cs

### 3.4 安定保証の境界が明示されている
「公開安定 API は Contract 層のみ」と明言されており、利用者が依存すべき層を判断しやすい。
- Evidence: docs/normative/contracts/overview.md
- Evidence: docs/index.md

### 3.5 認証情報の供給手段に選択肢がある
直接 credentials 指定、環境変数、ファイル、複合プロバイダーを選べるため、運用形態に合わせやすい。
- Evidence: src/Composition/Providers/Credentials/EnvironmentVariableApiCredentialProvider.cs
- Evidence: src/Composition/Providers/Credentials/FileApiCredentialProvider.cs
- Evidence: src/Composition/Providers/Credentials/CompositeCredentialProvider.cs

## 4. 改善提案（P0 / P1 / P2）

> ※ ここでは提案のみを記載し、実装は行わない。

### P0: 「最小導入ガイド（5分で1成功）」を docs/reference/reviews ではなく利用者向け導線に明示
- 期待効果: 初見離脱率の低下。Bot/分析/ツール開発いずれも最初の成功体験が速くなる。
- 副作用: ドキュメント保守対象が増える。
- 移行コスト: 低（既存 Factory/Extension の利用例を整理する中心）。
- Evidence: README.md
- Evidence: src/Exchanges/Bitflyer/Composition/BitflyerFactory.cs
- Evidence: src/Contracts/Facade/Extensions/PublicApiExtensions.cs

### P1: エラー判定の利用者向けガイドを追加（認証・429・通信断の判別フロー）
- 期待効果: リトライ判断と障害切り分けが容易になり、運用時のMTTR短縮。
- 副作用: 実装変更時にガイド更新が必要。
- 移行コスト: 低〜中（現行の `CallError` と `ExchangeApiException` の読み解き整理）。
- Evidence: src/Primitives/CallCommon/CallError.cs
- Evidence: src/Primitives/Errors/ExchangeApiException.cs
- Evidence: docs/normative/contracts/resilience.md

### P1: 型化方針の「実務的な使い分け例」を提示
- 期待効果: `Parse` / `TryParse` / `ParseOrThrow` の選択が明確になり、実装スタイルの揺れを抑制。
- 副作用: 記載が多すぎると逆に学習負荷増。
- 移行コスト: 低。
- Evidence: src/Primitives/DomainCommon/Types/Symbol.cs
- Evidence: src/Primitives/DomainCommon/Types/Period.cs
- Evidence: docs/normative/contracts/contracts.md

### P2: ドキュメント読了順を「利用目的別」に分岐
- 期待効果: 分析者は必要最小限、注文系開発者は安全性重視の導線に分けられ、探索コストを削減。
- 副作用: 導線が複数化し、更新時の整合確認が必要。
- 移行コスト: 低。
- Evidence: docs/index.md
- Evidence: docs/reference/navigation.md

## 5. 将来利用シナリオでの評価

### 5.1 Bot開発（短周期売買、例外処理と再試行重視）
- 評価: **A-**
- 理由: リトライ/タイムアウト/レート制限ポリシーが実装として整備され、`CancellationToken` も全APIで扱えるため運用しやすい。一方でエラー種別の実務判定は追加実装が必要。
- Evidence: src/Transport/Policy/RetryHttpPolicy.cs
- Evidence: src/Transport/Policy/TimeoutHttpPolicy.cs
- Evidence: src/Contracts/Facade/Interfaces/IPublicApi.cs
- Evidence: src/Primitives/CallCommon/CallErrorKind.cs

### 5.2 分析用途（板・約定・OHLC大量取得、ページング/レート制限重視）
- 評価: **B+**
- 理由: Contract API の統一は扱いやすいが、ページング戦略の利用者向け実践ガイドが少なく、取引所差異は利用者側で設計補完が必要。
- Evidence: docs/normative/contracts/resilience.md
- Evidence: docs/inventory/endpoints-contracts.md

### 5.3 注文系ツール（安全性・整合性重視）
- 評価: **A-**
- 理由: ValueObject による入力型安全、Private/Public capability 分離、安定境界の明示は強い。初期セットアップ（認証情報供給と client 生成）の導線が弱く、導入時に調査が必要。
- Evidence: src/Contracts/Common/Dtos/OrderRequest.cs
- Evidence: src/Contracts/Facade/Interfaces/IExchangeClient.cs
- Evidence: src/Exchanges/Bittrade/Composition/BittradeFactoryOptions.cs
- Evidence: docs/normative/contracts/overview.md

## 6. 利用者向けに欲しい最小サンプル構成（章立て案のみ）
1. **3分セットアップ**（パッケージ参照、最小 using、Public 接続）
2. **最初の1コール**（Ticker取得、`CallResult.Ok/Err` の分岐）
3. **認証あり接続**（Credentials 直接指定 / 環境変数 / ファイル）
4. **注文の基本**（`OrderLimitAsync`、失敗時の判定）
5. **再試行とタイムアウト**（`HttpPolicyOptions` の最小調整）
6. **分析向け連続取得**（OHLC/約定の取得ループ設計の最小形）
7. **テストしやすい利用形**（I/F境界でのモック方針）
8. **安定保証の境界**（どの層に依存すべきか）
- Evidence: src/Contracts/Facade/Interfaces/IPublicApi.cs
- Evidence: src/Contracts/Facade/Interfaces/IPrivateApi.cs
- Evidence: src/Transport/Policy/HttpPolicyOptions.cs
- Evidence: docs/normative/contracts/overview.md
