using System.Collections.Generic;
using System.Linq;
using BluePhyre.Core.Interfaces;

namespace BluePhyre.Core.Entities
{
    public static class ListItemExtensions
    {
        public static IEnumerable<T> AddEmpty<T>(this IEnumerable<T> list) where T : IListItem, new()
        {
            var temp = list.ToList();
            temp.Insert(0, new T { Active = true, Id = null, Name = "-- Select --"});
            return temp;
        }
    }
}