using System.Collections.Generic;
using BluePhyre.Core.Entities;

namespace BluePhyre.Web.Areas.Administration.Models
{
    public class GetDomainsViewModel
    {
        public IEnumerable<DomainDetail> Domains { get; set; }
        public bool IncludeInactive { get; set; } = false;  
    }
}