using CommonEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPI
{
    public static class FileManager
    {
        public static string FileName
        {
            set
            {
                FileInfo fileInfo = new FileInfo(value);
                var fileStream = File.Open(value, FileMode.OpenOrCreate);
                _fileStreamReader = new StreamReader(fileStream);
                _fileStreamWriter = new StreamWriter(fileStream);
            }
        }

        private static StreamReader _fileStreamReader;

        private static StreamWriter _fileStreamWriter;

        private static async Task SaveAllAsync(List<BookCard> bookCards)
        {
            try
            {
                await _fileStreamWriter.WriteAsync(JsonSerializer.Serialize(bookCards));
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error while writing to the file has occured " + ex.Message);
                throw ex;
            }
        }

        public static async Task CreateAsync(BookCard bookCard)
        {
            var buffer = ReadAllAsync();
            bookCard.Id = buffer.Max((item) => item.Id) + 1;
            buffer.Add(bookCard);
            await SaveAllAsync(buffer);
        }

        public static List<BookCard> ReadAllAsync()
        {
            try
            {
                lock (_fileStreamReader)
                {
                    var list = JsonSerializer.Deserialize<List<BookCard>>(_fileStreamReader.ReadToEnd());
                    _fileStreamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                    return list;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error while reading from the file has occured " + ex.Message);
                throw ex;
            }
        }

        public static async Task UpdateAsync(BookCard newVersion)
        {
            var buffer = ReadAllAsync();
            var oldVersionIndex = buffer.FindIndex(obj => obj.Id == newVersion.Id);
            buffer[oldVersionIndex] = newVersion ?? throw new InvalidDataException();
            await SaveAllAsync(buffer);
        }

        public static async Task DeleteAsync(int bookCardId)
        {
            var cards = ReadAllAsync();
            var objToDelete = cards.Find(obj => obj.Id == bookCardId);
            cards.Remove(objToDelete ?? throw new InvalidDataException());
            await SaveAllAsync(cards);
        }

        public static async Task DeleteRangeAsync(int[] idArray)
        {
            var cards = ReadAllAsync();
            var deletedAmount = cards.RemoveAll((item) => idArray.Contains(item.Id));
            if (deletedAmount == 0) throw new InvalidDataException();
            await SaveAllAsync(cards);
        }
    }
}