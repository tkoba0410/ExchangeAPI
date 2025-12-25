# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# RetryDecision

このドキュメントは、
**Retry / CircuitBreaker / Timeout Policy がどのような基準で振る舞いを決定するか**を定義する。

本ルールは `Common.Contracts` の一部であり、
すべての Transport / RestClient / Policy 実装はこれに従わなければならない。

---

## 目的

- Retry の判断基準をコード外に明示する
- 実装者ごとの解釈差を防ぐ
- プラグイン実装間の挙動を揃える

---

## 基本方針

### 1. Retry 判断は「意味」で行う

- HTTP StatusCode では判断しない
- `ExchangeErrorCategory` のみを判断材料とする

これにより、
- 取引所ごとの差異
- HTTP 実装の違い
- プロトコル変更

の影響を最小化する。

---

### 2. Retry 不可は明示的に限定する

以下の Category のみは **Retry 不可**とする。

- `Request`

理由：
- パラメータ不正
- 認証不備
- API パス誤り

は再試行しても改善しないため。

---

## Category 別の挙動

| ErrorCategory | Retry | CircuitBreaker | 備考 |
|---|---|---|---|
| Request | ❌ | ❌ | クライアント起因の失敗 |
| RateLimit | ⭕ | ❌ | 遅延付き Retry のみ |
| Network | ⭕ | ⭕ | 一時的障害 |
| Server | ⭕ | ⭕ | サーバ内部エラー |
| Unknown | ⭕ | ⭕ | デフォルト扱い |

---

## Retry Policy の責務

Retry Policy は以下を保証する。

- 最大試行回数を超えない
- 適切な遅延（指数バックオフ等）を挿入する
- 最終失敗時は `ExchangeApiException` をそのまま上位へ返す

Retry Policy は、
**成功させる責務を持たず、再試行する責務のみを持つ**。

---

## CircuitBreaker Policy の責務

CircuitBreaker は、
**障害が継続している場合に即座に失敗させるための仕組み**である。

### 動作方針

- Retry とは独立して失敗回数をカウントする
- Open 状態では即時 `ServiceUnavailable` 相当として失敗させる
- Half-Open / Close への遷移は時間に基づく

CircuitBreaker は、
Retry より **システム保護を優先**する。

---

## Timeout Policy の責務

Timeout は、
**処理が無限にブロックされることを防ぐ**ための仕組みである。

### 方針

- Timeout は Network 系失敗として扱う
- Timeout 発生時は Retry 対象となる
- Timeout は CircuitBreaker の失敗カウントに含まれる

---

## Policy の合成順序（推奨）

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

### 意図

- RateLimit は最初に制御
- Retry は通信前後を包む
- CircuitBreaker は失敗を短絡
- Timeout は最終防衛線

---

## 実装者への注意

- ErrorCategory の解釈を独自拡張しない
- Retry / CB の条件分岐を StatusCode で行わない
- 振る舞いを変えたい場合は **このドキュメントを更新する**

---

## まとめ

- Retry 判断は ErrorCategory に集約する
- Retry 不可は `Request` のみ
- CircuitBreaker はシステム保護を優先
- Timeout は Network 失敗として扱う

これらの規則により、
ExchangeAPI 全体で一貫した再試行戦略が保証される。

