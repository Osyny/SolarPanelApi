using SolarPanel_Api.Dtos;

namespace SolarPanel_Api.Controllers.SolarPanels.Dtos
{
    public class ConfiguratorResponse
    {
        public SolarPanelDto Panel { get; set; }
        public int Count { get; set; }
        public int TotalPower { get; set; }
    }
}
