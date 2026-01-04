using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Dtos;

public sealed record ParentOrder(
    ExchangeCode Exchange,
    Symbol Symbol,
    string ParentOrderId,
    string ParentOrderAcceptanceId,
    Side Side,
    string ParentOrderType,
    string ParentOrderState,
    Price? Price,
    Price? AveragePrice,
    Size Size,
    Size OutstandingSize,
    Size CancelSize,
    Size ExecutedSize,
    decimal TotalCommission,
    DateTimeOffset ParentOrderDate,
    DateTimeOffset ExpireDate);
