using System;
using System.Runtime.InteropServices;
using System.Text;
using Common.Contract.Interfaces;
using Common.Contract.Dtos;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace ExchangeApi.Factory.Credentials;

/// <summary>
/// Windows 資格情報マネージャーから API キー/シークレットを取得するプロバイダー。
/// ターゲット名: exchangeId/accountId/api_key | api_secret（小文字想定）。
/// </summary>
public sealed class WindowsCredentialManagerApiCredentialProvider : IApiCredentialProvider
{
    public ApiCredentials Get(string exchangeId, string accountId)
    {
        if (string.IsNullOrWhiteSpace(exchangeId))
        {
            throw new ArgumentException("ExchangeId is required.", nameof(exchangeId));
        }

        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new ArgumentException("AccountId is required.", nameof(accountId));
        }

        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Windows Credential Manager is only available on Windows.");
        }

        var apiKeyTarget = BuildTarget(exchangeId, accountId, "api_key");
        var apiSecretTarget = BuildTarget(exchangeId, accountId, "api_secret");

        var apiKey = ReadCredential(apiKeyTarget);
        var apiSecret = ReadCredential(apiSecretTarget);

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException($"Credential '{apiKeyTarget}' is not found or empty.");
        }

        if (string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new InvalidOperationException($"Credential '{apiSecretTarget}' is not found or empty.");
        }

        return new ApiCredentials(apiKey, apiSecret);
    }

    private static string BuildTarget(string exchangeId, string accountId, string keyName)
    {
        return $"{exchangeId.Trim()}/{accountId.Trim()}/{keyName}".ToLowerInvariant();
    }

    private static string ReadCredential(string targetName)
    {
        if (!CredRead(targetName, CredType.Generic, 0, out var credPtr))
        {
            throw new InvalidOperationException(
                $"Credential '{targetName}' could not be read. Win32Error={Marshal.GetLastWin32Error()}.");
        }

        try
        {
            var credential = Marshal.PtrToStructure<CREDENTIAL>(credPtr);
            if (credential.CredentialBlobSize <= 0 || credential.CredentialBlob == IntPtr.Zero)
            {
                return string.Empty;
            }

            var blob = new byte[credential.CredentialBlobSize];
            Marshal.Copy(credential.CredentialBlob, blob, 0, credential.CredentialBlobSize);

            // Credential Manager stores the blob as UTF-16 for generic credentials.
            return Encoding.Unicode.GetString(blob);
        }
        finally
        {
            CredFree(credPtr);
        }
    }

    [DllImport("advapi32", EntryPoint = "CredReadW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CredRead(
        string target,
        CredType type,
        int reservedFlag,
        out IntPtr credentialPtr);

    [DllImport("advapi32", SetLastError = true)]
    private static extern void CredFree(IntPtr buffer);

    private enum CredType
    {
        Generic = 1,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CREDENTIAL
    {
        public int Flags;
        public CredType Type;
        public string TargetName;
        public string Comment;
        public FILETIME LastWritten;
        public int CredentialBlobSize;
        public IntPtr CredentialBlob;
        public int Persist;
        public int AttributeCount;
        public IntPtr Attributes;
        public string TargetAlias;
        public string UserName;
    }
}
