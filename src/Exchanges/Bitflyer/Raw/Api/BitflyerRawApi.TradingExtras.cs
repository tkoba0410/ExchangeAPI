using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Api;

public sealed partial class BitflyerRawApi
{
    public Task<Call<CancelAllChildOrdersRequest, RawCancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.CancelAllChildOrders",
            BitflyerEndpoints.CancelAllChildOrders(
                BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CancelAllChildOrders")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawCancelAllChildOrdersResponse>(
                json,
                "Bitflyer.CancelAllChildOrders"));

    public Task<Call<CreateWithdrawalRequest, CreateWithdrawalResponse>> WithdrawCallAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.Withdraw",
            BitflyerEndpoints.Withdraw(
                BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.Withdraw")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<CreateWithdrawalResponse>(
                json,
                "Bitflyer.Withdraw"));
}
