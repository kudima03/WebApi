using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BooksAPI.ImageInfrastructure
{
    public class ImageManager
    {
        private readonly string _imageFolder;

        private readonly string _defaultBookImageFileName;

        public ImageManager(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _imageFolder = webHostEnvironment.WebRootPath;
            _defaultBookImageFileName = configuration.GetValue<string>("DefaultPictureFileName");
        }

        /// <returns>Returns created image filename</returns>
        public async Task<string> CreateBookImageAsync(int bookId, IFormFile? formFile)
        {
            if (formFile == null) return null;
            var extension = MimeTypeToImageExtension(formFile.ContentType);
            var fileName = bookId + extension;
            using (var fileStream = new FileStream(Path.Combine(_imageFolder, fileName), FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }
            return fileName;
        }

        /// <returns>Returns created image filename</returns>
        public async Task<string> CreateBookImageAsync(int bookId, byte[] image, string extension)
        {
            var fileName = bookId + extension;
            await File.WriteAllBytesAsync(Path.Combine(_imageFolder, fileName), image);
            return fileName;
        }

        /// <returns>Returns updated image filename</returns>
        public async Task<string> UpdateBookImageAsync(int bookId, IFormFile? formFile)
        {
            DeleteBookImage(bookId);
            return await CreateBookImageAsync(bookId, formFile);
        }

        /// <returns>Returns updated image filename</returns>
        public async Task<string> UpdateBookImageAsync(int bookId, byte[] image, string extension)
        {
            DeleteBookImage(bookId);
            return await CreateBookImageAsync(bookId, image, extension);
        }

        public void DeleteBookImage(int bookId)
        {
            var fileNames = Directory.EnumerateFiles(_imageFolder).Select(Path.GetFileName);
            var filename = fileNames.SingleOrDefault(x => x.Substring(0, x.IndexOf('.')).Equals(bookId.ToString()));
            if (filename == null) return;
            var path = Path.Combine(_imageFolder, filename);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        ///<returns>Returns MimeType as key, file content as value</returns>
        public async Task<KeyValuePair<string, byte[]>> GetImageAsync(int bookId)
        {
            var fileNames = Directory.EnumerateFiles(_imageFolder).Select(Path.GetFileName);
            var filename = fileNames.SingleOrDefault(x => x.Substring(0, x.IndexOf('.')).Equals(bookId.ToString()));
            if (filename == null)
            {
                filename = _defaultBookImageFileName;
            }
            var content = await File.ReadAllBytesAsync(Path.Combine(_imageFolder, filename));
            var fileMimeType = FileExtensionToMimeType('.' + filename.Split('.').Last());
            return new KeyValuePair<string, byte[]>(fileMimeType, content);
        }

        public string MimeTypeToImageExtension(string mimeType)
        {
            string extension = mimeType switch
            {
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/jpeg" => ".jpg",
                "image/bmp" => ".bmp",
                "image/tiff" => ".tiff",
                "image/wmf" => ".wmf",
                "image/jp2" => ".jp2",
                "image/svg+xml" => ".svg",
                _ => "",
            };
            return extension;
        }

        private string FileExtensionToMimeType(string extension)
        {
            string mimetype = extension switch
            {
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".bmp" => "image/bmp",
                ".tiff" => "image/tiff",
                ".wmf" => "image/wmf",
                ".jp2" => "image/jp2",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream",
            };
            return mimetype;
        }
    }
}
