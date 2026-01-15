using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Application.Trading;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Application.Interfaces;

public interface IOrderQueryApi
{
    Task<Call<GetOrderQuery, OrderStatusSnapshot>> GetOrderCallAsync(
        GetOrderQuery request,
        CancellationToken cancellationToken = default);
}
