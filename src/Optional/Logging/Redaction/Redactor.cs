using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace ExchangeApi.Optional.Logging.Redaction;

public sealed class Redactor
{
    private static readonly Regex KeyValueRegex = new(
        @"(?<key>apiKey|api_key|apiSecret|api_secret|secret|signature|Authorization|ACCESS-KEY|ACCESS-SIGN|ACCESS-TIMESTAMP|X-Bitflyer-Access-Key|X-Bitflyer-Access-Sign|X-Bitflyer-Access-Timestamp)(?<separator>\s*[:=]\s*)(?<value>""[^""]*""|'[^']*'|[^\s,}]+)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private readonly RedactionOptions _options;

    public Redactor(RedactionOptions? options = null)
    {
        _options = options ?? new RedactionOptions();
    }

    public string RedactText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var redacted = ReplaceSensitiveValues(text);
        return KeyValueRegex.Replace(
            redacted,
            match =>
            {
                var value = match.Groups["value"].Value;
                var replacement = IsQuoted(value)
                    ? $"{value[0]}{_options.Replacement}{value[^1]}"
                    : _options.Replacement;
                return $"{match.Groups["key"].Value}{match.Groups["separator"].Value}{replacement}";
            });
    }

    public string RedactJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        var node = JsonNode.Parse(json);
        if (node is null)
        {
            return json;
        }

        RedactNode(node);
        return ReplaceSensitiveValues(node.ToJsonString());
    }

    public JsonElement RedactElement(JsonElement element)
    {
        using var document = JsonDocument.Parse(RedactJson(element.GetRawText()));
        return document.RootElement.Clone();
    }

    private void RedactNode(JsonNode node)
    {
        if (node is JsonObject obj)
        {
            foreach (var property in obj.ToArray())
            {
                if (_options.SensitivePropertyNames.Contains(property.Key))
                {
                    obj[property.Key] = _options.Replacement;
                }
                else if (property.Value is not null)
                {
                    RedactNode(property.Value);
                }
            }
        }
        else if (node is JsonArray array)
        {
            foreach (var item in array)
            {
                if (item is not null)
                {
                    RedactNode(item);
                }
            }
        }
    }

    private string ReplaceSensitiveValues(string text)
    {
        var redacted = text;
        foreach (var value in _options.SensitiveValues.Where(static value => !string.IsNullOrEmpty(value)))
        {
            redacted = redacted.Replace(value, _options.Replacement, StringComparison.Ordinal);
        }

        return redacted;
    }

    private static bool IsQuoted(string value)
    {
        return value.Length >= 2
            && ((value[0] == '"' && value[^1] == '"') || (value[0] == '\'' && value[^1] == '\''));
    }
}
