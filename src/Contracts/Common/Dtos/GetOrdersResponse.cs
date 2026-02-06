namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record GetOrdersResponse(Page<OrderSnapshotItem> Value);
