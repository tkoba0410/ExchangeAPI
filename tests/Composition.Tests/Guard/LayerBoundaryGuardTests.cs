using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bitflyer.Composition;
using ExchangeApi.Exchanges.Bittrade.Composition;
using ExchangeApi.Transport.Protocol;
using BitflyerNormalizedClient = ExchangeApi.Exchanges.Bitflyer.Normalized.Api.INormalizedApi;
using BittradeNormalizedClient = ExchangeApi.Exchanges.Bittrade.Normalized.Api.INormalizedApi;
using BitflyerNormalizedApi = ExchangeApi.Exchanges.Bitflyer.Normalized.Api.NormalizedApi;
using BittradeNormalizedApi = ExchangeApi.Exchanges.Bittrade.Normalized.Api.NormalizedApi;

namespace ExchangeApi.Tests.Composition.Tests.Guard;

public class LayerBoundaryGuardTests
{
    [Fact]
    public void CreateClient_ReturnsNormalizedApi()
    {
        var bitflyerMethod = GetPublicStaticMethod(typeof(BitflyerFactory), "CreateClient");
        var bittradeMethod = GetPublicStaticMethod(typeof(BittradeFactory), "CreateClient");

        Assert.Equal(typeof(BitflyerNormalizedClient), bitflyerMethod.ReturnType);
        Assert.Equal(typeof(BittradeNormalizedClient), bittradeMethod.ReturnType);
    }

    [Fact]
    public void CreateContractPublicClient_ReturnsPublicContractClient()
    {
        var bitflyerMethod = GetPublicStaticMethod(typeof(BitflyerFactory), "CreateContractPublicClient");
        var bittradeMethod = GetPublicStaticMethod(typeof(BittradeFactory), "CreateContractPublicClient");

        Assert.Equal(typeof(IContractPublicClient), bitflyerMethod.ReturnType);
        Assert.Equal(typeof(IContractPublicClient), bittradeMethod.ReturnType);
    }

    [Fact]
    public void CreateContractPrivateClient_ReturnsPrivateContractClient()
    {
        var bitflyerMethod = GetPublicStaticMethod(typeof(BitflyerFactory), "CreateContractPrivateClient");
        var bittradeMethod = GetPublicStaticMethod(typeof(BittradeFactory), "CreateContractPrivateClient");

        Assert.Equal(typeof(IContractPrivateClient), bitflyerMethod.ReturnType);
        Assert.Equal(typeof(IContractPrivateClient), bittradeMethod.ReturnType);
    }

    [Fact]
    public void CreateContractPublicClient_CreatesPublicOnlyCapabilities()
    {
        var bitflyerClient = BitflyerFactory.CreateContractPublicClient();
        var bittradeClient = BittradeFactory.CreateContractPublicClient();

        Assert.IsAssignableFrom<IContractPublicClient>(bitflyerClient);
        Assert.IsAssignableFrom<IContractPublicClient>(bittradeClient);
        Assert.False(bitflyerClient is IContractCandlesticksClient);
        Assert.True(bittradeClient is IContractCandlesticksClient);
    }

