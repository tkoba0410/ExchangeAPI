# ExchangeAPI Architecture Change Notes

本ドキュメントは **既存の A010〜A060 文書は変更せず**、後で反映可能な形で、 「レイヤ構造ではなく、Boundary + Technical Modules」という観点からの **変更点・補足点だけを独立してまとめたもの**です。

---

## 1. 現状文書に対する補足方針（全体）

既存文書ではアーキテクチャが「4層構造」と読み取れる表現があるが、 より正確には **レイヤではなく、依存方向によって構成されるコンポーネント構造** である。

そのため本ドキュメントでは、元文書に追加すべき観点を次のように整理する：

* **Abstractions = Boundary（契約境界）**
* **Adapters / Protocol / Transport = Technical Modules（技術モジュール）**
* これらは上下の層ではなく、**平面的なモジュール群**であり、 依存方向だけで接続される。

---

## 2. 追加すべきアーキテクチャ概念

### 2.1 Boundary（契約境界）としての Abstractions

* 取引所非依存のインターフェースと共通 DTO だけを定義する唯一の安定境界
* ドメインに近い "domain-like contract" に相当
* 他モジュールはすべて Abstractions に依存する
* Abstractions は下位の技術には一切依存しない（Zero-dependency rule）

**→ 修正案として、Abstractions を「最上位層」ではなく「契約境界」と表現する方が正確**

---

### 2.2 Adapters / Protocol / Transport は "層" ではなく "技術モジュール"

#### Adapters（複数）

* 各取引所固有 API の実装
* 外部システム統合を担うモジュール
* Abstractions にのみ依存し、Protocol や Transport を利用可能

#### Protocol（単一）

* REST/WS の共通プロトコル処理
* 認証方式、標準リクエスト/レスポンス、署名、時刻同期など
* 技術的関心事に属し、Adapter が利用する

#### Transport（単一）

* HTTP / WebSocket の通信基盤
* レートリミット、リトライ、接続管理、共通ログなど
* Protocol より下位の技術モジュール

**→ 修正案として、これらを "層" として扱わず、"技術モジュール群" と呼ぶべき**

---

## 3. 正確な構造（将来の最終形）

### 3.1 図示（レイヤではなくモジュールの連結）

```
          ┌───────────────────────────┐
          │   Boundary / Abstractions  │
          │   (Interfaces + DTOs)      │
          └───────────────▲─────────────┘
                          │ depends on
              ┌───────────┼───────────┐
              │           │           │
┌─────────────┴────────┐  │  ┌─────────┴──────────────┐
│     Adapters (many)    │  │  │       Protocol (1)      │
│  Exchange-specific impl │  │  │ REST/WS common rules   │
└──────────────▲─────────┘  │  └───────────▲────────────┘
               │            │              │
               └────────────┴──────────────┘
                              uses
                      ┌──────────────────────┐
                      │   Transport (1)      │
                      │ HTTP/WS foundation   │
                      └──────────────────────┘
```

ポイント：

* 上下の階層ではなく、**Boundary を中心とした放射状構造**
* Adapters / Protocol / Transport は上下ではなく関係性で結ばれる
* レイヤードアーキテクチャではなく **Component-based Architecture**

---

## 4. 既存文書を修正するときの観点

後で A030-ARC などを改訂する際は、次の記述を追加すると整合性が取れる：

* 「4層」という表現ではなく **境界＋技術モジュール** と明記する
* Abstractions を "契約境界（Boundary）" と呼ぶ
* Adapters / Protocol / Transport を "技術モジュール（Technical Modules）" として分類
* 階層ではなく "依存方向による接続" を説明に採用

これにより、現在の縮退構造（Stage1）と将来構造（正典）を矛盾なく説明可能になる。

---

## 5. Stage1 における扱い

Stage1 では Transport / Protocol をまだ分離しないため：

* Adapters 配下に Raw + HTTP 呼び出しが一時的に内包されている
* これは将来の技術モジュール分離に向けた "縮退版" 実装である

**→ この点を既存文書とは別に注記しておくことで、後からの改訂が容易になる。**

---

## 6. まとめ

* 本ドキュメントは既存文書の代替ではなく、**後で反映するための追加ノート**
* 正しい表現は「4層構造」ではなく **Boundary + Technical Modules**
* Abstractions を唯一の契約境界とすることで、 将来の拡張（マルチ取引所・マルチアカウント・複数プロトコル）が容易になる

今後、必要であれば本ノートを A030-ARC や A010-OVR にマージするための **差分案（diff形式）** も作成できます。

