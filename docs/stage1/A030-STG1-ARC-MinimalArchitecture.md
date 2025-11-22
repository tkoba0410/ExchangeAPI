# 0310-ARC-STG1-MinimalArchitecture
Stage 1 最小アーキテクチャ構成（ARC）

---

## 1. 目的（Purpose）

このドキュメントは、Exchange API Library の **Stage 1 に必要な最小アーキテクチャ構造（ARC）** を定義する。

Stage 1 では、Public REST（bitFlyer Ticker）のみを対象とするため、複雑なアーキテクチャは不要。
本書では、**Abstractions / Adapter / Raw の 3 層構造** と、**依存関係ルール**、**プロジェクト構成**のみを明文化する。

正典は **4 層構造（Abstractions → Adapters → Protocol → Transport）** であるが、Stage1 では Protocol / Transport を利用しない **縮退版の 3 層（Abstractions / Adapter / Raw）** のみを採用する。対象が bitFlyer Public REST `getticker` のみに限定され、共通 Protocol や高機能 Transport を導入しないためである。

Stage 2 以降で拡張されるが、本 ARC は Stage 1 の範囲で揺らがない“正典”を提供する。

---

## 2. レイヤ構造（Layered Architecture）

Stage 1 は次の 3 層で構成される：

```
+---------------------------+
|      Abstractions 層      |
| (IExchangeClient, DTOs)   |
+---------------------------+
            ↑ 依存
+---------------------------+
|       Adapter 層          |
| (BitflyerExchangeClient)  |
+---------------------------+
            ↑ 依存
+---------------------------+
|       Raw モデル層        |
| (BitflyerTickerRaw)       |
+---------------------------+
```

### 2.1 Abstractions 層
- 役割：取引所に依存しない **共通 API と DTO** を提供する。
- 主な構成：
  - `IExchangeClient`
  - `Ticker` DTO
  - `Symbols` 定数
- 特徴：
  - **依存先なし（最上位）**
  - Adapter や Raw を知らない

### 2.2 Adapter 層
- 役割：bitFlyer Public REST を呼び出し、Raw モデルを一般化 DTO に変換する。
- 主な構成：
  - `BitflyerExchangeClient`
  - `IBitflyerPublicApi`
- 特徴：
  - Abstractions に依存して DTO を返す
  - Raw モデルを内部で利用
  - Stage1 では `HttpClient` を含む Transport の詳細は Adapter 層（`IBitflyerPublicApi` 実装）の内部実装とし、Abstractions からは見えない。

### 2.3 Raw モデル層
- 役割：bitFlyer のレスポンス JSON を **欠損なく保持する取引所固有モデル**。
- 主な構成：
  - `BitflyerTickerRaw`
- 特徴：
  - Adapter からのみ参照される
  - Abstractions の詳細を知らない（純粋な取引所仕様の写像）
  - Stage1 では **物理プロジェクトとして分けず、Adapter プロジェクト内の論理レイヤ** として実装する。

---

## 3. 依存関係ルール（Dependency Rules）

Stage 1 の依存方向は厳格に次の形を守る：

```
Abstractions   ←   Adapter   ←   Raw
```

### 3.1 許可される依存
- Adapter → Abstractions
- Adapter → Raw

### 3.2 許可されない依存
- Abstractions → Adapter（禁止）
- Abstractions → Raw（禁止）
- Raw → Abstractions（禁止）
- Raw → Adapter（禁止）

### 3.3 理由（Rationale）
- 将来の Stage2〜StageN で複数取引所を扱う際も、Abstractions が不変でいられるため。
- 新規取引所を追加しても Raw と Adapter を追加するだけでよく、**OCP（拡張開放）** を満たすため。
- Stage 1 の時点で設計の揺らぎを防ぐため。

OVR / REQ / SPC は本節を依存方向・禁止ルールの正典として参照し、各文書での重複列挙を行わない。

---

## 4. プロジェクト構成（Project Structure）

Stage 1 では **2 プロジェクト構成** を採用する。  
最小であるが、将来拡張にも耐えられる。

```
project-root/
  src/
    ExchangeApi.Abstractions/
      ExchangeApi.Abstractions.csproj
      IExchangeClient.cs
      Models/
        Ticker.cs
      Symbols.cs

    ExchangeApi.Bitflyer/
      ExchangeApi.Bitflyer.csproj
      IBitflyerPublicApi.cs
      Models/
        BitflyerTickerRaw.cs
      BitflyerExchangeClient.cs

  tests/
    ExchangeApi.Abstractions.Tests/
    ExchangeApi.Bitflyer.Tests/

  docs/
    0120-OVR-INTR-ProjectOverview.md
    0210-REQ-STG1-AbstractionsMVP.md
    0310-ARC-STG1-MinimalArchitecture.md
    Stage1-spec-full.md
```

### 4.1 この構成のメリット
- Abstractions と Adapter の分離が明確で可視化される
- bitFlyer Adapter を別の取引所（例: `ExchangeApi.Binance`, `ExchangeApi.Bybit`）に横展開しやすい
- テスト単位が明確（Abstractions / Adapter で分断可能）
- 依存方向を IDE 上で容易に確認できる

### 4.2 テストプロジェクトの依存ルール
- `ExchangeApi.Abstractions.Tests` は **Abstractions プロジェクトのみ** に依存することを推奨する。
- `ExchangeApi.Bitflyer.Tests` は Abstractions および Bitflyer Adapter（Raw を含む）に依存してよい。
- テストプロジェクトは本番コードの依存ルールを破ってもよいが、**Abstractions のテストは Abstractions のみを対象にする** 方針を基本とし、レイヤ間の責務分離を保つ。

---

## 5. Stage2 以降への拡張ポイント（参考）
※ Stage 1 の責務ではないが、アーキテクチャ保全のために記載

- Adapter 下に **PrivateAPI（認証）層** を追加可能
- Raw モデルは Board / Balance / Order などを段階的に拡張可能
- Transport 層（HttpClient ラッパ）を導入可能
- Abstractions は DTO とインターフェースを増やすだけで対応

これらの拡張は、Stage 1 の 3 層構造と依存ルールを崩さずに実現できる。

---

## 6. 結論
本 ARC 文書は、Stage 1 における **最小で揺らぎのないアーキテクチャ正典** を定義するものであり、
Spec と REQ を結ぶ“骨格”として設計された。

Stage 1 の範囲ではこの構造で十分であり、Stage 2 以降もこの構造を前提に拡張できる。

---

## 改訂履歴

| 版 | 日付 | 内容 |
|----|------|------|
| v1.1.1 | 2025-05-05 | 依存方向の正典が本節に一元化されることを明示し、改訂履歴を更新。 |
| v1.1.0 | 2025-05-05 | Stage1 縮退構造の位置づけを 4 層正典との関係で明記し、重複表現を整理。 |

