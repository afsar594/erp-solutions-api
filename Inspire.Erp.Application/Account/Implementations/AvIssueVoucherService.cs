using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Domain.DTO;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Models.ViewModels;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Domain.Modals.Common;

namespace Inspire.Erp.Application.Account.Implementations
{
    public class AvIssueVoucherService : IAvIssueVoucherService
    {
        private readonly IRepository<AvIssueVoucher> _AvIssueVoucher;
        private readonly IRepository<AvIssueVoucherDetails> _AvIssueVoucherDetails;
        private readonly IRepository<ItemMaster> _ItemMaster;
        private readonly IRepository<JobMaster> _JobMaster;
        private readonly IRepository<IssueWithStockTransfer> _IssueWithStockTransfer;
        private readonly IRepository<StockTransferDetailsJobWise> _StockTransferDetailsJobWise;
        private readonly IRepository<StockTransferJobWise> _StockTransferJobWise;
        private readonly InspireErpDBContext _context;

        public AvIssueVoucherService(IRepository<AvIssueVoucher> AvIssueVoucher, IRepository<AvIssueVoucherDetails> AvIssueVoucherDetails,
        IRepository<ItemMaster> ItemMaster, IRepository<JobMaster> JobMaster, IRepository<IssueWithStockTransfer> IssueWithStockTransfer,
        IRepository<StockTransferDetailsJobWise> StockTransferDetailsJobWise, IRepository<StockTransferJobWise> StockTransferJobWise,
        InspireErpDBContext context)
        {
            _AvIssueVoucher = AvIssueVoucher;
            _AvIssueVoucherDetails = AvIssueVoucherDetails;
            _ItemMaster = ItemMaster;
            _JobMaster = JobMaster;
            _IssueWithStockTransfer = IssueWithStockTransfer;
            _StockTransferDetailsJobWise = StockTransferDetailsJobWise;
            _StockTransferJobWise = StockTransferJobWise;
            _context = context;
        }

