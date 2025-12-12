# A020 STAGE7 RAW-FIRST ROADMAP

Raw-first を基本に、実装レベル（完全/主要/抽象/一部）で段階的に拡張するための変更ロードマップ。

## 方針
- レイヤーは一方向: Common(Transport/Policy/Contracts) → 取引所 Raw → 共通抽象ラッパ → 統合クライアント。
- 入口は Raw-first。抽象は各取引所に寄せたラッパとしてオプション提供。統合クライアントはそれらを束ねるだけにとどめる。
- 実装レベルを明示して期待値とテスト密度を管理する。

### 実装レベルの定義
- 完全: 公式 API ほぼ全網羅（Raw）、ライブ/モック両テスト、Breaking を明示。主力取引所向け。
- 主要: トレード基本セット（Ticker/Board/Executions、Send/Cancel、OpenOrders、Balance/Position）。Raw 優先、抽象は必要に応じて。
- 抽象: 共通インターフェースで主要機能を提供（差異は NotSupported 可）。DX 用の薄いラッパ。
- 一部: 探索/初期対応。限定エンドポイントのみ、モック中心テスト。将来どのレベルへ上げるかを Roadmap に記載。

## ロードマップ（案）
1. レイヤー整理と命名
   - フォルダ/名前空間を Raw と抽象で明示分離（例: Bitflyer.Raw / Bitflyer.Abstract）。
   - 統合クライアントに Primary 設定を持たせ、QuickStart のデフォルトを設定で差し替え可能にする。
2. レベル付与と範囲決定
   - 取引所ごとに実装レベルを宣言（例: bitFlyer=完全, Bittrade=主要, 新規=一部）。
   - README/Docs にレベル表を追加し、NotSupported や部分対応を明示。
3. テスト/CI 方針の切り分け
   - Raw を厚め、抽象/統合はスモークに抑えるルールを文書化。
   - ライブ統合テストはレベル「完全/主要」の主要経路のみを opt-in で実行。
4. ドキュメント更新
   - QuickStart を Raw-first に刷新。抽象は「共通化が必要なら」の章に分離。
   - 統合クライアントの Primary 切替方法、レベルの意味と対応表を追記。
5. 実装拡張の優先順位
   - 主力取引所（完全）の未実装 Raw API を優先消化。
   - 主要レベル取引所は基本セットを縦スライスで揃え、抽象ラッパを必要最低限で追加。
   - 一部レベルは探索的に進め、次のステップへ上げる条件を TODO に残す。

## 成果物
- レイヤー/名前空間整理後のプロジェクト構成と統合クライアントの Primary 設定。
- 実装レベル対応表（取引所 × レベル × 対応 API）。
- Raw-first QuickStart/Docs の更新と、抽象/統合の利用ガイド。
- テスト/CI のレベル別実行ポリシー。

## 段階的移行ステップ（現構成 → 最小プロジェクト構成）
0. 現状把握
   - 現在: `ExchangeApi.Contracts/Transport/Factory`, `adapter/Bitflyer|Bittrade|Common`, テストは対応する *.Tests。
   - 目標: プロジェクトを 3〜4 本に集約（Common.Core, Exchange.Bitflyer, Exchange.Bittrade, 任意で Unified.Client）、フォルダで Raw/Abstract を分離。
   - 命名: 新 csproj 名は `Common.Core`, `Exchange.Bitflyer`, `Exchange.Bittrade`, `Unified.Client`（任意）。新 namespace は `Common.*`, `Exchange.Bitflyer.*`, `Exchange.Bittrade.*`, `Unified.Client`.
1. Common をまとめる
   - `ExchangeApi.Contracts/Transport/Factory` を `Common.Core`（単一 csproj）に統合。名前空間は後方互換のため既存を保持しつつ新しいルートを段階導入。
   - テストも `Common.Core.Tests` にまとめる（既存テストをフォルダ移動）。
   - 作業チェック: `src/ExchangeApi.Contracts` 等の `Compile Include` を新 csproj に移動し、ソリューションに `Common.Core.csproj` を追加。テスト csproj も同様。
2. 取引所ごとの Raw/Abstract をフォルダで分離
   - `adapter/Bitflyer` 配下を `Raw/` と `Abstract/` フォルダに整理（プロジェクトは一つ: `Exchange.Bitflyer`）。抽象は Raw を呼ぶ薄い層に限定。
   - Bittrade も同様に整理し、共通コードは `Exchange/Common` か `Common.Core` に寄せる。
   - テストは `Exchange.Bitflyer.Tests` に集約し、フォルダで Raw/Abstract を分ける。
   - 作業チェック: 既存 `ExchangeApi.Adapter.Bitflyer` csproj を `src/Exchange.Bitflyer/Exchange.Bitflyer.csproj` に移動し、`<RootNamespace>` を `Exchange.Bitflyer` に設定。コード内 using を順次リネーム（旧 namespace は `using` alias で暫定対応可）。テスト csproj も同様。
