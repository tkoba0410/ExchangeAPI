# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# 文書群 全体像（配置案・目次・テンプレート）

本書は、現時点の憲法（最終仕様書）を頂点として、下位仕様・取引所仕様・運用文書を **どこに、何として**配置するかを確定するための「棚卸し」ドキュメントである。

---

## 1. ドキュメント階層（正本の序列）

1. **憲法（Top Spec）**：変更は原則破壊的（FIX）
2. **下位仕様（Cross-Exchange Spec）**：憲法に従い改訂可能
3. **取引所仕様（Exchange Spec）**：doc-api + Wire Samples を正本
4. **運用文書（Contributor/Guide）**：憲法・下位仕様を守るための手順

---

## 2. 配置案（ファイルパスの確定案）

```
/docs/
  /top/
    TopSpec.md                         # 憲法（最終仕様書：FIX）

  /cross/
    Contracts.ApiSpec.md               # Contracts API 仕様（IFの一覧/粒度/契約）
    Contracts.DtoCatalog.md            # Contracts DTO カタログ（型一覧/フィールド）
    Common.Spec.md                     # Common 語彙仕様（VO/型/Parse/精度）
    Errors.Spec.md                     # Error 仕様（分類/Retryability/返し方）
    Domain.UseCases.md                 # Domain ユースケース一覧（入出力/責務）

  /exchanges/
    README.md                          # 取引所仕様の読み方（doc-api⇔code対応）

  /guides/
    Contributing.md                    # 開発手順（憲法準拠の作業手順）
    Testing.md                         # サンプル/契約テスト方針（任意）

/README.md                             # 利用者向け（CreateClient中心）
/CONTRIBUTING.md                        # 入口（docs/guides/Contributing.mdへ誘導）
```

> 重要：`/docs/top/TopSpec.md` は **唯一の憲法**。
> それ以外は「憲法に従う下位仕様」または「取引所仕様正本（doc-api）」である。

---

## 3. 相互参照ルール（文書とコードの結び方）

### 3.1 TopSpec（憲法）→ 下位仕様

* TopSpec は、詳細を列挙しない。
* 代わりに `/docs/cross/*` を参照して「詳細はここ」と指し示す。

### 3.2 下位仕様 → TopSpec（必須）

* すべての下位仕様は冒頭に「従うべき憲法条文」を列挙する。

### 3.3 Exchange Spec（doc-api）→ code

* `doc-api` は取引所仕様の正本。
* `src/Exchanges/<Exchange>/Wire/Samples/*.json` は **doc-api の反映**。

---

## 4. 目次（各ドキュメントに何を書くか）

### 4.1 /docs/top/TopSpec.md（憲法）

* spec/domain 境界
* 層構造と物理構成
* Cross-Exchange 4種
* Contracts/Common/Domain の責務
* 依存方向
* 不変条件・禁止事項
* Factory 入口・命名規約
* FIX 宣言（変更ポリシー）

### 4.2 /docs/cross/Contracts.ApiSpec.md

* 目的（横断契約の API 仕様）
* 公開入口：`IExchangeClient`
* グループ：`IMarketDataApi/ITradingApi/IAccountApi/IExchangeInfoApi`
* 各メソッド：

  * シグネチャ（引数型は Common VO を優先）
  * 戻り値（Contracts DTO / Result / Exception 方針）
  * 失敗（Errors.Spec 参照）
* 非対応・capability 方針（任意）

### 4.3 /docs/cross/Contracts.DtoCatalog.md

* DTO 一覧（カテゴリ別）

  * Market：Ticker/OrderBook/Trades/Kline...
  * Trading：PlaceOrder/Cancel/Order...
  * Account：Balance/Positions...
* フィールド仕様（型・nullable・意味）
* Common 語彙の参照（Price/Size/Symbol/Timestamp etc）

### 4.4 /docs/cross/Common.Spec.md