---

## 7. 検証およびメリット・デメリット

### 7.1 検証

本変更点ドキュメントで提示した「Boundary + Technical Modules」の表現は、既存の段階的実装（Stage1〜Stage3）および将来の正典構造に適合するものであり、アーキテクチャの正確性を高める点で妥当である。特に、4 層モデルと解釈される危険性を解消し、依存方向に基づくコンポーネント構造としての理解が容易になる。

### 7.2 メリット

* **正確性の向上**：4層と誤解されがちな表現を修正し、実際の構成（Boundary + 技術モジュール）に即したものになる。
* **境界の明確化**：Abstractions を唯一の安定契約境界として明示し、他モジュールとの役割分担を明確化できる。
* **拡張性の向上**：取引所追加、プロトコル追加、通信方式変更などが契約境界を壊さずに行える。
* **縮退構造への対応**：Stage1 の Raw/HTTP 内包構造を矛盾なく説明でき、Stage2 以降の移行も容易。

### 7.3 デメリット

* **階層構造に慣れた読者には直感的でない**：伝統的なレイヤードアーキテクチャと異なるため理解コストが少し増える。
* **プロジェクト数が増え複雑に見える可能性**：技術モジュール分割により構成が一見複雑化する。
* **元文書への統合は手動作業が必要**：後で A010〜A060 に統合する際には編集が必要。
* **Stage1 実装との差異**：Transport/Protocol がまだ Adapter 内にあるため、現実装と最終構造にギャップが残る。

---

## 8. プロジェクト依存関係（csproj 観点）

### 8.1 依存関係の基本ルール

プロジェクト依存関係は、次のような方向で定義する。

* `ExchangeApi.Abstractions` は **どのプロジェクトにも依存しない**（Zero-dependency）。
* `ExchangeApi.Adapters.<Name>` は **Abstractions / Protocol / Transport に依存する**。
* `ExchangeApi.Protocol` は **Abstractions / Transport に依存することができる**（主に型共有と通信利用）。
* `ExchangeApi.Transport` は **他プロジェクトに依存しない**（下位技術基盤）。

### 8.2 矢印で表現した依存関係

略記：

* `abs` = `ExchangeApi.Abstractions`
* `adp` = `ExchangeApi.Adapters.<Name>`
* `pro` = `ExchangeApi.Protocol`
* `trn` = `ExchangeApi.Transport`

依存関係は次のようになる：

```text
abs ← adp → pro → trn
          ↘────────→ trn
```

これを展開すると：

* `abs ← adp`
  → Adapter は Abstractions のインターフェース（`IExchangeClient` 等）を実装するために依存する。

* `adp → pro`
  → Adapter は、REST / WebSocket の共通プロトコル処理を利用するために Protocol に依存する。

* `adp → trn`
  → Adapter は、単純な REST 呼び出しや WebSocket 接続など、Protocol を経由しない通信にも対応するため、Transport に直接依存できるようにする。

* `pro → trn`
  → Protocol は HTTP / WebSocket 通信を行うため、下位の Transport に依存する。

### 8.3 全体の依存構造図（プロジェクト間）

```text
           ┌─────────────────────┐
           │  ExchangeApi.Abs    │
           └────────▲────────────┘
                    │
          ┌─────────┴───────────┐
          │   ExchangeApi.Adapters.* │
          └────▲─────────▲──────┘
               │         │
               │ uses    │ uses
        ┌──────┴────┐ ┌──┴────────────────┐
        │ ExchangeApi.Protocol │  │ ExchangeApi.Transport │
        └──────────▲──────────┘  └──────────▲───────────┘
                   │                       │
                   └───────────────────────┘
                           uses
```

### 8.4 Stage1 における実装上の注意

* Stage1 では `Protocol` / `Transport` を独立プロジェクトとしてまだ切り出さず、`ExchangeApi.Bitflyer` 内部に縮退配置してもよい。
* その場合でも、将来の分離に備えて **依存方向（Abstractions にのみ上向き依存する）** というルールを守ることで、後から `Protocol` / `Transport` を独立プロジェクトに抽出しやすくなる。

このセクションは、後で csproj の参照構成を見直す際の指針として利用できる。

---

## 9. Abstractions の上位にクライアント束ね層（Orchestration）を追加する構想

### 9.1 背景

複数取引所・複数アカウントを前提にした場合、`IExchangeClient`（1取引所×1アカウント）の集合を
アプリケーションから扱いやすい形に束ねる「上位層」があると便利になる。

