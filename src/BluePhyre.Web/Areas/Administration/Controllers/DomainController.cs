using BluePhyre.Core.Entities;
using BluePhyre.Core.Interfaces.Repositories;
using BluePhyre.Web.Areas.Administration.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BluePhyre.Web.Areas.Administration.Controllers
{
    [Area("administration"), Authorize(Roles = "superadmin")]
    public class DomainController : Controller
    {
        private IClientRepository ClientRepository { get; }

        public DomainController(IClientRepository repository)
        {
            ClientRepository = repository;
        }

        public IActionResult Index(GetDomainsViewModel model = null)
        {
            ViewBag.ListOfClients = ClientRepository.GetClientListItems();

            if (model == null)
            {
                model = new GetDomainsViewModel();
            }

            model.Domains = ClientRepository.GetDomainDetails(model.IncludeInactive ? Status.All : Status.Active);

            return View(model);
        }

        public IActionResult ToggleStatus(long domainId)
        {
            var result = ClientRepository.ToggleDomainStatus(domainId);

            if (!result)
            {
                
            }

            return RedirectToAction("Index");
        }

        public IActionResult Create(long clientId, string name)
        {
            var result = ClientRepository.CreateDomain(clientId, name);

            return RedirectToAction("Index");
        }

    }
}