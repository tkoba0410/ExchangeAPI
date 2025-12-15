# HttpTransport

このドキュメントは、
`Common.Transport` における **IHttpTransport / HttpTransport / HttpClient の扱い**を定義する。

---

## 目的

- HttpClient のライフサイクルを安全にする
- Timeout の責務分担を明確にする
- Handler / Proxy / 証明書等の注入ポイントを整理する

---

## IHttpTransport の役割

`IHttpTransport` は、
**HttpClient による実送信を最小限の抽象に閉じ込める**ためのインターフェースである。

- 上位（RestClient / Policy）は HttpClient を直接触らない
- テストでは FakeTransport に差し替え可能

---

## HttpTransport の役割

`HttpTransport` は、
`IHttpTransport` の標準実装であり、
**HttpClient を使ってリクエストを送信する**。

責務：
- HttpRequestMessage の送信
- HttpResponseMessage の受領
- 例外のそのままの伝播（正規化は上位で実施）

禁止：
- エラー分類
- Retry 判断
- ドメイン知識

---

## HttpClient のライフサイクル

### 推奨

- 外部から `HttpClient` を注入する
- DI（IHttpClientFactory 等）で生成・再利用を管理する

理由：
- ソケット枯渇を防ぐ
- DNS 更新に追従できる

---

### Dispose の責務

- `HttpTransport` は原則として **注入された HttpClient を Dispose しない**
- ライフサイクルは生成側（DI）に委譲する

---

## Timeout の責務分担（重要）

Timeout は二重に設定できる。

- `HttpClient.Timeout`
- `TimeoutPolicy`

推奨は以下：

- **TimeoutPolicy を正（主）**とする
- `HttpClient.Timeout` は無制限または十分長くする

理由：
- Policy の観測（Observer）と統一した扱いができる
- Timeout を `ExchangeErrorCategory.Network` として正規化しやすい

---

## Handler / Proxy / 証明書

HttpClient の通信詳細は、
`HttpClientHandler`（または DelegatingHandler）で設定する。

- Proxy
- 証明書検証
- 圧縮
- リダイレクト

これらは HttpTransport ではなく、
**HttpClient 構築側の責務**である。

---

## テスト戦略

- Unit / Component テストでは `IHttpTransport` を Fake に差し替える
- 実ネットワークを使うテストは `Integration.Public.Tests` に隔離する

---

## 実装上の注意

- HttpResponseMessage の Dispose を適切に扱う
- 送信のたびに HttpClient を new しない
- 送信・受信以外の責務を持たない

---

## まとめ

- `IHttpTransport` は実送信の最小抽象
- HttpClient のライフサイクルは外部（DI）で管理
- Timeout は Policy を正とし、一貫した観測と正規化を行う
- Handler 設定は HttpClient 構築側の責務

