namespace ExchangeApi.Optional.Logging.Evidence;

public sealed class EvidenceRunDirectoryFactory
{
    private readonly string _repoRoot;
    private readonly Func<DateTimeOffset> _clock;

    public EvidenceRunDirectoryFactory(string repoRoot, Func<DateTimeOffset>? clock = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(repoRoot);
        _repoRoot = Path.GetFullPath(repoRoot);
        _clock = clock ?? (() => DateTimeOffset.Now);
    }

    public EvidenceRunDirectory Create(EvidencePhase phase, EvidenceRunLabel label)
    {
        ArgumentNullException.ThrowIfNull(phase);
        ArgumentNullException.ThrowIfNull(label);

        var datePrefix = _clock().ToString("yyyyMMdd");
        var baseName = $"{datePrefix}-{label.Value}";
        var phaseRoot = Path.Combine(_repoRoot, "local", "evidence", phase.Value);
        Directory.CreateDirectory(phaseRoot);

        var root = UniqueDirectory(phaseRoot, baseName);
        var runtime = Path.Combine(root, "runtime");
        var artifacts = Path.Combine(runtime, "artifacts");
        var logs = Path.Combine(runtime, "logs");
        var notes = Path.Combine(root, "notes");

        Directory.CreateDirectory(artifacts);
        Directory.CreateDirectory(logs);
        Directory.CreateDirectory(notes);

        return new EvidenceRunDirectory
        {
            Root = root,
            Runtime = runtime,
            Artifacts = artifacts,
            Logs = logs,
            Notes = notes,
        };
    }

    private static string UniqueDirectory(string parent, string baseName)
    {
        var candidate = Path.Combine(parent, baseName);
        if (!Directory.Exists(candidate))
        {
            Directory.CreateDirectory(candidate);
            return candidate;
        }

        for (var index = 2; index < int.MaxValue; index++)
        {
            candidate = Path.Combine(parent, $"{baseName}-{index}");
            if (!Directory.Exists(candidate))
            {
                Directory.CreateDirectory(candidate);
                return candidate;
            }
        }

        throw new IOException($"Could not allocate evidence run directory for {baseName}.");
    }
}
