using Azure;
using Azure.Data.Tables;

namespace SolarPanel_Api.Entities
{
    public class SolarPanel : ITableEntity
    {
        // Interface properties implementation
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Properties
        public int Width { get; set; }
        public int Length { get; set; }
        public int Power { get; set; }
        public int Stock { get; set; }

    }
}
