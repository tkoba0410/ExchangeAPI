using Xunit.Sdk;
using RawPrivateDtos = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;
using RawPublicRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Requests;

namespace Exchange.Bitflyer.LiveTests.Infrastructure;

internal static class BitflyerLiveSettings
{
    private const string LiveEnabledEnv = "EXCHANGEAPI_BITFLYER_LIVE";
    private const string ApiKeyEnv = "EXCHANGEAPI_BITFLYER_API_KEY";
    private const string ApiSecretEnv = "EXCHANGEAPI_BITFLYER_API_SECRET";
    private const string AccountIdEnv = "EXCHANGEAPI_BITFLYER_LIVE_ACCOUNT_ID";
    private const string AllowPostEnv = "EXCHANGEAPI_BITFLYER_LIVE_ALLOW_POST";
    private const string SymbolEnv = "EXCHANGEAPI_BITFLYER_LIVE_SYMBOL";
    private const string ProductCodeEnv = "EXCHANGEAPI_BITFLYER_LIVE_PRODUCT_CODE";
    private const string OrderSideEnv = "EXCHANGEAPI_BITFLYER_LIVE_ORDER_SIDE";
    private const string OrderSizeEnv = "EXCHANGEAPI_BITFLYER_LIVE_ORDER_SIZE";
    private const string OrderPriceEnv = "EXCHANGEAPI_BITFLYER_LIVE_ORDER_PRICE";
    private const string CredentialFilePathEnv = "CREDENTIAL_FILE_PATH";
    private const string AgeSecretKeyPathEnv = "AGE_SECRET_KEY_PATH";

