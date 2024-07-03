using AutoMapper;
using Inspire.Erp.Application.Account;
using Inspire.Erp.Application.StoreWareHouse.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
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

        private IRepository<ItemMaster> itemrepository;
        private readonly IRepository<SuppliersMaster> supplierrepository;
        private IRepository<UnitMaster> unitrepository;
        private IRepository<CostCenterMaster> costcenterrepository;
        private IRepository<JobMaster> jobrepository;
        private IRepository<CurrencyMaster> currencyrepository;
        private IChartofAccountsService chartofAccountsService;
        public GRNController(IGRNService grn, IMapper mapper, IRepository<ItemMaster> _itemrepository,
            IRepository<SuppliersMaster> _supplierrepository,
            IRepository<UnitMaster> _unitrepository
            , IRepository<CostCenterMaster> _countryrepository
            , IRepository<JobMaster> _jobrepository, IRepository<CurrencyMaster> _currencyrepository, IChartofAccountsService _chartofAccountsService
            )
        {
            _grn = grn;
            _mapper = mapper;

            supplierrepository = _supplierrepository;
            itemrepository = _itemrepository; unitrepository = _unitrepository;
            costcenterrepository = _countryrepository;
            jobrepository = _jobrepository;
            currencyrepository = _currencyrepository; chartofAccountsService = _chartofAccountsService;
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

        [HttpPost("saveGRNVoucher")]
        public IActionResult saveGRN([FromBody] PurchaseVoucher obj)
        {
            return Ok(_grn.saveGRN(obj));
        }

        [HttpPost("updateGRN")]
        public IActionResult updateGRN([FromBody] PurchaseVoucher obj)
        {
            return Ok(_grn.updateGRN(obj, obj.AccountsTransactions.ToList()));
        }

        [HttpPost("DeleteGRN")]
        public IActionResult DeleteGRN([FromBody] PurchaseVoucher obj)
        {
            return Ok(_grn.DeleteGRN(obj));
        }


        [HttpPost("loadPO")]
        public async Task<IEnumerable<GRNResponse>> loadPO(GRNRequest obj)
        {
            return _grn.loadPo(obj);
        }


        [HttpGet]
        [Route("LoadDropdown")]
        public ResponseInfo LoadDropdown()
        {
            var objectresponse = new ResponseInfo();
            var itemMaster = itemrepository.GetAsQueryable().Where(k => k.ItemMasterAccountNo == 0 && (k.ItemMasterDelStatus != true)
                     && k.ItemMasterItemType == ItemMasterStatus.Group).Select(k => new
                     {
                         k.ItemMasterItemId,
                         k.ItemMasterItemName
                     }).ToList();
            var SuppliersMaster = supplierrepository.GetAsQueryable().Where(a => a.SuppliersMasterSupplierDelStatus != true).Select(c => new
            {
                c.SuppliersMasterSupplierId,
                c.SuppliersMasterSupplierName
            }).ToList();

            var unitMasters = unitrepository.GetAsQueryable().Where(a => a.UnitMasterUnitDelStatus != true).Select(x => new
            {
                x.UnitMasterUnitId,
                x.UnitMasterUnitFullName,
                x.UnitMasterUnitShortName
            }).ToList();
           
            var jobMasters = jobrepository.GetAsQueryable().Where(a => a.JobMasterJobDelStatus != true).Select(c => new
            {
                c.JobMasterJobId,
                c.JobMasterJobName,
            }).ToList();
            var currencyMasters = currencyrepository.GetAsQueryable().Where(a => a.CurrencyMasterCurrencyDelStatus != true).Select(c => new
            {
                c.CurrencyMasterCurrencyId,
                c.CurrencyMasterCurrencyName,
                c.CurrencyMasterCurrencyRate
            }).ToList();
            var masterAccountsTables = chartofAccountsService.GetAllAccounts().Where(a => a.MaDelStatus != true).Select(c => new
            {
                c.MaAccNo,
                c.MaAccName,
                c.MaRelativeNo,
                c.MaSno
            }).ToList();
            objectresponse.ResultSet = new
            {
                itemMaster = itemMaster,
                SuppliersMaster = SuppliersMaster,
                unitMasters = unitMasters,                
                jobMasters = jobMasters,
                currencyMasters = currencyMasters,
                masterAccountsTables = masterAccountsTables,
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

    }
}