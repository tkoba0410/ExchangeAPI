using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos;

public sealed class GetPermissionsResponse : List<FreeText>;

public sealed class GetBalanceResponse : List<BalanceResponse>;

public sealed class GetCollateralAccountsResponse : List<CollateralAccount>;

public sealed class GetChildOrdersResponse : List<RawGetChildOrdersResponse>;

public sealed class GetParentOrdersResponse : List<RawGetParentOrdersResponse>;

public sealed class GetExecutionsPrivateResponse : List<ExecutionPrivateResponse>;

public sealed class GetPositionsResponse : List<PositionResponse>;
