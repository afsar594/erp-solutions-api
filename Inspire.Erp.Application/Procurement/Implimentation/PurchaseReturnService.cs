using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Procurement.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Procurement;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Procurement.Implementation
{
   public class PurchaseReturnService: IPurchaseReturnService
    {
        private readonly IRepository<VouchersNumbers> _voucherNumber;
        private readonly IRepository<PurchaseReturnVoucher> _purchaseReturn;
        private readonly IRepository<PurchaseVoucher> _purchaseVoucher;
        private readonly IRepository<AccountsTransactions> _accountTransaction;
        private readonly IRepository<StockRegister> _stockRegister;
        private readonly IRepository<SuppliersMaster> _supplierMaster;
        private readonly IRepository<PurchaseReturnVoucherDetails> _purchaseReturnDetail;
        private readonly IRepository<PurchaseVoucherDetails> _purchasevoucherDetail;
        private readonly IRepository<LocationMaster> _location;
        private readonly IRepository<JobMaster> _jobMaster;
        private readonly IUtilityService _utilityService;
        public PurchaseReturnService(IRepository<PurchaseReturnVoucher> purchaseReturn, IUtilityService utilityService,
            IRepository<PurchaseVoucherDetails> purchasevoucherDetail, IRepository<PurchaseVoucher> purchaseVoucher,
            IRepository<LocationMaster> location, IRepository<StockRegister> stockRegister, IRepository<JobMaster> jobMasters,
            IRepository<PurchaseReturnVoucherDetails> purchaseReturnDetail, IRepository<SuppliersMaster> supplierMaster)
        {
            _purchaseReturn = purchaseReturn;
            _purchaseReturnDetail = purchaseReturnDetail;
            _utilityService = utilityService;
            _supplierMaster = supplierMaster;
            _purchasevoucherDetail = purchasevoucherDetail;
            _purchaseVoucher = purchaseVoucher;
            _location = location;
            _jobMaster = jobMasters;
            _stockRegister = stockRegister;
        }
        public async Task<Response<List<DropdownResponse>>> GetLocationMasterDropdown()
        {
            try
            {
                var response = new List<DropdownResponse>();
                //response.Add(new DropdownResponse()
                //{
                //    Value = "",
                //    Name = "All"
                //});
                response.AddRange(await _location.ListSelectAsync(x => 1 == 1, x => new DropdownResponse
                {
                    Id = x.LocationMasterLocationId != null?Convert.ToInt32(x.LocationMasterLocationId):0,
                    Name = x.LocationMasterLocationName
                }));
                return Response<List<DropdownResponse>>.Success(response, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<DropdownResponse>>.Fail(new List<DropdownResponse>(), ex.Message);
            }
        }
        public async Task<Response<List<DropdownResponse>>> GetVoucherTypeDropdown()
        {
            try
            {
                var response = new List<DropdownResponse>();
                response.Add(new DropdownResponse()
                {
                    Value = "CASH",
                    Name = "CASH"
                });
                response.Add(new DropdownResponse()
                {
                    Value = "CREDIT",
                    Name = "CREDIT"
                });
                return Response<List<DropdownResponse>>.Success(response, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<DropdownResponse>>.Fail(new List<DropdownResponse>(), ex.Message);
            }
        }
        public async Task<Response<List<DropdownResponse>>> GetJObMasterDropdown()
        {
            try
            {
                var response = new List<DropdownResponse>();
                //response.Add(new DropdownResponse()
                //{
                //    Value = "",
                //    Name = "All"
                //});
                response.AddRange(await _jobMaster.ListSelectAsync(x => 1 == 1, x => new DropdownResponse
                {
                    Id = x.JobMasterJobId != null ? Convert.ToInt32(x.JobMasterJobId) : 0,
                    Name = x.JobMasterJobName
                }));
                return Response<List<DropdownResponse>>.Success(response, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<DropdownResponse>>.Fail(new List<DropdownResponse>(), ex.Message);
            }
        }
        public async Task<Response<List<DropdownResponse>>> GetSupplierMasterDropdown()
        {
            try
            {
                var response = new List<DropdownResponse>();
                //response.Add(new DropdownResponse()
                //{
                //    Value = "",
                //    Name = "All"
                //});
                response.AddRange(await _supplierMaster.ListSelectAsync(x => 1 == 1, x => new DropdownResponse
                {
                    Id = x.SuppliersMasterSupplierId != null ? Convert.ToInt32(x.SuppliersMasterSupplierId) : 0,
                    Name = x.SuppliersMasterSupplierName
                }));
                return Response<List<DropdownResponse>>.Success(response, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<DropdownResponse>>.Fail(new List<DropdownResponse>(), ex.Message);
            }
        }
        public async Task<Response<GridWrapperResponse<List<PurchaseReturnVoucherResponse>>>> GetPurchaseReturnList(GenericGridViewModel model)
        {
            try
            {
                List<PurchaseReturnVoucherResponse> response = new List<PurchaseReturnVoucherResponse>();
                var orders = await _purchaseReturn.GetAsQueryable().ToListAsync();
                response.AddRange(orders.Select(x => new PurchaseReturnVoucherResponse
                {
                    PurchaseReturnVoucherId = x.PurchaseReturnVoucherId,
                    PurchaseReturnVoucherGrno = x.PurchaseReturnVoucherGrno,
                    PurchaseReturnVoucherRetDate = x.PurchaseReturnVoucherRetDate,
                    PurchaseReturnVoucherRetId = x.PurchaseReturnVoucherRetId,
                    PurchaseReturnVoucherRetDateString = (x.PurchaseReturnVoucherRetDate != null ? x.PurchaseReturnVoucherRetDate : DateTime.MinValue).Value.ToString("dd-MM-yyyy"),
                    PurchaseReturnVoucherVoucherDateString = (x.PurchaseReturnVoucherVoucherDate != null? x.PurchaseReturnVoucherVoucherDate : DateTime.MinValue ).Value.ToString("dd-MM-yyyy"),
                    PurchaseReturnVoucherVoucherType = x.PurchaseReturnVoucherVoucherType,
                    PurchaseReturnVoucherNetAmount = x.PurchaseReturnVoucherNetAmount,
                    PurchaseReturnVoucherNarration = x.PurchaseReturnVoucherNarration,
                    PurchaseReturnVoucherUserId = x.PurchaseReturnVoucherUserId,
                }).ToList());
                var gridResponse = new GridWrapperResponse<List<PurchaseReturnVoucherResponse>>();
                gridResponse.Data = response;
                gridResponse.Total = 0;
                return Response<GridWrapperResponse<List<PurchaseReturnVoucherResponse>>>.Success(gridResponse, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<List<PurchaseReturnVoucherResponse>>>.Fail(new GridWrapperResponse<List<PurchaseReturnVoucherResponse>>(), ex.Message);
            }
        }
        public async Task<Response<PurchaseReturnVoucherResponse>> GetSpecificPurchaseReturn(string id =null)
        {
            try
            {
                PurchaseReturnVoucherResponse model = new PurchaseReturnVoucherResponse();
                await Task.Run(() =>
                {
                    var orders = _purchaseReturn.GetAsQueryable().FirstOrDefault(x => x.PurchaseReturnVoucherRetId == id);
                    model.PurchaseReturnVoucherId = orders.PurchaseReturnVoucherId;
                    model.PurchaseReturnVoucherGrno = orders.PurchaseReturnVoucherGrno;
                    model.PurchaseReturnVoucherRetDate = orders.PurchaseReturnVoucherRetDate;
                    model.PurchaseReturnVoucherRetId = orders.PurchaseReturnVoucherRetId;
                    model.PurchaseReturnVoucherVoucherType = orders.PurchaseReturnVoucherVoucherType;
                    model.PurchaseReturnVoucherNetAmount = orders.PurchaseReturnVoucherNetAmount;
                    model.PurchaseReturnVoucherNarration = orders.PurchaseReturnVoucherNarration;
                    model.PurchaseReturnVoucherUserId = orders.PurchaseReturnVoucherUserId;
                    model.PurchaseReturnVoucherDetails = _purchaseReturnDetail.GetAsQueryable()
                        .Where(x => x.PurchaseReturnVoucherDetailsRetId == id).Select(x => new PurchaseReturnVoucherDetailResponse
                        {
                            PurchaseReturnVoucherDetailsBatchCode=x.PurchaseReturnVoucherDetailsBatchCode,
                            PurchaseReturnVoucherDetailsRetId=x.PurchaseReturnVoucherDetailsRetId,
                            PurchaseReturnVoucherDetailsAmount=x.PurchaseReturnVoucherDetailsAmount,
                            PurchaseReturnVoucherDetailsBatch=x.PurchaseReturnVoucherDetailsBatch,
                            PurchaseReturnVoucherDetailsMaterialId=x.PurchaseReturnVoucherDetailsMaterialId,
                            PurchaseReturnVoucherDetailsQty=x.PurchaseReturnVoucherDetailsQty,
                            PurchaseReturnVoucherDetailsRemarks=x.PurchaseReturnVoucherDetailsRemarks,
                            PurchaseReturnVoucherDetailsSno=x.PurchaseReturnVoucherDetailsSno,
                            PurchaseReturnVoucherDetailsRate=x.PurchaseReturnVoucherDetailsRate,
                            PurchaseReturnVoucherDetailsVoucherNo=x.PurchaseReturnVoucherDetailsVoucherNo
                        }).ToList();

                });
                return Response<PurchaseReturnVoucherResponse>.Success(model, "Records FOund.");
            }
            catch (Exception ex)
            {
                return Response<PurchaseReturnVoucherResponse>.Fail(new PurchaseReturnVoucherResponse(), ex.Message);
            }
        }

        public async Task<Response<GridWrapperResponse<List<GetPurchaseVoucherResponse>>>> GetPurchaseVoucherList(GenericGridViewModel model)
        {
            try
            {
                string query = $@" 1 == 1 {model.Filter}";
                List<GetPurchaseVoucherResponse> response = new List<GetPurchaseVoucherResponse>();
                var orders = await _purchaseVoucher.GetAsQueryable()
                    .Where(x=>model.Search != null? x.PurchaseVoucherGRNo.Contains(model.Search) || x.PurchaseVoucherDescription.Contains(model.Search)
                    || x.PurchaseVoucherVoucherNo.Contains(model.Search):1==1).Where(query).ToListAsync();
                response.AddRange(orders.Select(x => new GetPurchaseVoucherResponse
                {
                    PurchaseVoucherActualAmount = x.PurchaseVoucherActualAmount,
                    PurchaseVoucherLpoNo = x.PurchaseVoucherLPONo,
                    PurchaseVoucherGrDate = x.PurchaseVoucherGRDate,
                    PurchaseVoucherGrDateString = x.PurchaseVoucherGRDate != null ? x.PurchaseVoucherGRDate.Value.ToString("dd-MM-yyyy") : "",
                    PurchaseVoucherVoucherNo = x.PurchaseVoucherVoucherNo,
                    PurchaseVoucherDescription = x.PurchaseVoucherDescription,
                    PurchaseVoucherCashSupplierName = x.PurchaseVoucherCashSupplierName,
                    PurchaseVoucherNetAmount = x.PurchaseVoucherNetAmount,
                    PurchaseVoucherPurchaseType = x.PurchaseVoucherPurchaseType,
                    PurchaseVoucherGrNo = x.PurchaseVoucherGRNo,
                    PurchaseVoucherPurId = (int)x.PurchaseVoucherPurID,

                }).ToList());
                var gridResponse = new GridWrapperResponse<List<GetPurchaseVoucherResponse>>();
                gridResponse.Data = response;
                gridResponse.Total = 0;
                return Response<GridWrapperResponse<List<GetPurchaseVoucherResponse>>>.Success(gridResponse, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<List<GetPurchaseVoucherResponse>>>.Fail(new GridWrapperResponse<List<GetPurchaseVoucherResponse>>(), ex.Message);
            }
        }
        public async Task<Response<PurchaseReturnVoucherResponse>> GetSpecificPurchaseVoucher(string id)
        {
            try
            {
                PurchaseReturnVoucherResponse model = new PurchaseReturnVoucherResponse();
                var itemmaster =await _utilityService.GetItemMaster();
                    var orders =await _purchaseVoucher.GetAsQueryable().FirstOrDefaultAsync(x => x.PurchaseVoucherVoucherNo == id);
                    model.PurchaseReturnVoucherGrno = orders.PurchaseVoucherVoucherNo;
                model.PurchaseReturnVoucherVoucherDate = orders.PurchaseVoucherGRDate;
                    model.PurchaseReturnVoucherVoucherType = orders.PurchaseVoucherPurchaseType;
                    model.PurchaseReturnVoucherLocationId = orders.PurchaseVoucherLocationID;
                    //model.PurchaseReturnVoucherSpId = orders.PurchaseVoucherSPID;
                    model.PurchaseReturnVoucherSpId = orders.PurchaseVoucherSPID;
                    model.PurchaseReturnVoucherNarration = orders.PurchaseVoucherDescription;
                    model.PurchaseReturnVoucherVatNo = orders.PurchaseVoucherVATNo;
                    model.PurchaseReturnVoucherSupInvNo = orders.PurchaseVoucherSupplyInvoiceNo;
                model.PurchaseReturnVoucherDetails = new List<PurchaseReturnVoucherDetailResponse>();
                var purchaseReturnVoucherDetails = await _purchasevoucherDetail.GetAsQueryable()
                 .Where(x => x.PurchaseVoucherDetailsVoucherNo == id)
                 .Select(x => new PurchaseReturnVoucherDetailResponse
                 {
                     PurchaseReturnVoucherDetailsSno = 0,
                     PurchaseReturnVoucherDetailsBatchCode = x.PurchaseVoucherDetailsBatchCode,
                     PurchaseReturnVoucherDetailsAmount = Convert.ToDouble(x.PurchaseVoucherDetailsAmount),
                     PurchaseReturnVoucherDetailsFocQty = x.PurchaseVoucherDetailsFoc,
                     PurchaseReturnVoucherDetailsMaterialId = x.PurchaseVoucherDetailsMaterialId,
                     PurchaseReturnVoucherDetailsPurQty = Convert.ToDouble(x.PurchaseVoucherDetailsQuantity),
                     PurchaseReturnVoucherDetailsRemarks = x.PurchaseVoucherDetailsRemarks,
                     PurchaseReturnVoucherDetailsRate = Convert.ToDouble(x.PurchaseVoucherDetailsRate),
                     PurchaseReturnVoucherDetailsUnitId = x.PurchaseVoucherDetailsUnitId,
                     PurchaseReturnVoucherDetailsVoucherNo = x.PurchaseVoucherDetailsGrnNo,
                     PurchaseReturnVoucherDetailsVatAmount = Convert.ToDouble(x.PurchaseVoucherDetailsVatAmount),
                     PurchaseReturnVoucherDetailsQty = 0
                 })
                 .ToListAsync();

                foreach (var purchaseReturnVoucherDetail in purchaseReturnVoucherDetails)
                {
                    var material = itemmaster.Result.FirstOrDefault(c => c.ItemMasterItemId == purchaseReturnVoucherDetail.PurchaseReturnVoucherDetailsMaterialId);
                    purchaseReturnVoucherDetail.MaterialName = material != null ? material.ItemMasterItemName : "";
                }

                model.PurchaseReturnVoucherDetails.AddRange(purchaseReturnVoucherDetails);
                return Response<PurchaseReturnVoucherResponse>.Success(model, "Records FOund.");
            }
            catch (Exception ex)
            {
                return Response<PurchaseReturnVoucherResponse>.Fail(new PurchaseReturnVoucherResponse(), ex.Message);
            }
        }
        public async Task<Response<bool>> AddEditPurchaseReturn(AddEditPurchaseReturnResponse model)
        {
            try
            {
                string message = null;
                if (model.PurchaseReturnVoucherId == 0)
                {

                        message =await AddPurchaseReturn(model);
                }
                else
                {

                        message =await EditPurchaseReturn(model);
                }
                if (message == null)
                {
                    return Response<bool>.Fail(false, "Something went wrong. Please try again later.");
                }
                return Response<bool>.Success(true, "Changes Saved Successfully.");
            }
            catch (Exception ex)
            {

                return Response<bool>.Fail(false, ex.Message);
            }
        }
        private async Task<string> AddPurchaseReturn(AddEditPurchaseReturnResponse model)
        {
            try
            {
                var financial = await _utilityService.GetFinancialPeriods();
                Random random = new Random();
                int vouhcerDetailid = random.Next();
               
                var supplier = _supplierMaster.GetAsQueryable().Where(x => x.SuppliersMasterSupplierId==model.PurchaseReturnVoucherSpId).FirstOrDefault();
                string supAccount = supplier != null ? supplier.SuppliersMasterSupplierReffAcNo : "";
                var vouchers = await _utilityService.AddVoucherNumber("Purchase Return", "PRV");
                PurchaseReturnVoucher order = new PurchaseReturnVoucher()
                {
                    PurchaseReturnVoucherVoucherId = vouchers.Result.Value,
                    PurchaseReturnVoucherRetId = vouchers.Result.Value,
                    PurchaseReturnVoucherDiscount = model.PurchaseReturnVoucherDiscount,
                    PurchaseReturnVoucherExcludeVat = model.PurchaseReturnVoucherExcludeVat,
                    PurchaseReturnVoucherFcDiscAmount = model.PurchaseReturnVoucherFcDiscAmount,
                    PurchaseReturnVoucherFsno = financial != null ? Convert.ToInt32(financial.Result.FinancialPeriodsFsno) : 0,
                    PurchaseReturnVoucherGrno = model.PurchaseReturnVoucherGrno,
                    PurchaseReturnVoucherRetDate = model.PurchaseReturnVoucherRetDate,
                    PurchaseReturnVoucherVoucherDate = model.PurchaseReturnVoucherVoucherDate,
                    PurchaseReturnVoucherNarration = model.PurchaseReturnVoucherNarration,
                    PurchaseReturnVoucherSupInvNo = model.PurchaseReturnVoucherSupInvNo,
                    PurchaseReturnVoucherUserId = 0,
                    
                    PurchaseReturnVoucherLocationId = model.PurchaseReturnVoucherLocationId,
                    PurchaseReturnVoucherVatNo = model.PurchaseReturnVoucherVatNo,
                    PurchaseReturnVoucherVoucherType = model.PurchaseReturnVoucherVoucherType,
                    PurchaseReturnVoucherTransportCost = model.PurchaseReturnVoucherTransportCost,
                   PurchaseReturnVoucherVatAmount= model.PurchaseReturnVoucherVatAmount,
                    PurchaseReturnVoucherSpId = model.PurchaseReturnVoucherSpId,
                    PurchaseReturnVoucherVatPercentage = model.PurchaseReturnVoucherVatPercentage,
                    PurchaseReturnVoucherVatRoundAmt = model.PurchaseReturnVoucherVatRoundAmt,
                    PurchaseReturnVoucherStatus = model.PurchaseReturnVoucherStatus,
                    PurchaseReturnVoucherNetAmount =model.PurchaseReturnVoucherNetAmount,
                    PurchaseReturnVoucherGrossAmount=model.PurchaseReturnVoucherGrossAmount,
                    PurchaseReturnVoucherVatRoundSign=model.PurchaseReturnVoucherVatRoundSign

                };
                _purchaseReturn.Insert(order);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel log = new AddActivityLogViewModel()
                {
                    Page = "Puchase Return",
                    Section = "Add Puchase Return",
                    Entity = "Puchase Return",

                };
                await _utilityService.AddUserTrackingLog(log);
                #endregion

                _purchaseReturn.SaveChangesAsync();
                List<PurchaseReturnVoucherDetails> orderDetails = new List<PurchaseReturnVoucherDetails>();
                orderDetails.AddRange(model.PurchaseReturnVoucherDetails.Select(x => new PurchaseReturnVoucherDetails
                {
                    PurchaseReturnVoucherDetailsBatchCode = x.PurchaseReturnVoucherDetailsBatchCode,
                    PurchaseReturnVoucherDetailsAmount = x.PurchaseReturnVoucherDetailsAmountNew,
                    PurchaseReturnVoucherDetailsBatch = x.PurchaseReturnVoucherDetailsBatch,
                    PurchaseReturnVoucherDetailsJobId = model.PurchaseReturnVoucherDetailsJobId,
                    PurchaseReturnVoucherDetailsFocQty = x.PurchaseReturnVoucherDetailsFocQty,
                    PurchaseReturnVoucherDetailsMaterialId = x.PurchaseReturnVoucherDetailsMaterialId,

                    PurchaseReturnVoucherDetailsQty = x.PurchaseReturnVoucherDetailsQty+x.PurchaseReturnVoucherDetailsFocQty,
                    PurchaseReturnVoucherDetailsRemarks = x.PurchaseReturnVoucherDetailsRemarks,
                    PurchaseReturnVoucherDetailsRetId = vouchers.Result.Value,
                    PurchaseReturnVoucherDetailsRate=x.PurchaseReturnVoucherDetailsRate,
                    PurchaseReturnVoucherDetailsUnitId=x.PurchaseReturnVoucherDetailsUnitId,
                    PurchaseReturnVoucherDetailsVoucherNo=x.PurchaseReturnVoucherDetailsVoucherNo,
                     PurchaseReturnVoucherDetailsVatAmount= x.PurchaseReturnVoucherDetailsVatAmount,
                    PurchaseReturnVoucherDetailsVatPercetage = x.PurchaseReturnVoucherDetailsVatPercetage,
                    PurchaseReturnVoucherDetailsPurQty = x.PurchaseReturnVoucherDetailsPurQty,
                    PurchaseReturnVoucherDetailsFcAmount = x.PurchaseReturnVoucherDetailsAmountNew,
                }));
                _purchaseReturnDetail.InsertList(orderDetails);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel Detaillog = new AddActivityLogViewModel()
                {
                    Page = "Puchase Return",
                    Section = "Add Puchase Return Detail",
                    Entity = "Puchase Return Detail",

                };
                await _utilityService.AddUserTrackingLog(Detaillog);
                #endregion

                _purchaseReturnDetail.SaveChangesAsync();

                var stockRegister = model.PurchaseReturnVoucherDetails.Select(x => new StockRegister
                {
                    StockRegisterAmount = x.PurchaseReturnVoucherDetailsAmountNew.HasValue
        ? Convert.ToDecimal(x.PurchaseReturnVoucherDetailsAmount)
        : 0,
                    StockRegisterAssignedDate =DateTime.Now,
                    StockRegisterFSNO=financial.Result.FinancialPeriodsFsno,
                    StockRegisterJobID=model.PurchaseReturnVoucherDetailsJobId,
                    StockRegisterBatchCode=x.PurchaseReturnVoucherDetailsBatchCode,
                    StockRegisterMaterialID=x.PurchaseReturnVoucherDetailsMaterialId,
                    StockRegisterQuantity = x.PurchaseReturnVoucherDetailsQty.HasValue
        ? Convert.ToDecimal(x.PurchaseReturnVoucherDetailsQty)
        : 0,
                    StockRegisterFCAmount = Convert.ToDecimal(x.PurchaseReturnVoucherDetailsAmountNew),
                     StockRegisterRefVoucherNo=vouchers.Result.Value,
                     StockRegisterExpDate=DateTime.Now,
                     StockRegisterLocationID=model.PurchaseReturnVoucherLocationId,
                    StockRegisterRate= Convert.ToDecimal(x.PurchaseReturnVoucherDetailsAmountNew),
                     StockRegisterTransType="Purchase Return",
                     StockRegisterRemarks=x.PurchaseReturnVoucherDetailsRemarks,
                     
                }).ToList();
                _stockRegister.InsertList(stockRegister);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel Detaillogs = new AddActivityLogViewModel()
                {
                    Page = "Puchase Return",
                    Section = "Add Puchase Return Stock",
                    Entity = "Puchase Return Stock",

                };
                await _utilityService.AddUserTrackingLog(Detaillogs);
                #endregion
                _stockRegister.SaveChangesAsync();
                foreach (var item in model.PurchaseReturnVoucherDetails)
                {
                  var debit=  new AccountsTransactions
                    {

                        AccountsTransactionsFsno = Convert.ToDecimal(financial.Result.FinancialPeriodsFsno),
                        AccountsTransactionsAccNo = supAccount,
                        AccountsTransactionsDebit=Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsFcDebit= Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                      AccountsTransactionsAllocBalance = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                      AccountsTransactionsFcAllocBalance= Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                      AccountsTransactionsTransDate=DateTime.Now,
                      AccountsTransactionsVoucherType="Purchase Return",
                      AccountsTransactionsParticulars= $@"Purchase Return as {supAccount} ",
                      AccountsTransactionsVoucherNo= vouchers.Result.Value,
                      AccountsTransactionsApprovalDt=DateTime.Now,
                      AccountsTransactionsDescription=model.PurchaseReturnVoucherNarration,
                      AccountsTransactionsUserId=0,
                      RefNo="",
                      AccountsTransactionsJobNo=model.PurchaseReturnVoucherDetailsJobId,
                      
                  };
                  await  _utilityService.SaveAccountTransaction(debit);
                    var credit = new AccountsTransactions
                    {
                        AccountsTransactionsFsno = Convert.ToDecimal(financial.Result.FinancialPeriodsFsno),
                        AccountsTransactionsAccNo = supAccount,
                        AccountsTransactionsCredit = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsFcCredit = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsAllocBalance = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsFcAllocBalance = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsTransDate = DateTime.Now,
                        AccountsTransactionsVoucherType = "Purchase Return",
                        AccountsTransactionsParticulars = $@"Purchase Return as {supAccount} ",
                        AccountsTransactionsVoucherNo = vouchers.Result.Value,
                        AccountsTransactionsApprovalDt = DateTime.Now,
                        AccountsTransactionsDescription = model.PurchaseReturnVoucherNarration,
                        AccountsTransactionsUserId = 0,
                        RefNo = "",
                        AccountsTransactionsJobNo = model.PurchaseReturnVoucherDetailsJobId,

                    };
                    await _utilityService.SaveAccountTransaction(credit);

                }
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel logs = new AddActivityLogViewModel()
                {
                    Page = "Purchase Return",
                    Section = "Add Purchase Return",
                    Entity = "Purchase Return",

                };
                await _utilityService.AddUserTrackingLog(logs);
                #endregion

                _stockRegister.SaveChangesAsync();
                return "Record Added successfully.";
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private async Task<string> EditPurchaseReturn(AddEditPurchaseReturnResponse model)
        {
            try
            {
                Random random = new Random();
                var financial = await _utilityService.GetFinancialPeriods();
                int vouhcerDetailid = random.Next();
                var supplier = _supplierMaster.GetAsQueryable().Where(x => x.SuppliersMasterSupplierId == model.PurchaseReturnVoucherSpId).FirstOrDefault();
                string supAccount = supplier != null ? supplier.SuppliersMasterSupplierReffAcNo : "";
                var vouchers = await _utilityService.AddVoucherNumber("Purchase Return", "PR");
                var orders = _purchaseReturn.GetAsQueryable().FirstOrDefault(x => x.PurchaseReturnVoucherId == model.PurchaseReturnVoucherId);
                orders.PurchaseReturnVoucherDiscount = model.PurchaseReturnVoucherDiscount;
                orders.PurchaseReturnVoucherExcludeVat = model.PurchaseReturnVoucherExcludeVat;
                orders.PurchaseReturnVoucherFcDiscAmount = model.PurchaseReturnVoucherFcDiscAmount;
                orders.PurchaseReturnVoucherGrno = model.PurchaseReturnVoucherGrno;
                orders.PurchaseReturnVoucherRetDate = model.PurchaseReturnVoucherRetDate;
                orders.PurchaseReturnVoucherVoucherDate = model.PurchaseReturnVoucherVoucherDate;
                orders.PurchaseReturnVoucherNarration = model.PurchaseReturnVoucherNarration;
                orders.PurchaseReturnVoucherSupInvNo = model.PurchaseReturnVoucherSupInvNo;
                orders.PurchaseReturnVoucherUserId = 0;
                orders.PurchaseReturnVoucherLocationId = model.PurchaseReturnVoucherLocationId;
                orders.PurchaseReturnVoucherVatNo = model.PurchaseReturnVoucherVatNo;
                orders.PurchaseReturnVoucherVoucherType = model.PurchaseReturnVoucherVoucherType;
                orders.PurchaseReturnVoucherTransportCost = model.PurchaseReturnVoucherTransportCost;
                orders.PurchaseReturnVoucherVatAmount = model.PurchaseReturnVoucherVatAmount;
                orders.PurchaseReturnVoucherSpId = model.PurchaseReturnVoucherSpId;
                orders.PurchaseReturnVoucherVatPercentage = model.PurchaseReturnVoucherVatPercentage;
                  orders.PurchaseReturnVoucherVatRoundAmt = model.PurchaseReturnVoucherVatRoundAmt;
                orders.PurchaseReturnVoucherStatus = model.PurchaseReturnVoucherStatus;
                orders.PurchaseReturnVoucherNetAmount = model.PurchaseReturnVoucherNetAmount;
                orders.PurchaseReturnVoucherGrossAmount = model.PurchaseReturnVoucherGrossAmount;
                _purchaseReturn.Update(orders);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel log = new AddActivityLogViewModel()
                {
                    Page = "Purchase Return",
                    Section = "Update Purchase Return",
                    Entity = "Purchase Return",

                };
                await _utilityService.AddUserTrackingLog(log);
                #endregion

                _purchaseReturn.SaveChangesAsync();

                var listOrdersDetails = _purchaseReturnDetail.GetAsQueryable().Where(x => x.PurchaseReturnVoucherDetailsRetId == model.PurchaseReturnVoucherRetId).ToList();
                _purchaseReturnDetail.DeleteList(listOrdersDetails);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel Deletelog = new AddActivityLogViewModel()
                {
                    Page = "Purchase Return",
                    Section = "Delete Purchase Return",
                    Entity = "Purchase Return",

                };
                await _utilityService.AddUserTrackingLog(Deletelog);
                #endregion

                _purchaseReturnDetail.SaveChangesAsync();
                List<PurchaseReturnVoucherDetails> orderDetails = new List<PurchaseReturnVoucherDetails>();
                orderDetails.AddRange(model.PurchaseReturnVoucherDetails.Select(x => new PurchaseReturnVoucherDetails
                {
                    PurchaseReturnVoucherDetailsBatchCode = x.PurchaseReturnVoucherDetailsBatchCode,
                    PurchaseReturnVoucherDetailsAmount = x.PurchaseReturnVoucherDetailsAmountNew,
                    PurchaseReturnVoucherDetailsBatch = x.PurchaseReturnVoucherDetailsBatch,
                    PurchaseReturnVoucherDetailsJobId = model.PurchaseReturnVoucherDetailsJobId,
                    PurchaseReturnVoucherDetailsFocQty = x.PurchaseReturnVoucherDetailsFocQty,
                    PurchaseReturnVoucherDetailsMaterialId = x.PurchaseReturnVoucherDetailsMaterialId,

                    PurchaseReturnVoucherDetailsQty = x.PurchaseReturnVoucherDetailsQty + x.PurchaseReturnVoucherDetailsFocQty,
                    PurchaseReturnVoucherDetailsRemarks = x.PurchaseReturnVoucherDetailsRemarks,
                    PurchaseReturnVoucherDetailsRetId = vouchers.Result.Value,
                    PurchaseReturnVoucherDetailsRate = x.PurchaseReturnVoucherDetailsRate,
                    PurchaseReturnVoucherDetailsUnitId = x.PurchaseReturnVoucherDetailsUnitId,
                    PurchaseReturnVoucherDetailsVoucherNo = x.PurchaseReturnVoucherDetailsVoucherNo,
                    PurchaseReturnVoucherDetailsVatAmount = x.PurchaseReturnVoucherDetailsVatAmount,
                    PurchaseReturnVoucherDetailsVatPercetage = x.PurchaseReturnVoucherDetailsVatPercetage,
                    PurchaseReturnVoucherDetailsPurQty = x.PurchaseReturnVoucherDetailsPurQty,
                    PurchaseReturnVoucherDetailsFcAmount = x.PurchaseReturnVoucherDetailsAmountNew,
                }));
                _purchaseReturnDetail.InsertList(orderDetails);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel Addlog = new AddActivityLogViewModel()
                {
                    Page = "Purchase Return",
                    Section = "Add Purchase Return Detail",
                    Entity = "Purchase Return Detail",

                };
                await _utilityService.AddUserTrackingLog(Addlog);
                #endregion

                _purchaseReturnDetail.SaveChangesAsync();

                var stockRegister = model.PurchaseReturnVoucherDetails.Select(x => new StockRegister
                {
                    StockRegisterAmount = x.PurchaseReturnVoucherDetailsAmountNew.HasValue
        ? Convert.ToDecimal(x.PurchaseReturnVoucherDetailsAmount)
        : 0,
                    StockRegisterAssignedDate = DateTime.Now,
                    StockRegisterFSNO = financial.Result.FinancialPeriodsFsno,
                    StockRegisterJobID = model.PurchaseReturnVoucherDetailsJobId,
                    StockRegisterBatchCode = x.PurchaseReturnVoucherDetailsBatchCode,
                    StockRegisterMaterialID = x.PurchaseReturnVoucherDetailsMaterialId,
                    StockRegisterQuantity = x.PurchaseReturnVoucherDetailsQty.HasValue
        ? Convert.ToDecimal(x.PurchaseReturnVoucherDetailsQty)
        : 0,
                    StockRegisterFCAmount = Convert.ToDecimal(x.PurchaseReturnVoucherDetailsAmountNew),
                    StockRegisterRefVoucherNo = vouchers.Result.Value,
                    StockRegisterExpDate = DateTime.Now,
                    StockRegisterLocationID = model.PurchaseReturnVoucherLocationId,
                    StockRegisterRate = Convert.ToDecimal(x.PurchaseReturnVoucherDetailsAmountNew),
                    StockRegisterTransType = "Purchase Return",
                    StockRegisterRemarks = x.PurchaseReturnVoucherDetailsRemarks,

                }).ToList();
                _stockRegister.InsertList(stockRegister);

                _stockRegister.SaveChangesAsync();
                foreach (var item in model.PurchaseReturnVoucherDetails)
                {
                    var debit = new AccountsTransactions
                    {

                        AccountsTransactionsFsno = Convert.ToDecimal(financial.Result.FinancialPeriodsFsno),
                        AccountsTransactionsAccNo = supAccount,
                        AccountsTransactionsDebit = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsFcDebit = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsAllocBalance = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsFcAllocBalance = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsTransDate = DateTime.Now,
                        AccountsTransactionsVoucherType = "Purchase Return",
                        AccountsTransactionsParticulars = $@"Purchase Return as {supAccount} ",
                        AccountsTransactionsVoucherNo = vouchers.Result.Value,
                        AccountsTransactionsApprovalDt = DateTime.Now,
                        AccountsTransactionsDescription = model.PurchaseReturnVoucherNarration,
                        AccountsTransactionsUserId = 0,
                        RefNo = "",
                        AccountsTransactionsJobNo = model.PurchaseReturnVoucherDetailsJobId,

                    };
                    await _utilityService.SaveAccountTransaction(debit);
                    var credit = new AccountsTransactions
                    {
                        AccountsTransactionsFsno = Convert.ToDecimal(financial.Result.FinancialPeriodsFsno),
                        AccountsTransactionsAccNo = supAccount,
                        AccountsTransactionsCredit = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsFcCredit = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsAllocBalance = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsFcAllocBalance = Convert.ToDecimal(item.PurchaseReturnVoucherDetailsAmountNew),
                        AccountsTransactionsTransDate = DateTime.Now,
                        AccountsTransactionsVoucherType = "Purchase Return",
                        AccountsTransactionsParticulars = $@"Purchase Return as {supAccount} ",
                        AccountsTransactionsVoucherNo = vouchers.Result.Value,
                        AccountsTransactionsApprovalDt = DateTime.Now,
                        AccountsTransactionsDescription = model.PurchaseReturnVoucherNarration,
                        AccountsTransactionsUserId = 0,
                        RefNo = "",
                        AccountsTransactionsJobNo = model.PurchaseReturnVoucherDetailsJobId,

                    };
                    await _utilityService.SaveAccountTransaction(credit);

                }
                _stockRegister.SaveChangesAsync();
                return "Record Updated successfully.";
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
