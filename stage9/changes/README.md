# Changes Policy

本ディレクトリは、破壊的変更（Breaking Change）を記録するための場所である。

ExchangeAPI は破壊的変更を許容する。
ただし、統治可能性を維持するため、必ず記録を残す。

---

# 記録が必要な場合

以下に該当する場合、変更記録を作成する。

* public surface の変更
* 既存APIの挙動変更
* 例外/エラーモデルの変更
* DTO構造の変更
* 呼び出しコードに修正が必要となる変更

---

# 記録ファイルの命名規則

```
CHANGE-YYYYMMDD-<short-description>.md
```

例:

```
CHANGE-20260216-normalized-error-model.md
```

---

# 記録内容（最小構成）

各変更記録には、以下を記載する。

## Summary

変更内容の要約

## What broke

何が壊れるか（影響範囲）

## Why

なぜこの変更を行ったか

## Migration

移行方法（最短手順）

## Bot impact

Botリポジトリへの影響（あれば）

---

# 原則

* Breaking Change は許容する
* ただし、無記録の破壊は許容しない
* 記録は簡潔でよいが曖昧にしない

---

本ポリシーは review-framework.md の Change 軸に基づく。
