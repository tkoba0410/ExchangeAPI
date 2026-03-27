using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class ExecutionOutcome
{
    public required int ExitCode { get; init; }
    public object? Response { get; init; }
    public required string Summary { get; init; }
    public string? Detail { get; init; }
    public string? ErrorKind { get; init; }
    public string? EndpointId { get; init; }
    public string? ProtocolPath { get; init; }
    public int? ProtocolStatusCode { get; init; }

    public static ExecutionOutcome Success(string summary, object? response)
    {
        return new ExecutionOutcome
        {
            ExitCode = CliExitCode.Success,
            Summary = summary,
            Response = response,
        };
    }

    public static ExecutionOutcome InputError(string summary, string? detail = null)
    {
        return new ExecutionOutcome
        {
            ExitCode = CliExitCode.ArgumentConfigOrSafetyError,
            Summary = summary,
            Detail = detail,
        };
    }

    public static ExecutionOutcome Unexpected(string summary, string? detail = null)
    {
        return new ExecutionOutcome
        {
            ExitCode = CliExitCode.UnexpectedInternalError,
            Summary = summary,
            Detail = detail,
        };
    }

    public static ExecutionOutcome FromCall<TRequest, TResponse>(CommandPath path, Call<TRequest, TResponse> call)
    {
        if (call.IsSuccess)
        {
            return Success($"{path.Identity}: success", call.Response);
        }

        var summary = $"{path.Identity}: {ClassifyCallFailure(call.Error?.Kind)}";
        var detail = call.Error?.Message;
        var child = call.Meta.Children?
            .OfType<Call<ProtocolRequest, ProtocolResponse>>()
            .FirstOrDefault();

        return new ExecutionOutcome
        {
            ExitCode = CliExitCode.FacadeCallFailure,
            Summary = summary,
            Detail = detail,
            ErrorKind = call.Error?.Kind,
            EndpointId = call.Meta.EndpointId,
            ProtocolPath = child?.Request.Path,
            ProtocolStatusCode = child?.Response?.StatusCode,
        };
    }

    private static string ClassifyCallFailure(string? errorKind)
    {
        return errorKind switch
        {
            CallErrorKinds.Transport => "protocol transport failure",
            CallErrorKinds.Http => "protocol http failure",
            CallErrorKinds.Codec => "native codec failure",
            CallErrorKinds.Semantic => "native semantic failure",
            CallErrorKinds.Mapping => "native mapping failure",
            _ => "facade call failure",
        };
    }
}
