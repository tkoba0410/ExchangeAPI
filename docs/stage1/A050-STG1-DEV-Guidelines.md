---

doc_id: A050-STG1-DEV-Guidelines
title: Stage1 開発ガイドライン（DEV）
version: 2.0.0
status: Draft
stage: Stage1
-------------

# A050-STG1-DEV-Guidelines

Stage1 開発ガイドライン（Development Guidelines）

本書は、Exchange API Library **Stage1（bitFlyer Public REST / Ticker）** の開発を円滑に進めるための
ガイドラインを定義する。A010（OVR）・A020（REQ）・A030（ARC）・A040（SPC）で定義された
目的・要求・構造・仕様に従い、**実装者が迷わず手を動かせるための最小ルール**をまとめる。

Stage1 は軽量フェーズであり、将来 Stage2（認証 / WebSocket / 複数取引所 / Transport & Protocol 拡張）へ
スムーズに移行できるよう、必要最小限の規律に限定する。

---

# 1. 目的（Purpose）

* Stage1 の開発を **軽く・速く・明確に** 進める。
* 主要文書（OVR/REQ/ARC/SPC）で定義された構造と矛盾しない範囲で、
  実装ストレスを極力減らす。
* 必要最小限のルールのみ残し、その他は Stage2 で段階的に強化する。

---

# 2. 適用範囲（Scope）

本ガイドラインが適用されるのは次の範囲である。

* Stage1 の開発（bitFlyer Ticker のみ）
* プロジェクト：`ExchangeApi.Contracts` / `ExchangeApi.Transport` / `Exchange.Bitflyer`
* テスト：各プロジェクトの単体テスト

Stage2 移行時には、本ガイドラインは一部または全体が強化される。

---

# 3. 開発の基本方針（Core Principles）

## 3.1 実装と構造の一貫性（Boundary + Modules）

* Abstractions（Boundary）は不変の中心であり、Stage1 でも最優先で守る。
* bitFlyer Adapter と Infrastructure は **Boundary を汚染しない**。
* 取引所固有モデル（Raw）は Adapter 内に閉じ込める。

## 3.2 重すぎない開発運用

* 複雑な設計プロセスは Stage1 では不要。
* REQ/ARC/SPC の整合は **実装後にまとめて確認してよい**（SHOULD）。
* コードレビューは柔軟に行うが、Stage1 の範囲では厳密な規律を要求しない。

## 3.3 小さなステップで進める

* 実装タスクは「数分〜十数分」で終わる粒度に分割する（SHOULD）。
* 1 ステップずつ動作確認を挟むことで、迷走を防ぐ。

## 3.4 仕様が正・コードが従う

* A020（REQ）/ A040（SPC）で定義した Ticker 仕様が正であり、
  コードが仕様に追いついていない場合はコード側を修正する（MUST）。

---

# 4. Stage1 で必ず守る最小ルール（MUST）

## 4.1 依存方向（ARC の正典）

```
Abstractions  ←  Infrastructure  ←  Bitflyer Adapter
```

* Abstractions へ逆依存してはならない（MUST NOT）。
* Raw モデルは `Exchange.Bitflyer` 内部に閉じ込める。

## 4.2 Abstractions の最小構成

* `IExchangeClient`
* `Ticker`（A020 / A040 の仕様）
* `Symbols`

## 4.3 REST / HTTP 実装の分離

* bitFlyer Adapter は `HttpClient` に直接触れない（MUST）。
* HTTP 通信は `IRestClient` / `IHttpTransport` のみ使用する（MUST）。

## 4.4 Ticker 正常取得（Stage1 DoD）

* `GetTickerAsync("BTC/JPY")` が正常に動作すること（MUST）。
* Raw → Ticker マッピングが A040 の規則に従うこと（MUST）。

---

# 5. Stage1 で守らなくてよいもの（不要 / 後回し）

Stage1 は「まず動くものを作る」フェーズであり、以下は求めない。

## 5.1 厳格な設計プロセス（ADR / Conformance）

