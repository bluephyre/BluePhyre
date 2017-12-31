using System.Collections.Generic;
using BluePhyre.Core.Entities;

namespace BluePhyre.Web.Areas.Administration.Models
{
    public class ListResourcesViewModel
    {
        public IEnumerable<ResourceDetail> Resources { get; set; }
        public bool IncludeInactive { get; set; } = false;
        public IEnumerable<long> UsedResources { get; set; }
    }
}