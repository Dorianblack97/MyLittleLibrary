using MyLittleLibrary.Domain;
using MyLittleLibrary.Application;

namespace MyLittleLibrary.Infrastructure;

public class BaseObjectService : IBaseObjectService
{
    private readonly BaseObjectRepository _repository;

    public BaseObjectService(BaseObjectRepository repository)
    {
        _repository = repository;
    }

    public Task<List<BaseObject>> GetMostRecentAsync(int count = 8) => _repository.GetMostRecentAsync(count);

    public Task<List<BaseObject>> GetMostRecentByTypeAsync(Collection collectionType, int count = 8) => _repository.GetMostRecentByTypeAsync(collectionType, count);

    public Task<List<BaseObject>> GetAllAsync() => _repository.GetAllAsync();

    public Task<BaseObject> GetByIdAsync(string id) => _repository.GetByIdAsync(id);
}