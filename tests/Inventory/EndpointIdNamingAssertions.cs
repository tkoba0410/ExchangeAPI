using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ExchangeApi.Tests.Inventory;

internal static class EndpointIdNamingAssertions
{
    public static void AssertCallAsyncMethodsExist(IEnumerable<string> endpointIds, Type apiInterfaceType)
    {
        if (endpointIds is null) throw new ArgumentNullException(nameof(endpointIds));
        if (apiInterfaceType is null) throw new ArgumentNullException(nameof(apiInterfaceType));

        var expected = endpointIds
            .Select(id => id + "CallAsync")
            .ToArray();

        var actual = apiInterfaceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Select(m => m.Name)
            .ToHashSet(StringComparer.Ordinal);

        var missing = expected
            .Where(name => !actual.Contains(name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            missing.Length == 0,
            $"{apiInterfaceType.Name} missing CallAsync methods for EndpointIds: {string.Join(", ", missing)}");
    }
}
