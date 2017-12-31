using BluePhyre.Core.Interfaces;

namespace BluePhyre.Core.Entities
{
    public class DomainListItem : IListItem
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}