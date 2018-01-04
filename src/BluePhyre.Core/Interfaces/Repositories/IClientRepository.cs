using System;
using System.Collections.Generic;
using BluePhyre.Core.Entities;

namespace BluePhyre.Core.Interfaces.Repositories
{
    public interface IClientRepository
    {
        IEnumerable<ClientDetail> GetClientDetails(Status status = Status.All);
        IEnumerable<DomainDetail> GetDomainDetails(Status status = Status.All);
        bool ToggleDomainStatus(long domainId);
        bool ToggleClientStatus(long clientId);
        bool ToggleResourceStatus(long resourceId);
        bool CreateDomain(long clientId, string name);
        IEnumerable<ClientListItem> GetClientListItems(Status status = Status.Active);
        IEnumerable<DomainListItem> GetDomainListItems(long clientId, Status status = Status.Active);
        IEnumerable<ResourceListItem> GetResourceListItems(Status status = Status.Active);
        IEnumerable<RecurringDetail> GetRecurringDetails(long? clientId = null);

        bool CreateLedgerEntry(long clientId, long domainId, long resourceId, DateTime entryDate, int quantity, decimal unitPrice);

        bool CreateInvoice(long clientId, DateTime invoiceDate);
        InvoiceSummary GetInvoiceSummary(long clientId, DateTime invoiceDate);

        bool UpdateRecurringAnniversary(long recurringId, string frequency, int frequencyMultiplier,
            DateTime anniversary);

        bool SaveRecurring(long? id, long clientId, long domainId, long resourceId, int quantity, decimal unitPrice,
            string frequency, int frequencyMultiplier, DateTime anniversary);

        bool DeleteRecurring(long id, long clientId);
        IEnumerable<ResourceDetail> GetResourceDetails(Status status = Status.Active);
        bool IsUserSuperAdmin(string identifier);
    }
}