using WebApiMongoPOC.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;

namespace WebApiMongoPOC.Services.Impl;

public class MongoDBService : IMongoDBService
{
    private readonly IMongoCollection<PlayList> _playListsCollection;

    public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
    {
        MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
        IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _playListsCollection = database.GetCollection<PlayList>(mongoDBSettings.Value.CollectionName);
    }

    public async Task CreatePlayListAsync(PlayList playList)
    {
        await _playListsCollection.InsertOneAsync(playList);
        return;
    }

    public async Task<List<PlayList>> GetPlayListsAsync()
    {
        return await _playListsCollection.Find(new BsonDocument()).ToListAsync();
    }

    public async Task UpdatePlayListAsync(PlayList playList)
    {
        FilterDefinition<PlayList> filter = Builders<PlayList>.Filter.Eq("Id", playList.Id);
        UpdateDefinition<PlayList> update = Builders<PlayList>.Update.AddToSetEach("movies", playList.movies);
        await _playListsCollection.UpdateOneAsync(filter, update);
        return;
    }

    public async Task DeleteAsync(string id)
    {
        FilterDefinition<PlayList> filter = Builders<PlayList>.Filter.Eq("Id", id);
        await _playListsCollection.DeleteOneAsync(filter);
        return;
    }
}