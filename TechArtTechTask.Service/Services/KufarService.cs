using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TechArtTechTask.Service.Interfaces;

namespace TechArtTechTask.Service.Services
{
    public class KufarService : IKufarService
    {
        private readonly HttpClient _httpClient;

        public KufarService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JArray> GetAds()
        {
            var response = await _httpClient.GetStringAsync("https://api.kufar.by/search-api/v2/search/rendered-paginated?cat=1010&cur=USD&gtsy=country-belarus~province-minsk~locality-minsk&lang=ru&size=30&typ=sell");
            var json = JObject.Parse(response);
            return (JArray)json["ads"];
        }
    }
}
