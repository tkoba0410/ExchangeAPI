# Stage12（キックオフ）

最終更新: 2026-03-31  
対象ブランチ: `stage12`  
基準 tag: `v0.11.0-stage11`

## 1. 位置づけ

本書は、Stage12 の kickoff 用 goal document である。  
Stage11 で確定した library / CLI / MCP Server の基盤を前提に、Stage12 の対象範囲をここから固定していく。

library の設計正本は引き続き [`docs/spec.md`](docs/spec.md) に置き、  
CLI と MCP Server の adapter 固有方針は [`docs/cli.md`](docs/cli.md) と [`docs/mcp-server.md`](docs/mcp-server.md) に置く。

## 2. 現時点の前提

- `v0.11.0-stage11` を Stage12 の開始基準とする
- Stage11 の `Engineering Complete` と `Live Verified` は完了済みとみなす
- Library / CLI / MCP Server の現行契約は、そのまま Stage12 の初期 baseline として扱う

## 3. 直近のゴール

- Stage12 の対象スコープを明文化する
- Stage11 の成果物互換性を壊さずに次の変更を進める
- 新しい scope は本書と各正本文書に反映してから実装へ入る

## 4. 非ゴール

- Stage11 の完了状態を曖昧に戻すこと
- 正本未更新のまま新しい surface を拡張すること
- Stage11 の distribution / verification 基準を後退させること

## 5. 参照先

- [`docs/spec.md`](docs/spec.md)
- [`docs/cli.md`](docs/cli.md)
- [`docs/mcp-server.md`](docs/mcp-server.md)
- [`README.md`](README.md)
