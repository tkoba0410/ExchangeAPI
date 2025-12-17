Summary
- Core/Common/Composition plus Bitflyer/Bittrade adapters are organized under `ExchangeApi.slnx`, all targeting net10.0 with warnings-as-errors.
- Source/doc/test trees are populated and documented up to their respective depths; transport and credential factories are centralized.
- Layering still permits Composition/Exchange projects to reference Core directly, so the intended Core ← Common ← Exchange topology is only partially enforced.
- Legacy Unified/MultiExchange/Registry features are absent, and transport DTOs now live solely under Core.

Repository Tree
#### src/ (depth ≤5, representative files)
```text
src
├── Common
│   ├── Common.csproj
│   ├── Dtos
│   │   ├── Account
│   │   │   ├── Balance.cs
│   │   │   └── Collateral.cs
│   │   ├── ExchangeInfo/ExchangeInfo.cs
│   │   └── Trading/OrderResult.cs
│   ├── Enums/CurrencyCode.cs
│   ├── Extensions/ExchangeInfoExtensions.cs
│   └── Interfaces
│       ├── IAccountApi.cs
│       ├── IApiCredentialProvider.cs
│       └── IMarketDataApi.cs
├── Composition
│   ├── Composition.csproj
│   ├── Credentials
│   │   ├── CompositeCredentialProvider.cs
│   │   └── FileApiCredentialProvider.cs
│   ├── ExchangeInfo/JsonExchangeInfoApi.cs
│   └── Transport/RestClientFactory.cs
├── Core
│   ├── Core.csproj
│   ├── Contracts
│   │   ├── Enums/ExchangeCode.cs
│   │   └── Transport
│   │       ├── ApiResult.cs
│   │       ├── HttpRequestMeta.cs
│   │       └── TransportMeta.cs
│   └── Transport
│       ├── Http/HttpTransport.cs
│       ├── Observability/IRestCallObserver.cs
│       ├── Policy/HttpPolicyPipeline.cs
│       └── Protocol/IRestClient.cs
└── Exchange
    ├── Bitflyer
    │   ├── Exchange.Bitflyer.csproj
    │   ├── Abstract
    │   │   ├── Adapters/BitflyerErrorMapper.cs
    │   │   ├── Apis
    │   │   │   ├── Market/BitflyerMarketApi.cs
    │   │   │   └── RawApi/BitflyerRawApiFacade.cs
    │   │   ├── Facade/BitflyerExchangeClient.cs
    │   │   └── Factory/BitflyerClientFactory.cs
    │   └── Raw
    │       ├── BitflyerConstants.cs
    │       └── PublicGet/Models/BitflyerBoard.cs
    └── Bittrade
        ├── Exchange.Bittrade.csproj
        ├── Abstract
        │   ├── Adapters/BittradeErrorClassifier.cs
        │   ├── Apis/BittradeMarketDataApi.cs
        │   ├── Facade/BittradeExchangeClient.cs
        │   └── Factory/BittradeClientFactory.cs
        └── Raw
            ├── PrivateGet/Models/BittradeBalancesResponse.cs
            └── PublicGet/Models/BittradeDepthResponse.cs
```

#### tests/ (depth ≤4)
```text
tests
├── Common.Tests
│   ├── Common.Tests.csproj
│   ├── Contracts/ExchangeApiExceptionTests.cs
│   └── Transport
│       ├── RestClientTests.cs
│       └── Logging/RestCallOpenTelemetryObserverTests.cs
├── Exchange.Bitflyer.Tests
│   ├── Exchange.Bitflyer.Tests.csproj
│   ├── Abstract/BitflyerExchangeClientTests.cs
│   └── Raw/BitflyerRequestSigner_Tests.cs
├── Exchange.Bittrade.Tests
│   ├── Exchange.Bittrade.Tests.csproj
│   └── Abstract/BittradeMarketDataApiTests.cs
├── Factory.Tests
│   ├── Factory.Tests.csproj
│   └── Transport/RestClientFactory_Tests.cs
└── Integration.Public.Tests
    ├── Integration.Public.Tests.csproj
    └── PublicApiLiveTests.cs
```

#### docs/ (depth ≤3)
```text
docs
├── STAGES-OVERVIEW.md
├── entry-guide.md
├── quickstart.md
├── Common
│   ├── README.md
│   ├── Contracts/ErrorMapping.md
│   └── Transport/Observability.md
├── Exchange.Bitflyer/RAW-API-LIST.md
├── Exchange.Bittrade/RAW-API-LIST.md
├── Exchange.Common/README.md
└── Factory/README.md
```

Projects & References
- Solution layout (`ExchangeApi.slnx`) enumerates five src projects and five test projects grouped under `/src` and `/tests` respectively.

