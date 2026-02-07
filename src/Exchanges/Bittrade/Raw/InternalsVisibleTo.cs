using System.Runtime.CompilerServices;

// Friend assemblies are intentionally restricted to keep Raw internals private.
[assembly: InternalsVisibleTo("Exchange.Bittrade.Adapter.Tests")]
[assembly: InternalsVisibleTo("ExchangeApi.Exchanges.Bittrade.Adapter")]
[assembly: InternalsVisibleTo("ExchangeApi.Exchanges.Bittrade.Normalized")]
[assembly: InternalsVisibleTo("Exchange.Bittrade.Raw.Tests")]
[assembly: InternalsVisibleTo("Exchange.Bittrade.Raw.Endpoints.Tests")]
namespace ExchangeApi.Exchanges.Bittrade.Raw;