* ADR（Architecture Decision Record）作成は不要（MAY）。
* REQ ⇔ ARC ⇔ SPC の完全な相互リンクは Stage1 では後回し（SHOULD）。

## 5.2 高度な Transport / Protocol

* Retry / RateLimit / CircuitBreaker などの Transport 拡張は不要（MAY）。
* 認証（署名生成等）は Stage2 対象のため不要（MUST NOT）。

## 5.3 高度なロギング

* OpenTelemetry、構造化ログ、詳細メトリクスは不要（MAY）。

## 5.4 TDD の厳密運用

* 「常にテスト先行」は Stage1 では要求しない（MAY）。
  ただしテストを書くこと自体は必須（SHOULD）。

---

# 6. 開発者向けガイド（How to Start）

Stage1 の実装をスムーズに進めるため、次の順番で作業を行うことを推奨する。

## Step 1: Abstractions を作る（最初に Boundary）

* `IExchangeClient`
* `Ticker` DTO
* `Symbols`

→ Boundary が固まることで、Adapter / Infrastructure の設計が明確になる。

## Step 2: bitFlyer Raw モデルを作る

* `BitflyerTickerRaw` を bitFlyer JSON のフィールドに合わせて定義する。
* 公式 API の JSON をそのまま写す。

## Step 3: Public API（IBitflyerPublicApi）を作る

* `GetTickerRawAsync("BTC_JPY")` を定義。
* `IRestClient` を使って REST 通信を行う。

## Step 4: ExchangeClient 実装（BitflyerExchangeClient）

* `symbol` 検証
* `BTC/JPY` → `BTC_JPY` 変換
* `GetTickerRawAsync` 呼び出し
* Raw → Ticker マッピング
* 例外処理

## Step 5: テスト

* Abstractions.Tests（DTO / symbol 検証）
* Bitflyer.Tests（Raw → Ticker / 実通信 or モック）
* Infrastructure.Tests（RestClient / HttpTransport）

---

# 7. コード品質の最小基準（Minimal Quality Standards）

## 7.1 スレッドセーフティ

* `BitflyerExchangeClient` はステートレスとし、複数スレッドで安全に利用できる構造が望ましい（SHOULD）。

## 7.2 例外処理

* 例外は `ArgumentException` / `SymbolNotSupportedException` / `ExchangeApiException` のいずれかを使う（MUST）。
* HTTP / JSON 失敗は Transport / Protocol 側で検出し、`ExchangeApiException` に包む（MUST）。

## 7.3 ログ（任意）

* Stage1 では最低限で良い（MAY）。
* `ILogger` を受け取れる設計にしておくと Stage2 で役立つ（SHOULD）。

---

# 8. Stage1 の終了条件（Definition of Done）

Stage1 の開発は次が満たされた時点で完了とする。

1. `GetTickerAsync("BTC/JPY")` が正常に動作する。
2. Raw → Ticker マッピングが A040（SPC）に一致する。
3. 依存方向（ARC）が守られている。
4. Infrastructure が REST/HTTP の最小実装を提供している。
5. README に使用例が掲載されている。
6. A010 / A020 / A030 / A040 と矛盾がない。

---

# 9. Stage2 への移行（Next Steps）

Stage2 では次の強化が行われる。

* Transport の高度化（Retry / RateLimit / CircuitBreaker）
* Protocol の正式導入（署名生成 / timestamp / nonce）
* 認証 REST（Balance / Order / Position）
* WebSocket（Streaming Ticker / Board / Executions）
* 複数取引所（Binance / Bybit 等）の Adapter を追加
* Orchestration 層の本格実装

Stage1 で整備した Boundary・Infrastructure・Adapter の構造は、
Stage2 のこれら拡張を破綻なく受け入れる基盤となる。

---

# 10. 改訂履歴

| 版     | 日付         | 内容                                                           |
| ----- | ---------- | ------------------------------------------------------------ |
| 2.0.0 | 2025-11-XX | Stage1 新設計方針（Boundary + Modules）に合わせて全面改訂。開発者向け最小ガイドラインを再構成。 |
