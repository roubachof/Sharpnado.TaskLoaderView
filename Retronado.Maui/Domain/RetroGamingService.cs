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
        private const int MinId = 1;
        private const int MaxId = 99999999;

        private const int AtariPlatformId = 63;

        private const int AmigaPlatformId = 16;

        private const int NesPlatformId = 18;

        private const int MegadrivePlatformId = 29;

        private const int GameBoyPlatformId = 33;

        private const int Atari2600PlatformId = 59;

        private const int SmsPlatformId = 64;

        //private const string GamesRequest = "fields name, genres.name, summary, storyline, first_release_date, cover.image_id, involved_companies.company.name, rating, screenshots.image_id;"
        //                                    //+ "sort rating desc;"
        //                                    + "sort aggregated_rating desc;"
        //                                    + "where platforms = ({0}) & id >= {1} & id < {2} & cover.image_id != null & screenshots.image_id != null;" + "limit {3};";

        private const string GamesRequest = "fields name, genres.name, summary, storyline, first_release_date, cover.image_id, involved_companies.company.name, rating, screenshots.image_id;"
            + "sort rating desc;"
            + "where rating != null & platforms = ({0}) & cover.image_id != null & screenshots.image_id != null;" + "limit {1};";

        private readonly ErrorEmulator _errorEmulator;

        private readonly IGDBRestClient _igdbClient;

        private readonly Random _randomizer = new Random();

        public RetroGamingService(ErrorEmulator errorEmulator)
        {
            _igdbClient = RefitClient.Create();
            _errorEmulator = errorEmulator;
        }

        public async Task<Game> GetRandomGame(bool mostPop = false)
        {
            var igdbModels = await _igdbClient.QueryAsync<IGDB.Models.Game>(
                    IGDBRestClient.Endpoints.Games,
                    GetStringRequest(
                        new[]
                        {
                            AtariPlatformId, // AmigaPlatformId,
                        }))
                .ConfigureAwait(false);

            var result = igdbModels.Select(ToDomainEntity).ToList();

            return result[_randomizer.Next(0, result.Count)];
        }

        private string GetStringRequest(int[] platforms, int lowerId = MinId, int higherId = MaxId, int limit = 20)
        {
            return string.Format(
                GamesRequest,
                string.Join(",", platforms),
                limit,
                higherId,
                limit);
        }

        public async Task<List<Game>> GetAtariAndAmigaGames(bool mostPop = false)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var bounds = mostPop ? (LowerId: MinId, HigherId: MaxId) : GetIdBounds();
            var igdbModels = await _igdbClient.QueryAsync<IGDB.Models.Game>(
                    IGDBRestClient.Endpoints.Games,
                    GetStringRequest(new[] { AtariPlatformId, AmigaPlatformId }, bounds.LowerId, bounds.HigherId))
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

        public async Task<List<Game>> GetNesAndSmsGames(bool mostPop = false)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var bounds = mostPop ? (LowerId: MinId, HigherId : MaxId) : GetIdBounds();
            var igdbModels = await _igdbClient.QueryAsync<IGDB.Models.Game>(
                    IGDBRestClient.Endpoints.Games,
                    GetStringRequest(
                        new[]
                        {
                            NesPlatformId, SmsPlatformId, MegadrivePlatformId, GameBoyPlatformId, Atari2600PlatformId,
                        },
                        bounds.LowerId,
                        bounds.HigherId))
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

            DateTime firstRelease = igdbModel.FirstReleaseDate?.DateTime ?? DateTime.MinValue;

            return new Game(
                igdbModel.Id.Value,
                coverUrl,
                screenshotUrl,
                firstRelease,
                genres,
                involvedCompanies,
                igdbModel.Name,
                igdbModel.Rating,
                igdbModel.Summary,
                igdbModel.Storyline);
        }
    }
}
