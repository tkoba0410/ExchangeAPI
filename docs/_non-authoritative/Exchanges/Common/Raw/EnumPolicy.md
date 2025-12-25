# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# Raw Enum Policy

## Goal
Raw 層では仕様を忠実に表現しつつ、トレードに影響する **重要な仕様追加** を見逃さないことを最優先とする。

このため、Raw で enum を使う場合は **常に strict（未知値は例外で落とす / fail fast）** とする。
Open set は増加を許容し、Raw では enum 化しない。

## Open set vs Closed set
Raw の値は「変化が大きい（増える前提）」と「変化が小さい（追加が起きたら重大）」に分類し、表現を分ける。

### Open set（増える前提で許容する）
追加されてもアプリを止めるべきではない対象。Raw では enum 化しない。

**例**
- symbol / product_code（通貨ペア）
- currency（通貨コード）
- exchange code（取引所コード）
- account-id 等の ID 群
- 銘柄リスト、通貨リスト
- err-code（増えることが多い）

**表現**
- string / number / 値オブジェクト（例: Symbol, CurrencyCode）で保持する
- Unknown 値でも通る（fail fast しない）

**理由**
- 追加頻度が高い、または追加のたびに本番停止するコストが大きい
- 変化検知が必要な場合は Live suite や監視で観測する

### Closed set（変化が小さい＋トレードに影響が大きい）
追加が起きたら必ず検知したい対象。Raw では enum（または厳格な型）を使い、未知値は必ず落とす。

**例**
- order side（buy/sell）
- order type（limit/market）
- time-in-force（GTC/IOC/FOK 等）
- order status/state（約定・取消・失効など）
- position side / margin mode / leverage mode（取引に影響するもの）
- kline interval（戦略に直結する場合）

**表現**
- 取引所専用 enum（例: OrderSide）を Raw 層に定義する
- JSON からのデシリアライズは strict（未知値は例外）

**理由**
- 仕様追加を知らずに処理が続くと、誤発注・誤判定など重大事故に繋がる

## Strict rule（常に落とす）
Closed set の enum は次を満たすこと。

- **未知値は必ず例外**（Unknown へのフォールバックは禁止）
- 文字列 enum:
  - 未知文字列は JsonException で落とす（既定挙動 or strict converter）
- 数値 enum:
  - 未定義値が通ってしまう場合があるため、JsonConverter で `Enum.IsDefined` を検査して throw する

## Testing requirement
Closed set の enum を追加/変更した場合、最低限のテストを追加する。

- 既知値のデシリアライズが成功すること
- 未知値のデシリアライズが例外で落ちること（fail fast を担保）

## Observability（Open set）
Open set は Raw で許容するが、変化検知が必要な場合は Live suite 等で観測する。

## Mapping to Common/Adapter
- Raw 層は Common enum を参照しない（取引所専用 enum を使う）
- 共通化が必要な場合は Adapter（または Common 層）でマッピングする
