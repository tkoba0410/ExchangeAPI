using System.Reflection;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Architecture.Tests;

public sealed class BitflyerArchitectureTests
{
    [Fact]
    public void Venue_Has_Single_Aggregate_Project_With_Primitives_Only()
    {
        var venueRoot = Path.Combine(RepoRoot(), "src", "Exchanges", "Bitflyer");
        var projectFiles = Directory.GetFiles(venueRoot, "*.csproj", SearchOption.AllDirectories);
        var projectText = File.ReadAllText(Path.Combine(venueRoot, "ExchangeApi.Exchanges.Bitflyer.csproj"));

        Assert.Equal([Path.Combine(venueRoot, "ExchangeApi.Exchanges.Bitflyer.csproj")], projectFiles);
        Assert.Contains("ExchangeApi.Primitives.csproj", projectText, StringComparison.Ordinal);
        Assert.DoesNotContain("Optional", projectText, StringComparison.Ordinal);
    }

    [Fact]
    public void Protocol_Source_DoesNotReference_Native_Namespace()
    {
        var protocolFiles = Directory.GetFiles(
            Path.Combine(RepoRoot(), "src", "Exchanges", "Bitflyer", "Protocol"),
            "*.cs",
            SearchOption.AllDirectories);

        foreach (var file in protocolFiles)
        {
            var text = File.ReadAllText(file);
            Assert.DoesNotContain("ExchangeApi.Exchanges.Bitflyer.Native", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Native_Source_DoesNotReference_Composition_Namespace()
    {
        var nativeFiles = Directory.GetFiles(
            Path.Combine(RepoRoot(), "src", "Exchanges", "Bitflyer", "Native"),
            "*.cs",
            SearchOption.AllDirectories);

        foreach (var file in nativeFiles)
        {
            var text = File.ReadAllText(file);
            Assert.DoesNotContain("ExchangeApi.Exchanges.Bitflyer.Composition", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Native_Source_DoesNotReference_Protocol_Runtime_Concrete_Types()
    {
        var nativeFiles = Directory.GetFiles(
            Path.Combine(RepoRoot(), "src", "Exchanges", "Bitflyer", "Native"),
            "*.cs",
            SearchOption.AllDirectories);

        foreach (var file in nativeFiles)
        {
            var text = File.ReadAllText(file);
            Assert.DoesNotContain("BitflyerProtocolTransport", text, StringComparison.Ordinal);
            Assert.DoesNotContain("BitflyerRequestSigner", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Facade_Source_DoesNotReference_Runtime_Signer_Or_Transport()
    {
        var apiFiles = Directory.GetFiles(
            Path.Combine(RepoRoot(), "src", "Exchanges", "Bitflyer"),
            "*.cs",
            SearchOption.AllDirectories)
            .Where(path => path.Contains($"{Path.DirectorySeparatorChar}Api{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();

        foreach (var file in apiFiles)
        {
            var text = File.ReadAllText(file);
            Assert.DoesNotContain("BitflyerProtocolTransport", text, StringComparison.Ordinal);
            Assert.DoesNotContain("IProtocolTransport", text, StringComparison.Ordinal);
            Assert.DoesNotContain("BitflyerRequestSigner", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Native_Public_Surface_DoesNotExpose_ProtocolResponse_Or_JsonElement()
    {
        AssertForbiddenTypes(
            typeof(IBitflyerPublicNativeApi).Assembly.GetExportedTypes().Where(static type => type.Namespace?.Contains(".Native.", StringComparison.Ordinal) == true),
            typeof(ProtocolResponse),
            typeof(JsonElement));
    }

    [Fact]
    public void Protocol_Public_Surface_DoesNotExpose_JsonElement()
    {
        AssertForbiddenTypes(
            typeof(IBitflyerPublicProtocolApi).Assembly.GetExportedTypes().Where(static type => type.Namespace?.Contains(".Protocol.", StringComparison.Ordinal) == true),
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
