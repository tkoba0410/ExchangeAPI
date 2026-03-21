using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using ExchangeApi.Transport.Observability;

namespace Exchange.Bitflyer.LiveTests.Infrastructure;

internal static class BitflyerLiveLogging
{
    private static readonly AsyncLocal<BitflyerLiveLogScopeContext?> CurrentScopeSlot = new();
    private static readonly Lazy<BitflyerLiveLogStore> StoreInstance = new(CreateStore);

    public static IRestCallObserver Observer => StoreInstance.Value.Observer;

    public static string LogDirectory => StoreInstance.Value.LogDirectory;

    public static IDisposable BeginScope(string flow, string layer, string testName)
    {
        if (string.IsNullOrWhiteSpace(flow))
        {
            throw new ArgumentException("Flow must not be null or whitespace.", nameof(flow));
        }

        if (string.IsNullOrWhiteSpace(layer))
        {
            throw new ArgumentException("Layer must not be null or whitespace.", nameof(layer));
        }

        if (string.IsNullOrWhiteSpace(testName))
        {
            throw new ArgumentException("Test name must not be null or whitespace.", nameof(testName));
        }

        var scope = new BitflyerLiveLogScopeContext(
            ScopeId: $"scope_{Guid.NewGuid():N}",
            Flow: flow,
            Layer: layer,
            TestName: testName,
            StartedAtUtc: DateTimeOffset.UtcNow);

        var previous = CurrentScopeSlot.Value;
        CurrentScopeSlot.Value = scope;
        StoreInstance.Value.WriteScopeEvent("start", scope, null);
        return new ScopeHandle(scope, previous);
    }

    internal static BitflyerLiveLogScopeContext? GetCurrentScope() => CurrentScopeSlot.Value;

    private static BitflyerLiveLogStore CreateStore()
    {
        var session = BitflyerLiveLogSession.Create();
        return new BitflyerLiveLogStore(session);
    }

    private sealed class ScopeHandle : IDisposable
    {
        private readonly BitflyerLiveLogScopeContext _scope;
        private readonly BitflyerLiveLogScopeContext? _previous;
        private bool _disposed;

        public ScopeHandle(BitflyerLiveLogScopeContext scope, BitflyerLiveLogScopeContext? previous)
        {
            _scope = scope;
            _previous = previous;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            var duration = DateTimeOffset.UtcNow - _scope.StartedAtUtc;
            StoreInstance.Value.WriteScopeEvent("end", _scope, duration);
            CurrentScopeSlot.Value = _previous;
        }
    }
}

internal sealed record BitflyerLiveLogScopeContext(
    string ScopeId,
    string Flow,
    string Layer,
    string TestName,
    DateTimeOffset StartedAtUtc);

internal sealed class BitflyerLiveLogStore
{
    private static readonly JsonSerializerOptions ManifestJsonOptions = new() { WriteIndented = true };
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    private readonly object _gate = new();
    private readonly BitflyerLiveLogSession _session;

    public BitflyerLiveLogStore(BitflyerLiveLogSession session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        Observer = new BitflyerLiveFileObserver(this);
        Directory.CreateDirectory(_session.LogDirectory);
        WriteManifest();
    }

    public BitflyerLiveFileObserver Observer { get; }

    public string RunId => _session.RunId;

    public string LogDirectory => _session.LogDirectory;

    public void WriteScopeEvent(string phase, BitflyerLiveLogScopeContext scope, TimeSpan? duration)
    {
        WriteEvent(new
        {
            timestamp = DateTimeOffset.UtcNow,
            event_type = "test_scope",
            phase,
            exchange = "bitflyer",
            run_id = _session.RunId,
            scope_id = scope.ScopeId,
            flow = scope.Flow,
            layer = scope.Layer,
            test_name = scope.TestName,
            duration_ms = duration is null ? (long?)null : (long)Math.Round(duration.Value.TotalMilliseconds),
        });
    }

