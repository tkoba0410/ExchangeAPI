using System.Runtime.CompilerServices;

// Friend assemblies are intentionally restricted to keep Raw internals private.
[assembly: InternalsVisibleTo("Exchange.Bitflyer.Adapter")]
[assembly: InternalsVisibleTo("Exchange.Bitflyer.Adapter.Tests")]
[assembly: InternalsVisibleTo("Exchange.Bitflyer.Raw.Tests")]
[assembly: InternalsVisibleTo("Exchange.Bitflyer.Wire")]
[assembly: InternalsVisibleTo("Exchange.Bitflyer.Wire.Tests")]
[assembly: InternalsVisibleTo("Composition")]
