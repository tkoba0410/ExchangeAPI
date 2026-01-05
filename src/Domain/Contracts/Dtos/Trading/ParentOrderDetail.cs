using System.Collections.Generic;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Dtos;

public sealed record ParentOrderDetail(
    ExchangeCode Exchange,
    string ParentOrderId,
    string ParentOrderAcceptanceId,
    string OrderMethod,
    string TimeInForce,
    IReadOnlyList<ParentOrderParameter> Parameters);

public sealed record ParentOrderParameter(
    string ProductCode,
    string ConditionType,
    Side Side,
    Size Size,
    Price? Price,
    Price? TriggerPrice,
    decimal Offset);
