using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Queries
{
    public interface IQueryProcessor
    {
        Task<TResult> ProcessAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken cancellationToken);

        TResult Process<TResult>(
            IQuery<TResult> query,
            CancellationToken cancellationToken);
    }
}