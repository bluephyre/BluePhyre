using System.Collections.Generic;
using System.Linq;
using BluePhyre.Core.Entities;
using BluePhyre.Core.Interfaces.Repositories;
using Dapper;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace BluePhyre.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private IConfiguration Configuration { get; }

        public ClientRepository(IConfiguration configuration)
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

        public IEnumerable<DomainDetail> GetDomainDetails(Status status = Status.All)
        {
            IList<DomainDetail> domains;

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                domains = connection.Query<DomainDetail>("SELECT d.clnt_id as ClientId, d.id, d.fqdn as Name, d.active, count(r.id) as RecurringCharges, min(r.anniversary) as NextAnniversary, count(s.id) as registered from domain d left join recurring r on d.id = r.dmn_id left join resource s on r.rsc_id = s.id and s.rclass = 'DREG' group by d.clnt_id, d.id, d.fqdn, d.active order by fqdn;").ToList();
            }

            switch (status)
            {
                case Status.Inactive:
                    return domains.Where(d => !d.Active);
                case Status.Active:
                    return domains.Where(d => d.Active);
            }

            return domains;
        }

        public bool ToggleDomainStatus(long domainId)
        {
            int result;

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                result = connection.Execute("UPDATE domain SET active = MOD(active + 1, 2) WHERE id = @Id", new { Id = domainId });
            }

            return result == 1;
        }

        public bool CreateDomain(long clientId, string name)
        {
            int result;

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                result = connection.Execute("INSERT INTO domain(created, clnt_id, fqdn, effective, active) VALUES(NOW(), @ClientId, @Name, NOW(), 1)", new { ClientId = clientId, Name = name });
            }

            return result == 1;
        }

        public IEnumerable<ClientListItem> GetClientListItems(Status status = Status.Active)
        {
            IList<ClientListItem> clients;

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                clients = connection.Query<ClientListItem>("SELECT id, if(isnull(company), adm_name, concat(adm_name, ' - ', company)) as name, if(isnull(expires), 1, 0) as active from client order by company, adm_name").ToList();
            }

            switch (status)
            {
                case Status.Active:
                    return clients.Where(c => c.Active);
                case Status.Inactive:
                    return clients.Where(c => !c.Active);
            }

            return clients;
        }

        public IEnumerable<DomainListItem> GetDomainListItems(long clientId, Status status = Status.Active)
        {
            IList<DomainListItem> domains;

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                domains = connection.Query<DomainListItem>("SELECT id, fqdn as name, if(isnull(expires), 1, 0) as active from domain " +
                                                           "WHERE clnt_id = @ClientId ORDER BY fqdn",
                                                           new { ClientId = clientId }).ToList();
            }

            switch (status)
            {
                case Status.Active:
                    return domains.Where(c => c.Active);
                case Status.Inactive:
                    return domains.Where(c => !c.Active);
            }

            return domains;
        }

        public IEnumerable<RecurringDetail> GetRecurringDetails(long? clientId = null)
        {
            IList<RecurringDetail> recurring;

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                CommandDefinition command;

                if (clientId.HasValue)
                {
                    command = new CommandDefinition("SELECT rc.id, rc.clnt_id as clientId, rc.rsc_id as resourceId, rs.name as resourceName, " +
                                                   "rc.dmn_id as domainId, dn.fqdn as domainName, rc.quantity, rc.unit_price as unitPrice, " +
                                                   "rc.gst_rate as gstRate, rc.pst_rate as pstRate, rc.frequency, " +
                                                   "rc.frequency_multiplier as frequencyMultiplier, rc.anniversary, " +
                                                   "(TO_DAYS(rc.anniversary) - TO_DAYS(now())) as daysleft " +
                                                   "FROM resource rs, recurring rc, domain dn " +
                                                   "WHERE rc.clnt_id = @Id AND rc.rsc_id = rs.id " +
                                                   "AND dn.id = rc.dmn_id ORDER BY rc.anniversary, dn.fqdn",
                                                   new { Id = clientId });
                }
                else
                {
                    command = new CommandDefinition("SELECT rc.id, rc.clnt_id as clientId, rc.rsc_id as resourceId, rs.name as resourceName, " +
                                                    "rc.dmn_id as domainId, dn.fqdn as domainName, rc.quantity, rc.unit_price as unitPrice, " +
                                                    "rc.gst_rate as gstRate, rc.pst_rate as pstRate, rc.frequency, " +
                                                    "rc.frequency_multiplier as frequencyMultiplier, rc.anniversary, " +
                                                    "(TO_DAYS(rc.anniversary) - TO_DAYS(now())) as daysleft " +
                                                    "FROM resource rs, recurring rc, domain dn " +
                                                    "WHERE rc.rsc_id = rs.id AND dn.id = rc.dmn_id ORDER BY rc.anniversary, dn.fqdn");
                }

                recurring = connection.Query<RecurringDetail>(command).ToList();

            }

            return recurring;
        }

        public bool CreateLedgerEntry(long clientId, long domainId, long resourceId, DateTime entryDate, int quantity, decimal unitPrice)
        {
            int result;

            var command = new CommandDefinition("INSERT INTO dledger(clnt_id, dmn_id, rsc_id, date, quantity, unit_price, gst_rate, pst_rate) " +
                                                "VALUES(@ClientId, @DomainId, @ResourceId, @Anniversary, @Quantity, @UnitPrice, 0, 0)",
                new
                {
                    ClientId = clientId,
                    DomainId = domainId,
                    ResourceId = resourceId,
                    Anniversary = entryDate,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                result = connection.Execute(command);
            }

            return result == 1;
        }

        public bool CreateInvoice(long clientId, DateTime invoiceDate)
        {
            if (GetInvoiceSummary(clientId, invoiceDate) != null)
            {
                return true;
            }

            int result;

            var command = new CommandDefinition("INSERT INTO invoice(date, clnt_id) " +
                                                "VALUES(@InvoiceDate, @ClientId)",
                new
                {
                    ClientId = clientId,
                    InvoiceDate = invoiceDate
                });

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                result = connection.Execute(command);
            }

            return result == 1;
        }

        public InvoiceSummary GetInvoiceSummary(long clientId, DateTime invoiceDate)
        {
            InvoiceSummary invoice;

            var command = new CommandDefinition("SELECT id, clnt_id as clientId, date as invoiceDate from invoice " +
                                                "WHERE clnt_id=@ClientId AND date = @InvoiceDate",
                                                new
                                                {
                                                    ClientId = clientId,
                                                    InvoiceDate = invoiceDate
                                                });

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                invoice = connection.QueryFirstOrDefault<InvoiceSummary>(command);
            }

            return invoice;
        }

        public bool UpdateRecurringAnniversary(long recurringId, string frequency, int frequencyMultiplier, DateTime anniversary)
        {
            int result;

            CommandDefinition command;

            switch (frequency.ToUpper())
            {
                case "Y":
                    command = new CommandDefinition("UPDATE recurring SET anniversary = DATE_ADD(@Anniversary, INTERVAL @FrequencyMultiplier YEAR) " +
                                                    "WHERE id = @RecurringId",
                        new
                        {
                            Anniversary = anniversary,
                            FrequencyMultiplier = frequencyMultiplier,
                            RecurringId = recurringId
                        });

                    break;
                default:
                    return false;
            }

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                result = connection.Execute(command);
            }

            return result == 1;
        }

        public IEnumerable<ResourceListItem> GetResourceListItems(Status status = Status.Active)
        {

            IList<ResourceListItem> resources;

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                resources = connection.Query<ResourceListItem>("SELECT id, name, active, rclass as 'group' FROM resource ORDER BY name").ToList();
            }

            switch (status)
            {
                case Status.Active:
                    return resources.Where(c => c.Active);
                case Status.Inactive:
                    return resources.Where(c => !c.Active);
            }

            return resources;
        }

        public bool SaveRecurring(long? id, long clientId, long domainId, long resourceId, int quantity, decimal unitPrice, 
            string frequency, int frequencyMultiplier, DateTime anniversary)
        {
            int result;
            CommandDefinition command;

            if (id.HasValue)
            {
                command = new CommandDefinition(
                    "UPDATE recurring SET dmn_id = @DomainId, rsc_id = @ResourceId, quantity = @Quantity, unit_price = @UnitPrice, gst_rate = 0, " +
                    "pst_rate = 0, frequency = @Frequency, frequency_multiplier = @FrequencyMultiplier, anniversary = @Anniversary " +
                    "WHERE id = @Id AND clnt_id = @ClientId",
                    new
                    {
                        Id = id,
                        ClientId = clientId,
                        DomainId = domainId,
                        ResourceId = resourceId,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        //GstRate = gstRate,
                        //PstRate = pstRate,
                        Frequency = frequency,
                        FrequencyMultiplier = frequencyMultiplier,
                        Anniversary = anniversary
                    });
            }
            else
            {
                command = new CommandDefinition(
                    "INSERT INTO recurring(clnt_id, dmn_id, rsc_id, quantity, unit_price, gst_rate, pst_rate, frequency, " +
                    "frequency_multiplier, anniversary) " +
                    "VALUES(@ClientId, @DomainId, @ResourceId, @Quantity, @UnitPrice, 0, 0, @Frequency, " +
                    "@FrequencyMultiplier, @Anniversary)",
                    new
                    {
                        ClientId = clientId,
                        DomainId = domainId,
                        ResourceId = resourceId,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        //GstRate = gstRate,
                        //PstRate = pstRate,
                        Frequency = frequency,
                        FrequencyMultiplier = frequencyMultiplier,
                        Anniversary = anniversary
                    });
            }

            using (var connection = new MySqlConnection(Configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                result = connection.Execute(command);
            }

            return result == 1;
        }
    }
}

