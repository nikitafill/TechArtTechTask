using System.Collections.Generic;
using System.Linq;
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

        public async Task<JArray> GetAdsInPolygon(List<(double Latitude, double Longitude)> polygonPoints)
        {
            if (polygonPoints.Count < 3)
            {
                throw new ArgumentException("At least three points are required to form a polygon.");
            }

            var ads = await GetAds();
            var result = new JArray();

            foreach (var ad in ads)
            {
                var adParameters = ad["ad_parameters"] as JArray;
                double? latitude = null;
                double? longitude = null;

                foreach (var parameter in adParameters)
                {
                    if (parameter["p"].ToString() == "coordinates")
                    {
                        var coordinates = parameter["v"] as JArray;
                        if (coordinates != null && coordinates.Count >= 2)
                        {
                            longitude = (double)coordinates[0];
                            latitude = (double)coordinates[1];
                        }
                        break; 
                    }
                }

                if (latitude.HasValue && longitude.HasValue)
                {
                    result.Add(ad);
                }
            }
            return result;
        }
        //Задание 3 до добавление 
        /*public async Task<JArray> GetRentAdsWithOnlineBooking(string district)
        {
            var response = await _httpClient.GetStringAsync("https://api.kufar.by/search-api/v2/search/rendered-paginated?cat=1010&cur=USD&gtsy=country-belarus~province-minsk~locality-minsk&lang=ru&rnt=2&size=30&typ=let");
            var json = JObject.Parse(response);
            var ads = (JArray)json["ads"];

            var filteredAds = ads
                .Where(ad =>
                    
                    ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "area")?["vl"]?.ToString()?.Contains(district, StringComparison.OrdinalIgnoreCase) == true
                    
                    && ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "booking_enabled")?["v"]?.ToObject<bool>() == true)
                .OrderBy(ad => (double?)ad["price_byn"])  
                .ToList();

            return new JArray(filteredAds); 
        }*/

        public async Task<JArray> GetRentAdsWithOnlineBooking(string district, DateTime? checkInDate, DateTime? checkOutDate)
        {
            var response = await _httpClient.GetStringAsync("https://api.kufar.by/search-api/v2/search/rendered-paginated?cat=1010&cur=USD&gtsy=country-belarus~province-minsk~locality-minsk&lang=ru&rnt=2&size=30&typ=let");
            var json = JObject.Parse(response);
            var ads = (JArray)json["ads"];

            var filteredAds = ads
                .Where(ad =>
                    ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "area")?["vl"]?.ToString()?.Contains(district, StringComparison.OrdinalIgnoreCase) == true
                    && ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "booking_enabled")?["v"]?.ToObject<bool>() == true
                    && IsAvailableForDates(ad, checkInDate, checkOutDate)
                )
                .OrderBy(ad => (double?)ad["price_byn"]) 
                .ToList();

            return new JArray(filteredAds);  
        }
        private bool IsAvailableForDates(JToken ad, DateTime? checkInDate, DateTime? checkOutDate)
        {
            if (checkInDate == null || checkOutDate == null)
                return true; 

            var bookingCalendar = ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "booking_calendar")?["v"]?.ToObject<List<int>>();
            if (bookingCalendar == null || !bookingCalendar.Any())
                return true; 

            var unavailableDates = bookingCalendar.Select(d => DateTimeOffset.FromUnixTimeSeconds(d).DateTime).ToList();

            return !unavailableDates.Any(date => date >= checkInDate && date < checkOutDate);
        }
    }
}