    public void WriteEvent(object payload)
    {
        var line = JsonSerializer.Serialize(payload);
        lock (_gate)
        {
            File.AppendAllText(_session.EventsPath, line + Environment.NewLine, Utf8NoBom);
        }
    }

    private void WriteManifest()
    {
        var manifest = new
        {
            schema_version = 1,
            exchange = "bitflyer",
            run_id = _session.RunId,
            started_at_utc = _session.StartedAtUtc,
            log_directory = _session.LogDirectory,
            events_file = _session.EventsPath,
            default_symbol = BitflyerLiveSettings.DefaultSymbol.Value,
            default_product_code = BitflyerLiveSettings.DefaultProductCode.Value,
            live_enabled = BitflyerLiveSettings.IsLiveEnabled(),
            allow_post = BitflyerLiveSettings.IsPostEnabled(),
            credential_source = BitflyerLiveSettings.DescribeCredentialSource(),
            sanitization = new
            {
                auth_fields = "masked",
                order_and_account_identifiers = "pseudonymized",
                private_balance_fields = "masked",
            },
        };

        var json = JsonSerializer.Serialize(manifest, ManifestJsonOptions);
        File.WriteAllText(_session.ManifestPath, json + Environment.NewLine, Utf8NoBom);
    }
}

internal sealed record BitflyerLiveLogSession(
    string RunId,
    DateTimeOffset StartedAtUtc,
    string LogDirectory,
    string ManifestPath,
    string EventsPath)
{
    public static BitflyerLiveLogSession Create()
    {
        var startedAtUtc = DateTimeOffset.UtcNow;
        var runId = $"run_{startedAtUtc:yyyyMMddTHHmmssfffZ}_{Guid.NewGuid():N}";
        var root = ResolveLogRootDirectory();
        var directory = Path.Combine(root, runId);
        return new BitflyerLiveLogSession(
            RunId: runId,
            StartedAtUtc: startedAtUtc,
            LogDirectory: directory,
            ManifestPath: Path.Combine(directory, "run.json"),
            EventsPath: Path.Combine(directory, "events.jsonl"));
    }

    private static string ResolveLogRootDirectory()
    {
        var configured = Environment.GetEnvironmentVariable("EXCHANGEAPI_BITFLYER_LIVE_LOG_DIR");
        var repositoryRoot = FindRepositoryRoot();
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return Path.GetFullPath(configured.Trim(), repositoryRoot);
        }

        return Path.Combine(repositoryRoot, "artifacts", "live-logs", "bitflyer");
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var solutionPath = Path.Combine(current.FullName, "ExchangeApi.slnx");
            if (File.Exists(solutionPath))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        return Directory.GetCurrentDirectory();
    }
}

internal sealed class BitflyerLiveFileObserver : IRestCallObserver
{
    private readonly BitflyerLiveLogStore _store;

