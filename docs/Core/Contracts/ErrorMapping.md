# ErrorMapping

このドキュメントは、
**HTTP / 例外 → `ExchangeErrorCategory` への正規化ルール**を定義する。

Transport / RestClient 層は、本ドキュメントの規則に従い、
下位で発生した事象を `ExchangeApiException` に変換しなければならない。

---

## 目的

- HTTP 実装差異を上位レイヤから隠蔽する
- Retry / CircuitBreaker の判断を一貫させる
- 取引所ごとの差異による挙動ブレを防ぐ

---

## 正規化の基本原則

1. **必ず `ExchangeApiException` に包む**  
   下位例外をそのまま上位に投げてはならない

2. **判断は StatusCode ではなく Category**  
   HTTP ステータスは参考情報であり、意味ではない

3. **詳細は InnerException に保持する**  
   デバッグ可能性を失わない

---

## HTTP ステータス → ErrorCategory

| HTTP Status | ErrorCategory | 理由 |
|---|---|---|
| 400 BadRequest | Request | リクエスト不正は再試行しても改善しない |
| 401 Unauthorized | Request | 認証情報の問題 |
| 403 Forbidden | Request | 権限・認可の問題 |
| 404 NotFound | Request | API パス・パラメータ不正 |
| 408 RequestTimeout | Network | 一時的通信障害の可能性 |
| 429 TooManyRequests | RateLimit | 明示的なレート制限 |
| 500 InternalServerError | Server | 一時的なサーバ障害 |
| 502 BadGateway | Server | 上流依存の一時障害 |
| 503 ServiceUnavailable | Server | 一時的なサービス停止 |
| 504 GatewayTimeout | Network | ネットワーク遅延 |
| その他 5xx | Server | 原則リトライ対象 |
| 不明 | Unknown | 判断不能（デフォルト） |

---

## 例外型 → ErrorCategory

| 例外型 | ErrorCategory | 備考 |
|---|---|---|
| HttpRequestException | Network / Server | StatusCode があれば上表に従う |
| TaskCanceledException | Network | Timeout とみなす |
| TimeoutException | Network | 明示的タイムアウト |
| JsonException | Server / Unknown | レスポンス破損・仕様逸脱 |
| その他 Exception | Unknown | 原則リトライ可 |

---

## JSON / Content 関連

### 空ボディ

- HTTP 200/204 でも Content が null / empty の場合
- `ExchangeApiException` を生成
- Category は `Server` または `Unknown`

### 不正 JSON

- `JsonException` を `InnerException` に設定
- Category は `Server`（原則）

---

## Retry 判断との関係

Retry / Policy は、以下を前提としてよい。

- `Request` は **Retry しない**
- `RateLimit` は **Retry する**（遅延あり）
- `Network / Server / Unknown` は **Retry 可能**

これらの判断は **ErrorCategory のみ**を参照する。

---

## 実装上の注意

- Transport 実装ごとに独自解釈を入れない
- 取引所固有エラーコードはここで扱わない
- 仕様変更があった場合は、必ず本ドキュメントを更新する

---

## まとめ

- ErrorMapping は **Common.Contracts の行動規範**
- 正規化ルールを 1 箇所に集約する
- プラグイン実装の一貫性を保証する