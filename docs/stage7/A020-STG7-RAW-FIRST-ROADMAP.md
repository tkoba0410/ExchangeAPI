# A020 STAGE7 RAW-FIRST ROADMAP

Raw-first を基本に、実装レベル（完全/主要/抽象/一部）で段階的に拡張するための変更ロードマップ。

## 方針
- 2段×2層（計4層）で一方向: 下段=実装系（Common: Transport/Policy/Contracts → 取引所 Raw）、上段=抽象系（取引所抽象 → 薄い統合ファサード）。
- 入口は Raw-first。抽象は「基本取引ロジックに必要な最小」だけをラップし、統合ファサードは束ねるだけ（追加抽象は作らない）。
- 実装レベル（完全/主要/抽象/一部）を明示して期待値とテスト密度を管理する。

### 実装レベルの定義
- 完全: 公式 API ほぼ全網羅（Raw）、ライブ/モック両テスト、Breaking を明示。主力取引所向け。
- 主要: トレード基本セット（Ticker/Board/Executions、Send/Cancel、OpenOrders、Balance/Position）。Raw 優先、抽象は必要に応じて。
- 抽象: 共通インターフェースで主要機能を提供（差異は NotSupported 可）。DX 用の薄いラッパ。
- 一部: 探索/初期対応。限定エンドポイントのみ、モック中心テスト。将来どのレベルへ上げるかを Roadmap に記載。

## ロードマップ（案）
1. レイヤー整理と命名
   - フォルダ/名前空間を「実装系（Common/Raw）」と「抽象系（Abstract/統合）」に分ける（例: Bitflyer.Raw / Bitflyer.Abstract、統合は Factory 内の薄いヘルパ）。
   - 統合クライアントに Primary 設定を持たせ、QuickStart のデフォルトを設定で差し替え可能にする。
2. レベル付与と範囲決定
   - 取引所ごとに実装レベルを宣言（例: bitFlyer=完全, Bittrade=主要, 新規=一部）。
   - README/Docs にレベル表を追加し、NotSupported や部分対応を明示。
3. テスト/CI 方針の切り分け
   - Raw を厚め、抽象/統合はスモークに抑えるルールを文書化。クロスカット（署名/リトライ/レート制限/ログ）は Common.Transport で吸収。
   - ライブ統合テストはレベル「完全/主要」の主要経路のみを opt-in で実行。
4. ドキュメント更新
   - QuickStart を Raw-first に刷新。抽象は「共通化が必要なら」の章に分離。
   - 統合クライアントの Primary 切替方法、レベルの意味と対応表を追記。
5. 実装拡張の優先順位
   - 主力取引所（完全）の未実装 Raw API を優先消化。
   - 主要レベル取引所は基本セットを縦スライスで揃え、抽象ラッパを必要最低限で追加。
   - 一部レベルは探索的に進め、次のステップへ上げる条件を TODO に残す。

## 成果物
- 2段×2層のレイヤー/名前空間整理後のプロジェクト構成と統合クライアントの Primary 設定。
- 実装レベル対応表（取引所 × レベル × 対応 API）。
- Raw-first QuickStart/Docs の更新と、抽象/統合の利用ガイド。
- テスト/CI のレベル別実行ポリシー。
- 使用パターンの明示: Bitflyer/Bittrade はそれぞれ単独で利用可能（各 Exchange.* の Factory を直接使用）。共通設定・認証・複数取引所の配線をまとめたい場合のみ Exchange.Factory を経由するオプション。

## 段階的移行ステップ（現構成 → 最小プロジェクト構成）
0. 現状把握
   - 現在: `Common.Core` に集約済み。`Exchange.Bitflyer` / `Exchange.Bittrade` は Raw/Abstract にフォルダ分離済み、`Exchange.Factory` に統合ヘルパあり。
   - 目標: 2段×2層を明示（実装系: Common + Raw、抽象系: 各取引所抽象 + 統合）。プロジェクト本数は 4 本を維持。
   - 命名: csproj 名は `Common.Core`, `Exchange.Bitflyer`, `Exchange.Bittrade`, `Exchange.Factory`。統合クライアントは Factory 内のヘルパとして扱う。
1. Common をまとめる
   - `ExchangeApi.Contracts/Transport/Factory` を `Common.Core`（単一 csproj）に統合。名前空間は後方互換のため既存を保持しつつ新しいルートを段階導入。
   - テストも `Common.Core.Tests` にまとめる（既存テストをフォルダ移動）。
   - 作業チェック: `src/ExchangeApi.Contracts` 等の `Compile Include` を新 csproj に移動し、ソリューションに `Common.Core.csproj` を追加。テスト csproj も同様。
2. 取引所ごとの Raw/Abstract を維持しつつ 2段×2層を明示
   - Raw は仕様準拠で厚め（PublicGet/PrivateGet/PrivatePost/Signer/RawApi に分離）。クロスカットは Common.Transport で吸収。
   - 抽象は「基本取引ロジックに必要な最小」（板/約定/発注/残高など）のみをラップ。差異は `NotSupported` を許容。
   - テストは Raw 厚め + 抽象スモークで分離し、`.Tests` フォルダも Raw/Abstract で区切る。
3. 統合クライアントの扱い
   - Exchange.Factory 内の薄いヘルパで、各取引所抽象を束ねるだけに留める。二重抽象は作らない。
   - スモークテストは Factory.Tests で実施。Primary 設定でデフォルト取引所を切り替え可能にする。
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
- プロジェクト: `Common.Core`（Transport/Policy/Contracts） → そのまま。`Exchange.Bitflyer`/`Exchange.Bittrade` → Raw/Abstract をフォルダ分離した単一 csproj のまま。`Exchange.Factory` → 統合ヘルパ（Unified を含む）。
- 名前空間: `ExchangeApi.*` 旧系は段階的に `Common.*` / `Exchange.*` に寄せる。既存 using は互換のため当面残してよい。

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
  Exchange.Factory/           # <csproj: Exchange.Factory> 組み立てヘルパ（統合クライアントの組み立てもここで実施）
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

最小のプロジェクト本数（推奨）：Common.Core / Exchange.Bitflyer / Exchange.Bittrade / Exchange.Factory の4本。統合クライアントが必要な場合も Factory 内のヘルパで扱い、プロジェクトは増やさない。