    public BitflyerLiveFileObserver(BitflyerLiveLogStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public void OnRequest(RestCallContext context)
    {
        var scope = BitflyerLiveLogging.GetCurrentScope();
        var request = context.Request;
        var uri = request.RequestUri;
        var isPrivate = IsPrivateEndpoint(uri);
        var body = BitflyerLiveLogSanitizer.SanitizeBody(TryReadContent(request.Content), isPrivate);

        _store.WriteEvent(new
        {
            timestamp = DateTimeOffset.UtcNow,
            event_type = "request",
            exchange = "bitflyer",
            run_id = _store.RunId,
            request_id = context.RequestId,
            scope_id = scope?.ScopeId,
            flow = scope?.Flow,
            layer = scope?.Layer,
            test_name = scope?.TestName,
            method = request.Method.Method,
            endpoint = context.Endpoint,
            uri = BitflyerLiveLogSanitizer.SanitizeUri(uri),
            headers = BitflyerLiveLogSanitizer.SanitizeHeaders(request.Headers, request.Content?.Headers),
            content_type = request.Content?.Headers?.ContentType?.MediaType,
            body_kind = body.Kind,
            body = body.Value,
        });
    }

    public void OnResponse(RestCallContext context, HttpResponseMessage response, string content, TimeSpan duration)
    {
        var scope = BitflyerLiveLogging.GetCurrentScope();
        var isPrivate = IsPrivateEndpoint(context.Request.RequestUri);
        var body = BitflyerLiveLogSanitizer.SanitizeBody(content, isPrivate);

        _store.WriteEvent(new
        {
            timestamp = DateTimeOffset.UtcNow,
            event_type = "response",
            exchange = "bitflyer",
            run_id = _store.RunId,
            request_id = context.RequestId,
            scope_id = scope?.ScopeId,
            flow = scope?.Flow,
            layer = scope?.Layer,
            test_name = scope?.TestName,
            method = context.Method,
            endpoint = context.Endpoint,
            status_code = (int)response.StatusCode,
            reason = response.ReasonPhrase,
            duration_ms = (long)Math.Round(duration.TotalMilliseconds),
            headers = BitflyerLiveLogSanitizer.SanitizeHeaders(response.Headers, response.Content?.Headers),
            content_type = response.Content?.Headers?.ContentType?.MediaType,
            body_kind = body.Kind,
            body = body.Value,
        });
    }

    public void OnError(RestCallContext context, Exception exception, TimeSpan duration, HttpStatusCode? statusCode = null)
    {
        var scope = BitflyerLiveLogging.GetCurrentScope();

        _store.WriteEvent(new
        {
            timestamp = DateTimeOffset.UtcNow,
            event_type = "error",
            exchange = "bitflyer",
            run_id = _store.RunId,
            request_id = context.RequestId,
            scope_id = scope?.ScopeId,
            flow = scope?.Flow,
            layer = scope?.Layer,
            test_name = scope?.TestName,
            method = context.Method,
            endpoint = context.Endpoint,
            uri = BitflyerLiveLogSanitizer.SanitizeUri(context.Request.RequestUri),
            duration_ms = (long)Math.Round(duration.TotalMilliseconds),
            status_code = statusCode is null ? (int?)null : (int)statusCode.Value,
            error_type = exception.GetType().FullName,
            error_message = BitflyerLiveLogSanitizer.RedactedMessage,
            error_ref = BitflyerLiveLogSanitizer.CreateErrorReference(exception),
        });
    }

    private static bool IsPrivateEndpoint(Uri? uri) =>
        uri?.AbsolutePath.Contains("/v1/me/", StringComparison.OrdinalIgnoreCase) == true;

    private static string? TryReadContent(HttpContent? content)
    {
        if (content is null)
        {
            return null;
        }

        try
        {
            return content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
        catch
        {
            return "<unavailable>";
        }
    }
}

internal static class BitflyerLiveLogSanitizer
{
    public const string Mask = "***";
    public const string RedactedMessage = "<redacted>";

    private const string OrderIdPrefix = "oidp_v1_";
    private const string AccountIdPrefix = "acctp_v1_";
    private static readonly byte[] PseudonymizationKey = CreatePseudonymizationKey();

    private static readonly HashSet<string> AllowedQueryKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "symbol",
        "product_code",
        "type",
        "types",
        "period",
        "size",
        "count",
        "before",
        "after",
        "from",
        "direct",
        "status",
        "currency",
        "currency_code",
        "start-date",
        "end-date",
        "start_time",
        "end_time",
        "from_date",
        "child_order_state",
        "parent_order_state",
    };

    private static readonly HashSet<string> MaskedQueryKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "signature",
        "signaturemethod",
        "signatureversion",
        "accesskeyid",
        "access-key",
        "access-sign",
        "access-timestamp",
        "api_key",
        "apikey",
        "secret",
        "token",
        "authorization",
        "passphrase",
        "nonce",
        "timestamp",
        "message_id",
    };

