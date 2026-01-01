using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerMarginMapper
{
    private const string CollateralCurrency = "JPY";

    public static IReadOnlyList<Position> MapPositions(Symbol symbol, IReadOnlyList<PositionResponse> rawPositions)
    {
        if (rawPositions is null) throw new ArgumentNullException(nameof(rawPositions));

        return rawPositions
            .Select(p =>
            {
                var side = BitflyerCommonMapper.MapSide(p.Side);
                var size = new Size(p.Size);
                var price = new Price(p.Price);
                return new Position(
                    ExchangeCode.Bitflyer,
                    symbol,
                    side,
                    size,
                    price,
                    p.OpenDate,
                    p.Pnl);
            })
            .ToArray();
    }

    public static Collateral MapCollateral(CollateralResponse raw)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));

        return new Collateral(
            ExchangeCode.Bitflyer,
            CollateralCurrency,
            raw.Collateral,
            raw.OpenPositionPnl,
            raw.RequireCollateral,
            raw.KeepRate);
    }
}
