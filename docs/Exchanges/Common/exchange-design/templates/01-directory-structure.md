# 01 Directory Structure Template (Raw / Wire / Adapter)

目的：
- Core（Transport/Protocol）は別責務（HTTP/deserialize）
- Exchange 層は取引所ごとに **Raw → Wire(Normalized) → Adapter(Common)** を並べる
- 利用者導線は `IExchangeClient`（Common） + `client.Raw<T>() / client.Wire<T>()`

## 推奨フォルダ構成（取引所ごと）

例：`src/Exchanges/Bitflyer/`

```
Bitflyer/
  BitflyerExchangeClient.cs              // internal: IExchangeClient + IHasRawAccess/IHasWireAccess
  BitflyerClientFactory.cs               // public or internal: 生成入口（Compositionから呼ぶ）
  BitflyerComposition.cs                 // optional: 組み立て専用（DI/手組み）

  Raw/
    Public/
      BitflyerRawMarketDataApi.cs
      BitflyerRawExchangeInfoApi.cs
    Private/
      BitflyerRawTradingApi.cs
      BitflyerRawAccountApi.cs
    Dtos/
      ...RawResponseDto.cs
    Requests/
      ...RequestDto.cs
    Json/
      ...JsonConverter.cs                // 必要なら（意味変換は禁止）
    IBitflyerRawApi.cs                   // raw bundle interface
    IBitflyerRawMarketDataApi.cs

  Wire/                                  // = Normalized（取引所内実用形）
    Public/
      BitflyerWireMarketDataApi.cs
    Private/
      BitflyerWireTradingApi.cs
    Models/
      ...WireModel.cs
    Mappers/
      BitflyerWireMapper.cs
    IBitflyerWireApi.cs                  // wire bundle interface
    IBitflyerWireMarketDataApi.cs

  Adapter/                               // Common抽象化
    BitflyerMarketDataApi.cs             // implements IMarketDataApi
    BitflyerTradingApi.cs                // implements ITradingApi
    BitflyerAccountApi.cs                // implements IAccountApi
    BitflyerExchangeInfoApi.cs           // implements IExchangeInfoApi
    Mappers/
      BitflyerMapper.cs
    BitflyerErrorMapper.cs               // Enrich/Category mapping
    Operations.cs                        // operation命名の集中管理（推奨）

  Tests/                                 // unit tests (Fast)
    ...
```

## 命名・責務（要点）

- Raw：公式鏡像（意味変換しない、Price/Size禁止、open setはstring）
- Wire：status判定・エラー抽出・Try-parse（ただしCommon化しない）
- Adapter：Wire → Common DTO、例外 Enrich（Exchange/Operation必須）
- Factory/Composition：public/private、rest client、raw/wire/adapter の束ねを担当
