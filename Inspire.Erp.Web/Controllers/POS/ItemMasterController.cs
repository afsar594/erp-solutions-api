using AutoMapper;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.POS
{
    [Route("api/pos/item")]
    [Produces("application/json")]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class POSController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IItemMasterService itemMasterService;
        public POSController(IItemMasterService _itemMasterService, IMapper mapper)
        {

            itemMasterService = _itemMasterService;
            _mapper = mapper;
        }
        

        [HttpGet]
        [Route("SearchItemByBarcodeLink/{barCode}")]
        public async Task<IActionResult> SearchItemByBarcodeLink(ItemBarCodeSearchFilter model)
        {
            return Ok(await itemMasterService.SearchItemByBarcode(model));

        }

        [HttpGet]
        [Route("GetWorkPeriods")]
        public async Task<IActionResult> GetWorkPeriods(WorkPeriodFilter model)
        {
            return Ok(await itemMasterService.GetWorkPeriod(model));

        }


    }
}