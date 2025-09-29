using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Widgets.CaseStudy.Models;
using Nop.Plugin.Widgets.CaseStudy.Services;
using Nop.Plugin.Widgets.CaseStudy.Domain;

namespace Nop.Plugin.Widgets.CaseStudy.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class CaseStudyController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly ICaseStudyService _service;
        private readonly ICaseStudyStepsService _steps;

        public CaseStudyController(IWorkContext workContext, ICaseStudyService service, ICaseStudyStepsService steps)
        {
            _workContext = workContext;
            _service = service;
            _steps = steps;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? projectid = null, int? casestudyid = null)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return Challenge();

            if (!projectid.HasValue || projectid.Value <= 0)
            {
                TempData["CaseStudyMessage"] = "A valid ProjectID is required.";
                TempData["CaseStudyMessageType"] = "error";
                return RedirectToRoute("Homepage");
            }

            // If a CaseStudyID is supplied explicitly, load it (edit mode)
            if (casestudyid.HasValue && casestudyid.Value > 0)
            {
                var entity = await _service.GetByIdForCustomerAsync(casestudyid.Value, customer.Id);
                if (entity == null || entity.ProjectId != projectid.Value)
                {
                    TempData["CaseStudyMessage"] = "We couldn’t find that case study or you don’t have access to it.";
                    TempData["CaseStudyMessageType"] = "error";
                    return RedirectToAction("Index", new { projectid = projectid.Value });
                }

                var model = new CaseStudyModel
                {
                    Id = entity.Id,
                    ProjectId = entity.ProjectId,
                    Existing = true,
                    CaseStudy = entity.CaseStudy
                };
                return View("~/Plugins/Widgets.CaseStudy/Views/CaseStudy/Index.cshtml", model);
            }

            // No CaseStudyID given: look it up from Steps for (CustomerId, ProjectId)
            var idFromSteps = await _steps.GetCaseStudyIdForAsync(customer.Id, projectid.Value);
            if (idFromSteps.HasValue && idFromSteps.Value > 0)
            {
                // Validate it belongs to this customer and project before redirecting
                var entity = await _service.GetByIdForCustomerAsync(idFromSteps.Value, customer.Id);
                if (entity != null && entity.ProjectId == projectid.Value)
                {
                    // Redirect so the URL always includes ?casestudyid=...
                    return RedirectToAction("Index", new { projectid = projectid.Value, casestudyid = entity.Id });
                }
            }

            // Fallback: blank form (no Steps link yet)
            var newModel = new CaseStudyModel { ProjectId = projectid.Value, Existing = false };
            return View("~/Plugins/Widgets.CaseStudy/Views/CaseStudy/Index.cshtml", newModel);

        }



        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(CaseStudyModel model)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return Challenge();

            if (model.ProjectId <= 0)
            {
                TempData["CaseStudyMessage"] = "A valid ProjectID is required.";
                TempData["CaseStudyMessageType"] = "error";
                return RedirectToRoute("Homepage");
            }

            // UPDATE
            if (model.Existing && model.Id > 0)
            {
                var entity = await _service.GetByIdForCustomerAsync(model.Id, customer.Id);
                if (entity == null || entity.ProjectId != model.ProjectId)
                {
                    TempData["CaseStudyMessage"] = "We couldn’t find that case study or you don’t have access to it.";
                    TempData["CaseStudyMessageType"] = "error";
                    return RedirectToAction("Index", new { projectid = model.ProjectId });
                }

                entity.CaseStudy = model.CaseStudy;
                entity.Published = true;
                entity.UpdatedOnUtc = DateTime.UtcNow;

                await _service.UpdateAsync(entity);
                await _steps.UpsertCaseStudyLinkAndStep4Async(customer.Id, entity.ProjectId, entity.Id);

                TempData["CaseStudyMessage"] = "Your case study was updated successfully.";
                TempData["CaseStudyMessageType"] = "success";
                return RedirectToAction("Index", new { projectid = entity.ProjectId, casestudyid = entity.Id });
            }

            // CREATE
            var newEntity = new CaseStudyRecord
            {
                CustomerId = customer.Id,
                ProjectId = model.ProjectId,
                CaseStudy = model.CaseStudy,
                Published = true,
                Deleted = false,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };

            await _service.InsertAsync(newEntity);
            await _steps.UpsertCaseStudyLinkAndStep4Async(customer.Id, newEntity.ProjectId, newEntity.Id);

            TempData["CaseStudyMessage"] = "Your case study was created successfully.";
            TempData["CaseStudyMessageType"] = "success";
            return RedirectToAction("Index", new { projectid = newEntity.ProjectId, casestudyid = newEntity.Id });
        }
    }
}