    [Fact]
    public void CreateContractPrivateClient_WithoutAuth_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => BitflyerFactory.CreateContractPrivateClient());
        Assert.Throws<InvalidOperationException>(() => BittradeFactory.CreateContractPrivateClient());
    }

    [Fact]
    public void CreateContractPrivateClient_WithRequestSigner_CreatesPrivateCapabilities()
    {
        var signer = new NoOpSigner();

        var bitflyerClient = BitflyerFactory.CreateContractPrivateClient(new BitflyerFactoryOptions
        {
            RequestSigner = signer,
        });

        var bittradeClient = BittradeFactory.CreateContractPrivateClient(new BittradeFactoryOptions
        {
            RequestSigner = signer,
            AccountId = "default",
        });

        Assert.IsAssignableFrom<IContractPrivateClient>(bitflyerClient);
        Assert.IsAssignableFrom<IContractPrivateClient>(bittradeClient);
        Assert.False(bitflyerClient is IContractCandlesticksClient);
        Assert.True(bittradeClient is IContractCandlesticksClient);
    }

    [Fact]
    public void NormalizedFacade_PublicSurface_DoesNotExposeRawOrWire()
    {
        var facadeTypes = GetFacadeTypes();
        var forbidden = new List<string>();

        foreach (var facadeType in facadeTypes)
        {
            foreach (var signatureType in EnumeratePublicSignatureTypes(facadeType))
            {
                if (IsForbiddenType(signatureType))
                {
                    forbidden.Add($"{facadeType.FullName}: {signatureType.FullName}");
                }
            }
        }

        if (forbidden.Count > 0)
        {
            var message = "Facade public surface exposes Raw/Wire types:\n" + string.Join("\n", forbidden);
            throw new Xunit.Sdk.XunitException(message);
        }
    }

    [Fact]
    public void Contracts_PublicSurface_DoesNotExposeRawWireOrJson()
    {
        var contracts = typeof(IContractPublicClient).Assembly;
        var forbidden = new List<string>();

        foreach (var type in contracts.GetExportedTypes())
        {
            foreach (var signatureType in EnumeratePublicSignatureTypes(type))
            {
                if (IsForbiddenContractsType(signatureType))
                {
                    forbidden.Add($"{type.FullName}: {signatureType.FullName}");
                }
            }
        }

        if (forbidden.Count > 0)
        {
            var message = "Contracts public surface exposes forbidden types:\n" + string.Join("\n", forbidden);
            throw new Xunit.Sdk.XunitException(message);
        }
    }

    private static MethodInfo GetPublicStaticMethod(Type factoryType, string methodName)
    {
        return factoryType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == methodName);
    }

    private static IEnumerable<Type> GetFacadeTypes()
    {
        var bitflyer = typeof(BitflyerNormalizedApi)
            .Assembly
            .GetExportedTypes()
            .Where(t => t.Namespace == "ExchangeApi.Exchanges.Bitflyer.Normalized.Api");

        var bittrade = typeof(BittradeNormalizedApi)
            .Assembly
            .GetExportedTypes()
            .Where(t => t.Namespace == "ExchangeApi.Exchanges.Bittrade.Normalized.Api");

        return bitflyer.Concat(bittrade);
    }

    private static IEnumerable<Type> EnumeratePublicSignatureTypes(Type facadeType)
    {
        var members = facadeType.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

        foreach (var member in members)
        {
            switch (member)
            {
                case PropertyInfo property:
                    foreach (var type in EnumerateSignatureTypes(property.PropertyType))
                    {
                        yield return type;
                    }
                    break;
                case MethodInfo method when !method.IsSpecialName:
                    foreach (var type in EnumerateSignatureTypes(method.ReturnType))
                    {
                        yield return type;
                    }
                    foreach (var parameter in method.GetParameters())
                    {
                        foreach (var type in EnumerateSignatureTypes(parameter.ParameterType))
                        {
                            yield return type;
                        }
                    }
                    break;
            }
        }
    }

    private static IEnumerable<Type> EnumerateSignatureTypes(Type type)
    {
        if (type.IsByRef && type.HasElementType)
        {
            type = type.GetElementType()!;
        }

        if (type.IsArray && type.HasElementType)
        {
            foreach (var elementType in EnumerateSignatureTypes(type.GetElementType()!))
            {
                yield return elementType;
            }
            yield break;
        }

        yield return type;

        if (type.IsGenericType)
        {
            foreach (var argument in type.GetGenericArguments())
            {
                foreach (var nested in EnumerateSignatureTypes(argument))
                {
                    yield return nested;
                }
            }
        }
    }

    private static bool IsForbiddenType(Type type)
    {
        if (type == typeof(void)) return false;

        var ns = type.Namespace ?? string.Empty;
        return ns.Contains(".Raw", StringComparison.Ordinal)
            || ns.Contains(".Wire", StringComparison.Ordinal);
    }

    private static bool IsForbiddenContractsType(Type type)
    {
        if (type == typeof(void)) return false;

        var ns = type.Namespace ?? string.Empty;
        if (ns.StartsWith("ExchangeApi.Exchanges.", StringComparison.Ordinal))
        {
            return true;
        }

        if (ns.Contains(".Raw", StringComparison.Ordinal) || ns.Contains(".Wire", StringComparison.Ordinal))
        {
            return true;
        }

        if (type == typeof(JsonElement))
        {
            return true;
        }

        return false;
    }

    private sealed class NoOpSigner : IRequestSigner
    {
        public Task SignAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