    private static readonly string DefaultCredentialFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".config",
        "exchangeapi",
        "secrets",
        "credentials.enc.json");

    private static readonly string DefaultAgeSecretKeyPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".config",
        "exchangeapi",
        "keys",
        "age.key");

    public static ProductCode DefaultProductCode =>
        ProductCode.ParseOrThrowNormalized(GetOptional(ProductCodeEnv, "BTC_JPY"));

    public static Symbol DefaultSymbol =>
        Symbol.ParseOrThrow(GetOptional(SymbolEnv, "BTC/JPY"));

    public static string DefaultAccountId => GetOptional(AccountIdEnv, "default");

    public static bool IsLiveEnabled() =>
        IsTruthy(Environment.GetEnvironmentVariable(LiveEnabledEnv));

    public static bool IsPostEnabled() =>
        IsTruthy(Environment.GetEnvironmentVariable(AllowPostEnv));

    public static string? GetPublicSkipReason()
    {
        if (IsLiveEnabled())
        {
            return null;
        }

        return $"Set {LiveEnabledEnv}=1 to enable bitFlyer live tests.";
    }

    public static string? GetAuthenticatedSkipReason()
    {
        var publicSkip = GetPublicSkipReason();
        if (publicSkip is not null)
        {
            return publicSkip;
        }

        if (!HasAuthenticatedCredentialSource())
        {
            return
                $"Set {ApiKeyEnv}/{ApiSecretEnv}, or provide {CredentialFilePathEnv}/{AgeSecretKeyPathEnv}, " +
                "to enable authenticated bitFlyer live tests.";
        }

        return null;
    }

    public static string? GetPostSkipReason()
    {
        var authenticatedSkip = GetAuthenticatedSkipReason();
        if (authenticatedSkip is not null)
        {
            return authenticatedSkip;
        }

        if (!IsPostEnabled())
        {
            return $"Set {AllowPostEnv}=1 to enable bitFlyer live order placement/cancel tests.";
        }

        try
        {
            _ = GetPostOrder();
            return null;
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message;
        }
        catch (FormatException ex)
        {
            return ex.Message;
        }
    }

    public static ApiCredentials GetCredentials()
    {
        var directApiKey = Environment.GetEnvironmentVariable(ApiKeyEnv);
        var directApiSecret = Environment.GetEnvironmentVariable(ApiSecretEnv);
        if (!string.IsNullOrWhiteSpace(directApiKey) &&
            !string.IsNullOrWhiteSpace(directApiSecret))
        {
            return new ApiCredentials(directApiKey.Trim(), directApiSecret.Trim());
        }

        var (credentialFilePath, ageSecretKeyPath) = ResolveCredentialPaths();
        if (credentialFilePath is null || ageSecretKeyPath is null)
        {
            throw new InvalidOperationException(
                $"Set {ApiKeyEnv}/{ApiSecretEnv}, or provide {CredentialFilePathEnv}/{AgeSecretKeyPathEnv}, before running this bitFlyer live test.");
        }

        var provider = new ExchangeApi.Composition.Providers.Credentials.AgeEncryptedFileApiCredentialProvider(
            credentialFilePath,
            "bitflyer",
            ageSecretKeyPath);

        return provider.Get(AccountId.ParseOrThrow(DefaultAccountId));
    }

    public static BitflyerLivePostOrder GetPostOrder()
    {
        var symbol = DefaultSymbol;
        var productCode = DefaultProductCode;

        var sideText = GetRequired(OrderSideEnv);
        if (!Enum.TryParse<Side>(sideText, ignoreCase: true, out var side))
        {
            throw new InvalidOperationException($"Set {OrderSideEnv} to Buy or Sell.");
        }

        Size size;
        try
        {
            size = Size.ParseSizeOrThrow(GetRequired(OrderSizeEnv));
        }
        catch (FormatException)
        {
            throw new FormatException($"Set {OrderSizeEnv} to a decimal size value.");
        }

        Price price;
        try
        {
            price = Price.ParsePriceOrThrow(GetRequired(OrderPriceEnv));
        }
        catch (FormatException)
        {
            throw new FormatException($"Set {OrderPriceEnv} to a decimal price value.");
        }

        return new BitflyerLivePostOrder(symbol, productCode, side, size, price);
    }

    public static string DescribeCredentialSource()
    {
        var directApiKey = Environment.GetEnvironmentVariable(ApiKeyEnv);
        var directApiSecret = Environment.GetEnvironmentVariable(ApiSecretEnv);
        if (!string.IsNullOrWhiteSpace(directApiKey) &&
            !string.IsNullOrWhiteSpace(directApiSecret))
        {
            return "direct-env";
        }

        var (credentialFilePath, ageSecretKeyPath) = ResolveCredentialPaths();
        return credentialFilePath is not null && ageSecretKeyPath is not null
            ? "age-credential-store"
            : "none";
    }

    private static string GetOptional(string envName, string fallback)
    {
        var value = Environment.GetEnvironmentVariable(envName);
        return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
    }

    private static string GetRequired(string envName)
    {
        var value = Environment.GetEnvironmentVariable(envName);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Set {envName} before running this bitFlyer live test.");
        }

        return value.Trim();
    }

    private static bool IsTruthy(string? value) =>
        value is not null &&
        (value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("yes", StringComparison.OrdinalIgnoreCase));

    private static bool HasAuthenticatedCredentialSource()
    {
        var directApiKey = Environment.GetEnvironmentVariable(ApiKeyEnv);
        var directApiSecret = Environment.GetEnvironmentVariable(ApiSecretEnv);
        if (!string.IsNullOrWhiteSpace(directApiKey) &&
            !string.IsNullOrWhiteSpace(directApiSecret))
        {
            return true;
        }

        var (credentialFilePath, ageSecretKeyPath) = ResolveCredentialPaths();
        return credentialFilePath is not null && ageSecretKeyPath is not null;
    }

    private static (string? CredentialFilePath, string? AgeSecretKeyPath) ResolveCredentialPaths()
    {
        var credentialFilePath = Environment.GetEnvironmentVariable(CredentialFilePathEnv);
        var ageSecretKeyPath = Environment.GetEnvironmentVariable(AgeSecretKeyPathEnv);

        if (!string.IsNullOrWhiteSpace(credentialFilePath) ||
            !string.IsNullOrWhiteSpace(ageSecretKeyPath))
        {
            return (
                NormalizeExistingPath(credentialFilePath),
                NormalizeExistingPath(ageSecretKeyPath));
        }

        return (
            File.Exists(DefaultCredentialFilePath) ? DefaultCredentialFilePath : null,
            File.Exists(DefaultAgeSecretKeyPath) ? DefaultAgeSecretKeyPath : null);
    }

    private static string? NormalizeExistingPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var trimmed = path.Trim();
        return File.Exists(trimmed) ? trimmed : null;
    }
}

internal sealed record BitflyerLivePostOrder(
    Symbol Symbol,
    ProductCode ProductCode,
    Side Side,
    Size Size,
    Price Price);

internal sealed class BitflyerLivePublicFactAttribute : FactAttribute
{
    public BitflyerLivePublicFactAttribute()
    {
        Skip = BitflyerLiveSettings.GetPublicSkipReason();
    }
}

