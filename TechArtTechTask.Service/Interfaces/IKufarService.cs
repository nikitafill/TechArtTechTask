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
    }
}