    private static readonly HashSet<string> OrderIdentifierKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "order_id",
        "client_order_id",
        "child_order_id",
        "parent_order_id",
        "child_order_acceptance_id",
        "parent_order_acceptance_id",
    };

    private static readonly HashSet<string> AccountIdentifierKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "account_id",
        "account-id",
        "accountId",
        "uid",
        "sub_account",
        "sub_account_id",
        "bank_account_id",
    };

    private static readonly HashSet<string> MaskedHeaderKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "authorization",
        "proxy-authorization",
        "access-key",
        "access-sign",
        "access-timestamp",
        "accesskeyid",
        "x-api-key",
        "api-key",
        "apikey",
        "signature",
        "passphrase",
        "cookie",
    };

    private static readonly HashSet<string> MaskedBodyKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "api_key",
        "api_secret",
        "access_key",
        "access_sign",
        "access_timestamp",
        "accesskeyid",
        "authorization",
        "signature",
        "signaturemethod",
        "signatureversion",
        "secret",
        "token",
        "passphrase",
        "nonce",
        "message_id",
    };

    private static readonly HashSet<string> PrivateNumericKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "amount",
        "available",
        "collateral",
        "require_collateral",
        "open_position_pnl",
        "keep_rate",
        "sfd",
        "swap_point_accumulate",
    };

    public static string CreateErrorReference(Exception exception)
    {
        var payload = Encoding.UTF8.GetBytes($"{exception.GetType().FullName}|{exception.Message}");
        using var hmac = new HMACSHA256(PseudonymizationKey);
        var hash = hmac.ComputeHash(payload);
        return $"errp_v1_{Convert.ToHexString(hash, 0, 8)}";
    }

    public static string SanitizeUri(Uri? uri)
    {
        if (uri is null)
        {
            return "<null>";
        }

        var baseUri = uri.GetLeftPart(UriPartial.Path);
        var rawQuery = uri.Query;
        if (string.IsNullOrWhiteSpace(rawQuery))
        {
            return baseUri;
        }

        var sanitizedPairs = new List<string>();
        foreach (var pair in rawQuery.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var splitIndex = pair.IndexOf('=');
            var encodedKey = splitIndex >= 0 ? pair[..splitIndex] : pair;
            var encodedValue = splitIndex >= 0 ? pair[(splitIndex + 1)..] : string.Empty;
            var key = Uri.UnescapeDataString(encodedKey);
            var value = Uri.UnescapeDataString(encodedValue);
            sanitizedPairs.Add($"{key}={SanitizeQueryValue(uri.Host, key, value)}");
        }

        return sanitizedPairs.Count == 0
            ? baseUri
            : $"{baseUri}?{string.Join("&", sanitizedPairs)}";
    }

    public static IReadOnlyDictionary<string, string[]> SanitizeHeaders(
        HttpHeaders headers,
        HttpHeaders? contentHeaders = null)
    {
        var items = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
        AddHeaders(items, headers);
        if (contentHeaders is not null)
        {
            AddHeaders(items, contentHeaders);
        }

        return new ReadOnlyDictionary<string, string[]>(items);
    }

    public static BitflyerLiveBodyLog SanitizeBody(string? content, bool isPrivate)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return new BitflyerLiveBodyLog("empty", null);
        }

        var trimmed = content.Trim();
        if (string.Equals(trimmed, "<unavailable>", StringComparison.Ordinal))
        {
            return new BitflyerLiveBodyLog("unavailable", trimmed);
        }

        try
        {
            using var document = JsonDocument.Parse(trimmed);
            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream))
            {
                WriteSanitizedElement(writer, document.RootElement, propertyName: null, isPrivate);
            }

            return new BitflyerLiveBodyLog("json", Encoding.UTF8.GetString(stream.ToArray()));
        }
        catch (JsonException)
        {
            if (isPrivate)
            {
                return new BitflyerLiveBodyLog("text", "<redacted-private-text-body>");
            }

            return new BitflyerLiveBodyLog("text", trimmed);
        }
    }

    private static void AddHeaders(IDictionary<string, string[]> target, HttpHeaders headers)
    {
        foreach (var header in headers)
        {
            var value = MaskedHeaderKeys.Contains(header.Key)
                ? new[] { Mask }
                : header.Value.ToArray();
            target[header.Key] = value;
        }
    }

    private static string SanitizeQueryValue(string host, string key, string value)
    {
        if (OrderIdentifierKeys.Contains(key))
        {
            return string.IsNullOrEmpty(value) ? Mask : Pseudonymize(host, key, value, OrderIdPrefix);
        }

        if (AccountIdentifierKeys.Contains(key))
        {
            return string.IsNullOrEmpty(value) ? Mask : Pseudonymize(host, key, value, AccountIdPrefix);
        }

        if (MaskedQueryKeys.Contains(key))
        {
            return Mask;
        }

        return AllowedQueryKeys.Contains(key) ? value : Mask;
    }

    private static void WriteSanitizedElement(
        Utf8JsonWriter writer,
        JsonElement element,
        string? propertyName,
        bool isPrivate)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject())
                {
                    writer.WritePropertyName(property.Name);
                    WriteSanitizedElement(writer, property.Value, property.Name, isPrivate);
                }

                writer.WriteEndObject();
                break;

            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                {
                    WriteSanitizedElement(writer, item, propertyName, isPrivate);
                }

                writer.WriteEndArray();
                break;

            case JsonValueKind.String:
                writer.WriteStringValue(SanitizeStringValue(element.GetString(), propertyName));
                break;

            case JsonValueKind.Number:
                if (ShouldMaskPrivateNumber(propertyName, isPrivate))
                {
                    writer.WriteStringValue(Mask);
                }
                else if (element.TryGetInt64(out var int64))
                {
                    writer.WriteNumberValue(int64);
                }
                else if (element.TryGetDecimal(out var decimalValue))
                {
                    writer.WriteNumberValue(decimalValue);
                }
                else if (element.TryGetDouble(out var doubleValue))
                {
                    writer.WriteNumberValue(doubleValue);
                }
                else
                {
                    writer.WriteRawValue(element.GetRawText());
                }

                break;

            case JsonValueKind.True:
                writer.WriteBooleanValue(true);
                break;

            case JsonValueKind.False:
                writer.WriteBooleanValue(false);
                break;

            case JsonValueKind.Null:
                writer.WriteNullValue();
                break;

            default:
                writer.WriteStringValue(RedactedMessage);
                break;
        }
    }

    private static string? SanitizeStringValue(string? value, string? propertyName)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(propertyName))
        {
            return value;
        }

        if (MaskedBodyKeys.Contains(propertyName))
        {
            return Mask;
        }

        if (OrderIdentifierKeys.Contains(propertyName))
        {
            return Pseudonymize("json", propertyName, value, OrderIdPrefix);
        }

        if (AccountIdentifierKeys.Contains(propertyName))
        {
            return Pseudonymize("json", propertyName, value, AccountIdPrefix);
        }

        return value;
    }

    private static bool ShouldMaskPrivateNumber(string? propertyName, bool isPrivate) =>
        isPrivate &&
        !string.IsNullOrWhiteSpace(propertyName) &&
        PrivateNumericKeys.Contains(propertyName);

    private static string Pseudonymize(string host, string key, string value, string prefix)
    {
        var payload = Encoding.UTF8.GetBytes($"{host}|{key}|{value}");
        using var hmac = new HMACSHA256(PseudonymizationKey);
        var hash = hmac.ComputeHash(payload);
        var token = Convert.ToHexString(hash, 0, 8);
        return $"{prefix}{token}";
    }

    private static byte[] CreatePseudonymizationKey()
    {
        var keyFromEnvironment = Environment.GetEnvironmentVariable("EXCHANGEAPI_LOG_MASK_KEY");
        if (!string.IsNullOrWhiteSpace(keyFromEnvironment))
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(keyFromEnvironment));
        }

        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return bytes;
    }
}

internal sealed record BitflyerLiveBodyLog(string Kind, string? Value);