internal sealed class BitflyerLiveAuthFactAttribute : FactAttribute
{
    public BitflyerLiveAuthFactAttribute()
    {
        Skip = BitflyerLiveSettings.GetAuthenticatedSkipReason();
    }
}

internal sealed class BitflyerLivePostFactAttribute : FactAttribute
{
    public BitflyerLivePostFactAttribute()
    {
        Skip = BitflyerLiveSettings.GetPostSkipReason();
    }
}

internal static class BitflyerLiveClientFactory
{
    private static readonly Uri BaseUri = new("https://api.bitflyer.com");
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    public static IRestClient CreatePublicRestClient() =>
        RestClientFactory.Create(
            BaseUri,
            new TransportConfig.ManagedHttp(Timeout),
            observer: BitflyerLiveLogging.Observer);

    public static IRestClient CreatePrivateRestClient()
    {
        var credentials = BitflyerLiveSettings.GetCredentials();
        var signer = new RequestSigner(credentials.ApiKey, credentials.ApiSecret, new SystemClock());

        return RestClientFactory.Create(
            BaseUri,
            new TransportConfig.ManagedHttp(Timeout),
            signer: signer,
            observer: BitflyerLiveLogging.Observer);
    }

    public static IRawApi CreateRawApi(IRestClient restClient)
    {
        var wire = new WireCallExecutor(new WireTransport(restClient));
        return new ExchangeApi.Exchanges.Bitflyer.Raw.Api.RawApi(wire);
    }

    public static INormalizedApi CreateNormalizedApi()
    {
        var credentials = BitflyerLiveSettings.GetCredentials();
        return BitflyerFactory.CreateClient(new BitflyerFactoryOptions
        {
            Credentials = credentials,
            TransportConfig = new TransportConfig.ManagedHttp(Timeout),
            Observer = BitflyerLiveLogging.Observer,
        });
    }
}

internal static class BitflyerLiveWireSpecs
{
    public static WireCallSpec GetTicker(ProductCode productCode) =>
        new(
            "GET",
            "/v1/getticker",
            EndpointIds.GetTicker,
            Query(("product_code", productCode.Value)));

    public static WireCallSpec GetBoard(ProductCode productCode) =>
        new(
            "GET",
            "/v1/getboard",
            EndpointIds.GetBoard,
            Query(("product_code", productCode.Value)));

    public static WireCallSpec GetExecutionsPublic(ProductCode productCode, int count = 10) =>
        new(
            "GET",
            "/v1/getexecutions",
            EndpointIds.GetExecutionsPublic,
            Query(
                ("product_code", productCode.Value),
                ("count", count.ToString(CultureInfo.InvariantCulture))));

    public static WireCallSpec GetBalance() =>
        new("GET", "/v1/me/getbalance", EndpointIds.GetBalance);

    public static WireCallSpec GetChildOrders(
        ProductCode productCode,
        string? childOrderAcceptanceId = null,
        string? childOrderState = "ACTIVE") =>
        new(
            "GET",
            "/v1/me/getchildorders",
            EndpointIds.GetChildOrders,
            Query(
                ("product_code", productCode.Value),
                ("child_order_state", childOrderState),
                ("child_order_acceptance_id", childOrderAcceptanceId)));

    public static WireCallSpec GetExecutionsPrivate(ProductCode productCode, int count = 10) =>
        new(
            "GET",
            "/v1/me/getexecutions",
            EndpointIds.GetExecutionsPrivate,
            Query(
                ("product_code", productCode.Value),
                ("count", count.ToString(CultureInfo.InvariantCulture))));

    public static WireCallSpec SendChildOrder(BitflyerLivePostOrder order) =>
        new(
            "POST",
            "/v1/me/sendchildorder",
            EndpointIds.SendChildOrder,
            BodyJson: JsonSerializer.Serialize(BitflyerLiveAssert.CreateSendChildOrderRequest(order)));

    public static WireCallSpec CancelChildOrder(ProductCode productCode, string acceptanceId) =>
        new(
            "POST",
            "/v1/me/cancelchildorder",
            EndpointIds.CancelChildOrder,
            BodyJson: JsonSerializer.Serialize(BitflyerLiveAssert.CreateCancelChildOrderRequest(productCode, acceptanceId)));

    private static string? Query(params (string Key, string? Value)[] pairs)
    {
        var items = new List<string>(pairs.Length);
        foreach (var (key, value) in pairs)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            items.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
        }

        return items.Count == 0 ? null : string.Join("&", items);
    }
}

