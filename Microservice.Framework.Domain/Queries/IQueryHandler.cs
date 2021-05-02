using System.Threading;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Queries
{
    public interface IQueryHandler
    {
    }

    public interface IQueryHandler<in TQuery, TResult> : IQueryHandler
        where TQuery : IQuery<TResult>
    {
        Task<TResult> ExecuteQueryAsync(TQuery query, CancellationToken cancellationToken);
    }
}