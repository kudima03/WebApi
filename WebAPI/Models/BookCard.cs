namespace WebAPI.Models
{
    public class BookCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] BinaryPhoto { get; set; }
        public BookCard()
        {
            Name = "placeholder";
            BinaryPhoto = new byte[1];
        }
        public BookCard(string name, byte[] binaryPhoto) : this()
        {
            Name = name;
            BinaryPhoto = BinaryPhoto;
        }
    }
}
