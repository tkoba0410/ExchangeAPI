namespace ExchangeApi.Contracts.Facade.Interfaces;

/// <summary>
/// Contracts の Private 機能（Public 含む）を提供する公開クライアント。
/// </summary>
public interface IContractPrivateClient : IContractPublicClient, IPrivateApi
{
}
