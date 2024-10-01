using AutoMapper;
using Inspire.Erp.Application.Store.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherPrintingController : ControllerBase
    {
        private Inspire.Erp.Application.Store.Interface.IvoucherPrinting vp;
        private readonly IMapper _mapper;
        public VoucherPrintingController(IvoucherPrinting _vp)
        {
            vp = _vp;
        }       
 
        [HttpGet("VoucherPrintingResponse")]
        public async Task<dynamic> VoucherPrintingResponse()
        {
            try
            {
                VoucherPrintingRequest obj = new VoucherPrintingRequest();
                return vp.VoucherPrintingRequest(obj);               

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
