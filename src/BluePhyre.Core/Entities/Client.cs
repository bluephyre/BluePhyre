using System;
using BluePhyre.Core.Interfaces;

namespace BluePhyre.Core.Entities
{
    public class Client : IHasStatus
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public string Username { get; set; }
        public DateTime? Expired { get; set; }
        public bool Active => !Expired.HasValue;

    }
}
