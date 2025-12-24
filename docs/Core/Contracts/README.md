# Common.Contracts

`Common.Contracts` は、
**ExchangeAPI における技術基盤レイヤ間の契約（Contract）を定義する領域**である。

このフォルダに置かれる契約は、
- 取引所ドメインに依存しない
- HTTP や Transport の実装詳細を上位に漏らさない
- Policy / Factory が一貫した判断を行える

ことを目的とする。

---

## このレイヤの責務

Common.Contracts が扱うのは、**「意味のある境界」**である。

- Transport → 上位レイヤへの結果・失敗の伝達
- Retry / CircuitBreaker が判断するための意味分類
- 実装差異（HTTP / JSON / 通信）の吸収

言い換えると、
**「技術的に起きたこと」を「意味として解釈した結果」**に変換する層である。

---

## 含まれる契約

### Errors

- `ExchangeApiException`
- `ExchangeErrorCategory`

HTTP エラー、通信例外、JSON 解析失敗などを
**上位が理解可能な失敗表現に正規化するための契約**。

Retry / Policy / Factory は、
StatusCode ではなく ErrorCategory を基準に振る舞う。

詳細は `Errors.md` を参照。

---

## 含まれないもの

Common.Contracts には、以下を含めない。

- 取引所固有の DTO / エラーコード
- Order / Balance / Trade などのドメインモデル
- API パスや JSON フォーマット
- 取引所選択・生成ロジック（Factory）

これらは **Exchange.Common** 側の責務である。

---

## 上位・下位レイヤとの関係

```
[ Application / User Code ]
            |
        Factory
            |
     -----------------
     | Common.Contracts |
     -----------------
            |
   Transport / Policy
            |
        HTTP / Network
```

- 下位レイヤは Common.Contracts の契約を **必ず満たす**
- 上位レイヤは Common.Contracts の情報だけを **信頼して判断する**

この関係を崩さないことが、
プラグイン化・将来拡張を安全にする前提となる。

---

## 設計原則

### 1. 意味で判断する

- HTTP ステータスではなく **意味（Category）**で判断する
- Retry 可否は ErrorCategory に集約する

### 2. 実装を漏らさない

- HttpClient / HttpResponseMessage を上位に出さない
- JSON ライブラリの差異を上位に出さない

### 3. 取引所ドメインを持ち込まない

- Bitflyer / Bittrade などの知識を含めない
- 他 API ドメインでも再利用可能であること

---

## このフォルダの構成

```
docs/Common/Contracts/
├─ README.md        // このファイル
├─ Errors.md        // 共通エラー契約
└─ (将来拡張)
   ├─ ErrorMapping.md
   └─ RetryDecision.md
```

---

## まとめ

- Common.Contracts は **技術基盤の契約層**
- 上位と下位の責務を明確に分離する
- プラグイン化・将来拡張のための土台

この契約を破らない限り、
Exchange 実装は静的・動的いずれにも拡張可能である。

