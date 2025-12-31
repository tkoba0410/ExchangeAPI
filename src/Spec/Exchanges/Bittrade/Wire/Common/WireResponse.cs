using ExchangeApi.Common.Enums;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

internal sealed record WireResponse<T>(ExchangeCode Exchange, T Response);