internal static class BitflyerLiveAssert
{
    private static readonly TimeSpan OrderPropagationTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);

    public static TResponse RequireOk<TRequest, TResponse>(Call<TRequest, TResponse> call)
    {
        return call.Result switch
        {
            CallResult<TResponse>.Ok ok => ok.Response,
            CallResult<TResponse>.Err err => throw new XunitException(
                $"Live call failed. endpoint={call.Meta.EndpointId}, layer={call.Meta.Layer}, component={call.Meta.Component}, " +
                $"kind={err.Error.Kind}, http={err.Error.HttpStatus}, message={err.Error.Message}, body={Truncate(err.Error.BodySnippet)}, " +
                $"log_dir={BitflyerLiveLogging.LogDirectory}"),
            _ => throw new XunitException("Unexpected call result type.")
        };
    }

    public static WireResponse RequireWireSuccess(
        Call<WireCallSpec, WireResponse> call,
        bool requireJsonBody = true)
    {
        var response = RequireOk(call);
        if (response.StatusCode != 200)
        {
            throw new XunitException(
                $"Expected HTTP 200. endpoint={call.Request.EndpointId}, status={response.StatusCode}, body={Truncate(response.Json)}, " +
                $"log_dir={BitflyerLiveLogging.LogDirectory}");
        }

        if (requireJsonBody && string.IsNullOrWhiteSpace(response.Json))
        {
            throw new XunitException(
                $"Expected a JSON payload for endpoint={call.Request.EndpointId}. log_dir={BitflyerLiveLogging.LogDirectory}");
        }

        return response;
    }

    public static string ParseAcceptanceIdFromSendOrderJson(string json)
    {
        using var document = JsonDocument.Parse(json);
        if (!document.RootElement.TryGetProperty("child_order_acceptance_id", out var property))
        {
            throw new XunitException("Wire sendchildorder response did not contain child_order_acceptance_id.");
        }

        var acceptanceId = property.GetString();
        if (string.IsNullOrWhiteSpace(acceptanceId))
        {
            throw new XunitException("child_order_acceptance_id was empty.");
        }

        return acceptanceId;
    }

    public static RawPrivateRequests.SendChildOrderRequest CreateSendChildOrderRequest(BitflyerLivePostOrder order) =>
        new()
        {
            ProductCode = order.ProductCode,
            Side = new FreeText(ToApiSide(order.Side)),
            ChildOrderType = new FreeText("LIMIT"),
            Size = order.Size.Value,
            Price = order.Price.Value,
        };

    public static RawPrivateRequests.CancelChildOrderRequest CreateCancelChildOrderRequest(
        ProductCode productCode,
        string acceptanceId) =>
        new()
        {
            ProductCode = productCode,
            ChildOrderAcceptanceId = new FreeText(acceptanceId),
        };

    public static RawPrivateRequests.GetChildOrdersRequest CreateGetChildOrdersRequest(
        ProductCode productCode,
        string acceptanceId) =>
        new(
            productCode,
            ChildOrderStatusState: new FreeText("ACTIVE"),
            ChildOrderAcceptanceId: new FreeText(acceptanceId));

    public static bool WireChildOrdersContains(string json, string acceptanceId)
    {
        using var document = JsonDocument.Parse(json);
        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new XunitException("Expected getchildorders wire response to be a JSON array.");
        }

        foreach (var element in document.RootElement.EnumerateArray())
        {
            if (element.TryGetProperty("child_order_acceptance_id", out var property) &&
                string.Equals(property.GetString(), acceptanceId, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    public static bool RawChildOrdersContains(RawPrivateDtos.GetChildOrdersResponse response, string acceptanceId) =>
        response.Any(item => string.Equals(item.ChildOrderAcceptanceId, acceptanceId, StringComparison.Ordinal));

    public static bool NormalizedChildOrdersContains(
        ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.GetChildOrdersResponse response,
        string acceptanceId) =>
        response.Items.Any(item => string.Equals(item.Value.AcceptanceId?.Value, acceptanceId, StringComparison.Ordinal));

    public static async Task WaitForWireChildOrderVisibilityAsync(
        WireTransport wire,
        ProductCode productCode,
        string acceptanceId,
        bool shouldExist,
        CancellationToken cancellationToken = default)
    {
        var deadline = DateTimeOffset.UtcNow + OrderPropagationTimeout;
        while (DateTimeOffset.UtcNow < deadline)
        {
            var call = await wire
                .SendAsync(BitflyerLiveWireSpecs.GetChildOrders(productCode, acceptanceId), cancellationToken)
                .ConfigureAwait(false);
            var response = RequireWireSuccess(call);
            if (WireChildOrdersContains(response.Json, acceptanceId) == shouldExist)
            {
                return;
            }

            await Task.Delay(PollInterval, cancellationToken).ConfigureAwait(false);
        }

        throw new XunitException(
            $"Timed out waiting for wire child order visibility to become {shouldExist}. acceptanceId={acceptanceId}, " +
            $"log_dir={BitflyerLiveLogging.LogDirectory}");
    }

    public static async Task WaitForRawChildOrderVisibilityAsync(
        IRawApi raw,
        ProductCode productCode,
        string acceptanceId,
        bool shouldExist,
        CancellationToken cancellationToken = default)
    {
        var request = CreateGetChildOrdersRequest(productCode, acceptanceId);
        var deadline = DateTimeOffset.UtcNow + OrderPropagationTimeout;
        while (DateTimeOffset.UtcNow < deadline)
        {
            var call = await raw.GetChildOrdersCallAsync(request, cancellationToken).ConfigureAwait(false);
            var response = RequireOk(call);
            if (RawChildOrdersContains(response, acceptanceId) == shouldExist)
            {
                return;
            }

            await Task.Delay(PollInterval, cancellationToken).ConfigureAwait(false);
        }

        throw new XunitException(
            $"Timed out waiting for raw child order visibility to become {shouldExist}. acceptanceId={acceptanceId}, " +
            $"log_dir={BitflyerLiveLogging.LogDirectory}");
    }

    public static async Task WaitForNormalizedChildOrderVisibilityAsync(
        INormalizedApi api,
        Symbol symbol,
        string acceptanceId,
        bool shouldExist,
        CancellationToken cancellationToken = default)
    {
        var deadline = DateTimeOffset.UtcNow + OrderPropagationTimeout;
        while (DateTimeOffset.UtcNow < deadline)
        {
            var call = await api.GetChildOrdersCallAsync(symbol, cancellationToken).ConfigureAwait(false);
            var response = RequireOk(call);
            if (NormalizedChildOrdersContains(response, acceptanceId) == shouldExist)
            {
                return;
            }

            await Task.Delay(PollInterval, cancellationToken).ConfigureAwait(false);
        }

        throw new XunitException(
            $"Timed out waiting for normalized child order visibility to become {shouldExist}. acceptanceId={acceptanceId}, " +
            $"log_dir={BitflyerLiveLogging.LogDirectory}");
    }

    public static async Task CancelWireChildOrderAsync(
        WireTransport wire,
        BitflyerLivePostOrder order,
        string acceptanceId,
        CancellationToken cancellationToken = default)
    {
        var cancelCall = await wire
            .SendAsync(BitflyerLiveWireSpecs.CancelChildOrder(order.ProductCode, acceptanceId), cancellationToken)
            .ConfigureAwait(false);
        _ = RequireWireSuccess(cancelCall, requireJsonBody: false);
        await WaitForWireChildOrderVisibilityAsync(wire, order.ProductCode, acceptanceId, shouldExist: false, cancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task CancelRawChildOrderAsync(
        IRawApi raw,
        ProductCode productCode,
        string acceptanceId,
        CancellationToken cancellationToken = default)
    {
        var cancelCall = await raw
            .CancelChildOrderCallAsync(CreateCancelChildOrderRequest(productCode, acceptanceId), cancellationToken)
            .ConfigureAwait(false);
        _ = RequireOk(cancelCall);
        await WaitForRawChildOrderVisibilityAsync(raw, productCode, acceptanceId, shouldExist: false, cancellationToken)
            .ConfigureAwait(false);
    }

    public static async Task CancelNormalizedChildOrderAsync(
        INormalizedApi api,
        Symbol symbol,
        string acceptanceId,
        CancellationToken cancellationToken = default)
    {
        var cancelCall = await api
            .CancelChildOrderCallAsync(symbol, new OrderKey(OrderIdKind.AcceptanceId, acceptanceId), cancellationToken)
            .ConfigureAwait(false);
        _ = RequireOk(cancelCall);
        await WaitForNormalizedChildOrderVisibilityAsync(api, symbol, acceptanceId, shouldExist: false, cancellationToken)
            .ConfigureAwait(false);
    }

    private static string ToApiSide(Side side) =>
        side switch
        {
            Side.Buy => "BUY",
            Side.Sell => "SELL",
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unsupported side.")
        };

    private static string Truncate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "<empty>";
        }

        return value.Length <= 240 ? value : value[..240];
    }
}
