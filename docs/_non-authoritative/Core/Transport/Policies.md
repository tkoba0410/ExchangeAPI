# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# Policies

このドキュメントは、
`Common.Transport` における **HttpPolicyPipeline / 各種 Policy / 設定オプション / 合成順序**を定義する。

Policy は通信の **安定性・保護・制御**を提供するが、
**判断基準そのものは持たない**。
判断基準は `Common.Contracts`（ErrorMapping / RetryDecision）に委譲される。

---

## 目的

- 再試行・制御の責務を明確にする
- 実装者ごとの解釈差を防ぐ
- 安全な合成順序を固定する

---

## HttpPolicyPipeline

`HttpPolicyPipeline` は、
複数の `IHttpPolicy` を **直列に合成**して適用するコンポーネントである。

```csharp
var pipeline = new HttpPolicyPipeline(
    rateLimit,
    retry,
    circuitBreaker,
    timeout);
```

- 各 Policy は「次の処理」を受け取る
- Policy は結果を変更してはならない（成功/失敗の意味を変えない）

---

## 推奨される合成順序（重要）

```
RateLimit
  ↓
Retry
  ↓
CircuitBreaker
  ↓
Timeout
  ↓
Transport
```

### 理由

- **RateLimit**：最初に流量を制御
- **Retry**：一時失敗を吸収
- **CircuitBreaker**：継続障害を短絡
- **Timeout**：最終防衛線

この順序を崩してはならない。

---

## 各 Policy の役割

### Retry

- 一時的失敗を再試行する
- 最大試行回数・遅延を尊重する
- Retry 可否は `ExchangeErrorCategory` に従う

Retry は **成功させる責務を持たない**。

---

### RateLimit

- 単位時間あたりのリクエスト数を制限
- Burst を許可する
- レート制限による失敗は `RateLimit` として扱う

---

### CircuitBreaker

- 障害が継続している場合に即時失敗させる
- Open 状態では実送信を行わない
- Half-Open / Close は時間に基づき遷移

CircuitBreaker は **システム保護を最優先**とする。

---

### Timeout

- 処理時間の上限を設ける
- Timeout は Network 系失敗として扱う
- Retry / CircuitBreaker の判断対象となる

---

## HttpPolicyOptions

`HttpPolicyOptions` は、
Policy の挙動を **設定値として集約**する。

代表的なオプション：

- MaxRetryAttemptsForGet
- MaxRetryAttemptsForOther
- RetryBaseDelay
- RetryMaxDelay
- RequestsPerSecond
- Timeout
- CircuitBreakerFailureThreshold

各オプションは **Policy の実装詳細ではなく、挙動の強度**を調整する。

---

## HttpPolicyFactory

`HttpPolicyFactory` は、
`HttpPolicyOptions` から **推奨構成の Pipeline**を生成する。

- 合成順序を固定
- オプション未指定時の安全なデフォルトを提供

利用者は、
原則として Factory 経由で Policy を構築する。

---

## PolicyObserver

`IPolicyObserver` は、
Policy の内部イベントを観測するためのフックである。

- Retry 実行
- CircuitBreaker 状態遷移
- Timeout 発生

Observer は **挙動に影響を与えてはならない**。

---

## 実装上の注意

- StatusCode による分岐を入れない
- Retry 可否の判断を Policy 内に持ち込まない
- 合成順序を変更しない

判断を変えたい場合は、
**Contracts 側（RetryDecision / ErrorMapping）を更新する**。

---

## まとめ

- Policy は通信の安定性を提供する
- 判断基準は Common.Contracts に集約
- 合成順序は固定
- Factory を使って安全に構築する

