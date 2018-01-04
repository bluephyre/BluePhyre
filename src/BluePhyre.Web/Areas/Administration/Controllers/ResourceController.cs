using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BluePhyre.Core.Entities;
using BluePhyre.Core.Interfaces.Repositories;
using BluePhyre.Web.Areas.Administration.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BluePhyre.Web.Areas.Administration.Controllers
{
    [Area("administration"), Authorize("superadmin")]
    public class ResourceController : Controller
    {
        private IClientRepository ClientRepository { get; }

        public ResourceController(IClientRepository repository)
        {
            ClientRepository = repository;
        }

        [HttpGet]
        public IActionResult Index(ListResourcesViewModel model = null)
        {
            model = model ?? new ListResourcesViewModel();

            model.Resources = ClientRepository.GetResourceDetails(model.IncludeInactive ? Status.All : Status.Active);
            model.UsedResources = ClientRepository.GetRecurringDetails().Select(r => r.ResourceId).Distinct();

            return View(model);
        }

        [HttpPost]
        public IActionResult GetDetail(long resourceId)
        {
            var resources = ClientRepository.GetResourceDetails(Status.All);

            return Json(resources?.FirstOrDefault(r => r.Id == resourceId));
        }

        [HttpGet]
        public IActionResult ToggleStatus(long resourceId)
        {
            ClientRepository.ToggleResourceStatus(resourceId);

            return RedirectToAction("Index");
        }
    }
}