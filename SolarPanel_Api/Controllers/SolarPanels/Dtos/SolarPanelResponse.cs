using SolarPanel_Api.Dtos;

namespace SolarPanel_Api.Controllers.SolarPanels.Dtos
{
    public class SolarPanelResponse
    {
        public List<SolarPanelDto> SolarPanels { get; set; }
        public int Total { get; set; }
    }
}
