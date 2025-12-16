namespace Core.Contracts.Transport;

/// <summary>ドメインデータと通信メタをまとめて返すラッパ。</summary>
public sealed record ApiResult<T>(T Data, TransportMeta Meta);
