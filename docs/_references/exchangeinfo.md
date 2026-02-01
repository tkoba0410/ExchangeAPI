> ✅ Reference / Frozen
> この文書は ExchangeInfo の実装方針・具体例をまとめた **参考資料** であり、正本ではない。
> 規範（MUST/NG）は docs/topspec.md を正とする。

# ExchangeInfo 実装ノート（参考）

## 0. 位置づけ

ExchangeInfo は「取引所仕様メタ情報」を扱う。
API エンドポイント分類や署名分類とは無関係であり、**API 系統とは独立したモジュール**として扱う。
構成は Contracts の ExchangeInfo と同一だが、**独自 DTO**を用いる。
独自 DTO は **Contracts と同構成 + 追加情報**を許容し、ここでは便宜上 **Local DTO** と呼ぶ。
Contracts への変換は Adapter で行い、追加情報は変換時に落とす（欠落ではなく境界）。

## 1. Static / Dynamic / Compose / Adapter の役割

- Static
  - 公式仕様や手動更新が必要な固定値を保持する。
  - 例: 市場一覧、手数料テーブル、固定フラグ。
- Dynamic
  - API から取得可能で変動する状態を保持する。
  - 例: 稼働状態、手数料率、最小数量、ティックサイズ。
- Compose
  - Static を基準に Dynamic で上書き・拡張する。
  - Static に存在しない Market は Dynamic で追加可能。
- Adapter
  - Local DTO を Contracts DTO に変換する。

## 2. フィールドの扱い方（指針）

- `Markets`
  - Static を基準にし、Dynamic で上書き（同一キーは動的優先）。
  - マッチングキーは ProductCode / Symbol 等、取引所が一意に扱う識別子。
- `Features`
  - Static を基準にし、Dynamic で補完・上書き。
- `RateLimits`
  - Static を基準にし、Dynamic で補完・上書き。
- `Maintenance`
  - Dynamic が取得可能な場合は Dynamic を優先する。

## 2.1 物理配置（参考）

```
src/Exchanges/<Exchange>/ExchangeInfo/
  Static/
  Dynamic/
  Compose/
  Adapter/
```

## 3. Dynamic ソースの例（Bitflyer）

- 市場一覧: `/v1/getmarkets`
- 手数料率: `/v1/me/gettradingcommission`
- 稼働状態: `/v1/gethealth`, `/v1/getboardstate`

## 4. データの責務と更新

- Static の更新は JSON 等の静的ファイルで管理し、レビュー・差分確認を前提とする。
- Dynamic は取得失敗時に Static を使用する（空・欠損で上書きしない）。

## 5. 参照先

- 正本: `docs/topspec.md`
- Contracts: `docs/contracts/contracts.md`
