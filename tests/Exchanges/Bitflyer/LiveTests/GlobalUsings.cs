global using Xunit;
global using System;
global using System.Collections.Generic;
global using System.Globalization;
global using System.Linq;
global using System.Text.Json;
global using System.Threading;
global using System.Threading.Tasks;
global using ExchangeApi.Composition.Bootstrap.Transport;
global using ExchangeApi.Composition.Dtos;
global using ExchangeApi.Exchanges.Bitflyer.Composition;
global using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
global using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
global using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
global using ExchangeApi.Exchanges.Bitflyer.Wire.Internal;
global using ExchangeApi.Exchanges.Bitflyer.Wire.Internal.Auth;
global using ExchangeApi.Primitives.CallCommon;
global using ExchangeApi.Primitives.DomainCommon.Enums;
global using ExchangeApi.Primitives.DomainCommon.Types;
global using ExchangeApi.Transport.Http;
global using ExchangeApi.Transport.Protocol;
global using ExchangeApi.Transport.Time;
global using ExchangeApi.Transport.Wire;
global using Exchange.Bitflyer.LiveTests.Infrastructure;

[assembly: CollectionBehavior(DisableTestParallelization = true, MaxParallelThreads = 1)]
