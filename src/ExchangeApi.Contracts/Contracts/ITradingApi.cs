using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;

namespace ExchangeApi.Contracts.Contracts;

/// <summary>
/// 取引（REST）の抽象インターフェース。
/// </summary>
public interface ITradingApi
{
    Task<OrderResult> SendOrderAsync(OrderRequest request, CancellationToken cancellationToken = default);

    Task<CancelResult> CancelOrderAsync(string productCode, string childOrderAcceptanceId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(string productCode, CancellationToken cancellationToken = default);
}
