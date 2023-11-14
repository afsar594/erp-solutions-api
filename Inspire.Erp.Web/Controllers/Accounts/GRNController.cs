using AutoMapper;
using Inspire.Erp.Application.StoreWareHouse.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Inspire.Erp.Web.ViewModels.Procurement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GRNController : ControllerBase
    {
        private IGRNService _grn;
        private readonly IMapper _mapper;
        public GRNController(IGRNService grn, IMapper mapper)
        {
            _grn = grn;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GenerateVoucherNo")]
        public IActionResult GenerateVoucherNo()
        {
            try
            {
                return Ok(_grn.GenerateVoucherNo(null));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllGRN")]
        public IActionResult GetAllGRN()
        {
            try
            {
                return Ok(_grn.GetAllGRN());
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetByID")]
        public IActionResult GetByID(string voucherNo)
        {
            try
            {
                return Ok(_grn.GetByID(voucherNo));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("saveGRN")]
        public async Task<bool> saveGRN([FromBody] PurchaseVoucher obj)
        {
            return await _grn.saveGRN(obj, obj.AccountsTransactions.ToList());
        }

        [HttpPost("updateGRN")]
        public async Task<bool> updateGRN([FromBody] PurchaseVoucher obj)
        {
            return await _grn.updateGRN(obj, obj.AccountsTransactions.ToList());
        }

        [HttpPost("DeleteGRN")]
        public async Task<bool> DeleteGRN([FromBody] GRNRequest obj)
        {
            return await _grn.DeleteGRN(obj);
        }

        [HttpPost("saveGRNVoucher")]
        public async Task<List<GRNVoucherRquest>> saveGRNVoucher([FromBody] List<GRNVoucherRquest> obj)
        {
            return _grn.saveGRNVoucher(obj).ToList();
        }

        [HttpPost("loadPO")]
        public async Task<IEnumerable<GRNResponse>> loadPO(GRNRequest obj)
        {
            return _grn.loadPo(obj);
        }
    }
}