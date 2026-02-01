using System.Runtime.CompilerServices;

// Friend assemblies are intentionally restricted to keep Raw internals private.
[assembly: InternalsVisibleTo("Exchange.Bittrade.Adapter.Tests")]
[assembly: InternalsVisibleTo("ExchangeApi.Exchanges.Bittrade.Api.Adapter")]
[assembly: InternalsVisibleTo("ExchangeApi.Exchanges.Bittrade.Api.Normalized")]
[assembly: InternalsVisibleTo("Exchange.Bittrade.Raw.Tests")]
[assembly: InternalsVisibleTo("Exchange.Bittrade.Raw.Endpoints.Tests")]
[assembly: InternalsVisibleTo("ExchangeApi.Composition")]
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw;
