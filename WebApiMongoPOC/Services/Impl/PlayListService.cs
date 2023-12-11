using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApiMongoPOC.Models;
using WebApiMongoPOC.Models.DTOs;

namespace WebApiMongoPOC.Services.Impl;

public class PlayListService : IPlayListService
{
    private readonly IMongoCollection<PlayList> _playListsCollection;
    private IMapper _mapper;

    public PlayListService(IOptions<MongoDBSettings> mongoDBSettings, IMapper mapper)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _playListsCollection = database.GetCollection<PlayList>(mongoDBSettings.Value.CollectionName);
        _mapper = mapper;
    }

    public async Task<string> CreatePlayListAsync(PlayList playList)
    {
        await _playListsCollection.InsertOneAsync(playList);
        return playList.Id;
    }

    public async Task<List<PlayListDTO>> GetPlayListsAsync()
    {
        var playLists = await _playListsCollection.Find(new BsonDocument()).ToListAsync();
        return _mapper.Map<List<PlayListDTO>>(playLists);
    }

    public async Task<PlayListDTO> GetPlayListByIdAsync(string id)
    {
        var filter = Builders<PlayList>.Filter.Eq(PlayList => PlayList.Id, id);
        var playList = await _playListsCollection.Find(filter).FirstOrDefaultAsync();
        if (playList == null) throw new InvalidOperationException("PlayList not found");
        return _mapper.Map<PlayListDTO>(playList);
    }

    public async Task UpdatePlayListAsync(PlayList playList)
    {
        FilterDefinition<PlayList> filter = Builders<PlayList>.Filter.Eq(PlayList => PlayList.Id, playList.Id);
        UpdateDefinition<PlayList> update = Builders<PlayList>.Update.AddToSetEach(PlayList => PlayList.movies, playList.movies);
        var response = await _playListsCollection.UpdateOneAsync(filter, update);
        if (!response.IsAcknowledged) throw new InvalidOperationException("Failed to update playlist");
        return;
    }

    public async Task DeleteAsync(string id)
    {
        FilterDefinition<PlayList> filter = Builders<PlayList>.Filter.Eq(PlayList => PlayList.Id, id);
        var response = await _playListsCollection.DeleteOneAsync(filter);
        if (!response.IsAcknowledged) throw new InvalidOperationException("Failed to delete playlist");
        return;
    }
}