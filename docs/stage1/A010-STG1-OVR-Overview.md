---
doc_id: 0120-OVR-INTR
title: Project Overview
version: 1.1.1
status: Draft
date: 2025-05-05
confidentiality: Public
---

# [OVR-INTR] プロジェクト概要

## 1. 文書の位置づけ（正典）

- 正典の対象範囲：本書は次の **規範領域** を包含し、相互の矛盾時は本書が優先する。  
  1) **アーキテクチャ規範**（4層と依存方向／層責務）  
  2) **開発手法規範**（STD‑DEVP: Spec‑Conformance Development の要点）  
  3) **文書管理規範**（STD‑DOCS: 文書体系・改訂・PR レビュー）
- 下位文書（STD‑DEVP / STD‑DOCS）が本書と矛盾する場合、**本書の記述が正**となる。

- 本書（**OVR-INTR**）は本プロジェクトの **唯一の正典（single source of truth）** である。
- 本書に反する過去・他文書の記述は **破棄/修正** する（整合性維持の責任は各文書にある）。
- 文書の優先順位（基本）：**OVR-INTR** ＞ **STD/REQ** ＞ **ARC** ＞ **IMP** ＞ **TST** ＞ **OPS**  
  ※ **SEC（禁則/法令順守）**は常に優先される。
- Exchange API Library の **4層構造（Abstractions → Adapters → Protocol → Transport）** を正とするが、Stage1 では **Protocol / Transport を利用しない縮退版として Abstractions / Adapter / Raw の 3 層のみを採用**する。縮退の理由は、スコープが bitFlyer Public REST `getticker` のみに限定され、共通 Protocol や高機能 Transport を導入しないためである。
  層名は **“Adapters”（複数形）** を用いる。ドメイン判断（戦略/意思決定）は **対象外**。
- レイヤ間の依存方向および禁止ルールの正典は `A030-STG1-ARC-MinimalArchitecture.md` に定義される。
- 変更管理：すべて **Pull Request 経由（MUST）**／少なくとも 1 名承認。  
  本書の意味変更は **ADR 必須 + SemVer 版上げ**。変更後は **0900-OVR-COMP** を再生成する。

## 2. 目的

本書は **Exchange API Library** の全体像を提示し、利用者・開発者・関係者が共通認識を持てるようにする。  
背景・狙い・基本構成を示し、以降の系統文書（REQ / ARC / IMP 等）への導入を担う。

---

## 3. 範囲

本書が対象とする範囲と対象外を明確化する。  

**対象範囲**  
- 暗号資産取引所の REST/WS API を横断して扱う統一化ライブラリ  
- API 差異の吸収と標準化  
- OSS 公開を前提とした品質基盤の提示  

**対象外**
- 個別の取引戦略・意思決定ロジック（**本ライブラリの対象外**）
- UI/UX アプリケーション層  
- 個別事業者の業務要件への直接的対応  

---

## 4. 背景

取引所ごとに REST/WS の仕様が大きく異なることにより、次の課題が生じる：  

- 認証方式・レート制御・レスポンス形式・WebSocket 仕様の相違  
- 実装の重複と保守コストの肥大化  
- OSS/商用で求められる拡張性・セキュリティの不足  

本ライブラリは、これらの課題を解消するための **抽象化レイヤ** を提供する。

---

## 5. プロジェクトの目的

- 取引所 API を **共通インターフェースで統一的に利用可能** とする  
- ドメイン判断（業務ポリシー/戦略/意思決定）は **ライブラリへ持ち込まない**  
- **DDD の Infrastructure 層を意識した設計**とし、接続と適応を責務とする  
- **抽象化できない取引所固有 API** も、安全に利用可能とする  
- **OSS公開を前提** とし、外部開発者にもわかりやすい文書体系を提供する  
- 金融システムに必要な **信頼性・セキュリティ** を担保する  
- **REST/WS** の両方をサポートし、**ストリーム通信**にも対応する  

---

## 6. 基本構造（NuGet パッケージ）

本ライブラリは、次の **4 層構造** と対応パッケージを基本とする。

### 1. Abstractions — `ExchangeApi.Abstractions`  
共通インターフェース（`IExchangeClient`, `IMarketStreamClient`）、DTO/VO、結果・例外体系、Capabilities を備える **契約層**。  
外部アプリは原則として本層のみに依存すればよい。

### 2. Adapters — `ExchangeApi.Adapters.<Name>`  
各取引所の具象実装。取引所固有のレスポンスを共通 DTO へマッピングし（Anti-Corruption）、機能差の吸収を担う。

### 3. Protocol — `ExchangeApi.Protocol`  
取引所に共通する **仕様上の要素** を集約する。  
- REST/WS の署名・認証  
- timestamp/nonce 規約  
- チャネル/サブスクメッセージ形式  
- エンドポイント表現  
- シリアライズ規約  

