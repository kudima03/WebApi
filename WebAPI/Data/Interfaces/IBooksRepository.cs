
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Data.Interfaces
{
    public interface IBooksRepository
    {
        Task SaveAllAsync(IEnumerable<BookCard> bookCards);       

        Task<int> CreateAsync(BookCard bookCard);

        Task UpdateAsync(BookCard bookCard);

        Task DeleteAsync(int bookCardId);

        Task<IEnumerable<BookCard>> GetAllAsync();

        Task DeleteRangeAsync(int[] ids);
    }
}
