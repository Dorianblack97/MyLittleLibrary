using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Queries;

public interface IBaseObjectQueryService
{
    Task<List<BaseObject>> GetMostRecentAsync(int count = 8, CancellationToken cancellationToken = default);
    Task<List<BaseObject>> GetMostRecentByTypeAsync(Collection collectionType, int count = 8, CancellationToken cancellationToken = default);
    Task<List<BaseObject>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BaseObject> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}