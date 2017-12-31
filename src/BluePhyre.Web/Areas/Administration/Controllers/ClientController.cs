using BluePhyre.Core.Entities;
using BluePhyre.Core.Interfaces.Repositories;
using BluePhyre.Web.Areas.Administration.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BluePhyre.Web.Areas.Administration.Controllers
{
    [Area("administration"), Authorize]
    public class ClientController : Controller
    {
        private IClientRepository ClientRepository { get; }

        public ClientController(IClientRepository repository)
        {
            ClientRepository = repository;
        }

        public IActionResult Index(GetClientsViewModel model = null)
        {
            if (model == null)
            {
                model = new GetClientsViewModel();
            }

            model.Clients = ClientRepository.GetClientDetails(model.IncludeInactive ? Status.All : Status.Active);

            return View(model);
        }

    }
}