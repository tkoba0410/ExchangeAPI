namespace ExchangeApi.Primitives.Calls;

public sealed class CallMeta
{
    public required string Layer { get; init; }
    public required string Component { get; init; }
    public required string EndpointId { get; init; }
    public required string Scope { get; init; }
    public required string Auth { get; init; }
    public IReadOnlyList<object>? Children { get; init; }
}

public static class CallLayers
{
    public const string Protocol = "Protocol";
    public const string Native = "Native";
    public const string Composition = "Composition";
    public const string Tests = "Tests";
}

public static class CallComponents
{
    public const string PublicFacade = "PublicFacade";
    public const string PrivateFacade = "PrivateFacade";
    public const string PublicEndpointModule = "PublicEndpointModule";
    public const string PrivateEndpointModule = "PrivateEndpointModule";
    public const string Transport = "Transport";
    public const string Factory = "Factory";
    public const string Bootstrap = "Bootstrap";
}
