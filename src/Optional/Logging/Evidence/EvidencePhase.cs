namespace ExchangeApi.Optional.Logging.Evidence;

public sealed record EvidencePhase
{
    public static readonly EvidencePhase Static = new("static");
    public static readonly EvidencePhase Verification = new("verification");
    public static readonly EvidencePhase LocalLive = new("local-live");
    public static readonly EvidencePhase TestOperation = new("test-operation");

    private static readonly IReadOnlyDictionary<string, EvidencePhase> Known = new Dictionary<string, EvidencePhase>(StringComparer.Ordinal)
    {
        [Static.Value] = Static,
        [Verification.Value] = Verification,
        [LocalLive.Value] = LocalLive,
        [TestOperation.Value] = TestOperation,
    };

    private EvidencePhase(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static EvidencePhase Parse(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (Known.TryGetValue(value, out var phase))
        {
            return phase;
        }

        throw new ArgumentException($"Unsupported evidence phase: {value}.", nameof(value));
    }

    public override string ToString()
    {
        return Value;
    }
}
