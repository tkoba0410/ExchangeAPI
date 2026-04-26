using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.Withdraw;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;

public interface IBitflyerPrivateNativeApi
{
    Task<CallResult<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsAsync(

        GetPermissionsRequest request,

        CancellationToken cancellationToken = default);


    Task<CallResult<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsAsync(

        GetPermissionsRequest request,

        IApiCredentialSession credentialSession,

        CancellationToken cancellationToken = default);

    Task<CallResult<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>> GetAddressesAsync(


        GetAddressesRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>> GetAddressesAsync(


        GetAddressesRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> GetCoinInsAsync(


        GetCoinInsRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> GetCoinInsAsync(


        GetCoinInsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetCoinOutsRequest, IReadOnlyList<GetCoinOuts.Item>>> GetCoinOutsAsync(


        GetCoinOutsRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetCoinOutsRequest, IReadOnlyList<GetCoinOuts.Item>>> GetCoinOutsAsync(


        GetCoinOutsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>> GetBankAccountsAsync(


        GetBankAccountsRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>> GetBankAccountsAsync(


        GetBankAccountsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>> GetDepositsAsync(


        GetDepositsRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>> GetDepositsAsync(


        GetDepositsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<WithdrawRequest, WithdrawResponse>> WithdrawAsync(


        WithdrawRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<WithdrawRequest, WithdrawResponse>> WithdrawAsync(


        WithdrawRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>> GetWithdrawalsAsync(


        GetWithdrawalsRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>> GetWithdrawalsAsync(


        GetWithdrawalsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceAsync(


        GetBalanceRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceAsync(


        GetBalanceRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> GetParentOrdersAsync(


        GetParentOrdersRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> GetParentOrdersAsync(


        GetParentOrdersRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderAsync(


        GetParentOrderRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderAsync(


        GetParentOrderRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetCollateralRequest, GetCollateralResponse>> GetCollateralAsync(


        GetCollateralRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetCollateralRequest, GetCollateralResponse>> GetCollateralAsync(


        GetCollateralRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> GetCollateralAccountsAsync(


        GetCollateralAccountsRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> GetCollateralAccountsAsync(


        GetCollateralAccountsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetChildOrdersAsync(


        GetChildOrdersRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetChildOrdersAsync(


        GetChildOrdersRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> GetExecutionsAsync(


        GetExecutionsRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> GetExecutionsAsync(


        GetExecutionsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>> GetBalanceHistoryAsync(


        GetBalanceHistoryRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>> GetBalanceHistoryAsync(


        GetBalanceHistoryRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> GetCollateralHistoryAsync(


        GetCollateralHistoryRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> GetCollateralHistoryAsync(


        GetCollateralHistoryRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsAsync(


        GetPositionsRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsAsync(


        GetPositionsRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionAsync(


        GetTradingCommissionRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionAsync(


        GetTradingCommissionRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderAsync(


        SendChildOrderRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderAsync(


        SendChildOrderRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderAsync(


        SendParentOrderRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderAsync(


        SendParentOrderRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<CancelChildOrderRequest, Unit>> CancelChildOrderAsync(


        CancelChildOrderRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<CancelChildOrderRequest, Unit>> CancelChildOrderAsync(


        CancelChildOrderRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<CancelAllChildOrdersRequest, Unit>> CancelAllChildOrdersAsync(


        CancelAllChildOrdersRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<CancelAllChildOrdersRequest, Unit>> CancelAllChildOrdersAsync(


        CancelAllChildOrdersRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<CancelParentOrderRequest, Unit>> CancelParentOrderAsync(


        CancelParentOrderRequest request,


        CancellationToken cancellationToken = default);



    Task<CallResult<CancelParentOrderRequest, Unit>> CancelParentOrderAsync(


        CancelParentOrderRequest request,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);
}
