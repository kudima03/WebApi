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
        public static string FileName { get; set; }

        private static async Task SaveAllAsync(List<BookCard> bookCards)
        {
            try
            {
                await File.WriteAllTextAsync(FileName, JsonSerializer.Serialize(bookCards));
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error while writing to the file has occured " + ex.Message);
                throw ex;
            }
        }

        public static async Task CreateAsync(BookCard bookCard)
        {
            var buffer = await ReadAllAsync();
            bookCard.Id = buffer.Max((item) => item.Id) + 1;
            buffer.Add(bookCard);
            await SaveAllAsync(buffer);
        }

        public static async Task<List<BookCard>> ReadAllAsync()
        {
            try
            {
                return JsonSerializer.Deserialize<List<BookCard>>(await File.ReadAllTextAsync(FileName));
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error while reading from the file has occured " + ex.Message);
                throw ex;
            }
        }

        public static async Task UpdateAsync(BookCard newVersion)
        {
            var buffer = await ReadAllAsync();
            var oldVersionIndex = buffer.FindIndex(obj => obj.Id == newVersion.Id);
            buffer[oldVersionIndex] = newVersion ?? throw new InvalidDataException();
            await SaveAllAsync(buffer);
        }

        public static async Task DeleteAsync(int bookCardId)
        {
            var cards = await ReadAllAsync();
            var objToDelete = cards.Find(obj => obj.Id == bookCardId);
            cards.Remove(objToDelete ?? throw new InvalidDataException());
            await SaveAllAsync(cards);
        }

        public static async Task DeleteRangeAsync(int[] idArray)
        {
            var cards = await ReadAllAsync();
            var deletedAmount = cards.RemoveAll((item) => idArray.Contains(item.Id));
            if (deletedAmount == 0) throw new InvalidDataException();
            await SaveAllAsync(cards);
        }
    }
}
