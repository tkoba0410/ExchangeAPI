# EndpointId 中心設計（共通条文）

## 目的

本プロジェクトでは、API コール名を主とせず、
**取引所ごとの EndpointId（Constants）を唯一の正本**とする設計を採用する。

各層（Wire / Raw / Normalized / Facade / Docs）は、
EndpointId から **共通の派生規則**に従って名称・API を生成する。

これにより、

* 命名の揺れを排除する
* 取引所差異を正直に露出させる
* 「嘘の共通化」を防ぐ

ことを目的とする。

---

## 用語定義

* **EndpointId**

  * 取引所ごとの API endpoint を一意に識別する定数名
  * 各取引所の `Wire.Constants` に定義される
  * 公式 API の path 文字列を値として保持する

* **派生 API 名**

  * EndpointId から共通規則で生成される API 名
  * 各層で手動命名してはならない

---

## 共通原則

1. EndpointId が **唯一の正本**である
2. API 名・メソッド名を直接設計してはならない
3. 各層の名称は EndpointId から **機械的に派生**させる
4. 派生規則は **全取引所共通**とする
5. 取引所差異は EndpointId の付け方にのみ現れる

---

## 共通派生規則

### Normalized / Client API

```
<EndpointId>CallAsync
```

### Interface / Facade

* 原則として Normalized と同一派生名を使用する
* Facade は Capability 単位で Optional とし、未対応は `null`

### Wire

* HTTP Path は EndpointId の値をそのまま使用する

### Docs

* 表示項目は以下を基本とする

  * EndpointId
  * HTTP Method
  * Path

---

## 禁止事項

* EndpointId 以外を正本とすること
* 各層で独自に命名すること
* 意味統一・共通化を EndpointId で行うこと

---

## 備考

* EndpointId の命名規則は **取引所ごとに定める**
* 本書は全取引所に共通して適用される
