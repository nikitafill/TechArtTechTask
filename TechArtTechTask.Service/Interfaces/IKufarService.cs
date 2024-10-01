using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechArtTechTask.Service.Interfaces
{
    public interface IKufarService
    {
        Task<JArray> GetAds();
        Task<JArray> GetAdsInPolygon(List<(double Latitude, double Longitude)> polygonPoints);
        //Task<JArray> GetRentAdsWithOnlineBooking(string district); 
        Task<JArray> GetRentAdsWithOnlineBooking(string district, DateTime? checkInDate, DateTime? checkOutDate);
    }
}
