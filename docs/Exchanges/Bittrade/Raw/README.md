# Bittrade Raw API

本ディレクトリは、**Bittrade REST API** をそのまま扱うための **Raw API ドキュメント群**です。
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

## 一次ソース（仕様書）

- `../Rest API共通情報 – BitTrade API Reference.htm`

---

## 共通仕様（要点）

- **Host**: `https://api-cloud.bittrade.co.jp`
- **署名/認証**: 公開 API 以外は署名必須
  - 署名パラメータ: `AccessKeyId`, `SignatureMethod=HmacSHA256`, `SignatureVersion=2`, `Timestamp`, `Signature`
  - **GET**: URL の全パラメータを署名対象に含める
  - **POST**: 署名パラメータのみを署名対象とし、その他の値は JSON Body に入れる
- **レート制限**:
  - 公開 API: IP 毎に 1 秒 10 回
  - 署名 API: API キー毎に 1 秒 10 回
- **共通エラー形式**（例）:
  - `status`: `ok` / `error`
  - `err-code`: エラーコード
  - `err-msg`: エラーメッセージ

---

## Raw API の位置づけ

```
Application
   ↑
Adapter API（任意）
   ↑
Bittrade Raw API（本ディレクトリ）
   ↑
Core / HTTP / Policy
```

- Raw API は **常に第一選択**です
- Adapter は Raw API を利用した翻訳層であり、必須ではありません
- Raw API は Adapter の都合を考慮しません

---

> Raw API は例外を恐れない。ただし例外は文書に残す。
