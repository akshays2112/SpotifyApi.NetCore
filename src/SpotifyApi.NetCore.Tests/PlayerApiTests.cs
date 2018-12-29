using System;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using SpotifyApi.NetCore.Tests.Integration;
using SpotifyApi.NetCore.Tests.Mocks;

namespace SpotifyApi.NetCore.Tests
{
    [TestClass]
    public class PlayerApiTests
    {
        [TestMethod]
        public async Task PlayTracks_AccessToken_PutInvokedWithAccessToken()
        {
            // arrange
            const string token = "abc123";
            const string trackUri = "spotify:track:7ouMYWpwJ422jRcDASZB7P";

            var http = new MockHttpClient();
            http.SetupSendAsync();
            var service = new Mock<PlayerApi>(http.HttpClient, token){CallBase = true};

            // act
            await service.Object.PlayTracks(trackUri, token);

            // assert
            service.Verify(s => s.Put(It.IsAny<string>(), It.IsAny<object>(), token));
        }

        [TestMethod]
        public async Task PlayAlbum_AccessToken_PutInvokedWithAccessToken()
        {
            // arrange
            const string token = "abc123";
            const string uri = "spotify:album:FooBar1234567";

            var http = new MockHttpClient();
            http.SetupSendAsync();
            var service = new Mock<PlayerApi>(http.HttpClient, token) { CallBase = true };

            // act
            await service.Object.PlayAlbum(uri, token);

            // assert
            service.Verify(s => s.Put(It.IsAny<string>(), It.IsAny<object>(), token));
        }

        [TestMethod]
        public async Task PlayPlaylist_AccessToken_PutInvokedWithAccessToken()
        {
            // arrange
            const string token = "abc123";
            const string uri = "spotify:playlist:FooBar1234567";

            var http = new MockHttpClient();
            http.SetupSendAsync();
            var service = new Mock<PlayerApi>(http.HttpClient, token) { CallBase = true };

            // act
            await service.Object.PlayPlaylist(uri, token);

            // assert
            service.Verify(s => s.Put(It.IsAny<string>(), It.IsAny<object>(), token));
        }

        [TestMethod]
        public async Task Play_AccessToken_PutInvokedWithAccessToken()
        {
            // arrange
            const string token = "abc123";

            var http = new MockHttpClient();
            http.SetupSendAsync();
            var service = new Mock<PlayerApi>(http.HttpClient, token) { CallBase = true };

            // act
            await service.Object.Play(token);

            // assert
            service.Verify(s => s.Put(It.IsAny<string>(), It.IsAny<object>(), token));
        }

        [TestMethod]
        public async Task GetDevices_AccessToken_PutInvokedWithAccessToken()
        {
            // arrange
            const string token = "abc123";
            string json = JsonConvert.SerializeObject(new { devices = new Device[] { new Device() } });
            var http = new MockHttpClient();
            http.SetupSendAsync(json);
            var service = new Mock<PlayerApi>(http.HttpClient, token) { CallBase = true };

            // act
            await service.Object.GetDevices<dynamic>(token);

            // assert
            service.Verify(s => s.GetModelFromProperty<dynamic>(It.IsAny<string>(), It.IsAny<string>(), token));
        }

        [TestMethod]
        public async Task GetCurrentPlaybackInfo_AccessToken_GetModelInvokedWithAccessToken()
        {
            // arrange
            const string token = "abc123";
            string json = JsonConvert.SerializeObject(new CurrentPlaybackContext());
            var http = new MockHttpClient();
            http.SetupSendAsync(json);
            var service = new Mock<PlayerApi>(http.HttpClient, token) { CallBase = true };

            // act
            await service.Object.GetCurrentPlaybackInfo(token);

            // assert
            service.Verify(s => s.GetModel<CurrentPlaybackContext>(It.IsAny<string>(), token));
        }

        //TODO: test GetDevices

        //TODO: test GetPlaybackInfo

        //[TestMethod] //TODO: Result changes if device online/offline
        [TestCategory("Integration")]
        public async Task PlayContext_SpotifyUri_SpotifyApiErrorException()
        {
            // arrange
            const string userHash = "E11AC28538A7C0A827A726DD9B30B710FC1FCAFFFE2E86FCA853AB90E7C710D2";
            const string spotifyUri = "spotify:user:palsvensson:playlist:2iL5fr6OmN8f4yoQvvuWSf";

            var http = new HttpClient();
            var config = TestsHelper.GetLocalConfig();

            var accounts = new UserAccountsService(http, config, new MockRefreshTokenStore(userHash).Object, null);
            var api = new PlayerApi(http, accounts);

            // act
            //try
            //{
                await api.PlayContext(userHash, spotifyUri);
            //}
            //catch (SpotifyApiErrorException ex)
            //{
                //Trace.WriteLine(ex.Message);
                
            //}

            // assert
        }

        //[TestMethod]
        public async Task Play_UserToken_HowDoesThisWork()
        {
            // arrange
            var tokens = new BearerTokenStore();

            // act
            var mockPlayerApi = new Mock<IPlayerApi>();

            await mockPlayerApi.Object.PlayAlbum("albumid", accessToken: tokens.GetToken("userId"));
            await mockPlayerApi.Object.PlayAlbum("albumid", deviceId: "deviceId");
            await mockPlayerApi.Object.PlayAlbumOffset("albumid", 1);
            await mockPlayerApi.Object.PlayAlbumOffset("albumid", 1, positionMs: 10000);
            await mockPlayerApi.Object.PlayAlbumOffset("albumid", "trackId");
            await mockPlayerApi.Object.PlayAlbumOffset("albumid", "trackId", positionMs: 10000);
        }

    }

    class BearerTokenStore
    {
        public string GetToken(string user)
        {
            return "foo";
        }
    }
}