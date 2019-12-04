using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Domain
{
    public interface IRetroGamingService
    {
        Task<List<Game>> GetAtariAndAmigaGames();

        Task<List<Game>> GetNesAndSmsGames();
    }
}