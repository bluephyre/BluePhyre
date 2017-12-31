using System;

namespace BluePhyre.Core.Entities
{
    public class RecurringDetail
    {
        public long Id { get; set; }
        public long ClientId { get; set; }
        public long ResourceId { get; set; }
        public string ResourceName { get; set; }
        public long DomainId { get; set; }
        public string DomainName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        //public decimal GstRate { get; set; }
        //public decimal PstRate { get; set; }
        public string Frequency { get; set; }
        public int FrequencyMultiplier { get; set; }
        public DateTime Anniversary { get; set; }
        public int DaysLeft { get; set; }
    }
}