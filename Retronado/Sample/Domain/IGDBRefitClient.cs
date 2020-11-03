using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using IGDB;
using IGDB.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Refit;

using Sample.Infrastructure;

namespace Sample.Domain
{
    public interface IGDBApiRefit
    {
        /// <summary>
        /// Queries a standard IGDB endpoint with an APIcalypse query. See endpoints in <see cref="IGDBRestClient.Endpoints" />.
        /// </summary>
        /// <param name="endpoint">The IGDB endpoint name to query, see <see cref="IGDBRestClient.Endpoints" /></param>
        /// <param name="query">The APIcalypse query to send</param>
        /// <typeparam name="T">The IGDB.Models.* entity to deserialize the response for.</typeparam>
        /// <returns>Array of IGDB models of the specified type</returns>
        [Post("/{endpoint}")]
        Task<T[]> QueryAsync<T>(
            string endpoint,
            [Header("Authorization")] string authToken,
            [Header("Client-ID")] string clientId,
            [Body] string query = null);

        /// <summary>
        /// Returns your API key status with usage information
        /// </summary>
        /// <returns></returns>
        [Get("/api_status")]
        Task<ApiStatus> GetApiStatus();
    }

    internal class TokenManager
    {
        private readonly TwitchOAuthClient _twitchClient;
        private readonly ITokenStore _tokenStore;

        public TokenManager(ITokenStore tokenStore, TwitchOAuthClient twitchClient)
        {
            _tokenStore = tokenStore;
            _twitchClient = twitchClient;
        }

        public async Task<TwitchAccessToken> AcquireTokenAsync()
        {
            var currentToken = await _tokenStore.GetTokenAsync();
            if (currentToken?.HasTokenExpired() == false)
            {
                return currentToken;
            }

            return await RefreshTokenAsync();
        }

        public async Task<TwitchAccessToken> RefreshTokenAsync()
        {
            var accessToken = await _twitchClient.GetClientCredentialTokenAsync();
            accessToken.TokenAcquiredAt = DateTimeOffset.UtcNow;
            var storedToken = await _tokenStore.StoreTokenAsync(accessToken);

            return storedToken;
        }
    }

    public class IGDBRestClient
    {
        private readonly IGDBApiRefit _refitService;

        private readonly TokenManager _tokenManager;

        private string _token;

        public IGDBRestClient(IGDBApiRefit refitService)
        {
            _refitService = refitService;
            string pipoId = "ebc5enrq0r5s2u7vain16detklybhj";
            string superPipo = "ld17xxwvga5atlcjtf3crv44t4xxp7";

            _tokenManager = new TokenManager(new InMemoryTokenStore(), new TwitchOAuthClient(pipoId, superPipo));
        }

        public async Task<T[]> QueryAsync<T>(
            string endpoint,
            [Body] string query = null)
        {
            if (string.IsNullOrEmpty(_token))
            {
                await GetTokenAsync();
            }

            return await _refitService.QueryAsync<T>(endpoint, $"Bearer {_token}", "ebc5enrq0r5s2u7vain16detklybhj", query);
        }

        private async Task GetTokenAsync()
        {
            var twitchToken = await _tokenManager.AcquireTokenAsync();

            if (twitchToken?.AccessToken != null)
            {
                 _token = twitchToken.AccessToken;
            }
        }

        public static class Endpoints
        {
            public const string AgeRating = "age_ratings";

            public const string AgeRatingContentDescriptions = "age_rating_content_descriptions";

            public const string AlternativeNames = "alternative_names";

            public const string Artworks = "artworks";

            public const string Characters = "characters";

            public const string CharacterMugShots = "character_mug_shots";

            public const string Collections = "collections";

            public const string Companies = "companies";

            public const string CompanyWebsites = "company_websites";

            public const string Covers = "covers";

            public const string ExternalGames = "external_games";

            public const string Franchies = "franchises";

            public const string Games = "games";

            public const string GameEngines = "game_engines";

            public const string GameEngineLogos = "game_engine_logos";

            public const string GameVersions = "game_versions";

            public const string GameModes = "game_modes";

            public const string GameVersionFeatures = "game_version_features";

            public const string GameVersionFeatureValues = "game_version_feature_values";

            public const string GameVideos = "game_videos";

            public const string Genres = "genres";

            public const string InvolvedCompanies = "involved_companies";

            public const string Keywords = "keywords";

            public const string MultiplayerModes = "multiplayer_modes";

            public const string Platforms = "platforms";

            public const string PlatformFamilies = "platform_families";

            public const string PlatformLogos = "platform_logos";

            public const string PlatformVersions = "platform_versions";

            public const string PlatformVersionCompanies = "platform_version_companies";

            public const string PlatformVersionReleaseDates = "platform_version_release_dates";

            public const string PlatformWebsites = "platform_websites";

            public const string PlayerPerspectives = "player_perspectives";

            public const string ReleaseDates = "release_dates";

            public const string Screenshots = "screenshots";

            public const string Search = "search";

            public const string Themes = "themes";

            public const string Websites = "websites";
        }
    }

    public static class RefitClient
    {
        public static JsonSerializerSettings JsonSerializationConfig =>
            new JsonSerializerSettings
                {
                    Converters =
                        new List<JsonConverter>() { new IdentityConverter(), new UnixTimestampConverter(), },
                    ContractResolver = new DefaultContractResolver()
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy(),
                        },
                };

        /// <summary>
        /// Create a default IGDB API client with specified API key.
        /// </summary>
        /// <returns></returns>
        public static IGDBRestClient Create()
        {
            // For logging
            // var httpClient = new HttpClient(new HttpLoggingHandler())
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.igdb.com/v4"),
            };

            return new IGDBRestClient(
                RestService.For<IGDBApiRefit>(
                    httpClient,
                    new RefitSettings
                    {
                        ContentSerializer = new JsonContentSerializer(JsonSerializationConfig),
                    }));
        }
    }
}