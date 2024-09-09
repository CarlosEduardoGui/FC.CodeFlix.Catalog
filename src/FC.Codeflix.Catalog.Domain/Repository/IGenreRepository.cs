using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Domain.Repository;
public interface IGenreRepository : IGenericRepository<Genre>, ISearchableRepository<Genre>
{
    Task<IReadOnlyList<Guid>> GetIdsListByIdsAsync(List<Guid> guids, CancellationToken cancellationToken);

    Task<IReadOnlyList<Genre>> GetListByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
}