* Values（VO）一覧と仕様

  * Parse 規約（Try/OrThrow）
  * 精度・丸め（必要な範囲）
* Types/Enums
* Parsing パッケージ（規約・例外）

### 4.5 /docs/cross/Errors.Spec.md

* エラー分類（ErrorCode）
* Retryability（再試行可否）
* 返却方針（Result/Exception）
* 取引所固有エラーのマッピング方針（Adapter）

### 4.6 /docs/cross/Domain.UseCases.md

* UseCase 一覧
* 各 UseCase の責務、入出力（Contracts+Commonのみ）
* 禁止事項（Exchanges/Wire/Normalized/Compositionへ依存しない）

### 4.7 /docs/exchanges/README.md

* doc-api の構造と読む順序
* `Wire/Samples` の位置づけ
* Converter/Mapper/Adapter の関係

### 4.8 /docs/guides/Contributing.md

* 新取引所追加の手順

  1. doc-api 収集
  2. Wire Samples 追加
  3. Converter
  4. Normalized Mapper
  5. Adapter
  6. Composition
  7. 契約テスト
* PR レビュー観点（TopSpec の禁止事項チェック）

---

## 5. テンプレート（コピペ用）

### 5.1 下位仕様の共通ヘッダ（必須）

```md
# <TITLE>

## 憲法（TopSpec）への準拠

本書は `/docs/top/TopSpec.md` に定義される最上位仕様（憲法）に準拠する。
特に次の条文に従う。

- <章番号>: <条文名>
- <章番号>: <条文名>

## 正本（source of truth）

- 本書（下位仕様）
- 取引所仕様（doc-api）
- コード：`src/Shared/*` / `src/Exchanges/*`
```

### 5.2 Contracts.ApiSpec.md テンプレ

```md
# Contracts API Spec

## 憲法（TopSpec）への準拠
- 3.8 Factory
- 3.9 Contracts 公開面
- 3.10 Contracts/Common 境界
- 3.13 命名規約

## 目的

## 公開入口
- IExchangeClient

## API グループ
### Market
- GetTickerAsync
- ...

### Trading
- PlaceOrderAsync
- ...

### Account
- GetBalancesAsync
- ...

## 返却・失敗
- Errors.Spec.md 参照

## 非対応・capability
- 方針
```

### 5.3 Contracts.DtoCatalog.md テンプレ

```md
# Contracts DTO Catalog

## 憲法（TopSpec）への準拠
- 3.10 Contracts/Common 境界

## DTO 一覧

## 各 DTO 定義
### TickerDto
- Symbol: Symbol
- BestBid: Price
- BestAsk: Price
- Timestamp: Timestamp

（…）
```

### 5.4 Common.Spec.md テンプレ

```md
# Common Spec

## 憲法（TopSpec）への準拠
- 10 Invariants（string境界、Try/OrThrow）

## Values
### Price
- 内部表現
- TryParse / ParseOrThrow
- 比較・演算

## Types/Enums

## Errors（参照）
- Errors.Spec.md
```

### 5.5 Errors.Spec.md テンプレ

```md
# Errors Spec

## 憲法（TopSpec）への準拠
- 6 Cross-Exchange 4種
- 3.10 Error の置き場

## ErrorCode

## Retryability

## Result/Exception 方針

## Adapter のマッピング
```

### 5.6 Domain.UseCases.md テンプレ

```md
# Domain UseCases

## 憲法（TopSpec）への準拠
- 3.12 Domain 公開形

## UseCase 一覧

## 各 UseCase
### OrderPolling.WaitForFilled
- 入力: ITradingApi, OrderId, ...
- 出力: ...
- 失敗: Errors.Spec.md
```

---

## 6. 次にやること（最短）

1. `docs/top/TopSpec.md` に現キャンバス内容をエクスポート（または同期）
2. `/docs/cross/*` の空テンプレを作成
3. README / CONTRIBUTING から TopSpec と Guides へ導線を張る
