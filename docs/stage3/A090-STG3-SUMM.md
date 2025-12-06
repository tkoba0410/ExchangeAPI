# A090-STG3-SUMM Stage3 FIX まとめ（send child order）

## 1. Stage3 の位置づけ
- Stage1：Public GET（板など）の設計・実装
- Stage2：Private GET の最初の縦スライス（`/v1/me/getbalance`）
- **Stage3：Private POST の最初の縦スライスとして `/v1/me/sendchildorder` を実装し、MARKET 注文を end-to-end で通すステージ**
- Stage4：PRIVATE GET 拡張（positions / executions）および Private POST 拡張（cancel）、エラー処理強化へ進む予定

Stage3 は、**「Private POST 呼び出しのテンプレート確立ステージ」**として FIX する。

---

## 2. Stage3 の scope（確定版）

### 2.1 やること（含める範囲）
- **Abstractions**
  - Trading ドメインモデル：`OrderSide` / `OrderType`（MARKET）/ `OrderRequest` / `OrderResult`
  - `IExchangeTradingClient.SendOrderAsync` の追加
  - `IExchangeClient` が Account + Trading を包括する形に拡張

- **Infrastructure**
  - POST 対応の `IRestClient.PostAsync`
  - `IRequestSigner` の POST 対応（body を含む署名）
  - `ExchangeApiException`（E1：HTTP ステータスベースの例外）

- **Bitflyer.Private**
  - DTO：`BitflyerSendChildOrderRequest` / `BitflyerSendChildOrderResponse`
  - `IBitflyerPrivateTradingApi.SendChildOrderAsync`
  - `BitflyerPrivateApi` による `/v1/me/sendchildorder` 呼び出し実装
  - PrivateApi は「bitFlyer の HTTP API をそのまま呼ぶ」以外の責務を持たない

- **Bitflyer.Adapter**
  - `BitflyerExchangeClient.SendOrderAsync` の実装
  - Domain → DTO → Private API → DTO → Domain の縦スライス確立

- **Factory**
  - Stage2 と同様の構成で、GET + POST の両方に対応した `IExchangeClient` を構築

- **ドキュメント**（A010〜A070）
  - A010: ゴール定義
  - A020: 要件定義
  - A030: レイヤ構成
  - A040: Private → Domain マッピング
  - A050: 実装ノート
  - A060: テスト観点
  - A070: 動作確認・運用メモ

---

### 2.2 やらないこと（除外範囲）
- Private GET の拡張（collateral / positions / executions）
- Private POST の拡張（cancelchildorder / cancelallchildorders）
- LIMIT / STOP / IFDOCO などの高機能注文
- time_in_force（IOC / FOK）の柔軟対応
- エラー処理レベル E2〜（取引所固有コードの分類、再試行制御など）
- CLI / GUI の構築

Stage3 は **MARKET 注文 1 本の縦スライスに限定し、POST 対応のテンプレートだけを確立する** ことを目的とする。

---

## 3. Stage3 の成果物

### 3.1 ドキュメント
- **A010-STG3-OVER**：ゴール定義（POST 縦スライス）
- **A020-STG3-REQR**：要件定義（OrderRequest / OrderResult / SendOrderAsync）
- **A030-STG3-ARCL**：レイヤ構成（Infrastructure → Private → Adapter）
- **A040-STG3-ARCB**：API マッピング（sendchildorder）
- **A050-STG3-IMPL**：実装ポイント整理
- **A060-STG3-TSTP**：テスト観点・代表テストケース
- **A070-STG3-OPS**：動作確認手順・運用メモ

### 3.2 コード（想定される構成）
- `ExchangeApi.Contracts`
  - OrderSide / OrderType / OrderRequest / OrderResult
  - IExchangeTradingClient
- `ExchangeApi.Transport`
  - IRestClient（PostAsync 追加）
  - IRequestSigner（POST 署名対応）
  - ExchangeApiException
- `ExchangeApi.Adapter.Bitflyer`
  - DTO：BitflyerSendChildOrderRequest / Response
  - IBitflyerPrivateTradingApi
  - BitflyerPrivateApi（GET + POST 両対応）
  - BitflyerExchangeClient（SendOrderAsync 実装）
- `ExchangeApi.Factory`
  - BitflyerClientFactory.Create（GET/POST 対応）

---

## 4. Stage3 の Definition of Done（FIX版）
Stage3 は以下の条件が揃った段階で完了とみなす：

### 4.1 設計面
- A010〜A070 の各文書が完全に揃っている
- Stage2 のアーキテクチャと整合性が保たれている
- Trading 用インターフェース・Domain モデルが抽象化されている

### 4.2 実装面
- `SendOrderAsync` が Domain → DTO → Private API → DTO → Domain のパスで動作する
- `RestClient.PostAsync` が署名付き JSON POST を正しく処理する
- `IRequestSigner` が body を含む POST 署名に対応している
- `BitflyerPrivateApi.SendChildOrderAsync` が Private API を正しく呼び出す
- Adapter 層がマッピングを正しく処理する

### 4.3 動作確認面
- 正常系：小額の MARKET 注文が実口座で成功する
- 403（認証エラー）が正しく検出される
- 400（残高不足）が正しく例外として扱われる
- タイムアウトなどの通信エラーが `ExchangeApiException` に正しく包まれる

### 4.4 テスト面
- POST の Infrastructure（RestClient・Signer）の単体テストが存在する
- DTO → Domain / Domain → DTO のテストがある
- Adapter（SendOrderAsync）のモックテストが存在する
- Factory の Trading 構築がテストされている

---

## 5. 制約と今後の見直しポイント（総括）
- Stage3 は MARKET 注文のみに限定した最小スコープであり、実戦レベルの Trading API と比較して機能は限定的
- LIMIT / STOP / IOC / FOK などを導入するには Domain モデルの拡張が必要
- bitFlyer のエラーコードの扱いは E2 に引き上げる必要がある（例：`INSUFFICIENT_FUNDS`、`INVALID_ORDER`）
- cancel 系 API は query + POST の混合形式のため、RestClient の柔軟性を再検証する必要がある

これらは Stage4 以降で段階的に拡張し、Trading API 全体の安定性を高める。

---

## 6. Stage4 への接続
Stage3 で、
- POST 呼び出し基盤
- Domain ⇄ DTO マッピング（Trading）
- Factory による Trading 構築
- MARKET 注文 1 本の end-to-end 動作

が確立したことで、Stage4 は次の発展ステップへ移行できる：

### Stage4 の主な拡張予定
- **Private GET の横展開**（positions / executions / collateral）
- **Private POST の拡張**（cancelchildorder / cancelallchildorders）
- **注文種別の拡張**（LIMIT / STOP / time_in_force）
- **E2 エラー処理**（bitFlyer 固有コードの分類）
- **レートリミット対応**（必要であれば）

Stage3 は、Trading 機能を持つ Exchange API の最初の重要な節目であり、
**「POST を含む縦のスライス」を確立した完成ステージとして FIX** する。

