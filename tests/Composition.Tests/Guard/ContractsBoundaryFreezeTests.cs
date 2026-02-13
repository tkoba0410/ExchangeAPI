using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExchangeApi.Contracts.Facade.Interfaces;

namespace ExchangeApi.Tests.Composition.Guard;

public sealed class ContractsBoundaryFreezeTests
{
    // TEMP: remove allowlist once boundary is cleaned.
    private static readonly HashSet<string> Allowlist = new(StringComparer.Ordinal)
    {
    };

    [Fact]
    public void FacadeInterfaces_MustNotExpose_String_InPublicSignatures()
    {
        var assembly = typeof(IPublicApi).Assembly;
        var interfaces = assembly.GetExportedTypes()
            .Where(t => t.IsInterface && t.Namespace == "ExchangeApi.Contracts.Facade.Interfaces")
            .OrderBy(t => t.FullName, StringComparer.Ordinal)
            .ToArray();

        var violations = new List<string>();

        foreach (var iface in interfaces)
        {
            var methods = iface.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .OrderBy(m => m.Name, StringComparer.Ordinal)
                .ToArray();

            foreach (var method in methods)
            {
                var allowKey = $"{iface.Name}.{method.Name}";
                if (Allowlist.Contains(allowKey))
                {
                    continue;
                }

                var returnPaths = new List<string>();
                CollectStringPaths(method.ReturnType, "return", returnPaths, new HashSet<Type>());
                foreach (var path in returnPaths)
                {
                    violations.Add($"{allowKey}: string appears in {path}");
                }

                var parameters = method.GetParameters();
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameterPaths = new List<string>();
                    CollectStringPaths(
                        parameters[i].ParameterType,
                        $"parameter[{i}] '{parameters[i].Name}'",
                        parameterPaths,
                        new HashSet<Type>());

                    foreach (var path in parameterPaths)
                    {
                        violations.Add($"{allowKey}: string appears in {path}");
                    }
                }
            }
        }

        Assert.True(
            violations.Count == 0,
            "Contracts boundary freeze violation(s):\n" + string.Join("\n", violations.OrderBy(v => v, StringComparer.Ordinal)));
    }

    private static void CollectStringPaths(
        Type type,
        string path,
        List<string> hits,
        HashSet<Type> visited)
    {
        if (type == typeof(string))
        {
            hits.Add(path);
            return;
        }

        if (!visited.Add(type))
        {
            return;
        }

        if (type.IsArray)
        {
            CollectStringPaths(type.GetElementType()!, path + "[]", hits, visited);
            return;
        }

        if (Nullable.GetUnderlyingType(type) is Type underlying)
        {
            CollectStringPaths(underlying, path + "?", hits, visited);
            return;
        }

        if (IsTupleType(type))
        {
            var tupleArgs = type.GetGenericArguments();
            for (var i = 0; i < tupleArgs.Length; i++)
            {
                CollectStringPaths(tupleArgs[i], path + $".tupleItem{i + 1}", hits, visited);
            }
            return;
        }

        if (type.IsGenericType)
        {
            var genericArgs = type.GetGenericArguments();
            for (var i = 0; i < genericArgs.Length; i++)
            {
                CollectStringPaths(genericArgs[i], path + $".genericArg[{i}]", hits, visited);
            }
        }
    }

    private static bool IsTupleType(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var definition = type.GetGenericTypeDefinition();
        return definition == typeof(ValueTuple<>)
            || definition == typeof(ValueTuple<,>)
            || definition == typeof(ValueTuple<,,>)
            || definition == typeof(ValueTuple<,,,>)
            || definition == typeof(ValueTuple<,,,,>)
            || definition == typeof(ValueTuple<,,,,,>)
            || definition == typeof(ValueTuple<,,,,,,>)
            || definition == typeof(ValueTuple<,,,,,,,>)
            || definition == typeof(Tuple<>)
            || definition == typeof(Tuple<,>)
            || definition == typeof(Tuple<,,>)
            || definition == typeof(Tuple<,,,>)
            || definition == typeof(Tuple<,,,,>)
            || definition == typeof(Tuple<,,,,,>)
            || definition == typeof(Tuple<,,,,,,>)
            || definition == typeof(Tuple<,,,,,,,>);
    }
}
