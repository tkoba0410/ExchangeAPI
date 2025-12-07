# Bitflyer 構成（最適設計案）

## レイヤ構成
- Http: Public/Private/Signer/Transport + Models（REST呼び出しとDTOのみ）。
- Raw: RawApi/BitflyerRawApiClient に全エンドポイントを集約（調査・デバッグ・特殊用途向け）。
- Adapters: *Mapper.cs を置き、ドメインマッピング専用（ロジックなし）。
- Apis: Trading/Market/Account/Margin/ExchangeInfo/RawApi に抽象API実装を分割。Raw+Mapperを内部利用し、ポーリングなどビジネスロジックを担う。
- Facade: Facade/BitflyerExchangeClient はAPIを束ねて委譲のみ。
- Factory: 新構造で組み立て。必要ならRawを取り出せるオプションまたはプロパティを用意。

## エラーハンドリング
- ErrorMapperを共通化し、エラーコード/HTTPステータスをカテゴリにマップ。
- 代表例: INSUFFICIENT_FUNDS→Balance、AUTHENTICATION_ERROR→Auth、TOO_MANY_REQUESTS→RateLimit、INVALID_ORDER/PARAM_ERROR→Request、SERVICE_UNAVAILABLE/INTERNAL_ERROR→Server、TIMEOUT→Network。

## テスト戦略
- Http/Raw: モックRestClientでリクエスト/レスポンス形を検証。
- Adapters: マッピング単体（正/異常系）。
- Apis: ポーリングやキャンセルフローなど挙動テスト。
- Facade/Factory: 配線テスト（DI組み立て）。

## 利点/懸念
- ユーザー視点: APIクラスが責務単位で明確。Rawも利用可能。
- 開発者視点: ロジック/マッピング/通信が分離しテスト容易。Mock差し替えが簡単。
- 概念視点: 単一責務・疎結合・拡張性。フォルダ/クラス増加とドキュメント乖離の調整は必要。

## 移行ステップ（小刻み）
1. フォルダ/名前空間を Http/Raw/Adapters/Apis/Facade に整理。
2. BitflyerExchangeClient の処理をAPI別クラスへ抽出、マッピングはAdaptersへ移動。
3. ErrorMapperを共通化。
4. Factoryを新構造に合わせ、必要なら旧Facade互換入口を残す。
5. テストをレイヤごとに追加/再配置。

## フォルダ構成イメージ（Bitflyer）
```
src/adapter/Bitflyer/
  Http/
    IBitflyerPublicApi.cs
    IBitflyerPrivateApi.cs
    IBitflyerPrivateTradingApi.cs
    BitflyerPublicApi.cs
    BitflyerPrivateApi.cs
    BitflyerRequestSigner.cs
    BitflyerSigningTransport.cs
    Models/
      Bitflyer*Request.cs
      Bitflyer*Response.cs
      BitflyerTickerRaw.cs
  RawApi/
    BitflyerRawApiClient.cs          // Public/Private をまとめた生APIクライアント
    BitflyerRawApiFacade.cs          // Raw を外部公開する薄いラッパー
  Adapters/
    BitflyerCommonMapper.cs          // Side/Status/Symbol 等の共通マッピング
    BitflyerTradingMapper.cs         // 注文系マッピング・バリデーション
    BitflyerMarketMapper.cs          // Ticker/OrderBook 等のマーケットデータ正規化
    BitflyerAccountMapper.cs         // 残高マッピング
    BitflyerMarginMapper.cs          // 建玉/証拠金マッピング
    BitflyerExchangeInfoMapper.cs    // ExchangeInfo 構築
    BitflyerErrorMapper.cs           // エラーカテゴリ/例外ラップ
  Apis/
    Trading/BitflyerTradingApi.cs    // ITradingApi 実装（ポーリング等含む）
    Market/BitflyerMarketApi.cs      // IMarketDataApi 実装
    Account/BitflyerAccountApi.cs    // IAccountApi 実装
    Margin/BitflyerMarginApi.cs      // IMarginAccountApi 実装
    ExchangeInfo/BitflyerExchangeInfoApi.cs
  Facade/
    BitflyerExchangeClient.cs        // 上記APIを束ねる薄いファサード
  Factory/
    BitflyerClientFactory.cs         // Http/Raw/Adapters/Apis/Facadeを組み立て、必要ならRawも返す
```

## 共通ユーティリティの足場
- `src/adapter/Common/` に共通アダプタ向けユーティリティのプレースホルダを用意（シンボル正規化/共通バリデーション/エラーマッピング補助などを想定）。現時点ではREADMEのみで実装なし。必要に応じてプロジェクト化する。
