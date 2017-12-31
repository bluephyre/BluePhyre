using System;

namespace BluePhyre.Core.Entities
{
    public class DomainDetail
    {
        public long Id { get; set; }
        public long ClientId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public int RecurringCharges { get; set; }
        public DateTime? NextAnniversary { get; set; }
        public bool Registered { get; set; }
    }
}