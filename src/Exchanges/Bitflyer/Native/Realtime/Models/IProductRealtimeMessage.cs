namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;

public interface IProductRealtimeMessage : IRealtimeMessage
{
    string ProductCode { get; }
}
