using System.Threading;
using System.Threading.Tasks;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;

internal interface IExchangeInfoProvider
{
    Task<Call<GetExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default);
}
