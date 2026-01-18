以下は **再検証済み・最終版**の `docs/exchanges-public-rules.md` です。

* Stage8 現状（Facade Optional / Normalized 公開 / Call-only）と完全整合
* TopSpec / contracts overview と **重複しない**
* 将来の設計判断に対する **判断停止条件**として十分最小

---

# 取引所 API 公開ルール（Public Rules）

本ドキュメントは、ExchangeAPI における
**「取引所 API をどこまで・何を・どの形で公開するか」**
を定める *最上位の設計ルール* である。

TopSpec / Contracts / Interfaces に記載される個別規則は、
本ドキュメントに **反しない範囲**でのみ有効とする。

---

## 1. 公開面の宣言（Public Surface）

### 1.1 利用者の一次入口

利用者が直接参照・使用してよい一次入口は、以下に限定する。

```
ExchangeApi.Exchanges.<Exchange>.Normalized
```

* `<Exchange>` は取引所ごとの物理単位（例: `Bitflyer`, `Bittrade`）。
* Normalized は「その取引所の公式 API を、型安全かつ意味確定した形で提供する層」である。

### 1.2 共通言語（Contracts.Common）

`ExchangeApi.Contracts.Common` は **共通言語層**である。

公開してよいもの：

* DTO（共通概念として成立するもの）
* primitive type / enum
* `Call` / `CallMeta` / `Errors`

これらは **全取引所共通で理解される語彙**として扱う。

### 1.3 横断抽象（Facade）

* `ExchangeApi.Contracts.Facade` は **Optional** とする。
* 存在してもよいが、**利用を必須としない**。
* 利用者が取引所ごとの Normalized API を直接使用することを、正式に許可する。

> Facade は「便利な横断抽象」であり、「正本」ではない。

---

## 2. Normalized 層の責務

Normalized は、**取引所内で意味を確定する層**である。

### 2.1 やること（MUST）

* wire / raw に存在する **表現揺れ**を吸収する

  * 数値表現（string / number 混在）
  * null / optional の扱い
  * 命名差異
* その取引所において **意味が確定した型・操作**を提供する

### 2.2 やらないこと（MUST NOT）

* 他取引所へ寄せるための統一（cross-exchange canonicalize）
* 取引所間の意味差を吸収する調整

これらを行う場合は、**Contracts 側（必要になった場合のみ）に限定**する。

> Normalized は「取引所正規化」であり、「取引所間正規化」ではない。

---

## 3. 公開してよい／いけないもの

### 3.1 公開してよいもの（Public OK）

* Normalized の API
* Normalized の DTO（必要最小限の request / response）
* Contracts.Common に属する DTO / type / enum / Call / Errors

### 3.2 公開してはいけないもの（Public NG）

以下は **内部実装詳細**として扱い、公開してはならない。

* transport
* wire
* raw
* mapper / normalizer
* DI / factory / credential provider
* 環境依存（設定・秘密情報・OS 依存）

> 利用者が知るべきなのは「何が呼べるか」「何が返るか」のみである。

---

## 4. 命名規則（Normalized API）

### 4.1 基本方針

* Normalized API のメソッド名は、**原則として公式 API パス由来**とする。
* 公式 API パスの **最終要素**を基準に命名する。

### 4.2 具体ルール

* snake_case / kebab-case / lowercase は単語分割し **PascalCase** に変換する
* `v1`, `v2`, `me` などの **固定セグメントは名前に含めない**

例：

| 公式 API パス               | Normalized API 名 |
| ----------------------- | ---------------- |
| `/v1/me/sendchildorder` | `SendChildOrder` |
| `/v1/me/getbalance`     | `GetBalance`     |
| `/v1/getboard`          | `GetBoard`       |

### 4.3 非同期・Call-only 命名

* I/O を伴う公開 API は **Call-only** とする
* 戻り値が `Task<Call<T>>` / `ValueTask<Call<T>>` の場合、
  メソッド名は必ず **`CallAsync`** で終える

例：

* `GetBalanceCallAsync`
* `SendChildOrderCallAsync`

`Async` のみ（例: `GetBalanceAsync`）は使用しない。

---

## 5. 本ドキュメントの位置づけ

* 本文書は「取引所 API 公開に関する最小かつ十分な規則」を定義する。
* ここに書かれていない事項は、

  * 取引所固有
  * 実装詳細
  * 将来拡張

として扱い、**本規則を破らない限り自由**とする。

---

以上。