| Project | Path | ProjectReferences |
| --- | --- | --- |
| Core | src/Core/Core.csproj | — |
| Common | src/Common/Common.csproj | Core |
| Composition | src/Composition/Composition.csproj | Common, Core |
| Exchange.Bitflyer | src/Exchange/Bitflyer/Exchange.Bitflyer.csproj | Common, Core |
| Exchange.Bittrade | src/Exchange/Bittrade/Exchange.Bittrade.csproj | Common, Core |
| Common.Tests | tests/Common.Tests/Common.Tests.csproj | Common, Core |
| Exchange.Bitflyer.Tests | tests/Exchange.Bitflyer.Tests/Exchange.Bitflyer.Tests.csproj | Common, Exchange.Bitflyer |
| Exchange.Bittrade.Tests | tests/Exchange.Bittrade.Tests/Exchange.Bittrade.Tests.csproj | Common, Exchange.Bittrade |
| Factory.Tests | tests/Factory.Tests/Factory.Tests.csproj | Common, Core, Composition |
| Integration.Public.Tests | tests/Integration.Public.Tests/Integration.Public.Tests.csproj | Exchange.Bitflyer, Exchange.Bittrade |

Dependency map (→ indicates `ProjectReference`):
```text
Core
↑
Common → Core
↑
Composition → Common and Core  [NG: violates strict layering]
Exchange.Bitflyer → Common and Core  [NG]
Exchange.Bittrade → Common and Core  [NG]
Tests → corresponding libraries (expected)
```

Public Entry Points (推定)
- Common
  - `src/Common/Interfaces/IMarketDataApi.cs`: canonical read-only market data surface implemented by adapters.
  - `src/Common/Interfaces/IApiCredentialProvider.cs`: pluggable credential supply contract consumed by Composition and factories.
- Composition
  - `src/Composition/Transport/RestClientFactory.cs`: static builder for `RestClient` instances, composes transports/policies/logging hooks.
  - `src/Composition/Credentials/CompositeCredentialProvider.cs`: chains multiple credential sources with fallback logic.
  - `src/Composition/ExchangeInfo/JsonExchangeInfoApi.cs`: loads and merges ExchangeInfo JSON assets.
- Core
  - `src/Core/Transport/Protocol/IRestClient.cs`: primary abstraction for HTTP+JSON calls.
  - `src/Core/Transport/Policy/HttpPolicyPipeline.cs`: orchestrates retry/resilience policies.
  - `src/Core/Transport/Observability/IRestCallObserver.cs`: observer hook for request lifecycle instrumentation.
- Exchange.Bitflyer
  - `src/Exchange/Bitflyer/Abstract/Facade/BitflyerExchangeClient.cs`: unified client exposing account/market/trading APIs plus optional raw bundle.
  - `src/Exchange/Bitflyer/Abstract/Factory/BitflyerClientFactory.cs`: main factory for public/private clients, wiring policy/logging/credentials.
  - `src/Exchange/Bitflyer/Abstract/Apis/RawApi/BitflyerRawApiFacade.cs`: thin façade over REST endpoints for low-level use.
- Exchange.Bittrade
  - `src/Exchange/Bittrade/Abstract/Facade/BittradeExchangeClient.cs`: combined market/trading/account facade returning `BittradeRawApiClient` for raw calls.
  - `src/Exchange/Bittrade/Abstract/Factory/BittradeClientFactory.cs`: constructs Bittrade clients and exposes helper builders for public/private contexts.
  - `src/Exchange/Bittrade/Raw/BittradeRawApiClient.cs`: raw endpoint aggregator used by factories and advanced callers.

Policy Compliance Checklist
| Check | Status | Evidence |
| --- | --- | --- |
| Unified/MultiExchange/Registry features removed | OK | `rg -n "Unified\|MultiExchange\|Registry" src` returned no hits, indicating no bundled registry remnants. |
| Transport DTOs (ApiResult/TransportMeta/etc.) outside Core | OK | DTOs now sit under `src/Core/Contracts/Transport/ApiResult.cs` and `.../TransportMeta.cs`; `src/Common` contains none. |
| Composition contained inside Common | OK | Composition lives as a sibling project under `src/Composition`, separate from `src/Common`. |
| Layering rule Exchange.* / Composition → Common → Core | NG | `src/Composition/Composition.csproj` and `src/Exchange/*/*.csproj` reference Core directly alongside Common. |
| Naming consistency (Observability vs Logging, Http vs Transport) | 要確認 | Code uses `Core/Transport/Observability/...` but tests remain under `tests/Common.Tests/Transport/Logging/...`, mixing terminology. |

Recommendations (P0/P1/P2)
- P0: Remove direct Core references from Composition and Exchange adapters (update csproj + inject abstractions) so dependencies become Exchange → Common → Core only, enforcing the intended layering without code churn.
- P1: Align naming for observability/logging artifacts (rename `tests/Common.Tests/Transport/Logging` to `.../Observability` and update namespaces) to prevent confusion in future onboarding.
- P2: Document credential/ExchangeInfo configuration flows in `docs/Factory/README.md` (or add a Composition README) so that JsonExchangeInfo/Credential providers are discoverable without diving into source.
