using System.Collections.Generic;
using System.Linq;

namespace BluePhyre.Core.Entities
{
    public static class DomainListItemExtensions
    {
        public static IEnumerable<DomainListItem> AddEmpty(this IEnumerable<DomainListItem> list)
        {
            var temp = list.ToList();
            temp.Insert(0, new DomainListItem {Active = true, Id = null, Name = "-- Select --"});
            return temp;
        }
    }
}