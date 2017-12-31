using System.Collections.Generic;
using BluePhyre.Core.Entities;

namespace BluePhyre.Web.Areas.Administration.Models
{
    public class GetRecurringsViewModel
    {
        public IEnumerable<ClientListItem> Clients { get; set; }
        public IEnumerable<RecurringDetail> Recurrings { get; set; }
    }
}