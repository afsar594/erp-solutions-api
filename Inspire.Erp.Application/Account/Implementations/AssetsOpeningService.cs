using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.AccountStatement;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inspire.Erp.Application.MODULE;

namespace Inspire.Erp.Application.Account.Implementations
{
   public class AssetsOpeningService: IAssetsOpeningService
    {
        private readonly IRepository<VouchersNumbers> _voucherNumber;
        private readonly IRepository<CurrencyMaster> _currencyMaster;
        private readonly IRepository<AssetMaster> _assetMaster;
        private readonly IRepository<FinancialPeriods> _financialPeriods;
        private readonly IRepository<AssetPurchaseVoucher> _assetopening;
        private readonly IRepository<AssetPurchaseVoucherDetails> _assetopeningDetail;
        private readonly IUtilityService _utilityService;
        private readonly string PurchaseAccNo = "EX6601";
        public AssetsOpeningService(IRepository<AssetPurchaseVoucher> assetopening, IRepository<VouchersNumbers> voucherNumber,
            IRepository<AssetPurchaseVoucherDetails> assetopeningDetail, IRepository<AssetMaster> assetMaster,
            IRepository<FinancialPeriods> financialPeriods, IRepository<CurrencyMaster> currencyMaster, IUtilityService utilityService)
        {
            _voucherNumber = voucherNumber;
            _assetopening = assetopening;
            _financialPeriods = financialPeriods;
            _assetopeningDetail = assetopeningDetail;
            _currencyMaster = currencyMaster;
            _assetMaster = assetMaster;
        }
        public async Task<Response<List<DropdownResponse>>> GetCurrencyMaster()
        {
            try
            {
                var response = new List<DropdownResponse>();
                //response.Add(new DropdownResponse()
                //{
                //    Value = "",
                //    Name = " All "
                //});
                response.AddRange(await _currencyMaster.ListSelectAsync(x => 1 == 1, x => new DropdownResponse
                {
                    Id = x.CurrencyMasterCurrencyId,
                    Name = x.CurrencyMasterCurrencyName
                }));
                return Response<List<DropdownResponse>>.Success(response, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<DropdownResponse>>.Fail(new List<DropdownResponse>(), ex.Message);
            }
        }
        public async Task<Response<List<GetAssetsMasterList>>> GetAssetMaster()
        {
            try
            {
                var response = new List<GetAssetsMasterList>();
                //response.Add(new DropdownResponse()
                //{
                //    Value = "",
                //    Name = " All "
                //});
                response.AddRange(await _assetMaster.ListSelectAsync(x => 1 == 1, x => new GetAssetsMasterList
                {
                    AssetMasterAssetAccountNo = x.AssetMasterAssetAccountNo,
                    AssetMasterAssetId = x.AssetMasterAssetId,
                    AssetMasterAssetCode=x.AssetMasterAssetCode,
                    AssetMasterAssetName=x.AssetMasterAssetName,
                    AssetMasterAssetType=x.AssetMasterAssetType,
                    AssetMasterAssetDepLibAccount=x.AssetMasterAssetDepLibAccount,
                    AssetMasterAssetDepExpAccount=x.AssetMasterAssetDepExpAccount,
                     AssetMasterAssetCreatedDate=x.AssetMasterAssetCreatedDate
                }));
                return Response<List<GetAssetsMasterList>>.Success(response, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<GetAssetsMasterList>>.Fail(new List<GetAssetsMasterList>(), ex.Message);
            }
        }
        public async Task<Response<GridWrapperResponse<List<GetAssetPurchaseVoucher>>>> GetAssetOpeningList(GenericGridViewModel model)
        {
            try
            {
                List<GetAssetPurchaseVoucher> response = new List<GetAssetPurchaseVoucher>();
                var orders = await _assetopening.GetAsQueryable().Where(x => 1 == 1).Skip(model.Skip).Take(model.Take).ToListAsync();
                response.AddRange(orders.Select(x => new GetAssetPurchaseVoucher
                {
                    AstPurID = x.AstPurID,
                    AstPurchaseIDNum = x.AstPurchaseIDNum,
                    AstPurchaseID = x.AstPurchaseID,
                    ActualAmount = x.ActualAmount,
                    AstPurchaseType = x.AstPurchaseType,
                    AstPurDt = x.AstPurDt,
                    NetAmount = x.NetAmount,
                    DrAccNo = x.DrAccNo,
                    DrAmount = x.DrAmount,
                    DisAmount = x.DisAmount,
                    Description = x.Description,
                    DisPer = x.DisPer,
                }).ToList());
                var gridResponse = new GridWrapperResponse<List<GetAssetPurchaseVoucher>>();
                gridResponse.Data = response;
                gridResponse.Total = 0;
                return Response<GridWrapperResponse<List<GetAssetPurchaseVoucher>>>.Success(gridResponse, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<List<GetAssetPurchaseVoucher>>>.Fail(new GridWrapperResponse<List<GetAssetPurchaseVoucher>>(), ex.Message);
            }
        }
        public async Task<Response<bool>> AddEditAssetOpening(AddEditAssetPurchaseVoucher model)
        {
            try
            {
                string message = null;
                if (model.AstPurID == 0)
                {
                   
                        message = await AddAssetOpening(model) ;
                    
                }
                else
                {
                    
                        message = await EditAssetOpening(model);
                  
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
        //public AssetPurchaseVoucherModel InsertAssetPurchaseVoucher(AssetPurchaseVoucher customerPurchaseOrder)
        //{
        //    try
        //    {
        //        _assetopening.BeginTransaction();
        //        string customerPurchaseOrderNumber = clsCommonFunctions.GenerateVoucherNo(customerPurchaseOrder.AstPurDt
        //            ,).VouchersNumbersVNo;
        //        customerPurchaseOrder.CustomerPurchaseOrderVoucherNo = customerPurchaseOrderNumber;
        //        customerPurchaseOrder.CustomerPurchaseOrderDate = DateTime.Now;

        //        int maxcount = 0;
        //        maxcount = Convert.ToInt32(
        //            _customerPurchaseOrderRepository.GetAsQueryable()
        //            .DefaultIfEmpty().Max(o => o == null ? 0 : o.CustomerPurchaseOrderId) + 1);

        //        customerPurchaseOrder.CustomerPurchaseOrderId = maxcount;

        //        customerPurchaseOrder.CustomerPurchaseOrderDetails = customerPurchaseOrder.CustomerPurchaseOrderDetails.Select((x) =>
        //        {
        //            x.CustomerPurchaseOrderId = maxcount;
        //            x.CustomerPurchaseOrderDetailsVoucherNo = customerPurchaseOrderNumber;
        //            return x;
        //        }).ToList();
        //        //_customerPurchaseOrderDetailsRepository.InsertList(customerPurchaseOrderDetails);

        //        accountsTransactions = accountsTransactions.Select((k) =>
        //        {
        //            //k.AccountsTransactionsTransDate = customerPurchaseOrder.SalesVoucherDate;
        //            k.AccountsTransactionsVoucherNo = customerPurchaseOrderNumber;
        //            k.AccountsTransactionsVoucherType = VoucherType.CustomerPurchaseOrder_TYPE;
        //            k.AccountsTransactionsStatus = AccountStatus.Approved;
        //            return k;
        //        }).ToList();
        //        _accountTransactionRepository.InsertList(accountsTransactions);


        //        //List<StockRegister> sr = new List<StockRegister>();

        //        //foreach (var item in customerPurchaseOrderDetails)
        //        //{
        //        //    sr.Add(new StockRegister()
        //        //    {
        //        //        StockRegisterRefVoucherNo = customerPurchaseOrderNumber,
        //        //        StockRegisterAssignedDate = DateTime.Now,
        //        //        StockRegisterTransType = VoucherType.CustomerPurchaseOrder_TYPE,
        //        //        StockRegisterStatus = AccountStatus.Approved,
        //        //        StockRegisterVoucherDate = customerPurchaseOrder.CustomerPurchaseOrderDate\,
        //        //        StockRegisterSout = item.SalesVoucherDetailsQuantity,
        //        //        StockRegisterMaterialID = item.SalesVoucherDetailsMatId,
        //        //    });
        //        //}
        //        //stockRegister = stockRegister.Select((k) =>
        //        //{
        //        //   k.StockRegisterVoucherDate = customerPurchaseOrder.SalesVoucherDate;
        //        //   k.StockRegisterRefVoucherNo = customerPurchaseOrderNumber;
        //        //   k.StockRegisterTransType = VoucherType.CustomerPurchaseOrder_TYPE;
        //        //   k.StockRegisterStatus = AccountStatus.Approved;
        //        //    k.StockRegisterSout = 
        //        //   return k;
        //        //}).ToList();
        //        // _stockRegisterRepository.InsertList(sr);





        //        _customerPurchaseOrderRepository.Insert(customerPurchaseOrder);


        //        //insert tracking register
        //        var listTrackRegs = new List<TrackingRegister>();
        //        var listTrackPurchOrderReg = new List<PurOrderRegister>();
        //        var listCustOrderRegisterReg = new List<CustomerOrderRegister>();
        //        foreach (var purOrderDet in customerPurchaseOrder.CustomerPurchaseOrderDetails)
        //        {
        //            var tr = new TrackingRegister
        //            {
        //                TrackingRegisterDetailsId = (int)purOrderDet.CustomerPurchaseOrderDetailsId,
        //                TrackingRegisterVoucherNo = purOrderDet.CustomerPurchaseOrderDetailsVoucherNo,
        //                TrackingRegisterMaterialId = (int)purOrderDet.CustomerPurchaseOrderDetailsItemId,
        //                TrackingRegisterVoucherType = VoucherType.CustomerPurchaseOrder_TYPE,
        //                TrackingRegisterQty = (int)purOrderDet.CustomerPurchaseOrderDetailsQty,
        //                TrackingRegisterQtyin = (int)purOrderDet.CustomerPurchaseOrderDetailsQty,
        //                TrackingRegisterQtyout = 0,
        //                TrackingRegisterRate = purOrderDet.CustomerPurchaseOrderDetailsAmount,
        //                TrackingRegisterTrackDate = customerPurchaseOrder.CustomerPurchaseOrderDate,
        //                TrackingRegisterFsno = purOrderDet.CustomerPurchaseOrderDetailsFsno,

        //            };
        //            listTrackRegs.Add(tr);

        //            var poReg = new PurOrderRegister
        //            {
        //                PurOrderRegisterOrderNo = purOrderDet.CustomerPurchaseOrderDetailsId.ToString(),
        //                PurOrderRegisterRefVoucherNo = purOrderDet.CustomerPurchaseOrderDetailsVoucherNo,
        //                PurOrderRegisterMaterialId = (int)purOrderDet.CustomerPurchaseOrderDetailsItemId,
        //                PurOrderRegisterTransType = VoucherType.CustomerPurchaseOrder_TYPE,
        //                PurOrderRegisterQtyOrder = purOrderDet.CustomerPurchaseOrderDetailsQty,
        //                PurOrderRegisterQtyIssued = 0,
        //                PurOrderRegisterRate = purOrderDet.CustomerPurchaseOrderDetailsAmount,
        //                PurOrderRegisterAmount = purOrderDet.CustomerPurchaseOrderDetailsAmount,
        //                PurOrderRegisterFcAmount = purOrderDet.CustomerPurchaseOrderDetailsFcAmount,
        //                PurOrderRegisterAssignedDate = DateTime.Now,
        //                PurOrderRegisterFsno = purOrderDet.CustomerPurchaseOrderDetailsFsno,//to be alloted,
        //                PurOrderRegisterSupplierId = (int)0,
        //                PurOrderRegisterStatus = "A"

        //            };
        //            listTrackPurchOrderReg.Add(poReg);

        //            var custOrdReg = new CustomerOrderRegister
        //            {
        //                CustomerOrderRegisterRefVoucherNo = purOrderDet.CustomerPurchaseOrderDetailsVoucherNo,
        //                CustomerOrderRegisterOrderNo = purOrderDet.CustomerPurchaseOrderDetailsId.ToString(),
        //                CustomerOrderRegisterMaterialId = (int)purOrderDet.CustomerPurchaseOrderDetailsItemId,
        //                CustomerOrderRegisterTransType = VoucherType.CustomerPurchaseOrder_TYPE,
        //                CustomerOrderRegisterQtyOrder = (int)purOrderDet.CustomerPurchaseOrderDetailsQty,
        //                CustomerOrderRegisterQtyIssued = 0,
        //                CustomerOrderRegisterRate = purOrderDet.CustomerPurchaseOrderDetailsAmount,
        //                CustomerOrderRegisterAssignedDate = DateTime.Now,
        //                CustomerOrderRegisterFsno = purOrderDet.CustomerPurchaseOrderDetailsFsno,//to be alloted,
        //                CustomerOrderRegisterCustomerId = (int)0,
        //                CustomerOrderRegisterStatus = "A"

        //            };
        //            listCustOrderRegisterReg.Add(custOrdReg);

        //        }

        //        _trackingRegisterRepository.InsertList(listTrackRegs);
        //        _purOrderRegisterRepository.InsertList(listTrackPurchOrderReg);
        //        _custOrderRegRepository.InsertList(listCustOrderRegisterReg);

        //        _customerPurchaseOrderRepository.TransactionCommit();
        //        return this.GetSavedCustomerPurchaseOrderDetails(customerPurchaseOrder.CustomerPurchaseOrderVoucherNo);

        //    }
        //    catch (Exception ex)
        //    {
        //        _customerPurchaseOrderRepository.TransactionRollback();
        //        throw ex;
        //    }

        //}
        private async Task <string> AddAssetOpening(AddEditAssetPurchaseVoucher model)
        {
            try
            {
                var maxid = _voucherNumber.GetAsQueryable();
                var financial = _financialPeriods.GetAsQueryable().FirstOrDefault();
                Random random = new Random();
                int vouhcerDetailid = random.Next();
                var Id = Convert.ToInt32(maxid.Max(x => Math.Round(Convert.ToDecimal(x.VouchersNumbersVsno)))+1);
                AssetPurchaseVoucher order = new AssetPurchaseVoucher()
                {
                    AstPurID = Id,
                    AstPurchaseIDNum = Id,
                    AstPurchaseID = "AO"+ Id,
                    ActualAmount=model.ActualAmount,
                    AstPurchaseType="Cash",
                    AstPurDt=Convert.ToDateTime(model.AstPurDt),
                    SPID=0,
                    GRNo="",
                    GRDate=DateTime.Now,
                    LPODate=DateTime.Now,
                    LPONo="",
                    NetAmount=model.NetAmount,
                    TransportCost=0,
                    Handlingcharges=0,
                    DrAccNo=PurchaseAccNo,
                    DrAmount=model.DrAmount,
                    DisAmount=model.DisAmount,
                    FSNO=financial != null? Convert.ToInt32(financial.FinancialPeriodsFsno):0,
                    Status="P",
                    UserID=0,
                    FcRate=0,
                    LocationID=0,
                    Description=model.Description,
                    DisPer=model.DisPer,
                    SupInvNo="",
                    CurrencyId=model.CurrencyId,
                    PONO="",

                };
                _assetopening.Insert(order);
                
                _assetopening.SaveChangesAsync();
                List<AssetPurchaseVoucherDetails> orderDetails = new List<AssetPurchaseVoucherDetails>();
                orderDetails.AddRange(model.AssetPurchaseVoucherDetail.Select(x => new AssetPurchaseVoucherDetails
                {
                    AstPurDetID= vouhcerDetailid,
                    AstPurchaseID=Id,
                    AssetSize=x.AssetSize,
                    AstPurDate=DateTime.Now,
                    Amount=x.Amount,
                    Quantity=x.Quantity,
                    BookValue=x.BookValue,
                    AccDepAmt=x.AccDepAmt,
                    Rate=x.Rate,
                    LifrInYrs=x.LifrInYrs,
                    DepMode=x.DepMode,
                    Colour=x.Colour,
                    AstID=x.AstID,
                    AstPurRefNo=x.AstPurRefNo,
                    BatchCode=x.BatchCode,
                }));
                _assetopeningDetail.InsertList(orderDetails);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel log = new AddActivityLogViewModel()
                {
                    Page = "Asset Opening",
                    Section = "Add Asset Opening ",
                    Entity = " Asset Opening ",

                };
                await _utilityService.AddUserTrackingLog(log);
                #endregion
                _assetopeningDetail.SaveChangesAsync();
                return "Record Added successfully.";
            }
            catch (Exception)
            {
                return null;
            }
        }
        private async Task<string> EditAssetOpening(AddEditAssetPurchaseVoucher model)
        {
            try
            {
                Random random = new Random();
                int vouhcerDetailid = random.Next();
                var orders = _assetopening.GetAsQueryable().FirstOrDefault(x => x.AstPurID == model.AstPurID);
                orders.ActualAmount = model.ActualAmount;
                // orders.AstPurchaseType = "Cash";
                //orders.AstPurDt = DateTime.Now;
                //   orders.GRDate = DateTime.Now;
                //  orders.LPODate = DateTime.Now;
                // orders.LPONo = "",
                orders.AstPurDt = Convert.ToDateTime(model.AstPurDt);
                orders.NetAmount = model.NetAmount;
                //orders.TransportCost = 0,
                // orders.Handlingcharges = 0,
                orders.DrAccNo = PurchaseAccNo;
                   orders.DrAmount = model.DrAmount;
                   orders.DisAmount = model.DisAmount;
                //   orders.FSNO = financial != null ? Convert.ToInt32(financial.FinancialPeriodsFsno) : 0,
                // orders.Status = "P",
                orders.UserID = 0;
                //orders.FcRate = 0,
                //orders.LocationID = 0,
                orders.Description = model.Description;
                orders.DisPer = model.DisPer;
                // orders.SupInvNo = "",
                orders.CurrencyId = model.CurrencyId;
                // orders.PONO = "",
                _assetopening.Update(orders);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel log = new AddActivityLogViewModel()
                {
                    Page = "Asset Opening",
                    Section = "Update Asset Opening ",
                    Entity = " Asset Opening ",

                };
                await _utilityService.AddUserTrackingLog(log);
                #endregion
                _assetopening.SaveChangesAsync();

                var listOrdersDetails = _assetopeningDetail.GetAsQueryable().Where(x => x.AstPurchaseID == model.AstPurID).ToList();
                _assetopeningDetail.DeleteList(listOrdersDetails);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel Deletelog = new AddActivityLogViewModel()
                {
                    Page = "Asset Opening",
                    Section = "Delete Asset Opening ",
                    Entity = " Asset Opening ",

                };
                await _utilityService.AddUserTrackingLog(Deletelog);
                #endregion
                _assetopeningDetail.SaveChangesAsync();
                List<AssetPurchaseVoucherDetails> orderDetails = new List<AssetPurchaseVoucherDetails>();
                orderDetails.AddRange(model.AssetPurchaseVoucherDetail.Select(x => new AssetPurchaseVoucherDetails
                {
                    AstPurDetID = vouhcerDetailid,
                    AstPurchaseID = model.AstPurID,
                    AssetSize = x.AssetSize,
                    AstPurDate = DateTime.Now,
                    Amount = x.Amount,
                    Quantity = x.Quantity,
                    BookValue = x.BookValue,
                    AccDepAmt = x.AccDepAmt,
                    Rate = x.Rate,
                    LifrInYrs = x.LifrInYrs,
                    DepMode = x.DepMode,
                    Colour = x.Colour,
                    AstID = x.AstID,
                    AstPurRefNo = x.AstPurRefNo,
                    BatchCode = x.BatchCode,
                }));
                _assetopeningDetail.InsertList(orderDetails);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel Detaillog = new AddActivityLogViewModel()
                {
                    Page = "Asset Opening ",
                    Section = "Update Asset Opening Detail",
                    Entity = " Asset Opening Detail",

                };
                await _utilityService.AddUserTrackingLog(Detaillog);
                #endregion
                _assetopeningDetail.SaveChangesAsync();

                return "Record Updated successfully.";
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Response<AddEditAssetPurchaseVoucher>> GetAssetOpening(int id = 0)
        {
            try
            {
                AddEditAssetPurchaseVoucher model = new AddEditAssetPurchaseVoucher();
                await Task.Run(() =>
                {
                    var orders = _assetopening.GetAsQueryable().FirstOrDefault(x => x.AstPurID == id);
                    model.AstPurchaseID = orders.AstPurchaseID;
                    model.AstPurID = orders.AstPurID;
                    model.AstPurDt = orders.AstPurDt;
                    model.ActualAmount = orders.ActualAmount;
                    model.NetAmount = orders.NetAmount;
                    model.DrAccNo = orders.DrAccNo;
                    model.DrAmount = orders.DrAmount;
                    model.DisAmount = orders.DisAmount;
                    model.Description = orders.Description;
                    model.DisPer = orders.DisPer;
                    model.CurrencyId = orders.CurrencyId;
                    model.AssetPurchaseVoucherDetail = _assetopeningDetail.GetAsQueryable()
                        .Where(x => x.AstPurchaseID == id).Select(x => new AddEditAssetPurchaseVoucherDetail
                        {
                            AstPurDetID = x.AstPurDetID,
                            AstPurchaseID = x.AstPurchaseID,
                            AssetSize = x.AssetSize,
                            AstPurDate = x.AstPurDate,
                            Amount = x.Amount,
                            Quantity = x.Quantity,
                            BookValue = x.BookValue,
                            AccDepAmt = x.AccDepAmt,
                            Rate = x.Rate,
                            LifrInYrs = x.LifrInYrs,
                            DepMode = x.DepMode,
                            Colour = x.Colour,
                            AstID = x.AstID,
                            AstPurRefNo = x.AstPurRefNo,
                            BatchCode = x.BatchCode,
                        }).ToList();

                });
                return Response<AddEditAssetPurchaseVoucher>.Success(model, "Records FOund.");
            }
            catch (Exception ex)
            {
                return Response<AddEditAssetPurchaseVoucher>.Fail(new AddEditAssetPurchaseVoucher(), ex.Message);
            }
        }
    }
}
