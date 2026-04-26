using System.Diagnostics;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Optional.Credentials.AgeFile;

public sealed class AgeCliCredentialFileDecryptor : IAgeCredentialFileDecryptor
{
    private readonly string _executablePath;

    public AgeCliCredentialFileDecryptor(string executablePath = "age")
    {
        _executablePath = executablePath;
    }

    public string Decrypt(string identityFilePath, string credentialsFilePath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _executablePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        startInfo.ArgumentList.Add("-d");
        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add(identityFilePath);
        startInfo.ArgumentList.Add(credentialsFilePath);

        try
        {
            using var process = Process.Start(startInfo)
                ?? throw new ApiCredentialException(ApiCredentialErrorKind.DecryptFailed, "age process could not be started.");
            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                var reason = string.IsNullOrWhiteSpace(stderr)
                    ? "age decryption failed."
                    : $"age decryption failed: {stderr.Trim()}";
                throw new ApiCredentialException(ApiCredentialErrorKind.DecryptFailed, reason);
            }

            return stdout;
        }
        catch (ApiCredentialException)
        {
            throw;
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            throw new ApiCredentialException(ApiCredentialErrorKind.DecryptFailed, "age decryption failed.", ex);
        }
    }
}
