using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media.Imaging;

namespace CommonEntities
{
    public class BookCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] BinaryPhoto;

        [JsonIgnore]
        public Bitmap BitmapPhoto
        {
            set
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(ms, value);
                    BinaryPhoto = ms.ToArray();
                }
            }
            get
            {
                using (MemoryStream ms = new MemoryStream(BinaryPhoto))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    return binaryFormatter.Deserialize(ms) as Bitmap;
                }
            }
        }

        [JsonIgnore]
        public BitmapImage BitmapImagePhoto
        {
            get
            {
                BitmapImage image = new BitmapImage();
                MemoryStream ms = new MemoryStream();
                BitmapPhoto.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                image.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }


        public BookCard()
        {
            Name = "placeholder";
            BinaryPhoto = new byte[1];
        }
        public BookCard(string name, Bitmap image) : this()
        {
            Name = name;
            BitmapPhoto = image;
        }
    }
}
