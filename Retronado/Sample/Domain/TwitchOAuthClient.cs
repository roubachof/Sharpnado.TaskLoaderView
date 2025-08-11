using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using IGDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;
using Sample.Infrastructure;

namespace Sample.Domain
{
    [Refit.Headers("Accept", "application/json")]
    public interface TwitchOAuthAPI
    {
        [Refit.Post("/oauth2/token")]
        Task<TwitchAccessToken> GetOAuth2Token([Refit.Body(BodySerializationMethod.UrlEncoded)] IDictionary<string, string> data);
    }

    public class TwitchOAuthClient
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

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly TwitchOAuthAPI _api;

        public TwitchOAuthClient(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;

            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://id.twitch.tv"),
            };

            _api = RestService.For<TwitchOAuthAPI>(
                httpClient,
                new RefitSettings
                {
                    ContentSerializer = new NewtonsoftJsonContentSerializer(JsonSerializationConfig),
                });
        }

        public Task<TwitchAccessToken> GetClientCredentialTokenAsync()
        {
            return _api.GetOAuth2Token(
                new Dictionary<string, string>()
                {
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                    { "grant_type", "client_credentials" },
                });
        }
    }
}