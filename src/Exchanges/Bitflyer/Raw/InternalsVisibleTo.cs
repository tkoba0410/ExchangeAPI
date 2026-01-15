using System.Runtime.CompilerServices;

// Friend assemblies are intentionally restricted to keep Raw internals private.
[assembly: InternalsVisibleTo("Exchange.Bitflyer.Adapter.Tests")]
[assembly: InternalsVisibleTo("ExchangeApi.Exchanges.Bitflyer.Adapter")]
[assembly: InternalsVisibleTo("ExchangeApi.Exchanges.Bitflyer.Normalized")]
[assembly: InternalsVisibleTo("Exchange.Bitflyer.Raw.Tests")]
[assembly: InternalsVisibleTo("Exchange.Bitflyer.Raw.Endpoints.Tests")]
[assembly: InternalsVisibleTo("ExchangeApi.Composition")]
namespace ExchangeApi.Exchanges.Bitflyer.Raw;
