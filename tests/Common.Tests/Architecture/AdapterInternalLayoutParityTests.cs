using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Common.Tests.Architecture;

public sealed class AdapterInternalLayoutParityTests
{
    [Fact]
    public void AdapterInternalLayout_ShouldMatchCanonicalPhaseStructure()
    {
        var root = FindRepoRoot();

        foreach (var exchange in new[] { "Bitflyer", "Bittrade" })
        {
            var adapterPath = Path.Combine(root, "src", "Exchanges", exchange, "Adapter");
            var wirePath = Path.Combine(root, "src", "Exchanges", exchange, "Wire");

            Assert.True(Directory.Exists(Path.Combine(adapterPath, "Public", "Api")));
            Assert.True(Directory.Exists(Path.Combine(adapterPath, "Private", "Api")));
            Assert.True(Directory.Exists(Path.Combine(adapterPath, "Bootstrap")));
            Assert.True(Directory.Exists(Path.Combine(adapterPath, "Internal", "Orchestration")));
            Assert.True(Directory.Exists(Path.Combine(adapterPath, "Internal", "Resolve")));
            Assert.True(Directory.Exists(Path.Combine(adapterPath, "Internal", "Execute")));
            Assert.True(Directory.Exists(Path.Combine(adapterPath, "Internal", "Map")));
            Assert.True(Directory.Exists(Path.Combine(adapterPath, "Internal", "Error")));

            Assert.True(File.Exists(Path.Combine(adapterPath, "Public", "Api", "PublicClient.cs")));
            Assert.True(File.Exists(Path.Combine(adapterPath, "Private", "Api", "ExchangeClient.cs")));
            Assert.True(File.Exists(Path.Combine(adapterPath, "Internal", "Orchestration", "PublicFlow.cs")));
            Assert.True(File.Exists(Path.Combine(adapterPath, "Internal", "Orchestration", "PrivateFlow.cs")));
            Assert.True(File.Exists(Path.Combine(adapterPath, "Internal", "Resolve", "ExchangeMarketCatalog.cs")));
            Assert.True(File.Exists(Path.Combine(adapterPath, "Internal", "Resolve", "ExchangeRequestResolver.cs")));
            Assert.True(File.Exists(Path.Combine(adapterPath, "Internal", "Resolve", "NormalizedRequestResolver.cs")));
            Assert.True(File.Exists(Path.Combine(adapterPath, "Internal", "Execute", "NormalizedExecutor.cs")));
            Assert.True(File.Exists(Path.Combine(adapterPath, "Internal", "Error", "CallErrorTranslator.cs")));
            Assert.True(File.Exists(Path.Combine(adapterPath, "Internal", "Error", "ErrorClassifier.cs")));

            var privateEndpointsPath = Path.Combine(wirePath, "Private", "Endpoints");
            var hasPrivateEndpoints = Directory.Exists(privateEndpointsPath) &&
                Directory.GetFiles(privateEndpointsPath, "*.cs", SearchOption.TopDirectoryOnly).Length > 0;
            if (hasPrivateEndpoints)
            {
                Assert.True(Directory.Exists(Path.Combine(wirePath, "Internal", "Auth")));
                Assert.True(Directory.Exists(Path.Combine(wirePath, "Constants")));
                Assert.True(File.Exists(Path.Combine(wirePath, "Internal", "Auth", "RequestSigner.cs")));
                Assert.True(File.Exists(Path.Combine(wirePath, "Constants", "AuthKeys.cs")));
            }

            var mapperFiles = Directory
                .GetFiles(Path.Combine(adapterPath, "Internal", "Map"), "ContractMapper*.cs", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .ToArray();
            Assert.NotEmpty(mapperFiles);

            Assert.False(Directory.Exists(Path.Combine(adapterPath, "Internal", "Mappers")));
            Assert.False(Directory.Exists(Path.Combine(adapterPath, "Internal", "MarketCatalog")));
            Assert.False(Directory.Exists(Path.Combine(adapterPath, "Internal", "Factory")));
            Assert.False(Directory.Exists(Path.Combine(adapterPath, "Internal", "Constants")));
            Assert.False(File.Exists(Path.Combine(adapterPath, "Internal", "RequestSigner.cs")));
        }
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "ExchangeApi.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Repository root not found (ExchangeApi.slnx missing).");
    }
}
