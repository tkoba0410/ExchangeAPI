using System.Runtime.CompilerServices;

// Friend assemblies are intentionally restricted to keep Raw internals private.
[assembly: InternalsVisibleTo("Exchange.Bittrade.Adapter")]
[assembly: InternalsVisibleTo("Exchange.Bittrade.Adapter.Tests")]
[assembly: InternalsVisibleTo("Exchange.Bittrade.Raw.Tests")]
[assembly: InternalsVisibleTo("Composition")]
