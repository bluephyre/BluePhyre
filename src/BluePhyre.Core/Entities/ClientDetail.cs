using System.Collections.Generic;
using System.Linq;

namespace BluePhyre.Core.Entities
{
    public class ClientDetail : ClientSummary
    {
        public IList<Domain> Domains { get; set; }

        public int TotalDomains => Domains.Count;
        public int ActiveDomains => Domains.Count(d => d.Active);
        public bool Flag => !Client.Expired.HasValue && ActiveDomains == 0;
    }
}