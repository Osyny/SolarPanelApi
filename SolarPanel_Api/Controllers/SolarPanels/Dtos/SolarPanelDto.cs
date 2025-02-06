namespace SolarPanel_Api.Controllers.SolarPanels.Models
{
    public class SolarPanelDto
    {
        public string? Id { get; set; }

        public int Width { get; set; }
        public int Length { get; set; }
        public decimal Power { get; set; }
        public int Stock { get; set; }
    }
}
