using System.Diagnostics;

namespace ExchangeApi.Adapters.Cli.Configuration;

public sealed class ProcessAgeCredentialDecryptor : IAgeCredentialDecryptor
{
    public static ProcessAgeCredentialDecryptor Instance { get; } = new();

    private ProcessAgeCredentialDecryptor()
    {
    }

    public bool IsAvailable()
    {
        var pathValue = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(pathValue))
        {
            return false;
        }

        foreach (var directory in pathValue.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var candidate = Path.Combine(directory, "age");
            if (File.Exists(candidate))
            {
                return true;
            }
        }

        return false;
    }

    public string Decrypt(string identityFilePath, string credentialsFilePath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "age",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add("-d");
        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add(identityFilePath);
        startInfo.ArgumentList.Add(credentialsFilePath);

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start the age process.");

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            var message = string.IsNullOrWhiteSpace(stderr)
                ? "age decryption failed."
                : $"age decryption failed: {stderr.Trim()}";

            throw new InvalidOperationException(message);
        }

        return stdout;
    }
}
