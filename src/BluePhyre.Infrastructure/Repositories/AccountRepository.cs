using System.Collections.Generic;
using System.Linq;
using BluePhyre.Core.Entities;
using BluePhyre.Core.Interfaces.Repositories;
using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace BluePhyre.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private IConfiguration Configuration { get; }

        public AccountRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IEnumerable<ClientDetail> GetClientDetails(Status status = Status.All)
        {
            IList<Client> clients;
            IList<Domain> domains;

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                clients = connection.Query<Client>("SELECT Id, account_name AS AccountName, expires AS Expired, username FROM client").ToList();
                domains = connection.Query<Domain>("SELECT Id, fqdn AS Name, expires AS Expired, clnt_id AS ClientId, active FROM domain").ToList();
            }

            var result = clients.Select(c => new ClientDetail
            {
                Client = c,
                Domains = domains.Where(d => d.ClientId == c.Id).ToList()
            });

            switch (status)
            {
                case Status.Inactive:
                    return result.Where(i => !i.Client.Active);
                case Status.Active:
                    return result.Where(i => i.Client.Active);
            }

            return result;

        }
    }
}