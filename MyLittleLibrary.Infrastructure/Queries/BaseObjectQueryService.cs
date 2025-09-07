using MyLittleLibrary.Domain;
using MyLittleLibrary.Application.Queries;

namespace MyLittleLibrary.Infrastructure.Queries;

public class BaseObjectQueryService : IBaseObjectQueryService
{
    private readonly BaseObjectRepository _repository;

    public BaseObjectQueryService(BaseObjectRepository repository)
    {
        _repository = repository;
    }

    public Task<List<BaseObject>> GetMostRecentAsync(int count = 8, CancellationToken cancellationToken = default) => _repository.GetMostRecentAsync(count, cancellationToken);

    public Task<List<BaseObject>> GetMostRecentByTypeAsync(Collection collectionType, int count = 8, CancellationToken cancellationToken = default) => _repository.GetMostRecentByTypeAsync(collectionType, count, cancellationToken);

    public Task<List<BaseObject>> GetAllAsync(CancellationToken cancellationToken = default) => _repository.GetAllAsync(cancellationToken);

    public Task<BaseObject> GetByIdAsync(string id, CancellationToken cancellationToken = default) => _repository.GetByIdAsync(id, cancellationToken);
}