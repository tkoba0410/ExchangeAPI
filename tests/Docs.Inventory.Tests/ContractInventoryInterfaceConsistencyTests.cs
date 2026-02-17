using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Tests.Inventory;

namespace ExchangeApi.Docs.Inventory.Tests;

public sealed class ContractInventoryInterfaceConsistencyTests
{
    [Fact]
    public void Contracts_Inventory_Must_Match_Interface_Signatures()
    {
        var path = Path.Combine(
            InventoryEndpointIdParser.FindRepoRoot(),
            "docs/inventory/endpoints-contracts.md".Replace('/', Path.DirectorySeparatorChar));
        Assert.True(File.Exists(path), $"Inventory file not found: {path}");

        var lines = File.ReadAllLines(path);
        var tables = ParseContractTables(lines);
        Assert.NotEmpty(tables);

        foreach (var table in tables)
        {
            Assert.DoesNotContain("Parameters", table.Header);
            Assert.Contains("ContractScope", table.Header);
            Assert.Contains("ContractMethod", table.Header);
            Assert.Contains("RequestType", table.Header);
            Assert.Contains("ResponseType", table.Header);

            foreach (var row in table.Rows)
            {
                var scope = Required(row, "ContractScope");
                var methodName = Required(row, "ContractMethod");
                var requestTypeName = Required(row, "RequestType");
                var responseTypeName = Required(row, "ResponseType");

                var apiInterface = ResolveApiInterface(scope, methodName);
                var method = apiInterface.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
                Assert.True(method is not null, $"Method '{methodName}' was not found on {apiInterface.Name}.");

                var parameters = method!.GetParameters();
                Assert.True(parameters.Length >= 1, $"Method '{apiInterface.Name}.{methodName}' must have a request parameter.");
                Assert.Equal(requestTypeName, parameters[0].ParameterType.Name);

                var callType = ResolveCallType(method);
                Assert.Equal(requestTypeName, callType.GetGenericArguments()[0].Name);
                Assert.Equal(responseTypeName, callType.GetGenericArguments()[1].Name);
            }
        }
    }

    private static IReadOnlyList<InventoryTable> ParseContractTables(string[] lines)
    {
        var tables = new List<InventoryTable>();

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (!line.TrimStart().StartsWith('|'))
            {
                continue;
            }

            var header = SplitRow(line);
            if (!header.Contains("ContractScope", StringComparer.Ordinal) ||
                !header.Contains("ContractMethod", StringComparer.Ordinal))
            {
                continue;
            }

            var rows = new List<Dictionary<string, string>>();

            for (var j = i + 2; j < lines.Length; j++)
            {
                var rowLine = lines[j];
                if (!rowLine.TrimStart().StartsWith('|'))
                {
                    i = j - 1;
                    break;
                }

                if (IsSeparatorRow(rowLine))
                {
                    continue;
                }

                var row = SplitRow(rowLine);
                if (row.Count < header.Count)
                {
                    continue;
                }

                var dict = new Dictionary<string, string>(StringComparer.Ordinal);
                for (var k = 0; k < header.Count; k++)
                {
                    dict[header[k]] = row[k];
                }

                rows.Add(dict);
                i = j;
            }

            tables.Add(new InventoryTable(header, rows));
        }

        return tables;
    }

    private static List<string> SplitRow(string line)
    {
        return line.Trim().Trim('|')
            .Split('|', StringSplitOptions.None)
            .Select(cell => cell.Trim())
            .ToList();
    }

    private static bool IsSeparatorRow(string line)
    {
        var trimmed = line.Trim().Trim('|');
        return trimmed.Length > 0 && trimmed.All(c => c == '-' || c == ' ');
    }

    private static Type ResolveApiInterface(string scope, string methodName)
    {
        return scope switch
        {
            "public" => ResolvePublicInterface(methodName),
            "private" => typeof(IPrivateApi),
            _ => throw new InvalidOperationException($"Unknown ContractScope: '{scope}'."),
        };
    }

    private static Type ResolvePublicInterface(string methodName)
    {
        var publicMethod = typeof(IPublicApi).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        if (publicMethod is not null)
        {
            return typeof(IPublicApi);
        }

        var capabilityMethod = typeof(ICandlesticksApi).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        if (capabilityMethod is not null)
        {
            return typeof(ICandlesticksApi);
        }

        throw new InvalidOperationException($"Unknown public ContractMethod: '{methodName}'.");
    }

    private static Type ResolveCallType(MethodInfo method)
    {
        var returnType = method.ReturnType;
        Assert.True(returnType.IsGenericType, $"Return type for '{method.Name}' must be Task<Call<,>>.");
        Assert.Equal(typeof(Task<>), returnType.GetGenericTypeDefinition());

        var taskArg = returnType.GetGenericArguments()[0];
        Assert.True(taskArg.IsGenericType, $"Task payload for '{method.Name}' must be Call<,>.");
        Assert.Equal(typeof(Call<,>), taskArg.GetGenericTypeDefinition());
        return taskArg;
    }

    private static string Required(IReadOnlyDictionary<string, string> row, string key)
    {
        if (!row.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Column '{key}' must not be empty.");
        }

        return value;
    }

    private sealed record InventoryTable(IReadOnlyList<string> Header, IReadOnlyList<Dictionary<string, string>> Rows);
}
