using System.Collections.Generic;
using System.Linq;

namespace BluePhyre.Core.Entities
{
    public static class ResourceListItemExtensions
    {
        public static IEnumerable<ResourceListItem> AddEmpty(this IEnumerable<ResourceListItem> list)
        {
            var temp = list.ToList();
            temp.Insert(0, new ResourceListItem { Active = true, Id = null, Name = "-- Select --" });
            return temp;
        }
    }
}