using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechArtTechTask.Service.Interfaces;
using TechArtTechTask.Service.Services;
using Xunit;

public class KufarServiceTests
{
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly IKufarService _kufarService;

    public KufarServiceTests()
    {
        _httpClientMock = new Mock<HttpClient>();
        _kufarService = new KufarService(_httpClientMock.Object);
    }

    [Fact]
    public async Task GetAdsInPolygon_ShouldThrow_WhenLessThanThreePoints()
    {
        var points = new List<(double Latitude, double Longitude)> { (0, 0), (1, 1) };

        await Assert.ThrowsAsync<ArgumentException>(() => _kufarService.GetAdsInPolygon(points));
    }

    [Fact]
    public async Task GetAdsInPolygon_ShouldReturnAdLinks_WhenPointsFormPolygon()
    {
        // Подготовка данных для теста
        var points = new List<(double Latitude, double Longitude)>
        {
            (53.8, 27.5),
            (53.9, 27.6),
            (53.85, 27.5)
        };

        var ads = await _kufarService.GetAdsInPolygon(points);

        Assert.NotEmpty(ads);
    }

}
