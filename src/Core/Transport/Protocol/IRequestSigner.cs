using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace Core.Transport.Protocol;

/// <summary>
/// HTTP リクエストに署名・認証ヘッダを付与する責務を持つインターフェース。
/// </summary>
public interface IRequestSigner
{
    /// <summary>
    /// リクエストに署名・認証ヘッダを付与する。
    /// </summary>
    Task SignAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}
