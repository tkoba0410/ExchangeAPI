# 命名チェックリスト（Non-Normative）

本書は命名の揺らぎを検出するためのチェックリストであり、仕様を定義しない。  
正本は `docs/normative/topspec.md` / `docs/naming-rules.md` / `docs/normative/contracts/contracts.md` / `docs/normative/governance.md` とする。

---

## 1. 層語彙の境界

- [ ] `Wire` は I/O 語彙（`Endpoint` / `Path` / `Query` / `Spec`）のみを使う
- [ ] `Raw` は表現語彙（`Raw` / `Json` / `Request` / `Response`）のみを使う
- [ ] `Normalized` は取引所内意味語彙（`Normalized` / 取引所固有型）を使う
- [ ] `Contracts` は取引所非依存語彙のみを使う

## 2. 形式ルール

- [ ] 型名は `PascalCase`
- [ ] メソッド名は `PascalCase`
- [ ] 非同期メソッドは `Async` で終わる
- [ ] ローカル変数・引数は `camelCase`
- [ ] プライベートフィールドは `_camelCase`

## 3. EndpointId 由来ルール

- [ ] `Raw/Normalized` の API は `<EndpointId>CallAsync`
- [ ] `RequestType/ResponseType` は `<EndpointId>` と対応している
- [ ] コレクション応答は `<EndpointId>Response`（コンテナ）/ `<EndpointId>Item`（要素）で命名している
- [ ] ルート配列 Response は `<EndpointId>Response : List<<EndpointId>Item>` の形にしている（該当時）
- [ ] ルート配列要素型は `<EndpointId>Item` になっている
- [ ] EndpointId と異なる独自別名を API 名に混在させていない

## 4. 同義語の統制

- [ ] 同じ意味に複数語（例: `Symbol` と `Market`）を混在させていない
- [ ] `Id` / `OrderId` / `ExchangeOrderId` を意味ごとに使い分けている
- [ ] `Request` / `Response` / `Result` の責務が混線していない
- [ ] `Request` / `Response` は API 境界の第1階層 DTO のみに付与している
- [ ] ルート配列の要素型が `<EndpointId>Item` になっている
- [ ] Internal 補助モデル（送信前エンコード / 受信後中間表現 / 変換専用モデル）の対象が明確に判別できる
- [ ] 第1階層 DTO 以外（ネスト要素 / Internal 補助モデル）に `Request` / `Response` を付けていない
- [ ] Internal 補助モデルに `Payload` / `Body` / `Envelope` / `Document` / `Encoded` / `Item` / `Entry` / `Record` など役割接尾辞を付与している
- [ ] オブジェクト内配列は `IReadOnlyList<T>` で公開している

## 5. 意味と名前の一致

- [ ] フィールド名が実データの意味を表している（例: 時刻値に `Id` を使わない）
- [ ] `Map` / `Resolve` / `Call` / `Get` など動詞が実装責務と一致している

## 6. 例外運用

- [ ] 命名の例外を許容する場合、理由が EndpointId 由来などで説明可能
- [ ] 仕様起因の例外は `docs/process/exceptions.md` に記録する
- [ ] `Exchanges/<Exchange>/Composition` の公開型は取引所プレフィックス付き（例: `BitflyerFactory`）
- [ ] `Exchanges/<Exchange>/Application` の公開型は取引所プレフィックス付き
- [ ] 取引所プレフィックスを排除した結果、型衝突が起きる場合は意味名で回避している（例: `ExchangeSide`, `ExchangeSymbol`）
- [ ] 衝突回避のために取引所名プレフィックスへ戻す前に、役割語での解決を検討している
- [ ] 衝突回避の意味名が優先順 `Contract -> Exchange -> Normalized -> Raw -> Wire` に従っている
- [ ] 型衝突時に、上位層で逃がさず下位層の型名を修正して解消している
- [ ] `using alias` による衝突回避は暫定対応としてのみ使用している

## 7. 機械チェック（推奨）

- `rg -n "ExchangeApi\\.Exchanges\\..*\\.Api\\." src tests docs`
- `rg -n "\\b(Market|Symbol)\\b" src/Contracts/Facade/Requests src/Contracts/Facade/Extensions`
- `rg -n "\\bOrderId Id\\b|\\bFreeText Id\\b" src/Exchanges`
- `rg -n "Get[A-Za-z]+CallAsync" src/Exchanges/*/{Raw,Normalized}`
- `rg -n "Raw.*(Request|Response)" src/Exchanges/*/Raw/Internal`
- `rg -n "class .*Request|record .*Request|class .*Response|record .*Response" src/Exchanges/*/{Raw,Normalized}/Internal`
