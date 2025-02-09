namespace SolarPanel_Api.Controllers.SolarPanels.Dtos
{
    public class DataInput
    {
        public int Rows { get; set; }
        public int Skip { get; set; } 
        public string? Sorting { get; set; }
    }
}
