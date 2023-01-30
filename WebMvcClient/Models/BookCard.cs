using System.Text.Json.Serialization;

namespace WebMvcClient.Models
{
    [JsonSerializable(typeof(BookCard))]
    public class BookCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public byte[] BinaryPhoto { get; set; }

        public BookCard()
        {

        }

        public BookCard(string name, byte[] binaryPhoto)
        {
            Name = name;
            BinaryPhoto = BinaryPhoto;
        }
    }
}