3. 統合クライアントを追加（任意）
   - `Unified.Client` を追加し、各取引所クライアントを束ねるだけの薄いファサードに留める。Primary 設定を注入できる形に。
   - スモークテストを `Unified.Client.Tests` で追加（主要経路のみ）。
   - 作業チェック: `Unified.Client.csproj` を作成し、Bitflyer/Bittrade プロジェクト参照を追加。`IUnifiedClient` の API を最小限で定義し、DI 拡張 `AddUnifiedClient(primary: Bitflyer|Bittrade)` を用意。
4. Raw-first へのドキュメント更新
   - README/QuickStart を Raw-first で書き直し、抽象/統合はオプションとして別セクションに分離。
   - 実装レベル（完全/主要/抽象/一部）の対応表を追加し、各取引所の位置付けを明示。
   - 作業チェック: README と `docs/quickstart*.md` に新しいパッケージ名/名前空間/QuickStart コードを反映。対応表は `docs/stage7/TODO.md` または本ファイルに追記。
5. リネーム/パッケージ分割（必要なら）
   - NuGet/パッケージ名を新構成に合わせて段階的にリネーム。互換パッケージを一時的に残すか、メジャーバージョンで切り替えを告知。
   - 作業チェック: `Directory.Build.props` などで `PackageId` を新命名に変更。旧パッケージが必要なら `Obsolete` 属性と README で移行を案内。
6. CI/テスト運用の見直し
   - レベル別にテストスイートを分離（Raw 厚め、Abstract/Unified はスモーク）。
   - ライブ統合テストは opt-in で、完全/主要レベルのみ主要経路を実行。
   - 作業チェック: `dotnet test` 対象を新 csproj に更新。ライブテスト用の環境変数名（例: `EXCHANGEAPI_LIVE=1`）を決めてパイプラインに記載。

## 旧→新 対応の目安
- プロジェクト: `ExchangeApi.Contracts/Transport/Factory` → `Common.Core`; `adapter/Bitflyer` → `Exchange.Bitflyer`; `adapter/Bittrade` → `Exchange.Bittrade`; `ExchangeApi.Factory` の統合クライアント要素 → `Unified.Client`（任意）。
- 名前空間: `ExchangeApi.Contracts.*` → `Common.*`; `ExchangeApi.Transport.*` → `Common.Transport.*`; `ExchangeApi.Adapter.Bitflyer.*` → `Exchange.Bitflyer.*`; `ExchangeApi.Adapter.Bittrade.*` → `Exchange.Bittrade.*`.

## 構成イメージ（フォルダ/プロジェクト）※最小 csproj 本数を維持
```
src/
  Common/                     # <csproj: Common.Core> Transport/Policy/Contracts を束ねる基盤
    Common.Contracts/         # DTO/Errors（ソースフォルダ）
    Common.Transport/         # RestClient/Policy/Logging（ソースフォルダ）
  Exchange/
    Bitflyer/                 # <csproj: Exchange.Bitflyer> Raw/Abstract をフォルダで分離
      Raw/                    # bitFlyer Raw 実装
      Abstract/               # Raw を包むラッパ
    Bittrade/                 # <csproj: Exchange.Bittrade> Raw/Abstract をフォルダで分離
      Raw/
      Abstract/
    Common/                   # 取引所間で共有する補助があれば（ソースフォルダ）
  Exchange.Factory/           # <csproj: Exchange.Factory> 組み立てヘルパ（必要なら Unified の組み立てもここで）
tests/
  Common.Tests/               # <csproj: Common.Core.Tests>
    Common.Contracts.Tests/   # （サブフォルダ）
    Common.Transport.Tests/
  Exchange/
    Bitflyer.Tests/           # <csproj: Exchange.Bitflyer.Tests> Raw/Abstract をフォルダで分離
      Raw/
      Abstract/
    Bittrade.Tests/           # <csproj: Exchange.Bittrade.Tests>
      Raw/
      Abstract/
  Exchange.Factory.Tests/     # <csproj: ExchangeApi.Factory.Tests>（統合クライアントのスモークもここで扱う想定）
docs/
  ...                         # QuickStart (Raw-first), Abstract/Unified の利用ガイド
```

最小のプロジェクト本数（推奨）：Common.Core / Exchange.Bitflyer / Exchange.Bittrade / Exchange.Factory の4本。Unified.Client を別にしたい場合のみ5本目を追加。
