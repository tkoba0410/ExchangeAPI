using System.Runtime.CompilerServices;

// Friend assemblies are intentionally restricted to keep Raw internals private.
[assembly: InternalsVisibleTo("Exchange.Bittrade.Adapter.Tests")]
[assembly: InternalsVisibleTo("ExchangeApi.Adapter")]
[assembly: InternalsVisibleTo("ExchangeApi.Normalized")]
[assembly: InternalsVisibleTo("Exchange.Bittrade.Raw.Tests")]
[assembly: InternalsVisibleTo("Exchange.Bittrade.Raw.Endpoints.Tests")]
[assembly: InternalsVisibleTo("Composition")]
namespace ExchangeApi.Exchanges.Bittrade.Raw;
