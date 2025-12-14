using System;
using System.Collections.Generic;
using System.Linq;
using Exchange.Bitflyer.Raw;
using Common.Contract.Dtos;
using Common.Contract.Enums;

namespace Exchange.Bitflyer.Abstract;

internal static class BitflyerMarginMapper
{
    public static IReadOnlyList<Position> MapPositions(IReadOnlyList<BitflyerPositionResponse> rawPositions)
    {
        if (rawPositions is null) throw new ArgumentNullException(nameof(rawPositions));

        return rawPositions
            .Select(p => new Position(
                Exchange: ExchangeCode.Bitflyer,
                Symbol: BitflyerCommonMapper.ToSymbol(BitflyerCommonMapper.ToApiProductCode(p.ProductCode)),
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
            ExchangeCode: ExchangeCode.Bitflyer,
            Currency: "JPY",
            Amount: raw.Collateral,
            OpenPositionPnl: raw.OpenPositionPnl,
            RequireCollateral: raw.RequireCollateral,
            KeepRate: raw.KeepRate);
    }
}
