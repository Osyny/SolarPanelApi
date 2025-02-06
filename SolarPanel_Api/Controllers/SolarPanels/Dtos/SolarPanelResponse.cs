using SolarPanel_Api.Controllers.SolarPanels.Models;

namespace SolarPanel_Api.Controllers.SolarPanels.Dtos
{
    public class SolarPanelResponse
    {
        public List<SolarPanelDto> SolarPanels { get; set; }
        public int Total { get; set; }
    }
}
