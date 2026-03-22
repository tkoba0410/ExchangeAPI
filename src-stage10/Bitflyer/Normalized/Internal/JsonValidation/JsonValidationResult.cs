using System.Text.Json;

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Internal.JsonValidation;

internal readonly record struct JsonValidationResult(JsonDocument Document, JsonElement Root);
