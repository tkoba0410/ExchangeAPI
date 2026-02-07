using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos;

public sealed class GetPermissionsResponse : List<FreeText>;

public sealed class GetBalanceResponse : List<GetBalanceItem>;

public sealed class GetCollateralAccountsResponse : List<GetCollateralAccountsItem>;

public sealed class GetChildOrdersResponse : List<GetChildOrdersItem>;

public sealed class GetParentOrdersResponse : List<GetParentOrdersItem>;

public sealed class GetExecutionsPrivateResponse : List<GetExecutionsPrivateItem>;

public sealed class GetPositionsResponse : List<GetPositionsItem>;
