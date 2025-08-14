using MyLittleLibrary.Domain;

namespace MyLittleLibrary.Application;

public interface IBaseObjectService
{
    Task<List<BaseObject>> GetMostRecentAsync(int count = 8);
    Task<List<BaseObject>> GetMostRecentByTypeAsync(Collection collectionType, int count = 8);
    Task<List<BaseObject>> GetAllAsync();
    Task<BaseObject> GetByIdAsync(string id);
}