### 4. Transport — `ExchangeApi.Transport`  
HTTP/WS の通信基盤と **Policy/Pipeline** を提供する。  
- リトライ、再接続、レート制御  
- サーキットブレーカ  
- 冪等化  
- ロギング／メトリクス／トレース  
- バックプレッシャ  

※ 旧「Rest.Extension」に相当する機能は、本層のポリシーとして統合する。  

> レイヤ間の依存方向および禁止ルールの正典は `A030-STG1-ARC-MinimalArchitecture.md` に定義される。

Stage1 では上記 4 層正典を縮退させ、**Abstractions / Adapter / Raw の 3 層構造のみを実際に使用**する。bitFlyer Public REST `getticker` のみを扱うため、共通 Protocol や Transport の導入は次フェーズ以降に委ねる。

---

## 7. 想定利用者

- **OSS 利用者**：統一的な Exchange API 呼び出しを必要とする外部開発者
- **プロジェクト開発者**：ライブラリの設計・実装・運用を担うメンバー
- **ステークホルダー**：金融システムや商用サービスでの適用を検討する関係者

---

## 8. Stage1 の通信・例外ポリシー（縮退版）

Stage1 における HTTP / 例外ポリシーは次に限定する。

- `HttpClient` は DI から供給し、使い捨てしない。
- タイムアウトは 5〜10 秒を推奨とする。
- `CancellationToken` は `SendAsync` に伝播する。
- `User-Agent` を明示的に設定する。
- 入力エラーは `ArgumentException` 系を使用する。
- API エラーおよび内部エラーは `ExchangeApiException` で通知する。
- Result 型など高度なエラーモデル拡張は **Stage1 の対象外** とする。

このポリシーは REQ（NFR-2/3）および SPC（6・7 章）と整合するよう維持する。

## 付録A. 開発手法（SCD）— 規範要約（正典）

- **要求ID**：`{Layer}-{Type}{Seq}`（例: `TRN-R001`）。一意／欠番再利用 **禁止 (MUST NOT)**。  
- **規範表現**：RFC2119 キーワード（MUST/SHOULD/MAY）を用いる **MUST**。  
- **適合検証**：要求ID ⇔ テスト ⇔ 実装を **Conformance Matrix** で連結 **MUST**。  
- **CIゲート**：変更関連の適合テスト **100% 合格 (MUST)**、`docs/0900-OVR-COMP` **重大 0 (MUST)**。  
- **TDD**：テスト先行で最小実装 → リファクタリング **MUST**。  
- **実装順**：Abstractions → Adapters → Protocol → Transport **SHOULD**（*依存方向の MUST を置換しない注記*）。  
- **逸脱/破壊的変更**：MUST の弱体化/削除 **禁止 (MUST NOT)**。変更は **ADR + SemVer (MUST)**。

> 詳細は `docs/0830-STD-DEVP-DevelopmentProcess.md` を参照（本付録が矛盾時に優先）。  
> なお、0830-STD-DEVP-DevelopmentProcess.md はプロジェクト固有の適用標準であり、  
> 上位規範として `docs/standards/0200-STD-DVP0-CycleGuide.md` を参照する。  
> プロジェクト固有の差異や追加ルールは 0830 内で明示される。

## 付録B. 文書管理規則 — 規範要約（正典）

- **更新経路**：すべて **Pull Request 経由 (MUST)**、最低 1 名の承認 **MUST**。  
- **改訂履歴**：各文書に記録 **MUST**。番号変更と内容変更は分離が望ましい **SHOULD**。  
- **命名/番号**：`docs/` 配下の分類・命名・4桁番号を **STD に従う (MUST)**。  
- **依存順記載**：主要文書は依存順（Abstractions → Adapters → Protocol → Transport）を明記 **SHOULD**。  
- **生成物の扱い**：`0900-OVR-COMP` は機械生成であり、診断対象からの除外または別扱いを**推奨 (SHOULD)**。

> 詳細は `docs/0800-STD-DOC0-DocumentPolicy.md` を参照（本付録が矛盾時に優先）。  
> なお、0800-STD-DOC0-DocumentPolicy.md はプロジェクト固有の文書管理標準であり、  
> 上位規範として `docs/standards/0100-STD-DOC0-DocumentPolicy.md` を参照する。  
> プロジェクト固有の差異や補足ルールは 0800 内で明示される。

---

## 改訂履歴

| 版 | 日付 | 内容 |
|----|------|------|
| v1.1.1 | 2025-05-05 | Stage1 HTTP/例外ポリシーの整合先（REQ/SPC）を明示し、版情報を更新。 |
| v1.1.0 | 2025-05-05 | Stage1 縮退構造の明記、依存ルール参照先の統一、通信/例外ポリシーの Stage1 固定化、章番号の更新を実施。 |
| v1.0.0 | 2025-10-10 | 初版。0830/0800を参照対象として統一し、各々が standards 群との差異を明示する方針を採用。 |

