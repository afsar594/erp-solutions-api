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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
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
        private IRepository<UnitMaster> unitrepository; IRepository<UnitDetails> unitDetailRepository;
        private IRepository<CostCenterMaster> costcenterrepository; private IRepository<VendorMaster> Brandrepository;
        private IRepository<JobMaster> jobrepository; private IRepository<LocationMaster> locationrepository;
        private IRepository<CurrencyMaster> currencyrepository; private IRepository<AccountSettings> _accountsSettingsRepo;
        private IChartofAccountsService chartofAccountsService;
        private IRepository<MasterAccountsTable> chartofAccountsrepository;

        public GRNController(IGRNService grn, IMapper mapper, IRepository<ItemMaster> _itemrepository,
            IRepository<SuppliersMaster> _supplierrepository, IRepository<VendorMaster> _Brandrepository,
            IRepository<UnitMaster> _unitrepository, IRepository<UnitDetails> _unitDetailRepository, IRepository<LocationMaster> _locationrepository
            , IRepository<CostCenterMaster> _countryrepository, IRepository<AccountSettings> accountsSettingsRepo
            , IRepository<JobMaster> _jobrepository, IRepository<CurrencyMaster> _currencyrepository, IChartofAccountsService _chartofAccountsService,
            IRepository<MasterAccountsTable> _chartofAccountsrepository
            )
        {
            _grn = grn;
            _mapper = mapper;
            supplierrepository = _supplierrepository; Brandrepository = _Brandrepository;
            itemrepository = _itemrepository; unitrepository = _unitrepository; unitDetailRepository = _unitDetailRepository;
            costcenterrepository = _countryrepository; _accountsSettingsRepo = accountsSettingsRepo;
            jobrepository = _jobrepository; locationrepository = _locationrepository;
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
        public IActionResult GetByID(int id)
        {
            try
            {
                return Ok(_grn.GetByID(id));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("saveGRNVoucher")]
        public ApiResponse<PurchaseVoucher> saveGRN([FromBody] PurchaseVoucher obj)
        {
            var x = _grn.saveGRN(obj);
            if (x != null)
            {
                ApiResponse<PurchaseVoucher> apiResponse = new ApiResponse<PurchaseVoucher>
                {
                    Valid = true,
                    Result = x,
                    Message = GRNMessage.SaveVoucher,
                };
                return apiResponse;
            }
            else
                return null;
            //return Ok(_grn.saveGRN(obj));
        }

        [HttpPost("updateGRN")]
        public ApiResponse<PurchaseVoucher> updateGRN([FromBody] PurchaseVoucher obj)
        {
            var x = _grn.updateGRN(obj, obj.AccountsTransactions.ToList());
            if (x != null)
            {
                ApiResponse<PurchaseVoucher> apiResponse = new ApiResponse<PurchaseVoucher>
                {
                    Valid = true,
                    Result = x,
                    Message = GRNMessage.UpdateVoucher,
                };
                return apiResponse;
            }
            else
                return null;
            //return Ok(_grn.updateGRN(obj, obj.AccountsTransactions.ToList()));
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
            var itemMaster = (from item in itemrepository.GetAsQueryable().AsNoTracking()
                              join unit in unitDetailRepository.GetAsQueryable().AsNoTracking()
                              on item.ItemMasterItemId equals unit.UnitDetailsItemId
                              where item.ItemMasterAccountNo != 0
                              && item.ItemMasterDelStatus != true
                              && item.ItemMasterItemType != ItemMasterStatus.Group
                              select new
                              {
                                  item.ItemMasterItemId,
                                  item.ItemMasterItemName,
                                  item.ItemMasterVenderId,
                                  item.ItemMasterItemSize,
                                  itemMasterBarcode = unit.UnitDetailsBarcode ?? ""
                              }).Distinct().OrderBy(x => x.ItemMasterItemName).AsEnumerable().ToList();

            var SuppliersMaster = supplierrepository.GetAsQueryable().AsNoTracking().Where(a => a.SuppliersMasterSupplierDelStatus != true).Select(c => new
            {
                c.SuppliersMasterSupplierId,
                c.SuppliersMasterSupplierName,
                c.SuppliersMasterSupplierVatNo,
                c.SuppliersMasterSupplierReffAcNo,
                c.SuppliersMasterSupplierAddress,
                c.SuppliersMasterSupplierContactPerson,
                c.SuppliersMasterSupplierEmail,
                c.SuppliersMasterSupplierMobile,
                c.SuppliersMasterSupplierTel1,
            }).OrderBy(x => x.SuppliersMasterSupplierName).AsEnumerable().ToList();

            var unitMasters = unitrepository.GetAsQueryable().AsNoTracking().Where(a => a.UnitMasterUnitDelStatus != true).Select(x => new
            {
                x.UnitMasterUnitId,
                UnitMasterUnitFullName = x.UnitMasterUnitFullName.Trim(),
                UnitMasterUnitShortName = x.UnitMasterUnitShortName.Trim(),
            }).OrderBy(x => x.UnitMasterUnitFullName).AsEnumerable().ToList();

            var jobMasters = jobrepository.GetAsQueryable().AsNoTracking().Where(a => a.JobMasterJobDelStatus != true).Select(c => new
            {
                c.JobMasterJobId,
                c.JobMasterJobName,
                c.JobMasterJobNumber,
            }).OrderBy(x => x.JobMasterJobName).AsEnumerable().ToList();
            var currencyMasters = currencyrepository.GetAsQueryable().AsNoTracking().Where(a => a.CurrencyMasterCurrencyDelStatus != true).Select(c => new
            {
                c.CurrencyMasterCurrencyId,
                c.CurrencyMasterCurrencyName,
                c.CurrencyMasterCurrencyRate
            }).OrderBy(x => x.CurrencyMasterCurrencyName).AsEnumerable().ToList();
            var masterAccountsTables = chartofAccountsService.GetAllAccounts().Where(a => a.MaDelStatus != true).Select(c => new
            {
                c.MaAccNo,
                c.MaAccName,
                c.MaRelativeNo,
                c.MaSno
            }).OrderBy(x => x.MaAccName).AsEnumerable().ToList();
            var costcenterMasters = costcenterrepository.GetAsQueryable().AsNoTracking().Where(a => a.CostCenterMasterCostCenterDelStatus != true).Select(c => new
            {
                c.CostCenterMasterCostCenterId,
                c.CostCenterMasterCostCenterName,
            }).OrderBy(x => x.CostCenterMasterCostCenterName).AsEnumerable().ToList();
            var LocationMaster = locationrepository.GetAsQueryable().AsNoTracking().Where(a => a.LocationMasterLocationDelStatus != true).Select(c => new
            {
                c.LocationMasterLocationId,
                c.LocationMasterLocationName,
                c.LocationMasterLocationStatus,
            }).OrderBy(x => x.LocationMasterLocationName).AsEnumerable().ToList();
            var BrandMasters = Brandrepository.GetAsQueryable().AsNoTracking().Where(a => a.VendorMasterVendorDelStatus != true).Select(c => new
            {
                c.VendorMasterVendorId,
                c.VendorMasterVendorName,
            }).OrderBy(x => x.VendorMasterVendorName).AsEnumerable().ToList();
            var accountsSettings = _accountsSettingsRepo.GetAsQueryable().AsNoTracking().Where(a => a.AccountSettingsAccountDelStatus != true).Select(c => new
            {
                c.AccountSettingsAccountId,
                AccountSettingsAccountDescription = c.AccountSettingsAccountDescription.Trim(),
                c.AccountSettingsAccountTextValue
            }).OrderBy(x => x.AccountSettingsAccountTextValue).AsEnumerable().ToList();

            objectresponse.ResultSet = new
            {
                itemMaster = itemMaster,
                SuppliersMaster = SuppliersMaster,
                unitMasters = unitMasters,
                jobMasters = jobMasters,
                currencyMasters = currencyMasters,
                masterAccountsTables = masterAccountsTables,
                costcenterMasters = costcenterMasters,
                LocationMaster = LocationMaster,
                accountsSettings = accountsSettings,
                BrandMasters = BrandMasters
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdown_NWE")]
        public ResponseInfo LoadDropdown_NEW()
        {
            var objectresponse = new ResponseInfo();
            var itemMaster = LoadDropdownItemMaster();
            var SuppliersMaster = LoadDropdownSuppliersMaster();
            var unitMasters = LoadDropdownunitMasters();
            var jobMasters = LoadDropdowjobMasters();
            var currencyMasters = LoadDropdowCurrencyMaster();
            var masterAccountsTables = LoadDropdowchartofAccounts();
            var costcenterMasters = LoadDropdowncostcenterMasters();
            var LocationMaster = LoadDropdownLocationMaster();
            var BrandMasters = LoadDropdownBrandMasters();
            var accountsSettings = LoadDropdownaccountsSettings();
            objectresponse.ResultSet = new
            {
                itemMaster = itemMaster,
                SuppliersMaster = SuppliersMaster,
                unitMasters = unitMasters,
                jobMasters = jobMasters,
                currencyMasters = currencyMasters,
                masterAccountsTables = masterAccountsTables,
                costcenterMasters = costcenterMasters,
                LocationMaster = LocationMaster,
                accountsSettings = accountsSettings,
                BrandMasters = BrandMasters
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }
        [HttpGet]
        [Route("LoadDropdownItemGroupMaster")]
        public ResponseInfo LoadDropdownItemGroupMaster()
        {
            var objectresponse = new ResponseInfo();
            var itemGroup = itemrepository.GetAsQueryable().Where(k => k.ItemMasterAccountNo == 0 && (k.ItemMasterDelStatus != true)
                                  && k.ItemMasterItemType == ItemMasterStatus.Group).Select(k => new
                                   {
                                     k.ItemMasterItemId,
                                     k.ItemMasterItemName,
                                   }).OrderBy(x => x.ItemMasterItemName).ToList();

            objectresponse.ResultSet = new
            {
                itemGroup = itemGroup
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdownItemMaster")]
        public ResponseInfo LoadDropdownItemMaster()
        {
            var objectresponse = new ResponseInfo();
            var itemMaster = (from item in itemrepository.GetAsQueryable().AsNoTracking()
                              join unit in unitDetailRepository.GetAsQueryable().AsNoTracking()
                              on item.ItemMasterItemId equals unit.UnitDetailsItemId
                              where item.ItemMasterAccountNo != 0
                              && item.ItemMasterDelStatus != true
                              && item.ItemMasterItemType != ItemMasterStatus.Group
                              select new
                              {
                                  item.ItemMasterItemId,
                                  item.ItemMasterItemName,
                                  item.ItemMasterVenderId,
                                  item.ItemMasterItemSize,
                                  itemMasterBarcode = unit.UnitDetailsBarcode ?? ""
                              }).Distinct().OrderBy(x => x.ItemMasterItemName).AsEnumerable().ToList();

            objectresponse.ResultSet = new
            {
                itemMaster = itemMaster
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdownSuppliersMaster")]
        public ResponseInfo LoadDropdownSuppliersMaster()
        {
            var objectresponse = new ResponseInfo();
            var SuppliersMaster = supplierrepository.GetAsQueryable().AsNoTracking().Where(a => a.SuppliersMasterSupplierDelStatus != true).Select(c => new
            {
                c.SuppliersMasterSupplierId,
                c.SuppliersMasterSupplierName,
                c.SuppliersMasterSupplierVatNo,
                c.SuppliersMasterSupplierReffAcNo,
                c.SuppliersMasterSupplierAddress,
                c.SuppliersMasterSupplierContactPerson,
                c.SuppliersMasterSupplierEmail,
                c.SuppliersMasterSupplierMobile,
                c.SuppliersMasterSupplierTel1,
            }).OrderBy(x => x.SuppliersMasterSupplierName).AsEnumerable().ToList();
            objectresponse.ResultSet = new
            {
                suppliersMaster = SuppliersMaster
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdownunitMasters")]
        public ResponseInfo LoadDropdownunitMasters()
        {
            var objectresponse = new ResponseInfo();
            var unitMasters = unitrepository.GetAsQueryable().AsNoTracking().Where(a => a.UnitMasterUnitDelStatus != true).Select(x => new
            {
                x.UnitMasterUnitId,
                UnitMasterUnitFullName = x.UnitMasterUnitFullName.Trim(),
                UnitMasterUnitShortName = x.UnitMasterUnitShortName.Trim(),
            }).OrderBy(x => x.UnitMasterUnitFullName).AsEnumerable().ToList();

            objectresponse.ResultSet = new
            {
                unitMasters = unitMasters
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }


        [HttpGet]
        [Route("LoadDropdowjobMasters")]
        public ResponseInfo LoadDropdowjobMasters()
        {
            var objectresponse = new ResponseInfo();
            var jobMasters = jobrepository.GetAsQueryable().AsNoTracking().Where(a => a.JobMasterJobDelStatus != true).Select(c => new
            {
                c.JobMasterJobId,
                c.JobMasterJobName,
                c.JobMasterJobNumber,
            }).OrderBy(x => x.JobMasterJobName).AsEnumerable().ToList();
            objectresponse.ResultSet = new
            {
                jobMasters = jobMasters
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdowCurrencyMaster")]
        public ResponseInfo LoadDropdowCurrencyMaster()
        {
            var objectresponse = new ResponseInfo();
            var currencyMasters = currencyrepository.GetAsQueryable().AsNoTracking().Where(a => a.CurrencyMasterCurrencyDelStatus != true).Select(c => new
            {
                c.CurrencyMasterCurrencyId,
                c.CurrencyMasterCurrencyName,
                c.CurrencyMasterCurrencyRate
            }).OrderBy(x => x.CurrencyMasterCurrencyName).AsEnumerable().ToList();
            objectresponse.ResultSet = new
            {
                CurrencyMasters = currencyMasters
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdowchartofAccounts")]
        public ResponseInfo LoadDropdowchartofAccounts()
        {
            var objectresponse = new ResponseInfo();

           

            var masterAccountsTables = chartofAccountsService.GetAllAccounts().Where(a => a.MaDelStatus != true && a.MaAccType == "A" ).Select(c => new
            {
                c.MaAccNo,
                c.MaAccName,
                c.MaRelativeNo,
                c.MaSno
            }).OrderBy(x => x.MaAccName).AsEnumerable().ToList();
            objectresponse.ResultSet = new
            {
                MasterAccountsTables = masterAccountsTables
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }
        [HttpGet]
        [Route("LoaAccountsforbank")]
        public ResponseInfo LoaAccountsforbank()
        {
            var objectresponse = new ResponseInfo();
            var bankacc = _accountsSettingsRepo.GetAsQueryable().AsNoTracking().Where(a => a.AccountSettingsAccountKeyValue.Trim() == "BANK_MASTER_AC_NO").Select(x => x.AccountSettingsAccountTextValue).FirstOrDefault();
            string[] parts = bankacc.Split(new string[] { "::" }, StringSplitOptions.None);

            string accountNumber = parts.Length > 1 ? parts[1].Trim() : string.Empty;

            var masterAccountsTables = chartofAccountsService.GetAllAccounts()
                .Where(ma => chartofAccountsService.GetAllAccounts()
                    .Where(m => m.MaAccNo == accountNumber && (m.MaDelStatus == false || m.MaDelStatus == null))
                    .Select(m => m.MaAccNo)
                    .Contains(ma.MaRelativeNo))
                  .Select(ma => new
                  {
                      ma.MaAccNo,
                      ma.MaAccName,
                      ma.MaRelativeNo,
                      ma.MaSno
                  })
                 .ToList();              

            objectresponse.ResultSet = new
            {
                MasterAccountsTables = masterAccountsTables
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }
        [HttpGet]
        [Route("LoaAccountsforcash")]
        public ResponseInfo LoaAccountsforcash()
        {
            var objectresponse = new ResponseInfo();

            var cashacc = _accountsSettingsRepo.GetAsQueryable().AsNoTracking().Where(a => a.AccountSettingsAccountKeyValue.Trim() == "CASH_MASTER_AC_NO").Select(x => x.AccountSettingsAccountTextValue).FirstOrDefault();
            string[] parts = cashacc.Split(new string[] { "::" }, StringSplitOptions.None);

            string accountNumber = parts.Length > 1 ? parts[1].Trim() : string.Empty;
            var masterAccountsTables = chartofAccountsService.GetAllAccounts()
                .Where(ma => chartofAccountsService.GetAllAccounts()
                    .Where(m => m.MaAccNo == accountNumber && (m.MaDelStatus == false || m.MaDelStatus == null))
                    .Select(m => m.MaAccNo)
                    .Contains(ma.MaRelativeNo))
                 .Select(ma => new
                 {
                     ma.MaAccNo,
                     ma.MaAccName,
                     ma.MaRelativeNo,
                     ma.MaSno
                 })
                 .ToList();
          
            objectresponse.ResultSet = new
            {
                MasterAccountsTables = masterAccountsTables
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }
        [HttpGet]
        [Route("LoadDropdowncostcenterMasters")]
        public ResponseInfo LoadDropdowncostcenterMasters()
        {
            var objectresponse = new ResponseInfo();
            var costcenterMasters = costcenterrepository.GetAsQueryable().AsNoTracking().Where(a => a.CostCenterMasterCostCenterDelStatus != true).Select(c => new
            {
                c.CostCenterMasterCostCenterId,
                c.CostCenterMasterCostCenterName,
            }).OrderBy(x => x.CostCenterMasterCostCenterName).AsEnumerable().ToList();
            objectresponse.ResultSet = new
            {
                CostcenterMasters = costcenterMasters
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdownLocationMaster")]
        public ResponseInfo LoadDropdownLocationMaster()
        {
            var objectresponse = new ResponseInfo();
            var LocationMaster = locationrepository.GetAsQueryable().AsNoTracking().Where(a => a.LocationMasterLocationDelStatus != true).Select(c => new
            {
                c.LocationMasterLocationId,
                c.LocationMasterLocationName,
                c.LocationMasterLocationStatus,
            }).OrderBy(x => x.LocationMasterLocationName).AsEnumerable().ToList();
            objectresponse.ResultSet = new
            {
                LocationMaster = LocationMaster
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdownBrandMasters")]
        public ResponseInfo LoadDropdownBrandMasters()
        {
            var objectresponse = new ResponseInfo();

            var BrandMasters = Brandrepository.GetAsQueryable().AsNoTracking().Where(a => a.VendorMasterVendorDelStatus != true).Select(c => new
            {
                c.VendorMasterVendorId,
                c.VendorMasterVendorName,
            }).OrderBy(x => x.VendorMasterVendorName).AsEnumerable().ToList();
            objectresponse.ResultSet = new
            {
                BrandMasters = BrandMasters
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }

        [HttpGet]
        [Route("LoadDropdownaccountsSettings")]
        public ResponseInfo LoadDropdownaccountsSettings()
        {
            var objectresponse = new ResponseInfo();


            var accountsSettings = _accountsSettingsRepo.GetAsQueryable().AsNoTracking().Where(a => a.AccountSettingsAccountDelStatus != true).Select(c => new
            {
                c.AccountSettingsAccountId,
                AccountSettingsAccountDescription = c.AccountSettingsAccountDescription.Trim(),
                c.AccountSettingsAccountTextValue
            }).OrderBy(x => x.AccountSettingsAccountTextValue).AsEnumerable().ToList();
            objectresponse.ResultSet = new
            {
                accountsSettings = accountsSettings
            };

            objectresponse.IsSuccess = true;
            return objectresponse;
        }


        [HttpGet]
        [Route("GetSupplierById/{id}")]
        public ResponseInfo GetSupplierById(int id)
        {
            var obj = new ResponseInfo();
            var supplier = supplierrepository.GetAsQueryable().Where(a => a.SuppliersMasterSupplierDelStatus != true && a.SuppliersMasterSupplierId == id).Select(c => new
            {
                c.SuppliersMasterSupplierId,
                c.SuppliersMasterSupplierName,
                SuppliersMasterSupplierVatNo = c.SuppliersMasterSupplierVatNo.Trim() ?? "",
                SuppliersMasterSupplierEmail = c.SuppliersMasterSupplierEmail.Trim() ?? "",
                SuppliersMasterSupplierFax = c.SuppliersMasterSupplierFax.Trim() ?? "",
                SuppliersMasterSupplierWebSite = c.SuppliersMasterSupplierWebSite.Trim() ?? "",
                SuppliersMasterSupplierContactPerson = c.SuppliersMasterSupplierContactPerson.Trim() ?? "",
                SuppliersMasterSupplierTel1 = c.SuppliersMasterSupplierTel1.Trim() ?? "",
                SuppliersMasterSupplierAddress = c.SuppliersMasterSupplierAddress.Trim() ?? "",
                SuppliersMasterSupplierReffAcNo = c.SuppliersMasterSupplierReffAcNo.Trim() ?? ""
            }).FirstOrDefault();

            obj.IsSuccess = true;
            obj.ResultSet = new
            {
                result = supplier
            };
            return obj;
        }

    }
}