ここでは、`ExchangeApi.Abstractions` の **上に** 追加される「オーケストレーション層（Orchestration）」を
独立モジュールとして導入する構想を整理する。

### 9.2 プロジェクト構成案

将来構成（例）：

````text
src/
 ├─ ExchangeApi.Abstractions        ← 契約境界（IExchangeClient, DTO）
 ├─ ExchangeApi.Infrastructure      ← Protocol + Transport
 ├─ ExchangeApi.Bitflyer            ← 取引所アダプタ #1
 ├─ ExchangeApi.Binance             ← 取引所アダプタ #2（将来）
 └─ ExchangeApi.Orchestration       ← 複数クライアントを束ねる上位層
``

依存関係（概念）：

```text
アプリ / Bot / UI
        ▲
        │ uses
        │
ExchangeApi.Orchestration   （MultiExchange 層：束ねる層）
        ▲
        │ uses
        │
ExchangeApi.Abstractions    （契約境界：IExchangeClient, DTO）
        ▲          ▲
        │          │
Adapters.*       Infrastructure
````

### 9.3 Orchestration 層の役割

* 複数の `IExchangeClient` をまとめて管理する
* `exchangeId` / `accountId` によるクライアント選択を担う
* 「全取引所から Ticker を取得して最良レートを選ぶ」等の横断ロジックをカプセル化する
* アプリケーション側から見た「マルチ取引所・マルチアカウント用のファサード」として振る舞う

### 9.4 代表的なインターフェース例（案）

`ExchangeApi.Abstractions` における `IExchangeClient` は、1取引所×1アカウントを表す前提を維持する：

```csharp
public interface IExchangeClient
{
    string ExchangeId { get; }  // "bitflyer", "binance" など
    string AccountId  { get; }  // "primary", "bot1" など

    Task<Ticker> GetTickerAsync(string symbol, CancellationToken ct = default);
}
```

そのうえで、`ExchangeApi.Orchestration` にて次のようなプロバイダ／ファサードを定義する案：

```csharp
public interface IExchangeClientProvider
{
    IExchangeClient Get(string exchangeId, string accountId);

    IReadOnlyCollection<IExchangeClient> GetAll();
    IReadOnlyCollection<IExchangeClient> GetByExchange(string exchangeId);
}

public sealed class ExchangeClientProvider : IExchangeClientProvider
{
    private readonly Dictionary<(string exchangeId, string accountId), IExchangeClient> _clients;

    public ExchangeClientProvider(IEnumerable<IExchangeClient> clients)
    {
        _clients = clients.ToDictionary(
            c => (c.ExchangeId, c.AccountId),
            StringComparer.OrdinalIgnoreCase);
    }

    public IExchangeClient Get(string exchangeId, string accountId)
        => _clients[(exchangeId, accountId)];

    public IReadOnlyCollection<IExchangeClient> GetAll()
        => _clients.Values.ToList();

    public IReadOnlyCollection<IExchangeClient> GetByExchange(string exchangeId)
        => _clients
            .Where(x => x.Key.exchangeId.Equals(exchangeId, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Value)
            .ToList();
}

public sealed class MultiExchangeClient
{
    private readonly IExchangeClientProvider _provider;

    public MultiExchangeClient(IExchangeClientProvider provider)
    {
        _provider = provider;
    }

