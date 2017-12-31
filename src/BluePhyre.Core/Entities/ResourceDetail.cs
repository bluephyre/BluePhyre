namespace BluePhyre.Core.Entities
{
    public class ResourceDetail
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public string RType { get; set; }
        public string RClass { get; set; }
        public bool Active { get; set; }
    }
}