namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record GetExecutionsPrivateResponse(Page<ExecutionItem> Value);
