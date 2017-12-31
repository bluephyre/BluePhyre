using BluePhyre.Core.Interfaces;

namespace BluePhyre.Core.Entities
{
    public class ResourceListItem : IListItem
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string Group { get; set; }
    }
}