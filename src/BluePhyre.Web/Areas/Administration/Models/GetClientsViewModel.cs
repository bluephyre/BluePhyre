using System.Collections.Generic;
using BluePhyre.Core.Entities;

namespace BluePhyre.Web.Areas.Administration.Models
{
    public class GetClientsViewModel
    {
        public IEnumerable<ClientDetail> Clients { get; set; }

        public bool IncludeInactive { get; set; } = false;
    }
}