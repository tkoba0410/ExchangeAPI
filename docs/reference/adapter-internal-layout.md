# Adapter Internal Layout (Non-Normative)

本書は `docs/normative/topspec.md` の実装運用を補助するための **Non-Normative** 文書である。  
規範判断は必ず `topspec` / `governance` を正とする。

## Purpose

- 取引所横断で `Adapter/Internal` の責務と配置を揃える
- 意味分類の再導入を防ぐ
- 新規取引所追加時の実装手順を固定する

## Canonical Layout

```text
src/Exchanges/<Exchange>/Adapter/
  Public/Api/
    PublicClient.cs
  Private/Api/
    ExchangeClient.cs
  Internal/
    Orchestration/
      PublicFlow.cs
      PrivateFlow.cs
    Resolve/
      ExchangeMarketCatalog.cs
      ExchangeRequestResolver.cs
      NormalizedRequestResolver.cs
    Execute/
      NormalizedExecutor.cs
    Map/
      ContractMapper.*.cs
    Error/
      CallErrorTranslator.cs
      ErrorClassifier.cs
```

## Responsibility Boundary

- `PublicClient` / `ExchangeClient`: entrypoint。業務処理を持たず委譲のみ。
- `Orchestration`: `Resolve -> Execute -> Map` のフロー制御。
- `Resolve`: Symbol/ProductCode 解決と市場定義保持。
- `Execute`: Normalized 呼び出しの共通実行ラッパ。
- `Map`: Normalized から Contracts への変換。
- `Error`: 例外・エラー分類の変換。

## Naming Rules

- namespace は物理配置に一致させる。
- `internal` 主分類に意味語彙（MarketData/Trading/Account/History）を使わない。
- `Resolve/Map/Error/Execute/Orchestration` を主分類軸として固定する。

## Migration Checklist

1. 旧 `Internal/Mappers` / `Internal/MarketCatalog` の型を新フェーズへ移動する。
2. namespace と `using` を物理配置に合わせる。
3. `PublicClient` / `ExchangeClient` が flow 委譲のみであることを確認する。
4. ShapeGuard を更新し、レイアウト逸脱を検知できる状態にする。
5. `dotnet build ExchangeApi.slnx -warnaserror` と `dotnet test ExchangeApi.slnx` を通す。

## Anti-Patterns

- `Internal/MarketData` / `Internal/Trading` のような意味分類フォルダ追加
- `PublicClient` や `ExchangeClient` での直接 map/resolve 実装
- `Adapter` から `Normalized.Internal.*` への直接依存
