using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Composition.Providers.Credentials;

namespace Composition.Tests.Credentials;

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
        Assert.Throws<PlatformNotSupportedException>(() => provider.Get(ExchangeCode.Bitflyer, "default"));
    }
}
