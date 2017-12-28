using BluePhyre.Core.Entities;
using BluePhyre.Core.Interfaces.Repositories;
using BluePhyre.Web.Areas.Administration.Models;
using Microsoft.AspNetCore.Mvc;

namespace BluePhyre.Web.Areas.Administration.Controllers
{
    [Area("administration")]
    public class AccountController : Controller
    {
        private IAccountRepository AccountRepository { get; }

        public AccountController(IAccountRepository repository)
        {
            AccountRepository = repository;
        }

        public IActionResult Index(Status status = Status.All)
        {
            return View(new GetClientsViewModel {Clients = AccountRepository.GetClientDetails(status)});
        }
    }
}