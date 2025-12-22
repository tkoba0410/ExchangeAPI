using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;

internal static class BitflyerMarginMapper
{
    public static IReadOnlyList<Position> MapPositions(IReadOnlyList<PositionResponse> rawPositions)
    {
        if (rawPositions is null) throw new ArgumentNullException(nameof(rawPositions));

        return rawPositions
            .Select(p => new Position(
                ExchangeCode: ExchangeCode.Bitflyer,
                Symbol: BitflyerCommonMapper.ToSymbol(BitflyerCommonMapper.ToApiProductCode(p.ProductCode)),
                Side: BitflyerCommonMapper.MapSide(p.Side),
                Size: new Size(p.Size),
                Price: new Price(p.Price),
                OpenDate: p.OpenDate,
                Pnl: p.Pnl))
            .ToArray();
    }

    public static Collateral MapCollateral(CollateralResponse raw)
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
