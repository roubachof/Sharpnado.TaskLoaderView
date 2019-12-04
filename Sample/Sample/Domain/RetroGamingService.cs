using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IGDB;

namespace Sample.Domain
{
    public class RetroGamingService : IRetroGamingService
    {
        private const int AtariPlatformId = 63;

        private const int AmigaPlatformId = 16;

        private const int NesPlatformId = 18;

        private const int SmsPlatformId = 64;

        private const string GamesRequest = "fields name, genres.name, first_release_date, cover.image_id, involved_companies.company.name, rating;"
                                            + "sort popularity desc;"
                                            + "where platforms = [{0}, {1}] & id >= {2} & id < {3};" + "limit 20;";

        private readonly IGDBApi _igdbClient;

        public RetroGamingService()
        {
            _igdbClient = Client.Create("b4f8877672fb840873c1bf1b5fe611fb");
        }

        public async Task<List<Game>> GetAtariAndAmigaGames()
        {
            var bounds = GetIdBounds();
            var igdbModels = await _igdbClient.QueryAsync<IGDB.Models.Game>(
                                IGDB.Client.Endpoints.Games,
                                query: string.Format(
                                    GamesRequest,
                                    AtariPlatformId,
                                    AmigaPlatformId,
                                    bounds.LowerId,
                                    bounds.HigherId))
                                 .ConfigureAwait(false);

            return igdbModels.Select(ToDomainEntity).ToList();
        }

        public async Task<List<Game>> GetNesAndSmsGames()
        {
            var bounds = GetIdBounds();
            var igdbModels = await _igdbClient.QueryAsync<IGDB.Models.Game>(
                                IGDB.Client.Endpoints.Games,
                                query: string.Format(
                                    GamesRequest,
                                    NesPlatformId,
                                    SmsPlatformId,
                                    bounds.LowerId,
                                    bounds.HigherId))
                                 .ConfigureAwait(false);

            return igdbModels.Select(ToDomainEntity).ToList();
        }

        private (int LowerId, int HigherId) GetIdBounds()
        {
            int lowerId = DateTime.Now.DayOfYear % 10 * 1000;
            int higherId = lowerId + 1000;
            return (lowerId, higherId);
        }

        private static Game ToDomainEntity(IGDB.Models.Game igdbModel)
        {
            string coverUrl = ImageHelper.GetImageUrl(imageId: igdbModel.Cover.Value.ImageId, size: ImageSize.CoverBig, retina: true);

            return new Game(
                igdbModel.Id.Value,
                coverUrl,
                igdbModel.FirstReleaseDate.Value.DateTime,
                igdbModel.Genres.Values.Select(g => new Genre(g.Id.Value, g.Name)).ToList(),
                igdbModel.InvolvedCompanies.Values
                    .Select(ic => new Company(ic.Company.Value.Id.Value, ic.Company.Value.Name)).ToList(),
                igdbModel.Name,
                igdbModel.Rating);
        }
    }
}
