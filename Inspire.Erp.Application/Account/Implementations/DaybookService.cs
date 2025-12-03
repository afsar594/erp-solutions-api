using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.NewFolder.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.AccountStatement;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.File;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Account.Implementations
{
    public class DaybookService : IDaybookService
    {
        private readonly IRepository<MasterAccountsTable> _masterAccountsTable;
        private readonly IRepository<TmpDayBook> _tmpDayBook;
        private readonly IRepository<AccountsTransactions> _accountTransaction;
        private readonly IRepository<StationMaster> _locationMaster;
        private IRepository<AccountSettings> _accountsSettingsRepo;
        private readonly IRepository<CostCenterMaster> _costCenterMaster;
        private readonly IRepository<JobMaster> _jobMaster;
        private readonly IFileService _fileService;
        public DaybookService(IRepository<MasterAccountsTable> masterAccountsTable, IFileService fileService, IRepository<AccountSettings> accountsSettingsRepo,
            IRepository<AccountsTransactions> accountTransaction, IRepository<StationMaster> locationMaster, IRepository<TmpDayBook> tmpDayBook, IRepository<CostCenterMaster> costCenterMaster, IRepository<JobMaster> jobMaster)
        {
            _masterAccountsTable = masterAccountsTable;
            _accountTransaction = accountTransaction;
            _fileService = fileService; _accountsSettingsRepo = accountsSettingsRepo;
            _locationMaster = locationMaster;
            _tmpDayBook = tmpDayBook;
            _costCenterMaster = costCenterMaster;
            _jobMaster = jobMaster;
        }
        public async Task<Response<GridWrapperResponse<GetDaybookResponse>>> GetDaybookAccountTransactions(GenericGridViewModel model)
        {
            try
            {
                var templist = await _tmpDayBook.GetAsQueryable().ToListAsync();
                await _tmpDayBook.DeleteListAsync(templist);
                var gridModel = new GetDaybookResponse();

                //var bankandcashAccount = new string[] { "BANK_MASTER_AC_NO", "CASH_MASTER_AC_NO" };
                var bankandcashAccount = new string[] { "BANK_MASTER_AC_NO" };
                var accountSettings = _accountsSettingsRepo.GetAsQueryable().AsNoTracking().Where(x => bankandcashAccount.Contains(x.AccountSettingsAccountKeyValue)).Select(X =>
                       X.AccountSettingsAccountTextValue.Split("::", StringSplitOptions.RemoveEmptyEntries)[1].Trim()
                ).ToList();

                var masterAccounts = _masterAccountsTable.GetAsQueryable().AsNoTracking().Where(x => accountSettings.Contains(x.MaRelativeNo.Trim())).Select(x => x.MaAccNo.Trim()).ToList();
                       var fullDataQuery =
                            from act in _accountTransaction.GetAsQueryable().AsNoTracking()
                            where masterAccounts.Contains(act.AccountsTransactionsAccNo.Trim())
                             && act.AccountsTransactionsTransDate.Value.Date >= model.FromDate.Value.Date
                             && act.AccountsTransactionsTransDate.Value.Date <= model.ToDate.Value.Date
                             && (act.AccountstransactionsDelStatus == false || act.AccountstransactionsDelStatus ==null)

                            join ccm in _costCenterMaster.GetAsQueryable().AsNoTracking()
                                 on act.AccountsTransactionsCostCenterId equals ccm.CostCenterMasterCostCenterId into ccmJoin
                            from ccm in ccmJoin.DefaultIfEmpty()

                            join jm in _jobMaster.GetAsQueryable().AsNoTracking()
                                 on act.AccountsTransactionsJobNo equals jm.JobMasterJobId into jmJoin
                            from jm in jmJoin.DefaultIfEmpty()

                            join ma in _masterAccountsTable.GetAsQueryable().AsNoTracking()
                                 on act.AccountsTransactionsAccNo equals ma.MaAccNo into maJoin
                            from ma in maJoin.DefaultIfEmpty()

                            select new GetDaybookAccountTransaction
                            {
                                AccountsTransactionsAccNo = act.AccountsTransactionsAccNo,
                                AccountsTransactionsDebit = act.AccountsTransactionsDebit,
                                AccountsTransactionsCredit = act.AccountsTransactionsCredit,
                                AccountsTransactionsAllocBalance = act.AccountsTransactionsAllocBalance,
                                AccountsTransactionsVoucherNo = act.AccountsTransactionsVoucherNo,
                                AccountsTransactionsVoucherType = act.AccountsTransactionsVoucherType,
                                AccountsTransactionsDescription = act.AccountsTransactionsDescription,
                                CostCenterName = ccm.CostCenterMasterCostCenterName,
                                JobName = jm.JobMasterJobName,
                                AccountsTransactionsTransDate = act.AccountsTransactionsTransDate.Value.Date,

                                AccountsTransactionsParticulars = act.AccountsTransactionsParticulars,
                                AccountsTransactionsTransSno = act.AccountsTransactionsTransSno,
                                RefNo = act.RefNo,

                                AccountsTransactionsAllocDebit = act.AccountsTransactionsAllocDebit,
                                AccountsTransactionsAllocCredit = act.AccountsTransactionsAllocCredit
                            };

                var fullResult = await fullDataQuery.ToListAsync();
                var paginatedResult = fullResult.Skip(model.Skip).Take(model.Take).ToList();
                
                gridModel.Details = new List<GetDaybookAccountTransaction>();

                foreach (var item in paginatedResult)
                {
                    gridModel.Details.Add(new GetDaybookAccountTransaction()
                    {
                        AccountsTransactionsTransDateString = item.AccountsTransactionsTransDate.ToString(),
                        AccountsTransactionsVoucherNo = item.AccountsTransactionsVoucherNo,
                        RefNo = item.RefNo,
                        AccountsTransactionsDescription = item.AccountsTransactionsDescription,
                        AccountsTransactionsParticulars = item.AccountsTransactionsParticulars,
                        AccountsTransactionsDebit = item.AccountsTransactionsDebit,
                        AccountsTransactionsCredit = item.AccountsTransactionsCredit,
                        AccountsTransactionsVoucherType = item.AccountsTransactionsVoucherType,
                        JobName = item.JobName,
                        CostCenterName = item.CostCenterName
                    });
                }

                var tmpList = fullResult.Select(r => new TmpDayBook
                {
                    AccountsTransactionsTransDateString = r.AccountsTransactionsTransDate.ToString(),
                    AccountsTransactionsVoucherNo = r.AccountsTransactionsVoucherNo,
                    RefNo = r.RefNo,
                    AccountNo = r.AccountsTransactionsAccNo,
                    AccountsTransactionsDescription = r.AccountsTransactionsDescription,
                    AccountsTransactionsParticulars = r.AccountsTransactionsParticulars,
                    AccountsTransactionsDebit = r.AccountsTransactionsDebit,
                    AccountsTransactionsCredit = r.AccountsTransactionsCredit,
                    AccountsTransactionsVoucherType = r.AccountsTransactionsVoucherType,
                    JobName = r.JobName,
                    CostCenterName = r.CostCenterName
                }).ToList();

                await _tmpDayBook.InsertListAsync(tmpList);
                var gridReponse = new GridWrapperResponse<GetDaybookResponse>()
                {
                    Data = gridModel,
                    Total = fullResult.Count()
                };
                return Response<GridWrapperResponse<GetDaybookResponse>>.Success(gridReponse, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<GetDaybookResponse>>.Fail(new GridWrapperResponse<GetDaybookResponse>(), ex.Message);
            }
        }
        public async Task<Response<ReturnPrintResponse>> DaybookPrint(GenericGridViewModel model)
        {
            try
            {
                string fileName = "Daybook";
                ReturnPrintResponse response = new ReturnPrintResponse();
                var dbResult = await GetDaybookAccountTransactions(model);
                if (dbResult.Result.Data.Details.Count < 1)
                {
                    return Response<ReturnPrintResponse>.Fail(new ReturnPrintResponse(), "No records");
                }
                string cssPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Styles", "Account", "daybookStyles.css");
                string outPutPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Files", @$"{fileName}.pdf");
                await _fileService.CheckFileExist(outPutPath);
                AddPDFResponse pDFResponse = new AddPDFResponse()
                {
                    Html = GetHTMLString(dbResult.Result.Data.Details),
                    CssPath = cssPath,
                    OutputPath = outPutPath
                };
                var pdf = await _fileService.CreatePDFFromHtml(pDFResponse);
                if (pdf.Result.Trim(' ') == "")
                {
                    return Response<ReturnPrintResponse>.Fail(new ReturnPrintResponse(), "No PDF Generated");
                }
                if (model.Extension == "PDF")
                {
                    var downloadPDF = await _fileService.DownloadFile(pdf.Result);
                    var pdfContentType = await _fileService.GetContentType(pdf.Result);
                    response.stream = downloadPDF.Result;
                    response.ContentType = pdfContentType.Result;
                    response.Path = pdf.Result;
                    return Response<ReturnPrintResponse>.Success(response, "File Created");
                }
                var file = await _fileService.CreateFileFromExtension(new GetFileFromExtensionResponse()
                {
                    Extension = model.Extension,
                    Path = pdf.Result,
                    FileName = fileName
                });
                if (file.Result.Trim(' ') == "")
                {
                    return Response<ReturnPrintResponse>.Fail(new ReturnPrintResponse(), "No File Generated");
                }
                var download = await _fileService.DownloadFile(file.Result);
                var ContentType = await _fileService.GetContentType(file.Result);
                response.stream = download.Result;
                response.ContentType = ContentType.Result;
                response.Path = file.Result;
                return Response<ReturnPrintResponse>.Success(response, "File Created");
            }
            catch (Exception ex)
            {
                return Response<ReturnPrintResponse>.Fail(new ReturnPrintResponse(), ex.Message);
            }
        }
        private string GetHTMLString(List<GetDaybookAccountTransaction> records)
        {
            var results = records;
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header'><h1>This is the generated PDF report!!!</h1></div>
                                <table align='center'>
                                    <tr>
                                        <th>Voucher Type</th>
                                        <th>Voucher Number</th>
                                        <th>Account Number</th>
                                        <th>Accout Name</th>
                                            <th>Particulars</th>
                                         <th>Date</th>
                                         <th>Debit</th>
                                         <th>Credit</th>
                                    </tr>");
            foreach (var item in results)
            {
                sb.Append(@$"<tr>
                                    <td>{item.AccountsTransactionsVoucherType}</td>
                                    <td>{item.AccountsTransactionsVoucherNo}</td>
                                    <td>{item.AccountsTransactionsAccNo}</td>
                                    <td>{item.AccountsTransactionsAccName}</td>
                                    <td>{item.AccountsTransactionsParticulars}</td>
                                    <td>{item.AccountsTransactionsTransDateString}</td>
                                    <td>{item.AccountsTransactionsDebit}</td>
                                    <td>{item.AccountsTransactionsCredit}</td>
                                  </tr>");
            }
            sb.Append(@"
                                </table>
                            </body>
                        </html>");
            return sb.ToString();
        }
    }
}
