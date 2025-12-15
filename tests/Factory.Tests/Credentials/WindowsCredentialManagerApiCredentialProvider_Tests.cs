using System;
using ExchangeApi.Factory.Credentials;

namespace ExchangeApi.Factory.Tests.Credentials;

public class WindowsCredentialManagerApiCredentialProvider_Tests
{
    [Fact]
    public void Get_ThrowsNotSupported_OnNonWindows()
    {
        // Arrange
        var provider = new WindowsCredentialManagerApiCredentialProvider();

        if (OperatingSystem.IsWindows())
        {
            // 環境依存の資格情報が必要になるため、Windows ではこのテストをスキップ。
            return;
        }

        // Act & Assert
        Assert.Throws<PlatformNotSupportedException>(() => provider.Get("bitflyer", "default"));
    }
}
