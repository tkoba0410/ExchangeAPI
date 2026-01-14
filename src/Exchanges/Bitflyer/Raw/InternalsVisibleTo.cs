using System.Runtime.CompilerServices;

// Friend assemblies are intentionally restricted to keep Raw internals private.
[assembly: InternalsVisibleTo("Exchange.Bitflyer.Adapter.Tests")]
[assembly: InternalsVisibleTo("ExchangeApi.Adapter")]
[assembly: InternalsVisibleTo("ExchangeApi.Normalized")]
[assembly: InternalsVisibleTo("Exchange.Bitflyer.Raw.Tests")]
[assembly: InternalsVisibleTo("Exchange.Bitflyer.Raw.Endpoints.Tests")]
[assembly: InternalsVisibleTo("Composition")]
namespace ExchangeApi.Exchanges.Bitflyer.Raw;
