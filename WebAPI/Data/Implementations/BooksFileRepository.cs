using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPI.Data.Interfaces;
using WebAPI.Models;

namespace WebAPI.Data.Implementations
{
    public class BooksFileRepository : IBooksRepository
    {
        private readonly string _fileName;

        private readonly ILogger<BooksFileRepository> _logger;

        private readonly IConfiguration _configuration;

        public BooksFileRepository(IConfiguration configuration, ILogger<BooksFileRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _fileName = configuration.GetValue<string>("FileName");
        }

        public async Task SaveAllAsync(IEnumerable<BookCard> bookCards)
        {
            try
            {
                await File.WriteAllTextAsync(_fileName, JsonSerializer.Serialize(bookCards));
            }
            catch (Exception ex)
            {
                _logger.LogError("An error while writing to the file has occured " + ex.Message);
                throw ex;
            }
        }

        public async Task CreateAsync(BookCard bookCard)
        {
            var buffer = (await GetAllAsync()).ToList();
            bookCard.Id = buffer.Max((item) => item.Id) + 1;
            buffer.Add(bookCard);
            await SaveAllAsync(buffer);
        }

        public async Task UpdateAsync(BookCard bookCard)
        {
            var buffer = (await GetAllAsync()).ToList();
            var oldVersionIndex = buffer.FindIndex(obj => obj.Id == bookCard.Id);
            buffer[oldVersionIndex] = bookCard ?? throw new InvalidDataException();
            await SaveAllAsync(buffer);
        }

        public async Task DeleteAsync(int bookCardId)
        {
            var cards = (await GetAllAsync()).ToList();
            var objToDelete = cards.Find(obj => obj.Id == bookCardId);
            cards.Remove(objToDelete ?? throw new InvalidDataException());
            await SaveAllAsync(cards);
        }

        public async Task<IEnumerable<BookCard>> GetAllAsync()
        {
            try
            {
                return JsonSerializer.Deserialize<List<BookCard>>(await File.ReadAllTextAsync(_fileName));
            }
            catch (Exception ex)
            {
                _logger.LogError("An error while reading from the file has occured " + ex.Message);
                throw ex;
            }
        }

        public async Task DeleteRangeAsync(int[] ids)
        {
            var cards = (await GetAllAsync()).ToList();
            var deletedAmount = cards.RemoveAll((item) => ids.Contains(item.Id));
            if (deletedAmount == 0) throw new InvalidDataException();
            await SaveAllAsync(cards);
        }
    }
}