using System.Threading;
using System.Threading.Tasks;
using Common.Contract.Dtos;

namespace Common.Contract.Contracts;

/// <summary>
/// 取引所の機能や市場情報を提供する抽象インターフェース。
/// </summary>
public interface IExchangeInfoApi
{
    Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default);
}
