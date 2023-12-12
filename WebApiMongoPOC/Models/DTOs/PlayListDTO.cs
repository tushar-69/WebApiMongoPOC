using System.ComponentModel.DataAnnotations;

namespace WebApiMongoPOC.Models.DTOs
{
    public class PlayListDTO
    {
        public string? Id { get; set; }

        public string name { get; set; }

        public List<string> movies { get; set; }
    }
}
