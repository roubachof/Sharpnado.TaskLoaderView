using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using IGDB;
using Sample.Infrastructure;
using Sample.Services;

namespace Sample.Domain
{
    public class RetroGamingService : IRetroGamingService
    {
        private readonly ErrorEmulator _errorEmulator;
        private const int AtariPlatformId = 63;

        private const int AmigaPlatformId = 16;

        private const int NesPlatformId = 18;

        private const int MegadrivePlatformId = 29;

        private const int GameBoyPlatformId = 33;

        private const int Atari2600PlatformId = 59;

        private const int SmsPlatformId = 64;

        private const string GamesRequest = "fields name, genres.name, first_release_date, cover.image_id, involved_companies.company.name, rating, screenshots.image_id;"
                                            + "sort popularity desc;"
                                            + "where platforms = ({0}) & id >= {1} & id < {2};" + "limit {3};";

        private readonly IGDBRestClient _igdbClient;

        public RetroGamingService(ErrorEmulator errorEmulator)
        {
            _errorEmulator = errorEmulator;
            _igdbClient = RefitClient.Create("b4f8877672fb840873c1bf1b5fe611fb");
        }

        public async Task<Game> GetRandomGame()
        {
            List<Game> result = DateTime.UtcNow.Millisecond % 2 == 0
                                    ? await GetNesAndSmsGames()
                                    : await GetAtariAndAmigaGames();

            return result[0];
        }

        public async Task<List<Game>> GetAtariAndAmigaGames()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var bounds = GetIdBounds();
            var igdbModels = await _igdbClient.QueryAsync<IGDB.Models.Game>(
                                IGDB.Client.Endpoints.Games,
                                query: string.Format(
                                    GamesRequest,
                                    string.Join(",", new[] { AtariPlatformId, AmigaPlatformId }),
                                    bounds.LowerId,
                                    bounds.HigherId,
                                    20))
                                 .ConfigureAwait(false);

            var result = igdbModels.Select(ToDomainEntity).ToList();
            watch.Stop();
            var remainingWaitingTime = TimeSpan.FromSeconds(2) - watch.Elapsed;
            if (remainingWaitingTime > TimeSpan.Zero)
            {
                // Sometimes the api is too good x)
                await Task.Delay(remainingWaitingTime);
            }

            switch (_errorEmulator.ErrorType)
            {
                case ErrorType.Unknown:
                    throw new InvalidOperationException();

                case ErrorType.Network:
                    throw new NetworkException();

                case ErrorType.Server:
                    throw new ServerException();

                case ErrorType.NoData:
                    return new List<Game>();

                case ErrorType.ErrorOnRefresh:
                    if (DateTime.Now.Second % 2 == 0)
                    {
                        throw new NetworkException();
                    }

                    throw new ServerException();
            }

            return result;
        }

        public async Task<List<Game>> GetNesAndSmsGames()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var bounds = GetIdBounds();
            var igdbModels = await _igdbClient.QueryAsync<IGDB.Models.Game>(
                                IGDB.Client.Endpoints.Games,
                                query: string.Format(
                                    GamesRequest,
                                    string.Join(",", new[] { NesPlatformId, SmsPlatformId, MegadrivePlatformId, GameBoyPlatformId, Atari2600PlatformId }),
                                    bounds.LowerId,
                                    bounds.HigherId,
                                    20))
                                 .ConfigureAwait(false);

            var result = igdbModels.Select(ToDomainEntity).ToList();
            watch.Stop();
            var remainingWaitingTime = TimeSpan.FromSeconds(2) - watch.Elapsed;
            if (remainingWaitingTime > TimeSpan.Zero)
            {
                // Sometimes the api is too good x)
                await Task.Delay(remainingWaitingTime);
            }

            switch (_errorEmulator.ErrorType)
            {
                case ErrorType.Unknown:
                    throw new InvalidOperationException();

                case ErrorType.Network:
                    throw new NetworkException();

                case ErrorType.Server:
                    throw new ServerException();

                case ErrorType.NoData:
                    return new List<Game>();

                case ErrorType.ErrorOnRefresh:
                    if (DateTime.Now.Second % 2 == 0)
                    {
                        throw new NetworkException();
                    }

                    throw new ServerException();
            }

            return result;
        }

        private (int LowerId, int HigherId) GetIdBounds(int interval = 1000)
        {
            int lowerId = DateTime.Now.DayOfYear % 10 * interval;
            int higherId = lowerId + interval;
            return (lowerId, higherId);
        }

        private static Game ToDomainEntity(IGDB.Models.Game igdbModel)
        {
            string coverUrl = "https:" + ImageHelper.GetImageUrl(
                                  imageId: igdbModel.Cover.Value.ImageId,
                                  size: ImageSize.CoverBig,
                                  retina: true);

            string screenshotUrl = igdbModel.Screenshots == null || igdbModel.Screenshots.Values.Length == 0
                                   ? null
                                   : "https:" + ImageHelper.GetImageUrl(
                                          imageId: igdbModel.Screenshots.Values.First().ImageId,
                                          size: ImageSize.ScreenshotMed,
                                          retina: true);

            var genres = igdbModel.Genres == null
                             ? new List<Genre>()
                             : igdbModel.Genres.Values.Select(g => new Genre(g.Id.Value, g.Name))
                                 .ToList();

            var involvedCompanies = igdbModel.InvolvedCompanies == null
                                        ? new List<Company>()
                                        : igdbModel.InvolvedCompanies.Values.Select(
                                                ic => new Company(ic.Company.Value.Id.Value, ic.Company.Value.Name))
                                            .ToList();

            return new Game(
                igdbModel.Id.Value,
                coverUrl,
                screenshotUrl,
                igdbModel.FirstReleaseDate.Value.DateTime,
                genres,
                involvedCompanies,
                igdbModel.Name,
                igdbModel.Rating);
        }
    }
}
