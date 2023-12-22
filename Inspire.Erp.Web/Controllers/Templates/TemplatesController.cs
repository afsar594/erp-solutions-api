using Inspire.Erp.Application.Templates.Interfaces;
using Inspire.Erp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Templates
{
    [Route("api/Templates")]
    [Produces("application/json")]
    [ApiController]
    public class TemplatesController : ControllerBase
    {
        private ITemplates template;

        public TemplatesController(ITemplates template)
        {
            this.template = template;
        }

        [HttpPost]
        [Route("DeleteTemplate")]
        public async Task<IActionResult> DeleteTemplate(int qtemplateId)
        {
            return Ok(await template.DeleteTemplate(qtemplateId));
        }
        [HttpPost]
        [Route("GetAllActiveTemplates")]
        public async Task<IActionResult> GetAllActiveTemplates()
        {
            return Ok(await template.GetAllActiveTemplates());
        }
        [HttpPost]
        [Route("GetTemplateById")]
        public async Task<IActionResult> GetTemplateById(int qtemplateId)
        {
            return Ok(await template.GetTemplateById(qtemplateId));
        }
        [HttpPost]
        [Route("GetTemplateBytype")]
        public async Task<IActionResult> GetTemplateBytype(string qtemplateType)
        {
            return Ok(await template.GetTemplateBytype(qtemplateType));
        }
        [HttpPost]
        [Route("InsterTemplate")]
        public async Task<IActionResult> InsterTemplate(Qtemplates qtemplate)
        {
            return Ok(await template.InsterTemplate(qtemplate));
        }
        [HttpPost]
        [Route("UpdateTemplate")]
        public async Task<IActionResult> UpdateTemplate(Qtemplates qtemplate)
        {
            return Ok(await template.UpdateTemplate(qtemplate));
        }

        [HttpPost]
        [Route("GetAllDeletedTemplates")]
        public async Task<IActionResult> GetAllDeletedTemplates()
        {
            return Ok(await template.GetAllDeletedTemplates());
        }
        [HttpPost]
        [Route("GetAllTemplates")]
        public async Task<IActionResult> GetAllTemplates()
        {
            return Ok(await template.GetAllTemplates());
        }

    }
}
