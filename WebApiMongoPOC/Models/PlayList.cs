using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace WebApiMongoPOC.Models;

public class PlayList
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [MinLength(3)]
    public string name { get; set; }

    [MinLength(1)]
    public List<string> movies { get; set; }
}