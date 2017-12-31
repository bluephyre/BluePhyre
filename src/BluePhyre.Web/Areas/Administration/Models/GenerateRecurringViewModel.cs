using System;
using System.Collections.Generic;

namespace BluePhyre.Web.Areas.Administration.Models
{
    public class GenerateRecurringViewModel
    {
        public bool CreateInvoices { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public List<long> RecurringId { get; set; }
    }
}