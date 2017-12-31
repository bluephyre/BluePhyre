using System;

namespace BluePhyre.Web.Areas.Administration.Models
{
    public class EditRecurringViewModel
    {
        public long? Id { get; set; }
        public long ClientId { get; set; }
        public long? DomainId { get; set; }
        public long? ResourceId { get; set; }
        public decimal UnitPrice { get; set; } = 0.00m;
        public int Quantity { get; set; } = 1;
        //public decimal GstRate { get; set; } = 0.00m;
        //public decimal PstRate { get; set; } = 0.00m;
        public int FrequencyMultiplier { get; set; } = 1;
        public string Frequency { get; set; } = "Y";
        public DateTime Anniversary { get; set; } = DateTime.Now;
    }
}