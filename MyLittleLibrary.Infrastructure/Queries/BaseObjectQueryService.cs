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

    public Task<List<BaseObject>> GetMostRecentAsync(int count = 8) => _repository.GetMostRecentAsync(count);

    public Task<List<BaseObject>> GetMostRecentByTypeAsync(Collection collectionType, int count = 8) => _repository.GetMostRecentByTypeAsync(collectionType, count);

    public Task<List<BaseObject>> GetAllAsync() => _repository.GetAllAsync();

    public Task<BaseObject> GetByIdAsync(string id) => _repository.GetByIdAsync(id);
}