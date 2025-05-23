using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Application.Store.Interface;
using Inspire.Erp.Domain.DTO;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Store.Implementation
{
    public class StockTransferService : IStockTransferService
    {
        private readonly IRepository<StockTransfer> _StockTransferRepo;
        private readonly IRepository<VouchersNumbers> _VoucherNumberRepo;
        private readonly IRepository<StockItemResponse> _StockItemResponse;
        private readonly IRepository<StockTransferResponseModel> _StockTransferResponse;
        private readonly IRepository<StockTransfer> _StockTransferResp;
        private readonly IRepository<StockTransferDetails> _StockTransferDetailRepo;
        private readonly IRepository<StockRegister> _StockRegisterRepo;
        private readonly IRepository<ItemMaster> _itemMasterRepo;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private static string prefix;
        private InspireErpDBContext _context; private IRepository<UnitDetails> _UnitDetailsRepository;
        private IRepository<ItemMaster> _itemMasterRepository;
        private readonly IUtilityService _utilityService;
        public StockTransferService(IRepository<StockTransfer> StockTransferRepo,
            IRepository<StockTransferDetails> StockTransferDetailRepo,
            IRepository<StockItemResponse> stockItemResponse, IRepository<UnitDetails> unitDetailsRepository,
            IRepository<StockTransferResponseModel> StockTransferResponse,
            IRepository<VouchersNumbers> VoucherNumberRepo,
            IRepository<StockRegister> StockRegisterRepo,
            IRepository<ProgramSettings> programsettingsRepository,
            InspireErpDBContext context,
            IRepository<ItemMaster> itemMasterRepository,
            IUtilityService utilityService, IRepository<ItemMaster> itemMasterRepo, IRepository<StockTransfer> StockTransferResp)
        {
            _StockRegisterRepo = StockRegisterRepo;
            _VoucherNumberRepo = VoucherNumberRepo;
            _StockItemResponse = stockItemResponse;
            _StockTransferResponse = StockTransferResponse;
            _StockTransferDetailRepo = StockTransferDetailRepo;
            _StockTransferRepo = StockTransferRepo;
            _programsettingsRepository = programsettingsRepository;
            _context = context; _UnitDetailsRepository = unitDetailsRepository;
            prefix = _programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == "HIDE_STOCKTRANSFER").FirstOrDefault().ProgramSettingsTextValue;
            _itemMasterRepository = itemMasterRepository;
            _utilityService = utilityService;
            _itemMasterRepo = itemMasterRepo;
            _StockTransferResp = StockTransferResp;

        }

        public async Task<Response<decimal>> getStockItemResponse(int? itemId, int? locationId)
        {
            try
            {
                decimal currStock = 0;
                var list = _context.StockRegister.AsNoTracking();
                if (itemId > 0)
                {
                    list = list.Where(a => a.StockRegisterMaterialID == itemId);
                }
                if (locationId > 0)
                {
                    list = list.Where(a => a.StockRegisterLocationID == locationId);
                }
                currStock = list.Select(x => new
                {
                    x.StockRegisterMaterialID,
                    StockIn = x.StockRegisterSIN,
                    StockOut = x.StockRegisterSout
                }).Sum(x => (x.StockIn ?? 0) - (x.StockOut ?? 0));
                // List<StockItemResponse> results = currStock;
                //var results = await (from im in _context.ItemMaster.Where(a => a.ItemMasterDelStatus != true)
                //                     join sr in _context.StockRegister on im.ItemMasterItemId equals sr.StockRegisterMaterialID.GetValueOrDefault() into gim
                //                     from y in gim.DefaultIfEmpty()

                //                     where im.ItemMasterItemType == "A"
                //                     select new StockItemResponse
                //                     {

                //                         Item_Master_Item_Id = im.ItemMasterItemId,
                //                         Item_Master_Item_Name = im.ItemMasterItemName,
                //                         Item_Master_Batch_Code = im.ItemMasterBatchCode,
                //                         Item_Master_Part_No = im.ItemMasterPartNo,
                //                         Item_Master_Barcode = im.ItemMasterBarcode,
                //                         //Stock=0
                //                         //Stock= itemS.Any()?itemS.Sum(y => y.StockRegisterSIN??0 - y.StockRegisterSout??0):0
                //                         Stock = (y.StockRegisterSIN ?? 0 - y.StockRegisterSout ?? 0)

                //                     }).ToListAsync();
                return Response<decimal>.Success(currStock, "Data found");



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public IEnumerable<StockTransfer> GetStockTransfer()
        {
            IEnumerable<StockTransfer> stockTransfer_ALL = _StockTransferRepo.GetAll().Where(k => k.StockTransferDelStatus == false || k.StockTransferDelStatus == null);
            return stockTransfer_ALL;
        }
        public async Task<StockTransferModel> InsertStockTransfer(StockTransfer stockTransfer, List<StockTransferDetails> stockTransferDetails
             , List<StockRegister> stockRegister)
        {
            try
            {

                _StockTransferRepo.BeginTransaction();

                string openingstockVoucherNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.StockTransferVoucher_TYPE, Prefix.StockTransfer_Prefix, _programsettingsRepository, _VoucherNumberRepo);
                stockTransfer.StockTransferVoucherNo = openingstockVoucherNumber;


                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    await _StockTransferRepo.GetAsQueryable()
                    .DefaultIfEmpty().MaxAsync(o => o == null ? 0 : o.StockTransferId) + 1);

                stockTransfer.StockTransferId = maxcount;

                stockTransfer.StockTransferDetails = stockTransfer.StockTransferDetails.Select((x) =>
                {
                    x.StockTransferId = maxcount;
                    x.StockTransferDetailsSNo = openingstockVoucherNumber;

                    return x;
                }).OrderBy(x => x.StockTransferDetailsSNo).AsEnumerable().ToList();

                //stockRegister = stockRegister.Select((k) =>
                //{
                //    k.StockRegisterVoucherDate = stockTransfer.StockTransferStDate;
                //    k.StockRegisterRefVoucherNo = openingstockVoucherNumber;
                //    k.StockRegisterTransType = VoucherType.StockTransferVoucher_TYPE;
                //    k.StockRegisterStatus = AccountStatus.Approved;
                //    return k;
                //}).ToList();

                //_StockRegisterRepo.InsertList(stockRegister);

                //var currencyRate = clsCommonFunctions.getConverionCurrencyRate(stockTransfer.c, _currencyMasterRepository);
                //var rate = (decimal)currencyRate;

                var stockReg = new List<StockRegister>();
                foreach (var item in stockTransfer.StockTransferDetails)
                {
                    var avgrate = item.StockTransferDetailsCostPrice ?? 1;
                    var converontype = this.getConverionTypebyUnitId(item.StockTransferDetailsMaterialId, item.StockTransferDetailsUnitId);

                    stockReg.Add(new StockRegister()
                    {
                        StockRegisterPurchaseID = stockTransfer.StockTransferVoucherNo,
                        StockRegisterRefVoucherNo = stockTransfer.StockTransferVoucherNo,
                        StockRegisterVoucherDate = DateTime.Now.Date,
                        StockRegisterMaterialID = item.StockTransferDetailsMaterialId,
                        StockRegisterQuantity = (decimal)item.StockTransferDetailsQty * (decimal)converontype,
                        StockRegisterSIN = 0,
                        StockRegisterSout = (decimal)item.StockTransferDetailsQty * (decimal)(converontype),
                        StockRegisterRate = avgrate,
                        StockRegisterAmount = (decimal)item.StockTransferDetailsQty * (decimal)(converontype) * avgrate,
                        StockRegisterFCAmount = (decimal)item.StockTransferDetailsQty * (decimal)(converontype) * avgrate,
                        StockRegisterAssignedDate = DateTime.Now.Date,
                        StockRegisterStatus = "A",
                        StockRegisterTransType = VoucherType.StockTransferVoucher_TYPE,
                        StockRegisterUnitID = item.StockTransferDetailsUnitId,
                        StockRegisterLocationID = stockTransfer.StockTransferLocationIdFrom,
                        StockRegisterJobID = stockTransfer.StockTransferJobId,
                        StockRegisterDepID = (int?)stockTransfer.StockTransferDepId,
                        StockRegisterDelStatus = false
                    });

                    stockReg.Add(new StockRegister()
                    {
                        StockRegisterPurchaseID = stockTransfer.StockTransferVoucherNo,
                        StockRegisterRefVoucherNo = stockTransfer.StockTransferVoucherNo,
                        StockRegisterVoucherDate = DateTime.Now.Date,
                        StockRegisterMaterialID = item.StockTransferDetailsMaterialId,
                        StockRegisterQuantity = (decimal)item.StockTransferDetailsQty * (decimal)converontype,
                        StockRegisterSIN = (decimal)item.StockTransferDetailsQty * (decimal)(converontype),
                        StockRegisterSout = 0,
                        StockRegisterRate = avgrate,
                        StockRegisterAmount = (decimal)item.StockTransferDetailsQty * (decimal)(converontype) * avgrate,
                        StockRegisterFCAmount = (decimal)item.StockTransferDetailsQty * (decimal)(converontype) * avgrate,
                        StockRegisterAssignedDate = DateTime.Now.Date,
                        StockRegisterStatus = "A",
                        StockRegisterTransType = VoucherType.StockTransferVoucher_TYPE,
                        StockRegisterUnitID = item.StockTransferDetailsUnitId,
                        StockRegisterLocationID = stockTransfer.StockTransferLocationIdTo,
                        StockRegisterJobID = stockTransfer.StockTransferJobId,
                        StockRegisterDepID = (int?)stockTransfer.StockTransferDepId,
                        StockRegisterDelStatus = false
                    });
                }
                _StockRegisterRepo.InsertList(stockReg);

                _StockTransferRepo.Insert(stockTransfer);
                _StockTransferRepo.TransactionCommit();
                return this.GetSavedStockTransferDetails((int)stockTransfer.StockTransferId);


            }
            catch (Exception ex)
            {
                _StockTransferRepo.TransactionRollback();
                throw ex;


            }
        }
        public IEnumerable<StockTransfer> DeleteStockTransfers(string Id)
        {
            try
            {
                var dataObj = _StockTransferRepo.GetAsQueryable().Where(o => o.StockTransferVoucherNo == Id).FirstOrDefault();
                dataObj.StockTransferDelStatus = true;
                var detailData = _StockTransferDetailRepo.GetAsQueryable().Where(o => o.StockTransferDetailsSNo == Id).ToList();
                detailData.ForEach(elemt =>
                {
                    elemt.StockTransferDetailsDelStatus = true;
                });
                _StockTransferRepo.Update(dataObj);
                _StockTransferDetailRepo.UpdateList(detailData);
                //==----------
                clsCommonFunctions.Delete_OldEntryOf_StockRegister(dataObj.StockTransferVoucherNo, VoucherType.StockTransferVoucher_TYPE, _StockRegisterRepo);
                //clsCommonFunctions.Delete_OldEntry_vouchernumber(dataObj.StockTransferVoucherNo, VoucherType.StockTransferVoucher_TYPE, _VoucherNumberRepo);
                //==-----------------

                return _StockTransferRepo.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseInfo getStockitembyId(int id)
        {
            var obj = new ResponseInfo();
            var data = _StockRegisterRepo.GetAsQueryable().Where(i => i.StockRegisterMaterialID == id).Select(m => new
            {
                m.StockRegisterMaterialID,
                m.StockRegisterQuantity,
                m.StockRegisterRate,
                m.StockRegisterUnitID,
            }).FirstOrDefault();
            obj.IsSuccess = true;
            obj.ResultSet = data;
            return obj;
        }

        public List<stocktransferRegister> getStockitembyIds(int ids)
        {
            //List<stocktransferRegister> stockMaster;
            //try
            //{
            //    stockMaster = _StockRegisterRepo.GetAsQueryable()
            //        .Where(k => k.StockRegisterMaterialID == ids &&
            //        (k.StockRegisterDelStatus != true || k.StockRegisterDelStatus != null) &&
            //        k.StockRegisterTransType != ItemMasterStatus.Group).GroupBy(a => a.StockRegisterMaterialID)
            //        .Select(k => new stocktransferRegister
            //        {
            //            StockRegisterMaterialID =(int)k.Key,
            //            StockRegisterQuantity = (int)k.Sum(x => x.StockRegisterSIN - x.StockRegisterSout),
            //            StockRegisterRate =(int)k.FirstOrDefault().StockRegisterRate, 
            //            StockRegisterUnitID = (int)k.FirstOrDefault().StockRegisterUnitID
            //        })
            //        .AsEnumerable().OrderBy(x => x.StockRegisterMaterialID)
            //        .ToList();

            //    var allUnitDetails = _UnitDetailsRepository.GetAsQueryable().Where(a => a.UnitDetailsDelStatus != true && a.UnitDetailsItemId == ids)
            //        .Select(c=> new
            //        {
            //            c.UnitDetailsId,
            //            c.UnitDetailsUnitId,
            //            c.UnitDetailsRate,
            //            c.UnitDetailsConversionType,

            //        }).ToList();               
            //}
            List<stocktransferRegister> stockMaster;
            try
            {
                 stockMaster = _StockRegisterRepo.GetAsQueryable()
                    .Where(k =>
                        k.StockRegisterMaterialID == ids &&
                        (k.StockRegisterDelStatus != true || k.StockRegisterDelStatus == null) &&
                        k.StockRegisterTransType != ItemMasterStatus.Group)
                    .AsEnumerable().GroupBy(k => k.StockRegisterMaterialID ?? 0)
                    .Select(group => new stocktransferRegister
                    {
                        StockRegisterMaterialID = (int)group.Key,
                        StockRegisterQuantity = (int)group.Sum(x => x.StockRegisterSIN - x.StockRegisterSout),
                        StockRegisterRate = (int)(group.FirstOrDefault()?.StockRegisterRate ?? 0),
                        StockRegisterUnitID = (int)(group.FirstOrDefault()?.StockRegisterUnitID ?? 0)
                    })
                    .OrderBy(x => x.StockRegisterMaterialID).ToList();

                var allUnitDetails = _UnitDetailsRepository.GetAsQueryable().Where(a =>  a.UnitDetailsItemId == ids)
                    .Select(c => new
                    {
                        c.UnitDetailsId,
                        c.UnitDetailsUnitId,
                        c.UnitDetailsRate,
                        c.UnitDetailsConversionType
                    }).ToList();
            }
           
            catch (Exception ex)
            {
                throw ex;
            }
            stockMaster = stockMaster.OrderBy(sm => sm.StockRegisterMaterialID).ToList();
            return stockMaster;
        }
        public StockTransferModel GetSavedStockTransferDetails(int pvno)
        {
            StockTransferModel issueVoucherModel = new StockTransferModel();
            issueVoucherModel.stockTransfer = _StockTransferRepo.GetAsQueryable().Where(k => k.StockTransferId == pvno).SingleOrDefault();
            issueVoucherModel.stockTransferDetails = _StockTransferDetailRepo.GetAsQueryable().Where(x => x.StockTransferDetailsSNo == issueVoucherModel.stockTransfer.StockTransferVoucherNo && (x.StockTransferDetailsDelStatus == false || x.StockTransferDetailsDelStatus == null)).OrderBy(x => x.StockTransferDetailsSNo).AsEnumerable().ToList();
            return issueVoucherModel;
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {


                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.StockTransfer_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_VoucherNumberRepo.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.StockTransferVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;



                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.StockTransferVoucher_TYPE,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = VoucherGenDate

                };
                _VoucherNumberRepo.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private decimal getConverionTypebyUnitId(int? itemid, int? unitDetailsid)
        {
            try
            {
                return (decimal)_UnitDetailsRepository.GetAsQueryable().FirstOrDefault(x => x.UnitDetailsItemId == itemid && x.UnitDetailsUnitId == unitDetailsid).UnitDetailsConversionType;
            }
            catch
            {
                return 1;
            }
        }

        public VouchersNumbers GetVouchersNumbers(string pvno)
        {
            try
            {
                VouchersNumbers vouchersNumbers = _VoucherNumberRepo.GetAsQueryable().Where(k => k.VouchersNumbersVNo == pvno).SingleOrDefault();
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex.Message);
                throw ex;
            }

        }
        public IEnumerable<StockTransferResponseModel> getStockTransferReport()
        {
            try
            {
                return _StockTransferResponse.GetBySP($"EXEC getStockTransferReport");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<bool> DeleteStockTransfer(StockTransfer stockTransfer, List<StockTransferDetails> stockTransferDetails
             , List<StockRegister> stockRegister
          )
        {
            try
            {
                _StockTransferRepo.BeginTransaction();

                //=================================
                clsCommonFunctions.Delete_OldEntryOf_StockRegister(stockTransfer.StockTransferVoucherNo, VoucherType.StockTransferVoucher_TYPE, _StockRegisterRepo);
                //clsCommonFunctions.Delete_OldEntry_vouchernumber(stockTransfer.StockTransferVoucherNo, VoucherType.StockTransferVoucher_TYPE, _VoucherNumberRepo);
                //=================================

                stockTransfer.StockTransferDelStatus = true;

                stockTransfer.StockTransferDetails = stockTransfer.StockTransferDetails.Select((k) =>
            {
                k.StockTransferDetailsDelStatus = true;
                return k;
            }).OrderBy(x => x.StockTransferDetailsSNo).AsEnumerable().ToList();


                stockRegister = stockRegister.Select((k) =>
                {
                    k.StockRegisterDelStatus = true;
                    return k;
                }).OrderBy(x => x.StockRegisterRefVoucherNo).AsEnumerable().ToList();
                _StockRegisterRepo.UpdateList(stockRegister);

                _StockTransferRepo.UpdateAsync(stockTransfer);

                var vchrnumer = await _VoucherNumberRepo.GetAsQueryable().Where(k => k.VouchersNumbersVNo == stockTransfer.StockTransferVoucherNo).FirstOrDefaultAsync();

                _VoucherNumberRepo.UpdateAsync(vchrnumer);

                _StockTransferRepo.TransactionCommit();
                return true;
            }
            catch (Exception ex)
            {
                _StockTransferRepo.TransactionRollback();

                throw ex;
            }



        }

        public async Task<StockTransferModel> UpdateStockTransfer(StockTransfer stockTransfer, List<StockTransferDetails> stockTransferDetails
             , List<StockRegister> stockRegister
    )
        {
            try
            {

                _StockTransferRepo.BeginTransaction();

                //=================================
                clsCommonFunctions.Delete_OldEntryOf_StockRegister(stockTransfer.StockTransferVoucherNo, VoucherType.StockTransferVoucher_TYPE, _StockRegisterRepo);
                // clsCommonFunctions.Delete_OldEntry_AccountsTransactions(stockTransfer.StockTransferVoucherNo, VoucherType.IssueVoucher_TYPE, _accountTransactionRepository);
                //=================================                

                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    await _StockTransferRepo.GetAsQueryable()
                    .DefaultIfEmpty().MaxAsync(o => o == null ? 0 : o.StockTransferId) + 1);

                stockTransfer.StockTransferDetails = stockTransfer.StockTransferDetails.Select((x) =>
                    {
                        x.StockTransferId = stockTransfer.StockTransferId;
                        x.StockTransferDetailsSNo = stockTransfer.StockTransferVoucherNo;

                        return x;
                    }).OrderBy(x => x.StockTransferDetailsSNo).AsEnumerable().ToList();

                //stockRegister = stockRegister.Select((k) =>
                //{
                //    k.StockRegisterVoucherDate = stockTransfer.StockTransferStDate;
                //    k.StockRegisterRefVoucherNo = stockTransfer.StockTransferVoucherNo;
                //    k.StockRegisterTransType = VoucherType.StockTransferVoucher_TYPE;
                //    k.StockRegisterStatus = AccountStatus.Approved;
                //    return k;
                //}).ToList();

                //_StockRegisterRepo.InsertList(stockRegister);


                var stockReg = new List<StockRegister>();
                foreach (var item in stockTransfer.StockTransferDetails)
                {
                    var avgrate = item.StockTransferDetailsCostPrice ?? 1;
                    var converontype = this.getConverionTypebyUnitId(item.StockTransferDetailsMaterialId, item.StockTransferDetailsUnitId);

                    stockReg.Add(new StockRegister()
                    {
                        StockRegisterPurchaseID = stockTransfer.StockTransferVoucherNo,
                        StockRegisterRefVoucherNo = stockTransfer.StockTransferVoucherNo,
                        StockRegisterVoucherDate = DateTime.Now.Date,
                        StockRegisterMaterialID = item.StockTransferDetailsMaterialId,
                        StockRegisterQuantity = (decimal)item.StockTransferDetailsQty * (decimal)converontype,
                        StockRegisterSIN = 0,
                        StockRegisterSout = (decimal)item.StockTransferDetailsQty * (decimal)(converontype),
                        StockRegisterRate = avgrate,
                        StockRegisterAmount = (decimal)item.StockTransferDetailsQty * (decimal)(converontype) * avgrate,
                        StockRegisterFCAmount = (decimal)item.StockTransferDetailsQty * (decimal)(converontype) * avgrate,
                        StockRegisterAssignedDate = DateTime.Now.Date,
                        StockRegisterStatus = "A",
                        StockRegisterTransType = VoucherType.StockTransferVoucher_TYPE,
                        StockRegisterUnitID = item.StockTransferDetailsUnitId,
                        StockRegisterLocationID = stockTransfer.StockTransferLocationIdFrom,
                        StockRegisterJobID = stockTransfer.StockTransferJobId,
                        StockRegisterDepID = (int?)stockTransfer.StockTransferDepId,
                        StockRegisterDelStatus = false
                    });
                    stockReg.Add(new StockRegister()
                    {
                        StockRegisterPurchaseID = stockTransfer.StockTransferVoucherNo,
                        StockRegisterRefVoucherNo = stockTransfer.StockTransferVoucherNo,
                        StockRegisterVoucherDate = DateTime.Now.Date,
                        StockRegisterMaterialID = item.StockTransferDetailsMaterialId,
                        StockRegisterQuantity = (decimal)item.StockTransferDetailsQty * (decimal)converontype,
                        StockRegisterSIN = (decimal)item.StockTransferDetailsQty * (decimal)(converontype),
                        StockRegisterSout = 0,
                        StockRegisterRate = avgrate,
                        StockRegisterAmount = (decimal)item.StockTransferDetailsQty * (decimal)(converontype) * avgrate,
                        StockRegisterFCAmount = (decimal)item.StockTransferDetailsQty * (decimal)(converontype) * avgrate,
                        StockRegisterAssignedDate = DateTime.Now.Date,
                        StockRegisterStatus = "A",
                        StockRegisterTransType = VoucherType.StockTransferVoucher_TYPE,
                        StockRegisterUnitID = item.StockTransferDetailsUnitId,
                        StockRegisterLocationID = stockTransfer.StockTransferLocationIdTo,
                        StockRegisterJobID = stockTransfer.StockTransferJobId,
                        StockRegisterDepID = (int?)stockTransfer.StockTransferDepId,
                        StockRegisterDelStatus = false
                    });
                }
                _StockRegisterRepo.InsertList(stockReg);


                _StockTransferRepo.Update(stockTransfer);
                _StockTransferRepo.TransactionCommit();
                return this.GetSavedStockTransferDetails((int)stockTransfer.StockTransferId);

            }
            catch (Exception ex)
            {
                _StockTransferRepo.TransactionRollback();
                throw ex;


            }
        }

        public async Task<ResponseInfo> getStockTransferReport(reportModel model)
        {
            var objresponse = new ResponseInfo();
            var query = (from st in _StockTransferResp.GetAsQueryable()
                         join sd in _StockTransferDetailRepo.GetAsQueryable() on st.StockTransferId equals sd.StockTransferId
                         join im in _itemMasterRepo.GetAsQueryable() on (int)sd.StockTransferDetailsMaterialId equals im.ItemMasterItemId
                         where (
                            (model.fromLocation == 0 || st.StockTransferLocationIdFrom >= model.fromLocation) &&
                            (model.toLocation == 0 || st.StockTransferLocationIdTo <= model.toLocation)
                            &&
                            (model.fromdate == null || st.StockTransferStDate.Value.Date >= model.fromdate.Value.Date)
                            && (model.todate == null || st.StockTransferStDate.Value.Date <= model.todate.Value.Date)
                            && st.StockTransferDelStatus != true)

                         select new StockTransferReport
                         {
                             StockTransferVoucherNo = st.StockTransferVoucherNo,
                             StockTransferStDate = st.StockTransferStDate,
                             ItemMasteritemName = im.ItemMasterItemName,
                             StockTransferLocationIdFrom = st.StockTransferLocationIdFrom,
                             StockTransferLocationIdTo = st.StockTransferLocationIdTo,
                             StockTransferStatus = st.StockTransferStatus,
                             StockTransfertotal =  sd.StockTransferDetailsQty ?? 0
                         }).ToList();


            objresponse.ResultSet = query;
            return objresponse;
        }

        public ResponseInfo StockTransferChangeStatus(string stockTransferVoucherNo)
        {
            var objresponse = new ResponseInfo();
            var model = _StockTransferRepo.GetAsQueryable().Where(x => x.StockTransferVoucherNo == stockTransferVoucherNo).FirstOrDefault();
            if(model != null) {

                 model.StockTransferStatus = "test";
                _StockTransferRepo.Update(model);
                objresponse.IsSuccess = true;
            }
            return objresponse;
        }
    }

}