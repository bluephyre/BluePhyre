using System;
using System.Collections.Generic;
using System.Linq;
using BluePhyre.Core.Entities;
using BluePhyre.Core.Interfaces.Repositories;
using BluePhyre.Web.Areas.Administration.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BluePhyre.Web.Areas.Administration.Controllers
{
    [Area("administration"), Authorize]
    public class RecurringController : Controller
    {
        private IClientRepository ClientRepository { get; }

        public RecurringController(IClientRepository repository)
        {
            ClientRepository = repository;
        }

        public IActionResult Index()
        {
            var model = new GetRecurringsViewModel
            {
                Clients = ClientRepository.GetClientListItems(),
                Recurrings = ClientRepository.GetRecurringDetails()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(long clientId, long? recurringId)
        {
            var recurrings = ClientRepository.GetRecurringDetails(clientId).ToList();

            EditRecurringViewModel model = new EditRecurringViewModel { ClientId = clientId };

            if (recurringId.HasValue && recurrings.Any(r => r.Id == recurringId))
            {
                var recurring = recurrings.First(r => r.Id == recurringId);

                model = new EditRecurringViewModel
                {
                    Id = recurring.Id,
                    ClientId = recurring.ClientId,
                    DomainId = recurring.DomainId,
                    ResourceId = recurring.ResourceId,
                    Quantity = recurring.Quantity,
                    UnitPrice = recurring.UnitPrice,
                    //GstRate = recurring.GstRate,
                    //PstRate = recurring.PstRate,
                    Frequency = recurring.Frequency,
                    FrequencyMultiplier = recurring.FrequencyMultiplier,
                    Anniversary = recurring.Anniversary
                };
            }

            ViewBag.Recurrings = recurrings;
            ViewBag.Domains = ClientRepository.GetDomainListItems(clientId).AddEmpty();
            ViewBag.Frequencies = new List<SelectListItem>
            {
                new SelectListItem {Text = "Yearly", Value = "Y", Selected = true},
                new SelectListItem {Text = "Quarterly", Value = "Q"},
                new SelectListItem {Text = "Monthly", Value = "M"}
            };
            ViewBag.Resources = ClientRepository.GetResourceListItems().AddEmpty();

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(EditRecurringViewModel model)
        {
            ClientRepository.SaveRecurring(model.Id, model.ClientId, model.DomainId.GetValueOrDefault(), model.ResourceId.GetValueOrDefault(), 
                model.Quantity, model.UnitPrice, model.Frequency, model.FrequencyMultiplier, model.Anniversary);

            return RedirectToAction("Edit", new {clientId = model.ClientId, recurringId = model.Id});
        }

        [HttpPost]
        public IActionResult Generate(GenerateRecurringViewModel model)
        {
            var invoiceDate = model.InvoiceDate ?? DateTime.Today;


            var recurrings = ClientRepository.GetRecurringDetails().Where(r => model.RecurringId.Contains(r.Id));

            var recurringsByClients = recurrings.GroupBy(r => r.ClientId, r => r);

            foreach (var clientGroup in recurringsByClients)
            {
                var clientId = clientGroup.Key;

                foreach (var recurringDetail in clientGroup)
                {
                    ClientRepository.CreateLedgerEntry(recurringDetail.ClientId, recurringDetail.DomainId,
                        recurringDetail.ResourceId, recurringDetail.Anniversary, recurringDetail.Quantity,
                        recurringDetail.UnitPrice);
                    ClientRepository.UpdateRecurringAnniversary(recurringDetail.Id, recurringDetail.Frequency,
                        recurringDetail.FrequencyMultiplier, recurringDetail.Anniversary);
                }

                if (model.CreateInvoices)
                {
                    ClientRepository.CreateInvoice(clientId, invoiceDate);
                }

            }

            return RedirectToAction("Index");
        }
    }
}