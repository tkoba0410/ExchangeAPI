using System.Reflection;
using System.Text.Json;
using ExchangeApi.Exchanges.Binance.Native.Public.Api;
using ExchangeApi.Exchanges.Binance.Protocol.Public.Api;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Binance.Architecture.Tests;

public sealed class BinanceArchitectureTests
{
    [Fact]
    public void Venue_Has_Single_Aggregate_Project_With_Primitives_Only()
    {
        var venueRoot = Path.Combine(RepoRoot(), "src", "Exchanges", "Binance");
        var projectFiles = Directory.GetFiles(venueRoot, "*.csproj", SearchOption.AllDirectories);
        var projectText = File.ReadAllText(Path.Combine(venueRoot, "ExchangeApi.Exchanges.Binance.csproj"));

        Assert.Equal([Path.Combine(venueRoot, "ExchangeApi.Exchanges.Binance.csproj")], projectFiles);
        Assert.Contains("ExchangeApi.Primitives.csproj", projectText, StringComparison.Ordinal);
        Assert.DoesNotContain("Optional", projectText, StringComparison.Ordinal);
    }

    [Fact]
    public void Protocol_Source_DoesNotReference_Native_Namespace()
    {
        var protocolFiles = Directory.GetFiles(
            Path.Combine(RepoRoot(), "src", "Exchanges", "Binance", "Protocol"),
            "*.cs",
            SearchOption.AllDirectories);

        foreach (var file in protocolFiles)
        {
            var text = File.ReadAllText(file);
            Assert.DoesNotContain("ExchangeApi.Exchanges.Binance.Native", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Native_Source_DoesNotReference_Composition_Namespace()
    {
        var nativeFiles = Directory.GetFiles(
            Path.Combine(RepoRoot(), "src", "Exchanges", "Binance", "Native"),
            "*.cs",
            SearchOption.AllDirectories);

        foreach (var file in nativeFiles)
        {
            var text = File.ReadAllText(file);
            Assert.DoesNotContain("ExchangeApi.Exchanges.Binance.Composition", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Native_Source_DoesNotReference_Protocol_Runtime_Concrete_Types()
    {
        var nativeFiles = Directory.GetFiles(
            Path.Combine(RepoRoot(), "src", "Exchanges", "Binance", "Native"),
            "*.cs",
            SearchOption.AllDirectories);

        foreach (var file in nativeFiles)
        {
            var text = File.ReadAllText(file);
            Assert.DoesNotContain("BinanceProtocolTransport", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Facade_Source_DoesNotReference_Runtime_Or_Transport()
    {
        var apiFiles = Directory.GetFiles(
            Path.Combine(RepoRoot(), "src", "Exchanges", "Binance"),
            "*.cs",
            SearchOption.AllDirectories)
            .Where(path => path.Contains($"{Path.DirectorySeparatorChar}Api{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();

        foreach (var file in apiFiles)
        {
            var text = File.ReadAllText(file);
            Assert.DoesNotContain("BinanceProtocolTransport", text, StringComparison.Ordinal);
            Assert.DoesNotContain("IProtocolTransport", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Native_Public_Surface_DoesNotExpose_ProtocolResponse_Or_JsonElement()
    {
        AssertForbiddenTypes(
            typeof(IBinancePublicNativeApi).Assembly.GetExportedTypes().Where(static type => type.Namespace?.Contains(".Native.", StringComparison.Ordinal) == true),
            typeof(ProtocolResponse),
            typeof(JsonElement));
    }

    [Fact]
    public void Protocol_Public_Surface_DoesNotExpose_JsonElement()
    {
        AssertForbiddenTypes(
            typeof(IBinancePublicProtocolApi).Assembly.GetExportedTypes().Where(static type => type.Namespace?.Contains(".Protocol.", StringComparison.Ordinal) == true),
            typeof(JsonElement));
    }

    private static void AssertForbiddenTypes(IEnumerable<Type> types, params Type[] forbiddenTypes)
    {
        foreach (var type in types)
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                Assert.DoesNotContain(property.PropertyType, forbiddenTypes);
            }

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                Assert.DoesNotContain(method.ReturnType, forbiddenTypes);

                foreach (var parameter in method.GetParameters())
                {
                    Assert.DoesNotContain(parameter.ParameterType, forbiddenTypes);
                }
            }
        }
    }

    private static string RepoRoot()
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

        throw new InvalidOperationException("Repository root was not found.");
    }
}
