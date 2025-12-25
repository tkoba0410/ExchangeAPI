# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# Observability

このドキュメントは、
`Common.Transport` における **ログ・メトリクス・トレース（可観測性）**の設計と利用方法を定義する。

Observability は、
**通信の結果や振る舞いを外部から観測可能にするための仕組み**であり、
通信の成否・再試行・制御の判断そのものには影響を与えない。

---

## 基本方針

1. **観測は副作用である**  
   Observability は処理結果を変更してはならない

2. **ログと観測を分離する**  
   人が読むログと、機械が集計するメトリクスを分けて扱う

3. **NoOp をデフォルトにする**  
   観測を設定しなくても正しく動作する

---

## 主な構成要素

### IRestClientLogger

`IRestClientLogger` は、
**通信結果をログとして出力するためのインターフェース**である。

- 成功・失敗を人間向けに記録する
- 構造化ログを前提とする

代表実装：
- `StructuredRestClientLogger`
- `NoOpRestClientLogger`

---

### IRestCallObserver

`IRestCallObserver` は、
**通信呼び出し単位のイベントを観測するためのインターフェース**である。

Observer は以下のイベントを受け取る。

- `OnRequest`
- `OnResponse`
- `OnError`

Observer は、
- ログ
- メトリクス
- トレース

のいずれにも利用できる。

---

### RestCallContext

`RestCallContext` は、
**1 回の通信呼び出しに紐づくコンテキスト情報**を保持する。

主な情報：

- RequestId
- HTTP メソッド
- URI
- 開始時刻
- 任意のタグ情報

Observer / Logger は、
この Context を用いて相関付けを行う。

---

## OpenTelemetry / Metrics

### OpenTelemetry Observer

- `RestCallOpenTelemetryObserver`
- 分散トレーシング用
- 呼び出し単位の Span を発行

---

### Metrics Observer

- `RestCallMetricsObserver`
- レイテンシ・回数・失敗数を計測
- Meter / Counter / Histogram を利用

---

## Observer の通知順序

1. `OnRequest`
2. 実送信・Retry・Policy 適用
3. `OnResponse`（成功時）
4. `OnError`（失敗時）

Observer は、
**通知順序に依存した副作用を持ってはならない**。

---

## NoOp 実装の役割

- 観測を行わない場合のデフォルト
- 条件分岐を不要にする
- パフォーマンスを最小化する

---

## 実装上の注意

- Observer 内で例外を投げてはならない
- 観測処理はできる限り軽量にする
- 通信処理の同期性を阻害しない

---

## Common.Contracts との関係

Observability は、
`ExchangeApiException` や `ExchangeErrorCategory` を参照できるが、
**判断や変換を行ってはならない**。

あくまで「観測」に徹する。

---

## まとめ

- Observability は副作用レイヤ
- Logger と Observer を分離
- OTel / Metrics による拡張が可能
- Transport の挙動に影響を与えない

