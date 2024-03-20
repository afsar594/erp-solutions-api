using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.AccountStatement;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Inspire.Erp.Application.Account.Implementations
{
    public class VoucherAllocationService : IVoucherAllocationService
    {
        private readonly IRepository<CustomerMaster> _customerMaster;
        private readonly IRepository<SuppliersMaster> _supplierMaster;
        private readonly IRepository<AccountsTransactions> _accountTransaction;
        private readonly IRepository<MasterAccountsTable> _masterAccountTable;
        private readonly IRepository<AllocationVoucher> _voucherAllocation;
        private readonly IRepository<AllocationVoucherDetails> _voucherAllocationDetails;
        private readonly IUtilityService _utilityService;
        private readonly IRepository<CurrencyMaster> _currencyMaster;

        public VoucherAllocationService(IRepository<CustomerMaster> customerMaster, IRepository<AccountsTransactions> accountTransaction,
            IRepository<SuppliersMaster> supplierMaster, IRepository<AllocationVoucherDetails> voucherAllocationDetails,
            IRepository<MasterAccountsTable> masterAccountTable, IRepository<AllocationVoucher> voucherAllocation, IUtilityService utilityService, IRepository<CurrencyMaster> currencyMaster)
        {
            _customerMaster = customerMaster;
            _supplierMaster = supplierMaster;
            _accountTransaction = accountTransaction;
            _masterAccountTable = masterAccountTable;
            _voucherAllocation = voucherAllocation;
            _voucherAllocationDetails = voucherAllocationDetails;
            _utilityService = utilityService;
            _currencyMaster = currencyMaster;
        }
        public async Task<Response<List<DropdownResponse>>> GetCustomerMasterDropdown()
        {
            try
            {
                var response = new List<DropdownResponse>();
                //response.Add(new DropdownResponse()
                //{
                //    Value = "",
                //    Name = " All "
                //});
                response.AddRange(await _customerMaster.ListSelectAsync(x => 1 == 1, x => new DropdownResponse
                {
                    Value = x.CustomerMasterCustomerReffAcNo,
                    Name = x.CustomerMasterCustomerName
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
                //    Name = " All "
                //});
                response.AddRange(await _supplierMaster.ListSelectAsync(x => 1 == 1, x => new DropdownResponse
                {
                    Value = x.SuppliersMasterSupplierReffAcNo,
                    Name = x.SuppliersMasterSupplierName
                }));
                return Response<List<DropdownResponse>>.Success(response, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<DropdownResponse>>.Fail(new List<DropdownResponse>(), ex.Message);
            }
        }

        public async Task<Response<GridWrapperResponse<List<GetAccountTransactionMutilAccountResponse>>>> GetAccountTransactions(GenericGridViewModel model)
        {
            try
            {
                string query = @$" AccountstransactionsDelStatus != true {model.Filter}";


                //var a = await _accountTransaction.GetAsQueryable().Where(x =>( x.AccountsTransactionsTransDate >= Convert.ToDateTime("")) &&  )
                //    .ToListAsync();va
                var result = await _accountTransaction.GetAsQueryable().Where(query).Select(x => new GetAccountTransactionMutilAccountResponse
                {
                    AccountsTransactionsAccNo = x.AccountsTransactionsAccNo,
                    //AccountsTransactionsAccName = _masterAccountTable.GetAsQueryable().FirstOrDefault(c => c.MasterAccountsTableAccNo == x.AccountsTransactionsAccNo).MasterAccountsTableAccName,
                    AccountsTransactionsDebit = Convert.ToDecimal(x.AccountsTransactionsDebit),
                    AccountsTransactionsCredit = Convert.ToDecimal(x.AccountsTransactionsCredit),
                    AccountsTransactionsAllocBalance = x.AccountsTransactionsAllocBalance,
                    AccountsTransactionsVoucherNo = x.AccountsTransactionsVoucherNo,
                    AccountsTransactionsVoucherType = x.AccountsTransactionsVoucherType,
                    AccountsTransactionsDescription = x.AccountsTransactionsDescription,
                    AccountsTransactionsTransDate = x.AccountsTransactionsTransDate != null ? Convert.ToDateTime(x.AccountsTransactionsTransDate) : DateTime.MinValue,
                    AccountsTransactionsTransDateString = Convert.ToDateTime(x.AccountsTransactionsTransDate).ToString("MM-dd-yyyy"),
                    AccountsTransactionsParticulars = x.AccountsTransactionsParticulars,
                    AccountsTransactionsFsno = x.AccountsTransactionsFsno,
                    RefNo = x.RefNo,
                    AccountsTransactionsTransSno = Convert.ToInt32(x.AccountsTransactionsTransSno),
                    AccountsTransactionsAllocDebit = x.AccountsTransactionsAllocDebit,
                    AccountsTransactionsAllocCredit = x.AccountsTransactionsAllocCredit,
                    AccountsTransactionsAllocUpdateBal = x.AccountsTransactionsAllocUpdateBal,
                    AccountsTransactionsAllocationbalance = x.AccountsTransactionsDebit > 0 ? x.AccountsTransactionsDebit : x.AccountsTransactionsCredit

                }).ToListAsync();

                var gridReponse = new GridWrapperResponse<List<GetAccountTransactionMutilAccountResponse>>();
                gridReponse.Data = result;
                var total = 0;
                gridReponse.Total = Convert.ToInt32(total);
                return Response<GridWrapperResponse<List<GetAccountTransactionMutilAccountResponse>>>.Success(gridReponse, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<List<GetAccountTransactionMutilAccountResponse>>>.Fail(new GridWrapperResponse<List<GetAccountTransactionMutilAccountResponse>>(), ex.Message);
            }
        }
        public async Task<Response<bool>> AddEditVoucherAllocation(AllocationVoucher model)
        {

            using (var trans = new TransactionScope())
            {
                try
                {
                    if (model.AllocationVoucherId == null || model.AllocationVoucherId == 0)
                    {

                        AllocationVoucher order = new AllocationVoucher()
                        {

                            AllocationVoucherType = model.AllocationVoucherType,
                            AllocationVoucherVoucherAccType = model.AllocationVoucherVoucherAccType,
                            AllocationVoucherDelStatus = false,
                            AllocationVoucherVoucherDate = model.AllocationVoucherVoucherDate,
                            AllocationVoucherVcreation = model.AllocationVoucherVcreation,
                            AllocationVoucherVoucherNo = model.AllocationVoucherVoucherNo,
                            AllocationVoucherVoucherAccNo = model.AllocationVoucherVoucherAccNo,
                            AllocationVoucherId = getMaxVoucherAllocation()

                        };
                        _voucherAllocation.Insert(order);
                        #region ADD ACTIVITY LOGS
                        AddActivityLogViewModel log = new AddActivityLogViewModel()
                        {
                            Page = "Voucher Allocation",
                            Section = "Add Voucher Allocation ",
                            Entity = "Voucher Allocation ",

                        };
                        //await _utilityService.AddUserTrackingLog(log);
                        #endregion
                        //_voucherAllocation.SaveChangesAsync();

                        if (model.VoucherAllocationDetails != null && model.VoucherAllocationDetails.Count() > 0)
                        {
                            var vouchetAlloc = _voucherAllocation.GetAll().LastOrDefault();
                            Console.WriteLine("************************************************** = " + vouchetAlloc.AllocationVoucherId);
                            List<AllocationVoucherDetails> orderDetails = new List<AllocationVoucherDetails>();

                            foreach (var x in model.VoucherAllocationDetails)
                            {
                                orderDetails.Add(
                                new AllocationVoucherDetails
                                {
                                    AllocationVoucherDetailsVno = x.AllocationVoucherDetailsVno,
                                    AllocationVoucherDetailsAllocDebit = x.AllocationVoucherDetailsAllocDebit,
                                    AllocationVoucherDetailsAllocCredit = x.AllocationVoucherDetailsAllocCredit,
                                    AllocationVoucherDetailsVtype = x.AllocationVoucherDetailsVtype,
                                    AllocationVoucherDetailsDelStatus = false,
                                    AllocationVoucherDetailsSno = Convert.ToInt32(vouchetAlloc.AllocationVoucherId),
                                    AllocationVoucherDetailsVFsno = x.AllocationVoucherDetailsRefFsno,
                                    AllocationVoucherDetailsVLocationId = x.AllocationVoucherDetailsVLocationId,
                                    AllocationVoucherDetailsTransSno = (int)x.AccountsTransactionsTransSno,
                                    AllocationVoucherDetailsAccNo = x.AllocationVoucherDetailsAccNo,
                                    AllocationVoucherDetailsFcAllocDebit = x.AllocationVoucherDetailsFcAllocDebit,
                                    AllocationVoucherDetailsFcAllocCredit = x.AllocationVoucherDetailsFcAllocCredit,
                                    AllocationVoucherDetailsRefVno = x.AllocationVoucherDetailsRefVno,
                                    AllocationVoucherDetailsRefVtype = x.AllocationVoucherDetailsRefVtype,
                                    AllocationVoucherDetailsRefLocationId = x.AllocationVoucherDetailsRefLocationId,
                                    AllocationVoucherDetailsRefFsno = x.AllocationVoucherDetailsRefFsno,
                                    //AllocationVoucherDetailsId = getMaxVoucherAllocationDetails()
                                });
                            }

                            //orderDetails.AddRange(model.VoucherAllocationDetails.Select(x => new AllocationVoucherDetails
                            //{
                            //    AllocationVoucherDetailsVno = x.AllocationVoucherDetailsVno,
                            //    AllocationVoucherDetailsAllocDebit = x.AllocationVoucherDetailsAllocDebit,
                            //    AllocationVoucherDetailsAllocCredit = x.AllocationVoucherDetailsAllocCredit,
                            //    AllocationVoucherDetailsVtype = x.AllocationVoucherDetailsVtype,
                            //    AllocationVoucherDetailsDelStatus = false,
                            //    AllocationVoucherDetailsSno = Convert.ToInt32(vouchetAlloc.AllocationVoucherId),
                            //    AllocationVoucherDetailsVFsno = x.AllocationVoucherDetailsRefFsno,
                            //    AllocationVoucherDetailsVLocationId = x.AllocationVoucherDetailsVLocationId,
                            //    AllocationVoucherDetailsTransSno = (int)x.AccountsTransactionsTransSno,
                            //    AllocationVoucherDetailsAccNo = x.AllocationVoucherDetailsAccNo,
                            //    AllocationVoucherDetailsFcAllocDebit = x.AllocationVoucherDetailsFcAllocDebit,
                            //    AllocationVoucherDetailsFcAllocCredit = x.AllocationVoucherDetailsFcAllocCredit,
                            //    AllocationVoucherDetailsRefVno = x.AllocationVoucherDetailsRefVno,
                            //    AllocationVoucherDetailsRefVtype = x.AllocationVoucherDetailsRefVtype,
                            //    AllocationVoucherDetailsRefLocationId = x.AllocationVoucherDetailsRefLocationId,
                            //    AllocationVoucherDetailsRefFsno = x.AllocationVoucherDetailsRefFsno,
                            //    //AllocationVoucherDetailsId = getMaxVoucherAllocationDetails()
                            //}));
                            _voucherAllocationDetails.InsertList(orderDetails);
                            _voucherAllocationDetails.SaveChangesAsync();

                            //here need to update the accounts transaction table for allocation
                            List<AccountsTransactions> accountsTransactions = new List<AccountsTransactions>();
                            foreach (var item in model.VoucherAllocationDetails)
                            {
                                AccountsTransactions acc = _accountTransaction.GetAsQueryable().Where(x => x.AccountsTransactionsTransSno == item.AccountsTransactionsTransSno).FirstOrDefault();
                                var currency = _currencyMaster.GetAsQueryable().Where(c => c.CurrencyMasterCurrencyId == acc.AccountsTransactionsCurrencyId).Distinct().FirstOrDefault();
                                if (acc != null)
                                {
                                    if (item.AllocationVoucherDetailsAllocDebit > 0)
                                    {

                                        acc.AccountsTransactionsAllocDebit = (decimal)(item.AccountsTransactionsAllocBalance != null ? item.AccountsTransactionsAllocBalance : 0); //NetAlloc;
                                        acc.AccountsTransactionsFcAllocDebit = (decimal)(item.AllocationVoucherDetailsAllocDebit != null ? item.AllocationVoucherDetailsAllocDebit : 1) * (currency != null ? (decimal)(currency.CurrencyMasterCurrencyRate != null ? currency.CurrencyMasterCurrencyRate : 1) : 1);
                                        acc.AccountsTransactionsFcAllocBalance = acc.AccountsTransactionsDebit - (acc.AccountsTransactionsFcAllocDebit != null ? acc.AccountsTransactionsFcAllocDebit : 0 + (decimal)(item.AllocationVoucherDetailsAllocDebit != null ? item.AllocationVoucherDetailsAllocDebit : 0));
                                        acc.AccountsTransactionsAllocBalance = acc.AccountsTransactionsFcDebit != null ? acc.AccountsTransactionsFcDebit : 0 - (acc.AccountsTransactionsAllocDebit != null ? acc.AccountsTransactionsAllocDebit : 0 + ((decimal)(item.AllocationVoucherDetailsAllocDebit != null ? item.AllocationVoucherDetailsAllocDebit : 1) * (currency != null ? (decimal)(currency.CurrencyMasterCurrencyRate != null ? currency.CurrencyMasterCurrencyRate : 1) : 1)));
                                    }
                                    else if (item.AllocationVoucherDetailsAllocCredit > 0)
                                    {
                                        acc.AccountsTransactionsAllocCredit = (decimal)(item.AccountsTransactionsAllocBalance != null ? item.AccountsTransactionsAllocBalance : 0); //NetAlloc;;
                                        acc.AccountsTransactionsFcAllocCredit = (decimal)(item.AllocationVoucherDetailsAllocCredit != null ? item.AllocationVoucherDetailsAllocCredit : 1) * (currency != null ? (decimal)(currency.CurrencyMasterCurrencyRate != null ? currency.CurrencyMasterCurrencyRate : 1) : 1);
                                        acc.AccountsTransactionsFcAllocBalance = acc.AccountsTransactionsCredit - (acc.AccountsTransactionsFcAllocCredit != null ? acc.AccountsTransactionsFcAllocCredit : 0 + (decimal)(item.AllocationVoucherDetailsAllocCredit != null ? item.AllocationVoucherDetailsAllocCredit : 0));
                                        acc.AccountsTransactionsAllocBalance = acc.AccountsTransactionsFcCredit != null ? acc.AccountsTransactionsFcCredit : 0 - (acc.AccountsTransactionsAllocCredit != null ? acc.AccountsTransactionsAllocCredit : 0 + ((decimal)(item.AllocationVoucherDetailsAllocCredit != null ? item.AllocationVoucherDetailsAllocCredit : 1) * (currency != null ? (decimal)(currency.CurrencyMasterCurrencyRate != null ? currency.CurrencyMasterCurrencyRate : 1) : 1)));
                                    }

                                    accountsTransactions.Add(acc);
                                }
                            }
                            _accountTransaction.UpdateList(accountsTransactions);
                        }

                    }
                    else
                    {

                        var orders = _voucherAllocation.GetAsQueryable().FirstOrDefault(x => x.AllocationVoucherId == model.AllocationVoucherId);
                        orders.AllocationVoucherType = model.AllocationVoucherType;
                        orders.AllocationVoucherDelStatus = false;
                        orders.AllocationVoucherVcreation = model.AllocationVoucherVcreation;
                        orders.AllocationVoucherVoucherNo = model.AllocationVoucherVoucherNo;
                        orders.AllocationVoucherVoucherAccType = model.AllocationVoucherVoucherAccType;
                        orders.AllocationVoucherVoucherAccNo = model.AllocationVoucherVoucherAccNo;
                        orders.AllocationVoucherVoucherDate = model.AllocationVoucherVoucherDate;
                        _voucherAllocation.Update(orders);
                        #region ADD ACTIVITY LOGS
                        AddActivityLogViewModel log = new AddActivityLogViewModel()
                        {
                            Page = "Voucher Allocation",
                            Section = "Update Voucher Allocation ",
                            Entity = "Voucher Allocation ",

                        };
                        await _utilityService.AddUserTrackingLog(log);
                        #endregion
                        _voucherAllocation.SaveChangesAsync();

                        var listOrdersDetails = _voucherAllocationDetails.GetAsQueryable().Where(x => x.AllocationVoucherDetailsSno == model.AllocationVoucherId).ToList();
                        _voucherAllocationDetails.DeleteList(listOrdersDetails);
                        #region ADD ACTIVITY LOGS
                        AddActivityLogViewModel Deletelog = new AddActivityLogViewModel()
                        {
                            Page = "Voucher Allocation",
                            Section = "Delete Voucher Allocation ",
                            Entity = "Voucher Allocation ",

                        };
                        //await _utilityService.AddUserTrackingLog(Deletelog);
                        #endregion
                        _voucherAllocationDetails.SaveChangesAsync();

                        List<AllocationVoucherDetails> orderDetails = new List<AllocationVoucherDetails>();
                        orderDetails.AddRange(model.VoucherAllocationDetails.Select(x => new AllocationVoucherDetails
                        {
                            AllocationVoucherDetailsVno = x.AllocationVoucherDetailsVno,
                            AllocationVoucherDetailsAllocDebit = x.AllocationVoucherDetailsAllocDebit,
                            AllocationVoucherDetailsAllocCredit = x.AllocationVoucherDetailsAllocCredit,
                            AllocationVoucherDetailsId = x.AllocationVoucherDetailsId,
                            AllocationVoucherDetailsVtype = x.AllocationVoucherDetailsVtype,
                            AllocationVoucherDetailsDelStatus = false,
                            AllocationVoucherDetailsSno = Convert.ToInt32(model.AllocationVoucherId),
                        }));
                        _voucherAllocationDetails.InsertList(orderDetails);
                        _voucherAllocationDetails.SaveChangesAsync();
                    }
                    trans.Complete();
                    return Response<bool>.Success(true, "Changes Saved Successfully.");

                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    return Response<bool>.Fail(false, ex.Message);
                }
            }
        }

        private int getMaxVoucherAllocationDetails()
        {
            try
            {
                int? maxValue = _voucherAllocationDetails.GetAll().Max(x => x.AllocationVoucherDetailsId);
                int? incrementedValue = maxValue.HasValue ? maxValue + 1 : 1;
                return (int)incrementedValue;
            }
            catch
            {
                return 1;
            }
        }

        private int getMaxVoucherAllocation()
        {
            try
            {
                int? maxValue = _voucherAllocation.GetAll().Max(x => x.AllocationVoucherId);
                int? incrementedValue = maxValue.HasValue ? maxValue + 1 : 1;
                return (int)incrementedValue;
            }
            catch
            {
                return 1;
            }
        }

        private async Task<string> AddVoucherAllocation(AddEditVoucherAllocationResponse model)
        {
            try
            {
                // var maxid = _voucherNumber.GetAsQueryable();
                Random random = new Random();
                int voucherId = random.Next();
                int vouhcerDetailid = random.Next();
                //var a= maxid.Max(x => Math.Round(Convert.ToDecimal(x.VouchersNumbersVsno)))+1;
                AllocationVoucher order = new AllocationVoucher()
                {
                    AllocationVoucherId = voucherId,
                    AllocationVoucherType = model.AllocationVoucherType,
                    AllocationVoucherVoucherAccType = model.AllocationVoucherVoucherAccType,
                    AllocationVoucherDelStatus = false,
                    AllocationVoucherVoucherDate = model.AllocationVoucherVoucherDate,
                    AllocationVoucherVcreation = model.AllocationVoucherVcreation,
                    AllocationVoucherVoucherNo = model.AllocationVoucherVoucherNo,
                    AllocationVoucherVoucherAccNo = model.AllocationVoucherVoucherAccNo,

                };
                _voucherAllocation.Insert(order);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel logs = new AddActivityLogViewModel()
                {
                    Page = "Voucher Allocation",
                    Section = "Add Voucher Allocation",
                    Entity = "Voucher Allocation",

                };
                await _utilityService.AddUserTrackingLog(logs);
                #endregion
                _voucherAllocation.SaveChangesAsync();
                List<AllocationVoucherDetails> orderDetails = new List<AllocationVoucherDetails>();
                orderDetails.AddRange(model.VoucherAllocationDetails.Select(x => new AllocationVoucherDetails
                {
                    AllocationVoucherDetailsVno = x.AllocationVoucherDetailsVno,
                    AllocationVoucherDetailsAllocDebit = x.AllocationVoucherDetailsAllocDebit,
                    AllocationVoucherDetailsAllocCredit = x.AllocationVoucherDetailsAllocCredit,
                    AllocationVoucherDetailsId = vouhcerDetailid,
                    AllocationVoucherDetailsVtype = x.AllocationVoucherDetailsVtype,
                    AllocationVoucherDetailsDelStatus = false,
                    AllocationVoucherDetailsSno = voucherId,
                }));
                _voucherAllocationDetails.InsertList(orderDetails);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel log = new AddActivityLogViewModel()
                {
                    Page = "Voucher Allocation",
                    Section = "Add Voucher Allocation Detail",
                    Entity = "Voucher Allocation Detail",

                };
                await _utilityService.AddUserTrackingLog(logs);
                #endregion
                _voucherAllocationDetails.SaveChangesAsync();
                return "Record Added successfully.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<string> EditVoucherAllocation(AddEditVoucherAllocationResponse model)
        {
            try
            {
                Random random = new Random();
                int vouhcerDetailid = random.Next();
                var orders = _voucherAllocation.GetAsQueryable().FirstOrDefault(x => x.AllocationVoucherId == model.AllocationVoucherId);
                orders.AllocationVoucherType = model.AllocationVoucherType;
                orders.AllocationVoucherDelStatus = false;
                orders.AllocationVoucherVcreation = model.AllocationVoucherVcreation;
                orders.AllocationVoucherVoucherNo = model.AllocationVoucherVoucherNo;
                orders.AllocationVoucherVoucherAccType = model.AllocationVoucherVoucherAccType;
                orders.AllocationVoucherVoucherAccNo = model.AllocationVoucherVoucherAccNo;
                orders.AllocationVoucherVoucherDate = model.AllocationVoucherVoucherDate;
                _voucherAllocation.Update(orders);
                #region ADD ACTIVITY LOGS
                AddActivityLogViewModel logs = new AddActivityLogViewModel()
                {
                    Page = "Voucher Allocation",
                    Section = "Update Voucher Allocation",
                    Entity = "Voucher Allocation ",

                };
                await _utilityService.AddUserTrackingLog(logs);
                #endregion
                _voucherAllocation.SaveChangesAsync();

                var listOrdersDetails = _voucherAllocationDetails.GetAsQueryable().Where(x => x.AllocationVoucherDetailsSno == model.AllocationVoucherId).ToList();
                _voucherAllocationDetails.DeleteList(listOrdersDetails);
                #region  
                AddActivityLogViewModel log = new AddActivityLogViewModel()
                {
                    Page = "Voucher Allocation",
                    Section = "Delete Voucher Allocation Detail",
                    Entity = "Voucher Allocation Detail",

                };
                await _utilityService.AddUserTrackingLog(log);
                #endregion
                _voucherAllocationDetails.SaveChangesAsync();
                List<AllocationVoucherDetails> orderDetails = new List<AllocationVoucherDetails>();
                orderDetails.AddRange(model.VoucherAllocationDetails.Select(x => new AllocationVoucherDetails
                {
                    AllocationVoucherDetailsVno = x.AllocationVoucherDetailsVno,
                    AllocationVoucherDetailsAllocDebit = x.AllocationVoucherDetailsAllocDebit,
                    AllocationVoucherDetailsAllocCredit = x.AllocationVoucherDetailsAllocCredit,
                    AllocationVoucherDetailsId = vouhcerDetailid,
                    AllocationVoucherDetailsVtype = x.AllocationVoucherDetailsVtype,
                    AllocationVoucherDetailsDelStatus = false,
                    AllocationVoucherDetailsSno = Convert.ToInt32(model.AllocationVoucherId),
                }));
                _voucherAllocationDetails.InsertList(orderDetails);
                #region  
                AddActivityLogViewModel loglogDetail = new AddActivityLogViewModel()
                {
                    Page = "Voucher Allocation",
                    Section = "Delete Voucher Allocation Detail",
                    Entity = "Voucher Allocation Detail",

                };
                await _utilityService.AddUserTrackingLog(log);
                #endregion
                _voucherAllocationDetails.SaveChangesAsync();

                return "Record Updated successfully.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Response<GridWrapperResponse<List<GetVoucherAllocationListResponse>>>> GetVoucherAllocationsList(GenericGridViewModel model)
        {
            try
            {
                List<GetVoucherAllocationListResponse> response = new List<GetVoucherAllocationListResponse>();
                var orders = await _voucherAllocation.GetAsQueryable().ToListAsync();
                response.AddRange(orders.Select(x => new GetVoucherAllocationListResponse
                {
                    AllocationVoucherId = x.AllocationVoucherId,
                    AllocationVoucherType = x.AllocationVoucherType,
                    AllocationVoucherDelStatus = x.AllocationVoucherDelStatus,
                    AllocationVoucherVcreation = x.AllocationVoucherVcreation,
                    AllocationVoucherVoucherNo = x.AllocationVoucherVoucherNo,
                    AllocationVoucherVoucherAccNo = x.AllocationVoucherVoucherAccNo,
                    AllocationVoucherVoucherDate = x.AllocationVoucherVoucherDate
                }).ToList());
                var gridResponse = new GridWrapperResponse<List<GetVoucherAllocationListResponse>>();
                gridResponse.Data = response;
                gridResponse.Total = 0;
                return Response<GridWrapperResponse<List<GetVoucherAllocationListResponse>>>.Success(gridResponse, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<List<GetVoucherAllocationListResponse>>>.Fail(new GridWrapperResponse<List<GetVoucherAllocationListResponse>>(), ex.Message);
            }
        }
        public async Task<Response<GetVoucherAllocationListResponse>> GetSpecificVoucherAllocation(int id = 0)
        {
            try
            {
                GetVoucherAllocationListResponse model = new GetVoucherAllocationListResponse();
                await Task.Run(() =>
                {
                    var orders = _voucherAllocation.GetAsQueryable().FirstOrDefault(x => x.AllocationVoucherId == id);
                    model.AllocationVoucherType = orders.AllocationVoucherType != null ? orders.AllocationVoucherType : 0;
                    model.AllocationVoucherVcreation = orders.AllocationVoucherVcreation != null ? orders.AllocationVoucherVcreation : "";
                    model.AllocationVoucherVoucherNo = orders.AllocationVoucherVoucherNo != null ? orders.AllocationVoucherVoucherNo : "";
                    model.AllocationVoucherVoucherAccNo = orders.AllocationVoucherVoucherAccNo != null ? orders.AllocationVoucherVoucherAccNo : "";
                    model.AllocationVoucherVoucherAccType = orders.AllocationVoucherVoucherAccType != null ? orders.AllocationVoucherVoucherAccType : "";
                    model.AllocationVoucherVoucherDate = orders.AllocationVoucherVoucherDate != null ? orders.AllocationVoucherVoucherDate : DateTime.MinValue;
                    model.VoucherAllocationDetails = _voucherAllocationDetails.GetAsQueryable()
                        .Where(x => x.AllocationVoucherDetailsSno == id).Select(x => new VoucherAllocationDetailsResponse
                        {
                            AllocationVoucherDetailsVno = x.AllocationVoucherDetailsVno,
                            AllocationVoucherDetailsAllocDebit = x.AllocationVoucherDetailsAllocDebit,
                            AllocationVoucherDetailsAllocCredit = x.AllocationVoucherDetailsAllocCredit,
                            AllocationVoucherDetailsId = x.AllocationVoucherDetailsId,
                            AllocationVoucherDetailsVtype = x.AllocationVoucherDetailsVtype,
                            AllocationVoucherDetailsSno = Convert.ToInt32(model.AllocationVoucherId),
                        }).ToList();

                });
                return Response<GetVoucherAllocationListResponse>.Success(model, "Records FOund.");
            }
            catch (Exception ex)
            {
                return Response<GetVoucherAllocationListResponse>.Fail(new GetVoucherAllocationListResponse(), ex.Message);
            }
        }

        public IQueryable GetVoucherAllocation()
        {
            try
            {
                var dataList = _voucherAllocation.GetAll().ToList().AsQueryable();
                return dataList;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public AllocationVoucher DeleteAllocationVoucher(long ID)
        {
            using (var trans = new TransactionScope())
            {
                try
                {
                    var allocationDetails = _voucherAllocation.GetAll().FirstOrDefault(o => o.AllocationVoucherId == ID);
                    var allocationDetailsList = _voucherAllocationDetails.GetAll().Where(o => o.AllocationVoucherDetailsSno == ID).ToList();
                    _voucherAllocation.Delete(allocationDetails);
                    if (allocationDetailsList != null && allocationDetailsList.Count() > 0)
                    {
                        _voucherAllocationDetails.DeleteList(allocationDetailsList);
                    }

                    //here need to update the accounts transaction table for allocation
                    List<AccountsTransactions> accountsTransactions = new List<AccountsTransactions>();
                    foreach (var item in allocationDetailsList)
                    {
                        AccountsTransactions acc = _accountTransaction.GetAsQueryable().Where(x => x.AccountsTransactionsTransSno == item.AccountsTransactionsTransSno).FirstOrDefault();
                        var currency = _currencyMaster.GetAsQueryable().Where(c => c.CurrencyMasterCurrencyId == acc.AccountsTransactionsCurrencyId).Distinct().FirstOrDefault();
                        if (acc != null)
                        {
                            if (item.AllocationVoucherDetailsAllocDebit > 0)
                            {
                                acc.AccountsTransactionsAllocDebit = 0; //+= (decimal)item.AllocationVoucherDetailsAllocDebit; //NetAlloc;
                                acc.AccountsTransactionsFcAllocDebit = 0; //+= (decimal)item.AllocationVoucherDetailsAllocDebit * (decimal)currency.CurrencyMasterCurrencyRate;
                                acc.AccountsTransactionsFcAllocBalance = 0; //= acc.AccountsTransactionsDebit - (acc.AccountsTransactionsFcAllocDebit + (decimal)item.AllocationVoucherDetailsAllocDebit);
                                acc.AccountsTransactionsFcAllocBalance = 0; //= acc.AccountsTransactionsFcDebit - (acc.AccountsTransactionsAllocDebit + ((decimal)item.AllocationVoucherDetailsAllocDebit * (decimal)currency.CurrencyMasterCurrencyRate));
                            }
                            else if (item.AllocationVoucherDetailsAllocCredit > 0)
                            {
                                acc.AccountsTransactionsAllocCredit = 0; //+= (decimal)item.AllocationVoucherDetailsAllocCredit; //NetAlloc;;
                                acc.AccountsTransactionsFcAllocCredit = 0; //+= (decimal)item.AllocationVoucherDetailsAllocCredit * (decimal)currency.CurrencyMasterCurrencyRate;
                                acc.AccountsTransactionsAllocBalance = 0; //= acc.AccountsTransactionsCredit - (acc.AccountsTransactionsFcAllocCredit + (decimal)item.AllocationVoucherDetailsAllocCredit);
                                acc.AccountsTransactionsFcAllocBalance = 0; //= acc.AccountsTransactionsFcCredit - (acc.AccountsTransactionsAllocCredit + ((decimal)item.AllocationVoucherDetailsAllocCredit * (decimal)currency.CurrencyMasterCurrencyRate));
                            }

                            accountsTransactions.Add(acc);
                        }
                    }
                    _accountTransaction.UpdateList(accountsTransactions);
                    trans.Complete();
                    return allocationDetails;
                }
                catch (System.Exception ex)
                {
                    trans.Dispose();
                    throw ex;
                }
            }

        }


    }
}


