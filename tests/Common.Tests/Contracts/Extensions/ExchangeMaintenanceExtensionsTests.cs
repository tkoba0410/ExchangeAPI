using System;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Composition.Extensions;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.Contracts.Extensions;

public class ExchangeMaintenanceExtensionsTests
{
    private static readonly DateTimeOffset Now = new(2024, 01, 01, 00, 00, 00, TimeSpan.Zero);

    [Fact]
    public void Normal_ShouldReturnFalse()
    {
        var m = new ExchangeMaintenance(ExchangeMaintenanceStatus.Normal, PlannedUntil: Now.AddMinutes(5));

        Assert.False(m.IsInMaintenance(Now));
    }

    [Fact]
    public void Null_ShouldReturnFalse()
    {
        ExchangeMaintenance? m = null;

        Assert.False(m.IsInMaintenance(Now));
    }

    [Fact]
    public void Unplanned_ShouldReturnTrue()
    {
        var m = new ExchangeMaintenance(ExchangeMaintenanceStatus.Unplanned, PlannedUntil: null);

        Assert.True(m.IsInMaintenance(Now));
    }

    [Fact]
    public void PlannedWithoutEnd_ShouldReturnTrue()
    {
        var m = new ExchangeMaintenance(ExchangeMaintenanceStatus.Planned, PlannedUntil: null);

        Assert.True(m.IsInMaintenance(Now));
    }

    [Fact]
    public void PlannedWithEnd_BeforeEnd_ShouldReturnTrue()
    {
        var m = new ExchangeMaintenance(ExchangeMaintenanceStatus.Planned, PlannedUntil: Now.AddMinutes(1));

        Assert.True(m.IsInMaintenance(Now));
    }

    [Fact]
    public void PlannedWithEnd_AfterEnd_ShouldReturnFalse()
    {
        var m = new ExchangeMaintenance(ExchangeMaintenanceStatus.Planned, PlannedUntil: Now.AddMinutes(-1));

        Assert.False(m.IsInMaintenance(Now));
    }

    [Fact]
    public void PlannedWithEnd_GracePeriod_ShouldHonorGrace()
    {
        var m = new ExchangeMaintenance(ExchangeMaintenanceStatus.Planned, PlannedUntil: Now.AddMinutes(-1));

        Assert.True(m.IsInMaintenance(Now, TimeSpan.FromMinutes(2)));
    }
}