    public Task<Ticker> GetTickerAsync(string exchangeId, string accountId, string symbol, CancellationToken ct = default)
    {
        var client = _provider.Get(exchangeId, accountId);
        return client.GetTickerAsync(symbol, ct);
    }
}
```

このように、アプリケーションは `MultiExchangeClient` や `IExchangeClientProvider` を通じて
複数取引所・複数アカウントを一括して扱えるようになる。

### 9.5 この構想の位置付け

* 本 Orchestration 層は、`ExchangeApi.Abstractions` を汚さずに、
  その **上位に追加されるオプションレイヤ** として扱う。
* Stage1 ではアプリケーション側に直接定義してもよく、汎用性が確認できた段階で
  `ExchangeApi.Orchestration` プロジェクトとして切り出すことを想定する。
* プラグイン化（取引所アダプタ DLL の後付け読み込み）を行う場合にも、
  Provider/Orchestration 層があることで、マルチ取引所を横断的に扱う API を
  アプリケーションに対して安定して提供できる。

---

## 10. 推奨フォルダ構成（プロジェクト別）

以下は、Abstractions / Infrastructure / Adapter（取引所）/ Orchestration を想定した
**拡張可能で一貫性のあるフォルダ構成案**である。
将来複数取引所が追加されても破綻しにくい形を意図している。

### 10.1 リポジトリ全体

```text
ExchangeApi/
├─ src/
│  ├─ ExchangeApi.Abstractions/
│  ├─ ExchangeApi.Infrastructure/
│  ├─ ExchangeApi.Bitflyer/
│  ├─ ExchangeApi.Binance/             # 将来追加
│  └─ ExchangeApi.Orchestration/       # 将来（束ね層）
├─ tests/
│  ├─ ExchangeApi.Abstractions.Tests/
│  ├─ ExchangeApi.Infrastructure.Tests/
│  ├─ ExchangeApi.Bitflyer.Tests/
│  └─ ExchangeApi.Orchestration.Tests/
└─ docs/   # A010〜A060 などのドキュメント
```

---

### 10.2 Abstractions（契約境界）

最小でシンプルな構造を維持する。取引所共通の型だけをここに置く。

```text
ExchangeApi.Abstractions/
└─ src/
   ├─ Contracts/
   │   └─ IExchangeClient.cs
   ├─ Dtos/
   │   ├─ Ticker.cs
   │   └─ Symbol.cs
   └─ Errors/
       ├─ ExchangeApiException.cs
       └─ SymbolNotSupportedException.cs
```

* **Contracts** … インターフェース類
* **Dtos** … 取引所非依存の DTO
* **Errors** … 共通例外

---

### 10.3 Infrastructure（Protocol + Transport）

論理的には Protocol と Transport に分かれているが、物理上は 1 プロジェクトにまとめる構成。
後に必要なら分離可能なよう、明確にフォルダを分割する。

```text
ExchangeApi.Infrastructure/
└─ src/
   ├─ Protocol/
   │   ├─ Rest/
   │   │   ├─ RestRequest.cs
   │   │   ├─ RestResponse.cs
   │   │   └─ IRestProtocolClient.cs
   │   ├─ WebSocket/
   │   │   ├─ IWebSocketProtocolClient.cs
   │   │   └─ SubscriptionModels.cs
   │   └─ Auth/
   │       ├─ IHmacSigner.cs
   │       └─ TimestampProvider.cs
   ├─ Transport/
   │   ├─ Http/
   │   │   ├─ IHttpTransport.cs
   │   │   ├─ HttpTransport.cs
   │   │   └─ HttpRetryPolicy.cs
   │   └─ WebSocket/
   │       ├─ IWebSocketTransport.cs
   │       └─ WebSocketTransport.cs
   └─ Serialization/
       ├─ IJsonSerializer.cs
       └─ SystemTextJsonSerializer.cs
```

---

### 10.4 Adapter（取引所アダプタ：Bitflyer 例）

各取引所ごとに独立プロジェクトを作成し、同じパターンで整理する。

```text
ExchangeApi.Bitflyer/
└─ src/
   ├─ Client/
   │   └─ BitflyerExchangeClient.cs    # IExchangeClient 実装
   ├─ Raw/
   │   └─ BitflyerTickerRaw.cs
   ├─ PublicApi/
   │   ├─ IBitflyerPublicApi.cs
   │   └─ BitflyerPublicApi.cs         # Stage1: Transport/Protocol内包可
   ├─ Mapping/
   │   ├─ BitflyerTickerMapper.cs
   │   └─ BitflyerSymbolMap.cs
   └─ Config/
       └─ BitflyerEndpoints.cs
```

この構造をコピーすれば、新しい取引所（Binance / Bybit / OKX など）も同様に展開できる。

---

### 10.5 Orchestration（将来の「束ねる層」）

複数取引所・複数アカウントを統合して扱うための高レベル API を管理する層。
最初はアプリ実装側に置き、必要に応じて独立プロジェクト化すればよい。

```text
ExchangeApi.Orchestration/
└─ src/
   ├─ Providers/
   │   ├─ IExchangeClientProvider.cs
   │   └─ ExchangeClientProvider.cs
   ├─ Facade/
   │   └─ MultiExchangeClient.cs
   └─ Strategies/
       └─ BestPriceTickerService.cs
```

---

### 10.6 まとめ

* 取引所ごとにプロジェクトを新規作成して並列に配置できる。
* Infrastructure は Protocol/Transport の論理層を明確に分離しつつ、物理的には 1 プロジェクト。
* Orchestration は Abstractions の上位に追加され、アプリ側からの利用を平易にする。
* 各層・各プロジェクトで責務を明確に分割しておくことで、将来の拡張（複数取引所・複数アカウント・プラグイン化）が容易になる。
