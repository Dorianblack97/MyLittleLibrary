using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application.Queries;

public interface IBaseObjectQueryService
{
    Task<List<BaseObject>> GetMostRecentAsync(int count = 8);
    Task<List<BaseObject>> GetMostRecentByTypeAsync(Collection collectionType, int count = 8);
    Task<List<BaseObject>> GetAllAsync();
    Task<BaseObject> GetByIdAsync(string id);
}