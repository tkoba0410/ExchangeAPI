using System;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
namespace ExchangeApi.Domain.Extensions;

/// <summary>
/// ExchangeMaintenance 判定用の簡易ヘルパ。
/// </summary>
public static class ExchangeMaintenanceExtensions
{
    /// <summary>
    /// 現在がメンテナンス中かを判定する。Planned で終了予定が無い場合はメンテ中とみなす。
    /// gracePeriod を指定すると終了時刻を猶予分だけ延長して判定する。
    /// </summary>
    public static bool IsInMaintenance(this ExchangeMaintenance? maintenance, DateTimeOffset nowUtc, TimeSpan? gracePeriod = null)
    {
        if (maintenance is null) return false;

        var grace = gracePeriod ?? TimeSpan.Zero;

        return maintenance.Status switch
        {
            ExchangeMaintenanceStatus.Unplanned => true,
            ExchangeMaintenanceStatus.Planned => IsPlannedActive(maintenance.PlannedUntil, nowUtc, grace),
            _ => false
        };
    }

    private static bool IsPlannedActive(DateTimeOffset? plannedUntilUtc, DateTimeOffset nowUtc, TimeSpan grace)
    {
        if (plannedUntilUtc is null)
        {
            // 終了時刻不明な計画メンテは進行中とみなす。
            return true;
        }

        var end = plannedUntilUtc.Value + grace;
        return nowUtc <= end;
    }
}
