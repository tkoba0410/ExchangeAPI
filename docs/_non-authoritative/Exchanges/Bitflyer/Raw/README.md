# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# bitFlyer Raw API

本ディレクトリは、**bitFlyer Lightning REST API** をそのまま扱うための **Raw API ドキュメント群**です。

Raw API は公式 API の **鏡像（faithful mapping）** として設計されており、
取引所固有の構造・語彙・制約を **抽象化せずに公開**します。

> Raw first. Faithful mapping. No abstraction.

---

## このディレクトリの読み方

正本は Raw-only の一覧である `ApiMap.md` です。抽象層（Adapter/Facade）の公開状況は補助ビューとして参照します。

1. **ApiMap.md**（正本 / Raw-only）  
   → [`ApiMap.md`](ApiMap.md)
2. **ApiMap.Decomposition.md**（補助: 命名分解ビュー）  
   → [`ApiMap.Decomposition.md`](ApiMap.Decomposition.md)
3. **Requests.md**（DTO 一覧）  
   → [`Requests.md`](Requests.md)
4. **抽象層公開状況**（補助ビュー）  
   → [`../Adapter/ApiExposureMap.md`](../Adapter/ApiExposureMap.md)

---

## Raw API の位置づけ

```
Application
   ↑
Adapter API（任意）
   ↑
bitFlyer Raw API（本ディレクトリ）
   ↑
Core / HTTP / Policy
```

- Raw API は **常に第一選択**です
- Adapter は Raw API を利用した翻訳層であり、必須ではありません
- Raw API は Adapter の都合を考慮しません

---

## 何が含まれるか

このディレクトリには、以下の情報を用途別に分割して配置します。

- **ApiMap.md**  
  公式 REST API の Endpoint と Raw-only 対応表（正本）
- **ApiMap.Decomposition.md**  
  命名分解の補助ビュー
- **Requests.md**  
  Request DTO（Body / Query 集約）の一覧と用途

---

## Raw API が提供しないもの

- 複数取引所を横断する統合 API
- 意味の補完・推測・共通化
- 業務ロジック・取引戦略

それらは **Application** または **Adapter** の責務です。

---

## 補足

- 抽象層で未露出の API も、Raw 層では **意図的に保持**されます
- 抽象層への昇格は、ユースケースが明確になった段階で検討します

---

> Raw API は例外を恐れない。ただし例外は文書に残す。
