using WebApiMongoPOC.Models;

namespace WebApiMongoPOC.Services
{
    public interface IMongoDBService
    {
        Task CreatePlayListAsync(PlayList playList);

        Task<List<PlayList>> GetPlayListsAsync();

        Task UpdatePlayListAsync(PlayList playList);

        Task DeleteAsync(string id);
    }
}
