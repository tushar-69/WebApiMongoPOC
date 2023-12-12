using WebApiMongoPOC.Models;
using WebApiMongoPOC.Models.DTOs;

namespace WebApiMongoPOC.Services
{
    public interface IPlayListService
    {
        Task<string> CreatePlayListAsync(PlayList playList);

        Task<List<PlayListDTO>> GetPlayListsAsync();

        Task<PlayListDTO> GetPlayListByIdAsync(string id);

        Task UpdatePlayListAsync(PlayList playList);

        Task DeleteAsync(string id);
    }
}
