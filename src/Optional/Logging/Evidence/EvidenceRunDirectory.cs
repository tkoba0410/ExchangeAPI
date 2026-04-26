namespace ExchangeApi.Optional.Logging.Evidence;

public sealed class EvidenceRunDirectory
{
    public required string Root { get; init; }

    public required string Runtime { get; init; }

    public required string Artifacts { get; init; }

    public required string Logs { get; init; }

    public required string Notes { get; init; }
}
