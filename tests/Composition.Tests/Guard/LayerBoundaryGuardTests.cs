using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using ExchangeApi.Composition.Bootstrap.Factories;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Call;
using ExchangeApi.Exchanges.Bittrade.Normalize.Call;

namespace Composition.Tests.Guard;

public class LayerBoundaryGuardTests
{
    [Fact]
    public void CreateClient_ReturnsExchangeClient()
    {
        var bitflyerMethod = GetCreateClientMethod(typeof(BitflyerFactory));
        var bittradeMethod = GetCreateClientMethod(typeof(BittradeFactory));

        Assert.Equal(typeof(IExchangeClient), bitflyerMethod.ReturnType);
        Assert.Equal(typeof(IExchangeClient), bittradeMethod.ReturnType);
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
        var contracts = typeof(IExchangeClient).Assembly;
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

    private static MethodInfo GetCreateClientMethod(Type factoryType)
    {
        return factoryType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == "CreateClient");
    }

    private static IEnumerable<Type> GetFacadeTypes()
    {
        var bitflyer = typeof(BitflyerNormalizedApi)
            .Assembly
            .GetExportedTypes()
            .Where(t => t.Namespace == "ExchangeApi.Exchanges.Bitflyer.Normalize.Call");

        var bittrade = typeof(BittradeNormalizedApi)
            .Assembly
            .GetExportedTypes()
            .Where(t => t.Namespace == "ExchangeApi.Exchanges.Bittrade.Normalize.Call");

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
}
