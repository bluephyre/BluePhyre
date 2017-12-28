using System;
using BluePhyre.Core.Interfaces;

namespace BluePhyre.Core.Entities
{
    public class Domain : IHasStatus
    {
        public long Id { get; set; }
        public long ClientId { get; set; }
        public string Name { get; set; }
        public DateTime? Expired { get; set; }
        public bool Active { get; set; }
    }
}