using Inspire.Erp.Application.Procurement.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Procurement
{
    
    [Route("api/PurchaseRequisitionReports")]
    [Produces("application/json")]
    [ApiController]
    public class PurchaseRequisitionReportsController : Controller
    {

        IPurchaseRequisitionStatusReport purchaseRequisitionStatus;
        private IRepository<ItemMaster> itemrepository; private IRepository<UnitMaster> unitrepository;
        private readonly IRepository<SuppliersMaster> supplierrepository; private IRepository<JobMaster> jobrepository;
        private IRepository<LocationMaster> locationrepository;
        private IRepository<CurrencyMaster> currencyrepository;

        public PurchaseRequisitionReportsController(IPurchaseRequisitionStatusReport purchaseRequisitionStatus, IRepository<SuppliersMaster> _supplierrepository, IRepository<ItemMaster> _itemrepository
            , IRepository<UnitMaster> _unitrepository, IRepository<JobMaster> _jobrepository, IRepository<LocationMaster> _locationrepository, IRepository<CurrencyMaster> _currencyrepository)
        {
            this.purchaseRequisitionStatus = purchaseRequisitionStatus;
            supplierrepository = _supplierrepository; itemrepository = _itemrepository; unitrepository = _unitrepository; jobrepository = _jobrepository;
            locationrepository = _locationrepository;
            currencyrepository = _currencyrepository;
        }

        [HttpPost]
        [Route("GetPurchaseRequisitionStatusReport")]
        public async Task<IActionResult> GetPurchaseRequisitionStatusReport(ReportFilter model)
        {
            return Ok(await this.purchaseRequisitionStatus.GetPurchaseRequisitionStatusReport(model));
        }

        [HttpGet]
        [Route("RequisitionReportDropdown")]
        public ResponseInfo RequisitionReportDropdown()
        {
            var obj = new ResponseInfo();

            var itemMaster = itemrepository.GetAsQueryable().AsNoTracking().Where(a => a.ItemMasterDelStatus != true && a.ItemMasterItemType != ItemMasterStatus.Group).Select(c => new
            {
                c.ItemMasterItemId,
                c.ItemMasterItemName
            }).OrderBy(x => x.ItemMasterItemName).ToList();
            var itemMasterGroup = itemrepository.GetAsQueryable().AsNoTracking().Where(a => a.ItemMasterDelStatus != true && a.ItemMasterItemType == ItemMasterStatus.Group).Select(c => new
            {
                c.ItemMasterItemId,
                c.ItemMasterItemName
            }).OrderBy(x => x.ItemMasterItemName).ToList();

            var suppliersMaster = supplierrepository.GetAsQueryable().AsNoTracking().Where(a => a.SuppliersMasterSupplierDelStatus != true).Select(c => new
            {
                c.SuppliersMasterSupplierId,
                c.SuppliersMasterSupplierName,
            }).OrderBy(x => x.SuppliersMasterSupplierName).ToList();

            var unitMasters = unitrepository.GetAsQueryable().AsNoTracking().Where(a => a.UnitMasterUnitDelStatus != true).Select(x => new
            {
                x.UnitMasterUnitId,
                UnitMasterUnitFullName = x.UnitMasterUnitFullName.Trim(),
                UnitMasterUnitShortName = x.UnitMasterUnitShortName.Trim(),
            }).OrderBy(x => x.UnitMasterUnitShortName).ToList();

            var jobMasters = jobrepository.GetAsQueryable().AsNoTracking().Where(a => a.JobMasterJobDelStatus != true).Select(c => new
            {
                c.JobMasterJobId,
                c.JobMasterJobName,
            }).OrderBy(x => x.JobMasterJobName).ToList();

            var currencyMasters = currencyrepository.GetAsQueryable().AsNoTracking().Where(a => a.CurrencyMasterCurrencyDelStatus != true).Select(c => new
            {
                c.CurrencyMasterCurrencyId,
                c.CurrencyMasterCurrencyName,
            }).OrderBy(x => x.CurrencyMasterCurrencyName).ToList();
            var LocationMaster = locationrepository.GetAsQueryable().AsNoTracking().Where(a => a.LocationMasterLocationDelStatus != true).Select(c => new
            {
                c.LocationMasterLocationId,
                c.LocationMasterLocationStatus,
                c.LocationMasterLocationName,
            }).OrderBy(x => x.LocationMasterLocationName).ToList();
            obj.ResultSet = new
            {
                itemMaster = itemMaster,
                itemMasterGroup = itemMasterGroup,
                suppliersMaster = suppliersMaster,
                unitMasters = unitMasters,
                jobMasters = jobMasters,
                currencyMasters = currencyMasters,
                LocationMaster = LocationMaster
            };

            return obj;

        }
    }
}