        public ResponseInfo GetDetailWiseReport(DTOIssueReportParam model)
        {
            try
            {
                var objresponse = new ResponseInfo();
                var StockTransferDetailsJobWiseList = _context.StockTransferDetailsJobWise.ToList();
                var StockTransferJobWiseList = _context.StockTransferJobWise.ToList();
                var ItemMasterList = _context.ItemMaster.ToList();
                var data = (from dtl in _context.IssueVoucherDetails.ToList()
                            join hdr in _context.IssueVoucher.ToList()
                                on dtl.IssueVoucherDetailsNo equals hdr.IssueVoucherNo
                            join itemmaster in ItemMasterList
                                on (long?)dtl.IssueVoucherDetailsMatId equals itemmaster.ItemMasterItemId into itm
                            from immaster in itm.DefaultIfEmpty()
                            select new DTOIssueReport()
                            {
                                JobId = hdr.IssueVoucherJobId,
                                DepartmentId = hdr.IssueVoucherDepartmentId != null ? (int?)hdr.IssueVoucherDepartmentId : null,
                                ItemId = immaster != null ? (long?)immaster.ItemMasterItemId : null,
                                IssueNo = hdr.IssueVoucherNo,
                                IssueDate = hdr.IssueVoucherDate,
                                PartNo = immaster != null ? immaster.ItemMasterPartNo : "",
                                ItemName = immaster != null ? immaster.ItemMasterItemName : "",
                                Quantity = dtl.IssueVoucherDetailsQuantity != null ? (int?)dtl.IssueVoucherDetailsQuantity : 0,
                                Rate = dtl.IssueVoucherDetailsRate != null ? (int?)dtl.IssueVoucherDetailsRate : 0,
                                Amount = (double?)((dtl.IssueVoucherDetailsRate != null && dtl.IssueVoucherDetailsQuantity != null) ? (int?)dtl.IssueVoucherDetailsRate * dtl.IssueVoucherDetailsQuantity : 0)
                            }).OrderBy(o => o.ItemName).AsQueryable().ToList();

                var data2 = (from dtl in StockTransferDetailsJobWiseList
                             join hdr in StockTransferJobWiseList
                                 on dtl.StockTransferDetailsJobWiseSno equals hdr.StockTransferJobWiseStid into leftDetail
                             from StockTrans in leftDetail.DefaultIfEmpty()
                             join itemMaster in ItemMasterList
                                 on (long?)dtl.StockTransferDetailsJobWiseMaterialId equals itemMaster.ItemMasterItemId into left
                             from item in left.DefaultIfEmpty()
                             select new DTOIssueReport()
                             {
                                 JobId = StockTrans?.StockTransferJobWiseJobIdFrom ?? 0, // Safe null check
                                 DepartmentId = 0, // Defaulted to 0 as per original logic
                                 ItemId = item?.ItemMasterItemId, // Safe null check for itemMaster
                                 IssueNo = StockTrans != null ? StockTrans.StockTransferJobWiseStid.ToString() : "0", // Safe null check for StockTrans
                                 IssueDate = StockTrans?.StockTransferJobWiseDate, // Nullable, no conversion needed
                                 PartNo = StockTrans?.StockTransferJobWiseVno, // Safe null check
                                 ItemName = item?.ItemMasterItemName, // Safe null check for itemMaster
                                 Quantity = dtl.StockTransferDetailsJobWiseQty ?? 0, // Handles null Quantity
                                 Rate = dtl.StockTransferDetailsJobWiseRate ?? 0, // Handles null Rate
                                 Amount = (dtl.StockTransferDetailsJobWiseQty ?? 0) * (dtl.StockTransferDetailsJobWiseRate ?? 0) // Safe calculation
                             }).AsQueryable().ToList();


                var data3 = (from dtl in StockTransferDetailsJobWiseList
                             join hdr in StockTransferJobWiseList
                                 on dtl.StockTransferDetailsJobWiseSno equals hdr.StockTransferJobWiseStid into leftDetail
                             from StockTrans in leftDetail.DefaultIfEmpty()
                             join itemMaster in ItemMasterList
                                 on (long?)dtl.StockTransferDetailsJobWiseMaterialId equals itemMaster.ItemMasterItemId into left
                             from item in left.DefaultIfEmpty()
                             select new DTOIssueReport()
                             {
                                 JobId = StockTrans?.StockTransferJobWiseJobIdFrom ?? 0,
                                 DepartmentId = 0,
                                 ItemId = item?.ItemMasterItemId,
                                 IssueNo = StockTrans != null ? StockTrans.StockTransferJobWiseStid.ToString() : "0",
                                 IssueDate = StockTrans?.StockTransferJobWiseDate,
                                 PartNo = StockTrans?.StockTransferJobWiseVno,
                                 ItemName = item?.ItemMasterItemName,
                                 Quantity = (dtl.StockTransferDetailsJobWiseQty ?? 0) * -1,
                                 Rate = (dtl.StockTransferDetailsJobWiseRate ?? 0) * -1,
                                 Amount = (dtl.StockTransferDetailsJobWiseQty ?? 0) * (dtl.StockTransferDetailsJobWiseRate ?? 0)
                             }).AsQueryable().ToList();


                var finalData = data.Union(data2).Union(data3).AsQueryable().ToList();

                if (finalData != null && finalData.Count() > 0)
                {
                    finalData = finalData.Where(o => (model.itemId == null ? true : o.ItemId == model.itemId) && (model.partNo == "" ? true : o.PartNo == model.partNo) && (model.departmentId == null ? true : o.DepartmentId == model.departmentId)
                    && (model.jobId == null ? true : o.JobId == model.jobId)).AsQueryable().ToList();

                    if (model.isDate == true)
                    {
                        finalData = finalData.Where(a => a.IssueDate >= model.dateFrom && a.IssueDate <= model.toDate).ToList();
                    }
                }
                Console.WriteLine("**********************" + finalData.Count());
                objresponse.ResultSet = finalData;
                return objresponse;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public ResponseInfo GetJobWiseReport(DTOIssueReportParam model)
        {
            try
            {
                var objresponse = new ResponseInfo();
                var data = (from dtl in _context.IssueVoucherDetails
                            join hdr in _context.IssueVoucher on dtl.IssueVoucherDetailsNo equals hdr.IssueVoucherNo
                            join itemmaster in _ItemMaster.GetAll() on (long)dtl.IssueVoucherDetailsMatId equals itemmaster.ItemMasterItemId
                            into itm
                            from immaster in itm.DefaultIfEmpty()
                            join jobmaster in _JobMaster.GetAll() on hdr.IssueVoucherJobId equals jobmaster.JobMasterJobId
                            into jobm
                            from jobmas in jobm.DefaultIfEmpty()
                            select new
                            {
                                IssueNo = hdr.IssueVoucherNo,
                                IssueDate = hdr.IssueVoucherDate,
                                PartNo = immaster != null ? immaster.ItemMasterPartNo : "",
                                ItemName = immaster != null ? immaster.ItemMasterItemName : "",
                                Quantity = dtl.IssueVoucherDetailsQuantity,
                                Rate = dtl.IssueVoucherDetailsRate,
                                Amount = dtl.IssueVoucherDetailsRate * dtl.IssueVoucherDetailsQuantity,
                                JobName = jobmas != null ? jobmas.JobMasterJobName : "",
                                JobNumber = jobmas != null ? jobmas.JobMasterJobNo : "",
                                JobId = hdr.IssueVoucherJobId,
                                RelativeNo = immaster != null ? immaster.ItemMasterRef1 : "",
                                ItemId = immaster != null ? (long?)immaster.ItemMasterItemId : null,
                                DepId = hdr.IssueVoucherDepartmentId,
                                CostCenter = hdr.IssueVoucherCostCenterId,
                                Services = immaster != null ? immaster.ItemMasterServices : false,
                            }).AsQueryable().ToList();

                if (data != null && data.Count() > 0)
                {
                    data = data.Where(o => (model.itemId == null ? true : o.ItemId == model.itemId) && (model.partNo == "" ? true : o.PartNo == model.partNo) && (model.departmentId == null ? true : o.DepId == model.departmentId)
                    && (model.jobId == null ? true : o.JobId == model.jobId)).AsQueryable().ToList();
                    if (model.isDate == true)
                    {
                        data = data.Where(a => a.IssueDate >= model.dateFrom && a.IssueDate <= model.toDate).ToList();
                    }
                }
                objresponse.ResultSet = data;
                return objresponse;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable GetSummaryWiseReport(long? itemId, string partNo, long? departmentId, long? jobId, DateTime? dateFrom, DateTime? toDate)
        {
            try
            {
                var data = _IssueWithStockTransfer.GetAll().Where(o => o.AV_Issue_Voucher_AVSV_NO == null && o.Item_Master_Item_ID == 714).Select(o => new
                {
                    SV_NO = o.AV_Issue_Voucher_AVSV_NO,
                    SV_Dt = o.AV_Issue_Voucher_AVSV_Date,
                    PartNo = o.PartNo,
                    ItemName = o.Item_Master_Item_Name,
                    Quantity = o.Quantity,
                    Rate = o.AV_Issue_Voucher_Details_Rate,
                    Amount = o.Amount,
                    JobNumber = o.Job_Master_Job_Number,
                    JobName = o.Job_Master_Job_Number,
                    DepId = o.DepId,
                    ItemId = o.Item_Master_Item_ID,
                    JobId = o.AV_Issue_Voucher_Job_ID
                }).OrderBy(o => o.SV_NO).ThenBy(o => o.SV_Dt).ThenBy(o => o.JobName).ThenBy(o => o.JobNumber).AsQueryable();
                if (data != null && data.Count() > 0)
                {
                    data = data.Where(o => (itemId == null ? true : o.ItemId == itemId) && (partNo == "" ? true : o.PartNo == partNo) && (departmentId == null ? true : o.DepId == departmentId)
                    && (jobId == null ? true : o.JobId == jobId)).AsQueryable();
                }
                return data;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}