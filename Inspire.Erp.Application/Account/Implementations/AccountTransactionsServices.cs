using Dapper;
using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Infrastructure;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Account.Implementations
{
    public class AccountTransactionsServices : IAccountTransactionsServices
    {
        private IRepository<AllocationVoucherDetails> allocationVDlts;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private readonly IDbConnection _dbConnection;
        public AccountTransactionsServices(IOptions<ApplicationSettings> applicationSettings, IRepository<AllocationVoucherDetails> allocationVDlts, IRepository<AccountsTransactions> accountTransactionRepository)
        {
            this.allocationVDlts = allocationVDlts;
            _accountTransactionRepository = accountTransactionRepository;
            _dbConnection = new SqlConnection(applicationSettings.Value.ConnectionString);
        }

        public async Task<ApiResponse<List<AllocationDetails>>> GetReceiptVoucherAllocations(string accNo, string refNo, string vType)
        {
            try
            {
                await Task.Delay(500);
                //var result = (from AD in this.allocationVDlts.GetAll()
                //              join AT in _accountTransactionRepository.GetAll() on AD.AllocationVoucherDetailsTransSno equals (int)AT.AccountsTransactionsTransSno
                //              where AD.AllocationVoucherDetailsAccNo == accNo && AD.AllocationVoucherDetailsVno.ToUpper() == refNo.ToUpper() && AD.AllocationVoucherDetailsRefVtype.ToUpper() == vType.ToUpper()
                //              select new Domain.Models.AllocationDetails()
                //              {
                //                  TransNo = AT.AccountsTransactionsTransSno,
                //                  Status = "P",
                //                  TransDate = AT.AccountsTransactionsTransDate,
                //                  VoucherNo = AD.AllocationVoucherDetailsVno,
                //                  Type = AD.AllocationVoucherDetailsRefVtype,
                //                  Credit = (decimal)AT.AccountsTransactionsCredit > 0 ? AT.AccountsTransactionsCredit : 0,
                //                  Debit = (decimal)AT.AccountsTransactionsDebit > 0 ? AT.AccountsTransactionsDebit : 0,
                //                  Balance = AT.AccountsTransactionsAllocBalance,
                //                  AllocAmount = AT.AccountsTransactionsAllocCredit > 0 ? AT.AccountsTransactionsAllocCredit : AT.AccountsTransactionsAllocDebit,
                //                  NetAllocation = getNatAllocationAmount(AT.AccountsTransactionsTransSno),
                //                  RefLocation = AD.AllocationVoucherDetailsRefLocationId.ToString()
                //              }).ToList();

                var transValues = _accountTransactionRepository.GetAsQueryable()
                                      .Where(a => a.AccountsTransactionsAccNo.ToUpper() == accNo.ToUpper() && a.AccountsTransactionsAllocBalance > 0 && a.AccountsTransactionsDebit != 0)
                                        .Select(g => new Domain.Models.AllocationDetails()
                                        {
                                            TransNo = g.AccountsTransactionsTransSno,
                                            Status = "P",
                                            TransDate = g.AccountsTransactionsTransDate,
                                            VoucherNo = g.AccountsTransactionsVoucherNo,
                                            Type = g.AccountsTransactionsVoucherType,
                                            Credit = (decimal)g.AccountsTransactionsCredit > 0 ? g.AccountsTransactionsCredit : 0,
                                            Debit = (decimal)g.AccountsTransactionsDebit > 0 ? g.AccountsTransactionsDebit : 0,
                                            Balance = g.AccountsTransactionsAllocBalance,
                                            AllocAmount = g.AccountsTransactionsAllocCredit > 0 ? g.AccountsTransactionsAllocCredit : g.AccountsTransactionsAllocDebit,
                                            RefLocation = g.AccountsTransactionsLocation.ToString()
                                        }).OrderBy(x => x.VoucherNo).AsEnumerable().ToList();

        

                //var transValues = _accountTransactionRepository.GetAsQueryable()
                //                      .Where(a => a.AccountsTransactionsAccNo.ToUpper() == accNo.ToUpper()
                //                        && a.AccountsTransactionsAllocBalance > 0
                //                        && ((vType == VoucherType.Receipt && a.AccountsTransactionsDebit != 0 && a.AccountsTransactionsVoucherType == VoucherType.SalesVoucher_TYPE)
                //                      || (vType == VoucherType.Payment && a.AccountsTransactionsCredit != 0 && a.AccountsTransactionsVoucherType == VoucherType.PurchaseVoucher_TYPE))
                //                      || vType == VoucherType.Receipt && a.AccountsTransactionsVoucherType == VoucherType.Receipt && a.AccountsTransactionsAllocCredit > 0 && a.AccountsTransactionsVoucherNo == refNo
                //                      || vType == VoucherType.Payment && a.AccountsTransactionsVoucherType == VoucherType.Payment && a.AccountsTransactionsAllocDebit > 0 && a.AccountsTransactionsVoucherNo == refNo)
                //                        .Select(g => new Domain.Models.AllocationDetails()
                //                        {
                //                            TransNo = g.AccountsTransactionsTransSno,
                //                            Status = "P",
                //                            TransDate = g.AccountsTransactionsTransDate,
                //                            VoucherNo = g.AccountsTransactionsVoucherNo,
                //                            Type = g.AccountsTransactionsVoucherType,
                //                            Credit = (decimal)g.AccountsTransactionsCredit > 0 ? g.AccountsTransactionsCredit : 0,
                //                            Debit = (decimal)g.AccountsTransactionsDebit > 0 ? g.AccountsTransactionsDebit : 0,
                //                            Balance = g.AccountsTransactionsAllocBalance,
                //                            AllocAmount = g.AccountsTransactionsAllocCredit > 0 ? g.AccountsTransactionsAllocCredit : g.AccountsTransactionsAllocDebit,
                //                            RefLocation = g.AccountsTransactionsLocation.ToString()
                //                        }).OrderBy(x => x.VoucherNo).AsEnumerable().ToList();



                //result.AddRange(transValues);

                return ApiResponse<List<AllocationDetails>>.Success(transValues, "Data Found");
            }
            catch (Exception ex)
            {

                return ApiResponse<List<AllocationDetails>>.Fail(ex.Message);
            }
        }

        public async Task<ApiResponse<List<AllocationDetails>>> GetPaymentVoucherAllocations(string accNo, string refNo, string vType)
        {
            try
            {
                //await Task.Delay(500);
               

                //var transValues = _accountTransactionRepository.GetAsQueryable()
                //                      .Where(a => a.AccountsTransactionsAccNo.ToUpper() == accNo.ToUpper()
                //                        && a.AccountsTransactionsAllocBalance > 0
                //                        && a.AccountsTransactionsCredit != 0)
                //                        .Select(g => new Domain.Models.AllocationDetails()
                //                        {
                //                            TransNo = g.AccountsTransactionsTransSno,
                //                            Status = "P",
                //                            TransDate = g.AccountsTransactionsTransDate,
                //                            VoucherNo = g.AccountsTransactionsVoucherNo,
                //                            Type = g.AccountsTransactionsVoucherType,
                //                            Credit = (decimal)g.AccountsTransactionsCredit > 0 ? g.AccountsTransactionsCredit : 0,
                //                            Debit = (decimal)g.AccountsTransactionsDebit > 0 ? g.AccountsTransactionsDebit : 0,
                //                            Balance = g.AccountsTransactionsAllocBalance,
                //                            AllocAmount = g.AccountsTransactionsAllocCredit > 0 ? g.AccountsTransactionsAllocCredit : g.AccountsTransactionsAllocDebit,
                //                            RefLocation = g.AccountsTransactionsLocation.ToString()
                //                        }).OrderBy(x => x.VoucherNo).AsEnumerable().ToList();
                var sql = @$";WITH AllocatedAmount AS (
                                                SELECT 
                                                    Allocation_Voucher_Details_VNO,
                                                    SUM(ISNULL(Allocation_Voucher_Details_Alloc_Debit,0)) AS AllocAmount
                                                FROM Allocation_Voucher_Details
                                                where Allocation_Voucher_Details_DelStatus = 0
                                                GROUP BY Allocation_Voucher_Details_VNO
                                            )
                                            SELECT
                                                a.AccountsTransactions_TransSno             AS TransNo,
                                                'P'                                                                 AS Status,
                                                a.AccountsTransactions_TransDate            AS TransDate,
                                                a.AccountsTransactions_VoucherNo          AS VoucherNo,
                                                a.AccountsTransactions_VoucherType       AS Type,

                                              CASE 
                                                    WHEN a.AccountsTransactions_Debit = 0 AND  a.AccountsTransactions_Alloc_Balance > 0 THEN 
                                                         a.AccountsTransactions_Alloc_Balance 
                                                    ELSE a.AccountsTransactions_Credit
                                              END AS Credit,

                                              CASE 
                                                    WHEN a.AccountsTransactions_Credit = 0  AND a.AccountsTransactions_Alloc_Balance > 0 
                                                    THEN  a.AccountsTransactions_Alloc_Balance
                                                    ELSE a.AccountsTransactions_Debit
                                              END AS Debit,

                                                a.AccountsTransactions_Alloc_Balance    AS Balance,
                                                ISNULL(am.AllocAmount,0)                    AS AllocAmount,
                                                case when ISNULL(a.AccountsTransactions_Alloc_Balance,0) > 0 then 0 else 1 end as IsDisabled,
                                                a.AccountsTransactions_Location             AS RefLocation
                                            FROM AccountsTransactions a
                                            LEFT JOIN AllocatedAmount am
                                              ON am.Allocation_Voucher_Details_VNO = a.AccountsTransactions_VoucherNo
                                            WHERE  a.AccountsTransactions_AccNo = '{accNo}'
                                              AND (
                                                    upper(a.AccountsTransactions_VoucherType) = upper('{GetVoucherByType(vType)}') 
                                                 OR (upper(a.AccountsTransactions_VoucherType) = upper('{vType}')
                                                     AND a.AccountsTransactions_VoucherNo = '{refNo}')
                                              )
                                            ORDER BY a.AccountsTransactions_TransDate;";

                var transValues = (await _dbConnection.QueryAsync<AllocationDetails>(sql)).ToList();



                return ApiResponse<List<AllocationDetails>>.Success(transValues, "Data Found");
            }
            catch (Exception ex)
            {

                return ApiResponse<List<AllocationDetails>>.Fail(ex.Message);
            }
        }

        private string GetVoucherByType(string vType)
        {
            switch (vType) 
            {
                case "PAYMENT":
                    return "PurchaseVoucher";
                case "RECEIPT":
                    return "SalesVoucher";
                default:
                    return "";
            }
        }

        private decimal? getNatAllocationAmount(long transNo)
        {
            try
            {
                var allocValue = allocationVDlts.GetAll()
                                        .Where(a => a.AllocationVoucherDetailsTransSno == transNo)
                                        .GroupBy(a => true)
                                        .Select(g => Math.Abs((g.Sum(a => a.AllocationVoucherDetailsAllocDebit ?? 0) - g.Sum(a => a.AllocationVoucherDetailsAllocCredit ?? 0))))
                                        .FirstOrDefault();
                var transValue = _accountTransactionRepository.GetAll()
                                        .Where(a => a.AccountsTransactionsTransSno == transNo)
                                        .GroupBy(a => true)
                                        .Select(g => Math.Abs((g.Sum(a => a.AccountsTransactionsDebit) - g.Sum(a => a.AccountsTransactionsCredit))))
                                        .FirstOrDefault();
                var diff = transValue - (decimal)allocValue;
                return diff;
            }
            catch (Exception) { return 0; }
        }

        //public async Task<ResponseInfo> getAllocationforPrint(string accNo, DateTime date, string vType)
        //{
        //    var objresponse = new ResponseInfo();
        //    var dt = date.Date;
        //    var details = await this.allocationVDlts.GetAsQueryable().Where(a => a.RefSPDate.Value.Date == dt && a.RefSPAccountNo == accNo && a.AllocationVoucherDetailsRefVtype == vType).Select(c => new
        //    {
        //        c.AllocationVoucherDetailsVno,
        //        c.AllocationVoucherDetailsRefVtype,
        //        c.RefSPAccountNo,
        //        c.AllocationVoucherDetailsAllocCredit,
        //        c.AllocationVoucherDetailsAllocDebit,
        //        c.RefSPDate
        //    }).ToListAsync();
        //    objresponse.ResultSet = new
        //    {
        //        details = details,
        //    };
        //    return objresponse;
        //}

        public async Task<ResponseInfo> getAllocationforPrint(int? Id)
        {
            var objresponse = new ResponseInfo();            
            var details = await this.allocationVDlts.GetAsQueryable().Where(a => a.AllocationVoucherDetailsSno== Id).Select(c => new
            {
                c.AllocationVoucherDetailsVno,
                c.AllocationVoucherDetailsRefVtype,
                //c.RefSPAccountNo,
                c.AllocationVoucherDetailsAllocCredit,
                c.AllocationVoucherDetailsAllocDebit,
                c.RefSPDate.Value.Date
            }).ToListAsync();
            objresponse.ResultSet = new
            {
                details = details,
            };
            return objresponse;
        }
    }
}
