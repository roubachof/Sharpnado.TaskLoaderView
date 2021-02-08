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
        Task<List<Game>> GetAtariAndAmigaGames(bool mostPop = false);

        Task<List<Game>> GetNesAndSmsGames(bool mostPop = false);

        Task<Game> GetRandomGame(bool mostPop = false);
    }
}