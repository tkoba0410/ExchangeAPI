# DTO Debt Audit

Date: 2026-02-13
Scope:
- `src/Exchanges/**/{Wire,Raw,Normalized}`
- `src/Contracts/**`

Method:
- DTO-like files were enumerated from `Dtos` paths.
- Type references were searched across `src/` and `tests/` using whole-word matches (`rg -w`).
- Serialization-risk markers (`JsonConverter`, `JsonPropertyName`, etc.) were flagged as `Needs review` if external refs were 0.

## High confidence unused

| File path | Type name(s) | Category | Evidence (refs in src/tests) | Notes | Status |
|---|---|---|---|---|---|
| `src/Exchanges/Bitflyer/Normalized/Private/Dtos/CollateralNormalized.cs` | `CollateralNormalized` | High confidence unused | `CollateralNormalized:0` | no external refs in src/tests | Deleted in B2 |
| `src/Exchanges/Bitflyer/Normalized/Private/Dtos/RawJsonNormalized.cs` | `RawJsonNormalized` | High confidence unused | `RawJsonNormalized:0` | no external refs in src/tests | Deleted in B2 |
| `src/Exchanges/Bitflyer/Normalized/Public/Dtos/CorporateLeverageNormalized.cs` | `CorporateLeverageNormalized` | High confidence unused | `CorporateLeverageNormalized:0` | no external refs in src/tests | Deleted in B2 |
| `src/Exchanges/Bitflyer/Normalized/Public/Dtos/FundingRateNormalized.cs` | `FundingRateNormalized` | High confidence unused | `FundingRateNormalized:0` | no external refs in src/tests | Deleted in B2 |

## Medium confidence

| File path | Type name(s) | Category | Evidence (refs in src/tests) | Notes |
|---|---|---|---|---|
| `src/Contracts/Common/Dtos/MarketFillResult.cs` | `FillEstimate` | Medium confidence | `FillEstimate:1` | very low external refs |
| `src/Exchanges/Bitflyer/Normalized/Private/Dtos/CollateralAccountNormalized.cs` | `CollateralAccountNormalized` | Medium confidence | `CollateralAccountNormalized:2` | very low external refs |
| `src/Exchanges/Bitflyer/Normalized/Private/Dtos/PositionNormalized.cs` | `PositionNormalized` | Medium confidence | `PositionNormalized:2` | very low external refs |
| `src/Exchanges/Bitflyer/Normalized/Private/Dtos/TradingCommissionNormalized.cs` | `TradingCommissionNormalized` | Medium confidence | `TradingCommissionNormalized:1` | very low external refs |
| `src/Exchanges/Bitflyer/Normalized/Public/Dtos/BoardStateNormalized.cs` | `BoardStateNormalized` | Medium confidence | `BoardStateNormalized:1` | very low external refs |
| `src/Exchanges/Bitflyer/Normalized/Public/Dtos/HealthNormalized.cs` | `HealthNormalized` | Medium confidence | `HealthNormalized:1` | very low external refs |
| `src/Exchanges/Bitflyer/Raw/Private/Dtos/ParentOrderDetailResponse.cs` | `ParentOrderDetailResponse`, `ParentOrderParameterItem` | Medium confidence | `ParentOrderDetailResponse:0; ParentOrderParameterItem:2` | very low external refs |
| `src/Exchanges/Bitflyer/Raw/Private/Dtos/RawJsonResponse.cs` | `RawJsonResponse` | Medium confidence | `RawJsonResponse:2` | very low external refs |
| `src/Exchanges/Bittrade/Normalized/Private/Dtos/AccountNormalized.cs` | `AccountNormalized` | Medium confidence | `AccountNormalized:2` | very low external refs |
| `src/Exchanges/Bittrade/Normalized/Private/Dtos/DepositWithdrawNormalized.cs` | `DepositWithdrawNormalized` | Medium confidence | `DepositWithdrawNormalized:2` | very low external refs |
| `src/Exchanges/Bittrade/Normalized/Private/Dtos/OrderSummaryNormalized.cs` | `OrderSummaryNormalized` | Medium confidence | `OrderSummaryNormalized:2` | very low external refs |
| `src/Exchanges/Bittrade/Normalized/Private/Dtos/RetailBalanceEntryNormalized.cs` | `RetailBalanceEntryNormalized` | Medium confidence | `RetailBalanceEntryNormalized:2` | very low external refs |
| `src/Exchanges/Bittrade/Normalized/Private/Dtos/WithdrawVirtualAddressNormalized.cs` | `WithdrawVirtualAddressNormalized` | Medium confidence | `WithdrawVirtualAddressNormalized:2` | very low external refs |
| `src/Exchanges/Bittrade/Normalized/Public/Dtos/TickerEntryNormalized.cs` | `TickerEntryNormalized` | Medium confidence | `TickerEntryNormalized:2` | very low external refs |
| `src/Exchanges/Bittrade/Raw/Private/Dtos/GetAccountsItem.cs` | `GetAccountsItem` | Medium confidence | `GetAccountsItem:2` | very low external refs |
| `src/Exchanges/Bittrade/Raw/Public/Dtos/GetDepthLevel.cs` | `GetDepthLevel` | Medium confidence | `GetDepthLevel:2` | very low external refs |
| `src/Exchanges/Bittrade/Raw/Public/Dtos/GetDetailMergedTick.cs` | `GetDetailMergedTick` | Medium confidence | `GetDetailMergedTick:1` | very low external refs |
| `src/Exchanges/Bittrade/Raw/Public/Dtos/GetTradeItem.cs` | `GetTradeItem` | Medium confidence | `GetTradeItem:1` | very low external refs |

Status:
- Retained after B3 review (all items had real code references in `src/`).

## Needs review

| File path | Type name(s) | Category | Evidence (refs in src/tests) | Notes |
|---|---|---|---|---|
| `src/Exchanges/Bittrade/Raw/Public/Dtos/RawCurrenciesResponse.cs` | `RawCurrenciesResponse` | Needs review | `RawCurrenciesResponse:0` | `Json*` attributes present (serialization coupling) |
| `src/Exchanges/Bittrade/Raw/Public/Dtos/RawTimestampResponse.cs` | `RawTimestampResponse` | Needs review | `RawTimestampResponse:0` | `Json*` attributes present (serialization coupling) |

## Deletion summary

- B2 deleted 4 high-confidence unused DTO files.
- `dotnet build` and `dotnet test` both passed after deletion.
