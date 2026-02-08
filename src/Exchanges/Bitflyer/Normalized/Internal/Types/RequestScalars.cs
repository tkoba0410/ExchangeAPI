namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;

public readonly record struct RequestCount(int Value);

public readonly record struct RequestBefore(long Value);

public readonly record struct RequestAfter(long Value);

public readonly record struct BankAccountId(int Value);

public readonly record struct WithdrawAmount(decimal Value);

public readonly record struct MinuteToExpire(int Value);

public readonly record struct PriceOffset(decimal Value);
