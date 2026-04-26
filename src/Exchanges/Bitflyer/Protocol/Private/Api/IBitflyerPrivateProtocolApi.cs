using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;

public interface IBitflyerPrivateProtocolApi
{
    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetPermissionsAsync(

        CancellationToken cancellationToken = default);


    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetPermissionsAsync(

        IApiCredentialSession credentialSession,

        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetAddressesAsync(


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetAddressesAsync(


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCoinInsAsync(


        int? count,


        long? before,


        long? after,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCoinInsAsync(


        int? count,


        long? before,


        long? after,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCoinOutsAsync(


        int? count,


        long? before,


        long? after,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCoinOutsAsync(


        int? count,


        long? before,


        long? after,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBankAccountsAsync(


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBankAccountsAsync(


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetDepositsAsync(


        int? count,


        long? before,


        long? after,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetDepositsAsync(


        int? count,


        long? before,


        long? after,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> WithdrawAsync(


        string bodyJson,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> WithdrawAsync(


        string bodyJson,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetWithdrawalsAsync(


        int? count,


        long? before,


        long? after,


        string? messageId,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetWithdrawalsAsync(


        int? count,


        long? before,


        long? after,


        string? messageId,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBalanceAsync(


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBalanceAsync(


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetParentOrdersAsync(


        string? productCode,


        int? count,


        long? before,


        long? after,


        string? parentOrderState,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetParentOrdersAsync(


        string? productCode,


        int? count,


        long? before,


        long? after,


        string? parentOrderState,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetParentOrderAsync(


        string? parentOrderId,


        string? parentOrderAcceptanceId,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetParentOrderAsync(


        string? parentOrderId,


        string? parentOrderAcceptanceId,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCollateralAsync(


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCollateralAsync(


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCollateralAccountsAsync(


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCollateralAccountsAsync(


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetChildOrdersAsync(


        string? productCode,


        int? count,


        long? before,


        long? after,


        string? childOrderState,


        string? childOrderId,


        string? childOrderAcceptanceId,


        string? parentOrderId,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetChildOrdersAsync(


        string? productCode,


        int? count,


        long? before,


        long? after,


        string? childOrderState,


        string? childOrderId,


        string? childOrderAcceptanceId,


        string? parentOrderId,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetExecutionsAsync(


        string productCode,


        int? count,


        long? before,


        long? after,


        string? childOrderId,


        string? childOrderAcceptanceId,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetExecutionsAsync(


        string productCode,


        int? count,


        long? before,


        long? after,


        string? childOrderId,


        string? childOrderAcceptanceId,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBalanceHistoryAsync(


        string? currencyCode,


        int? count,


        long? before,


        long? after,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBalanceHistoryAsync(


        string? currencyCode,


        int? count,


        long? before,


        long? after,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCollateralHistoryAsync(


        int? count,


        long? before,


        long? after,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCollateralHistoryAsync(


        int? count,


        long? before,


        long? after,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetPositionsAsync(


        string productCode,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetPositionsAsync(


        string productCode,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetTradingCommissionAsync(


        string productCode,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetTradingCommissionAsync(


        string productCode,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendChildOrderAsync(


        string bodyJson,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendChildOrderAsync(


        string bodyJson,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendParentOrderAsync(


        string bodyJson,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendParentOrderAsync(


        string bodyJson,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> CancelChildOrderAsync(


        string bodyJson,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> CancelChildOrderAsync(


        string bodyJson,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> CancelAllChildOrdersAsync(


        string bodyJson,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> CancelAllChildOrdersAsync(


        string bodyJson,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> CancelParentOrderAsync(


        string bodyJson,


        CancellationToken cancellationToken = default);



    Task<CallResult<ProtocolRequest, ProtocolResponse>> CancelParentOrderAsync(


        string bodyJson,


        IApiCredentialSession credentialSession,


        CancellationToken cancellationToken = default);
}
