using System;
using System.Collections.Generic;
using System.Linq;
using Exchange.Bitflyer.Raw;
using ExchangeApi.Contracts.Dtos;

namespace Exchange.Bitflyer.Abstract;

internal static class BitflyerMarginMapper
{
    public static IReadOnlyList<Position> MapPositions(IReadOnlyList<BitflyerPositionResponse> rawPositions)
    {
        if (rawPositions is null) throw new ArgumentNullException(nameof(rawPositions));

        return rawPositions
            .Select(p => new Position(
                ProductCode: p.ProductCode,
                Side: BitflyerCommonMapper.MapSide(p.Side),
                Size: p.Size,
                Price: p.Price,
                OpenDate: p.OpenDate,
                Pnl: p.Pnl))
            .ToArray();
    }

    public static Collateral MapCollateral(BitflyerCollateralResponse raw)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));

        return new Collateral(
            Amount: raw.Collateral,
            OpenPositionPnl: raw.OpenPositionPnl,
            RequireCollateral: raw.RequireCollateral,
            KeepRate: raw.KeepRate);
    }
}
