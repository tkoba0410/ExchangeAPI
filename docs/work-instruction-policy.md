# ExchangeAPI Work Instruction Policy

最終更新: 2026-04-27
位置づけ: 実施指示の運用ガイド

## 1. 目的

本書は、チャットで合意した実施指示を repository 内の文書へ固定するための運用を定義する。

目的は次の 3 点である。

- release scope、非対象、完了条件を後から追跡できるようにする
- チャットだけに残った判断を減らす
- 実装中に scope や裁定が変わった場合の更新先を明確にする

本書は設計正本ではない。
実装 contract は `docs/spec.md`、venue / adapter / verification の各正本文書を優先する。

## 2. 原則

- 実施指示はチャットだけに残さない
- release / milestone 単位の指示は `docs/plan-vX.Y.Z.md` に固定する
- 継続的に参照する設計主題は専用文書へ分離する
- 実装中に scope、非対象、完了条件、裁定理由が変わった場合、対応する plan または topic doc も更新する
- release 後は結果を `docs/release-notes/vX.Y.Z.md` に残す
- 古い plan は当該 version の判断履歴として残す

## 3. 文書の役割

### 3.1 `docs/plan-vX.Y.Z.md`

その release の実施指示を固定する文書である。

置く内容:

- 目的
- scope / non-scope
- 実装順
- verification
- 完了条件
- release 中に行った主要裁定

置かない内容:

- version をまたいで維持する exact contract の全文
- endpoint / adapter / realtime などの継続正本
- release 後の結果だけを並べた release note

### 3.2 `docs/<topic>.md`

継続的に参照する設計正本または台帳である。

例:

- `docs/endpoints-bitflyer.md`
- `docs/endpoints-binance.md`
- `docs/cli.md`
- `docs/mcp-server.md`
- `docs/verification.md`
- `docs/realtime-bitflyer.md`

置く内容:

- version をまたいで維持する contract
- exact shape / support boundary / lifecycle rule
- 実装と test が従う固定点

### 3.3 `docs/roadmap-post-v2.md`

将来候補と見送り理由を置く文書である。

置く内容:

- version placement
- 採用候補
- 見送り候補
- まだ設計正本化しない将来テーマ

### 3.4 `docs/release-notes/vX.Y.Z.md`

release 結果を利用者向けにまとめる文書である。

置く内容:

- highlights
- compatibility
- migration note
- verification summary
- safety note

## 4. 標準フロー

1. チャットで方針を詰める
2. 合意した実施指示を `docs/plan-vX.Y.Z.md` に保存する
3. 作業前に plan と関連正本文書を読む
4. 実装中に裁定が変わった場合、plan または topic doc を更新する
5. verification 結果を plan または release checklist に反映する
6. release 時に `docs/release-notes/vX.Y.Z.md` へ結果をまとめる
7. release 後、次 version の plan を新しく作る

## 5. 禁止

- チャットの指示だけを根拠に release scope を進める
- plan 文書を更新せずに scope を広げる
- 完了条件を文書化せずに release する
- topic doc を更新せずに public surface、endpoint contract、adapter contract を変える
- release note を plan の代わりに使う

## 6. 判断基準

指示を `docs/plan-vX.Y.Z.md` に置くべき場合:

- release scope に影響する
- non-scope を固定する
- verification や完了条件を定義する
- 実装順や作業分割を固定する

topic doc に置くべき場合:

- version をまたいで参照される
- 実装や test が継続的に従う
- endpoint / adapter / realtime などの contract を定義する

`docs/roadmap-post-v2.md` に置くべき場合:

- まだ実装しない
- version placement だけを決めたい
- 見送り理由を残したい

`docs/release-notes/vX.Y.Z.md` に置くべき場合:

- release 済み内容を利用者に説明する
- migration impact をまとめる
- verification summary を残す
