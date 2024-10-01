using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TechArtTechTask.Service.Interfaces;
using TechArtTechTask.Service.Models;
using TechArtTechTask.Service.Services;

[ApiController]
[Route("api/[controller]")]
public class KufarController : ControllerBase
{
    private readonly IKufarService _kufarService;

    public KufarController(IKufarService kufarService)
    {
        _kufarService = kufarService;
    }

    [HttpGet("price-by-floor")]
    public async Task<IActionResult> GetPriceByFloor()
    {
        var ads = await _kufarService.GetAds();

        var result = ads
            .Where(ad => ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "square_meter") != null
                         && ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "floor") != null)
            .GroupBy(ad => ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "floor")["v"]?.ToString())
            .Select(g => new
            {
                Floor = g.Key,
                AveragePricePerMeter = g.Average(ad => (double)ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "square_meter")["v"])
            });

        return Ok(result);
    }

    [HttpGet("price-by-rooms")]
    public async Task<IActionResult> GetPriceByRooms()
    {
        var ads = await _kufarService.GetAds();

        var result = ads
            .Where(ad => ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "square_meter") != null
                         && ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "rooms") != null)
            .GroupBy(ad => ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "rooms")["v"]?.ToString())
            .Select(g => new
            {
                Rooms = g.Key,
                AveragePricePerMeter = g.Average(ad => (double)ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "square_meter")["v"])
            });

        return Ok(result);
    }

    [HttpGet("price-by-metro")]
    public async Task<IActionResult> GetPriceByMetro()
    {
        var ads = await _kufarService.GetAds();

        var result = ads
            .Where(ad => ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "square_meter") != null
                         && ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "metro") != null)
            .GroupBy(ad => ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "metro")["vl"]?.First?.ToString())
            .Select(g => new
            {
                Metro = g.Key,
                AveragePricePerMeter = g.Average(ad => (double)ad["ad_parameters"]?.FirstOrDefault(p => p["p"]?.ToString() == "square_meter")["v"])
            });

        return Ok(result);
    }

    [HttpPost("ads-in-polygon")]
    public async Task<IActionResult> GetAdsInPolygon([FromBody] PolygonPointsRequest request)
    {
        if (request.PolygonPoints == null || request.PolygonPoints.Count < 3)
        {
            return BadRequest("At least three points are required to form a polygon.");
        }

        var polygonPoints = request.PolygonPoints.Select(p => (p.Latitude, p.Longitude)).ToList();

        try
        {
            var ads = await _kufarService.GetAdsInPolygon(polygonPoints);

            var result = ads.ToString();

            if (!result.Any())
            {
                return NotFound("No ads found in the specified polygon.");
            }
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("rent-with-online-booking")]
    public async Task<IActionResult> GetRentAdsWithOnlineBookingByDistrict([FromBody] String request)
    {
        if (string.IsNullOrEmpty(request))
        {
            return BadRequest("District is required.");
        }

        try
        {
            var ads = await _kufarService.GetRentAdsWithOnlineBooking(request);
            var result = ads.ToString();
            if (!result.Any())
            {
                return NotFound($"No ads found in the district {request} with online booking option.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}
