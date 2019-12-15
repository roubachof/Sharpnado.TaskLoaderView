using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Domain
{
    public enum GamePlatform
    {
        Computer = 0,
        Console,
    }

    public interface IRetroGamingService
    {
        Task<List<Game>> GetAtariAndAmigaGames();

        Task<List<Game>> GetNesAndSmsGames();

        Task<Game> GetRandomGame();
    }
}