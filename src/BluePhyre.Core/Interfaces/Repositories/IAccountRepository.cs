using System.Collections.Generic;
using BluePhyre.Core.Entities;

namespace BluePhyre.Core.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<ClientDetail> GetClientDetails(Status status = Status.All);
    }
}