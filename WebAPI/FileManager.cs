using CommonEntities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPI
{
    public static class FileManager
    {
        public static string FileName { get; set; }

        private static async Task<bool> SaveAllAsync(List<BookCard> bookCards)
        {
            try
            {
                using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    fs.SetLength(0);
                    await JsonSerializer.SerializeAsync(fs, bookCards);
                }
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("An error while writing to the file has occured");
                return false;
            }
        }

        public static async Task<bool> CreateAsync(BookCard bookCard)
        {
            var buffer = await ReadAllAsync();
            bookCard.Id = buffer.Count + 1;
            buffer.Add(bookCard);
            return await SaveAllAsync(buffer);
        }

        public static async Task<List<BookCard>> ReadAllAsync()
        {
            try
            {
                List<BookCard> list = null;
                var sb = new StringBuilder();
                using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    byte[] buffer = new byte[4096];
                    int numRead;
                    while ((numRead = await fs.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        string text = Encoding.UTF8.GetString(buffer, 0, numRead);
                        sb.Append(text);
                    }
                }
                list = JsonSerializer.Deserialize<List<BookCard>>(sb.ToString());
                return list;
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine(ex.Message);
                return new List<BookCard>();
            }
            catch (Exception)
            {
                Console.WriteLine("An error while reading from the file has occured");
                return new List<BookCard>();
            }
        }

        public static async Task<bool> UpdateAsync(BookCard newVersion)
        {
            var buffer = await ReadAllAsync();
            var oldVersionIndex = buffer.FindIndex(obj => obj.Id == newVersion.Id);
            if (oldVersionIndex != -1)
            {
                buffer[oldVersionIndex] = newVersion;
                return await SaveAllAsync(buffer);
            }
            else
            {
                return false;
            }

        }

        public static async Task<bool> DeleteAsync(int bookCardId)
        {
            var cards = await ReadAllAsync();
            var objToDelete = cards.Find(obj => obj.Id == bookCardId);
            if (objToDelete != null)
            {
                cards.Remove(objToDelete);
                await SaveAllAsync(cards);
                return true;
            }
            else
            {
                return false;
            }

        }

        public static async Task<bool> DeleteRangeAsync(int[] idArray)
        {
            var cards = await ReadAllAsync();
            foreach (var bookCardId in idArray)
            {
                var objToDelete = cards.Find(obj => obj.Id == bookCardId);
                if (objToDelete != null)
                {
                    cards.Remove(objToDelete);
                }
                else
                {
                    return false;
                }
            }
            await SaveAllAsync(cards);
            return true;
        }
    }
}
