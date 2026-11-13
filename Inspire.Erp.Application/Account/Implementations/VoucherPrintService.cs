using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.NewFolder.Interfaces;
using Inspire.Erp.Domain.DTO.ItemMaster;
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
    public class VoucherPrintService : IVoucherPrintService
    {
        private readonly IRepository<MasterAccountsTable> _masterAccountsTable;
        private readonly IRepository<AccountsTransactions> _accountTransaction;
        private readonly IRepository<VouchersNumbers> _voucherNumber;
        private readonly IFileService _fileService;
        private readonly IRepository<StationMaster> _locationMaster;
        public VoucherPrintService(IRepository<AccountsTransactions> accountTransaction, IFileService fileService,
            IRepository<MasterAccountsTable> masterAccountsTable, IRepository<StationMaster> locationMaster,
            IRepository<VouchersNumbers> voucherNumber)
        {
            _accountTransaction = accountTransaction;
            _voucherNumber = voucherNumber;
            _masterAccountsTable = masterAccountsTable;
            _fileService = fileService;
            _locationMaster = locationMaster;
        }
        public async Task<Response<List<DropdownResponse>>> GetVoucherTypeDropdown()
        {
            try
            {
                var response =new List<DropdownResponse>();
                response.Add(new DropdownResponse()
                {
                    Value="",
                    Name="All"
                });
                 response.AddRange( await _voucherNumber.GetAsQueryable().Select(x => x.VouchersNumbersVType)
                    .Distinct().Select(x => new DropdownResponse
                    {
                        Value = x,
                        Name = x
                    }).OrderBy(x=>x.Name).ToListAsync());

                //response.AddRange(response.OrderBy(x => x.Name)); // Order by Name
                return Response<List<DropdownResponse>>.Success(response, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<DropdownResponse>>.Fail(new List<DropdownResponse>(), ex.Message);
            }
        }
        public async Task<Response<List<DropdownResponse>>> GetVoucherNoDropdown()
        {
            try
            {
                var response = new List<DropdownResponse>();
                response.Add(new DropdownResponse()
                {
                    Value = "",
                    Name = "All"
                });
               response.AddRange( await _voucherNumber.GetAsQueryable().Select(x => new DropdownResponse
                {
                    Value = x.VouchersNumbersVNo,
                    Name = x.VouchersNumbersVType + @$" ({x.VouchersNumbersVNo})"
                }).ToListAsync());
                return Response<List<DropdownResponse>>.Success(response, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<DropdownResponse>>.Fail(new List<DropdownResponse>(), ex.Message);
            }
        }

        public async Task<Response<DropdownPagedResponse>> GetVoucherNoAutoComplete(PagedSearchRequest request)
        {
            try
            {
                var query = _voucherNumber.GetAsQueryable();

                // Apply search filter if search term is provided
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    query = query.Where(x =>
                        x.VouchersNumbersVNo.Contains(request.SearchTerm) ||
                        x.VouchersNumbersVType.Contains(request.SearchTerm));
                }

                // Get total count for pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var items = await query
                    .OrderBy(x => x.VouchersNumbersVType)
                    .ThenBy(x => x.VouchersNumbersVNo)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new DropdownResponse
                    {
                        Value = x.VouchersNumbersVNo,
                        Name = x.VouchersNumbersVType + @$" ({x.VouchersNumbersVNo})"
                    })
                    .ToListAsync();

                // Add "All" option only for first page without search
                if (request.PageNumber == 1 && string.IsNullOrEmpty(request.SearchTerm))
                {
                    items.Insert(0, new DropdownResponse()
                    {
                        Value = "",
                        Name = "All"
                    });
                }

                var response = new DropdownPagedResponse
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    HasNextPage = (request.PageNumber * request.PageSize) < totalCount
                };

                return Response<DropdownPagedResponse>.Success(response, "Data found");
            }
            catch (Exception ex)
            {
                return Response<DropdownPagedResponse>.Fail(null, ex.Message);
            }
        }


        public async Task<Response<GetVoucherPrintingResponse>> GetAccountTransactions(GenericGridViewModel model)
        {
            try
            {
                GetVoucherPrintingResponse gridModel = new GetVoucherPrintingResponse();
                gridModel.Details = new List<GetAccountTransactionResponse>();
                string query = @$"1==1 {model.Filter}";
                var location = await _locationMaster.GetAsQueryable().FirstOrDefaultAsync();
                var baseQuery = _accountTransaction.GetAsQueryable();
                var filtered = baseQuery.Where(query);
                var totalCount = await filtered.CountAsync();
                var joined = filtered
                    .GroupJoin(_masterAccountsTable.GetAsQueryable(),
                        t => t.AccountsTransactionsAccNo,
                        m => m.MaAccNo,
                        (t, ms) => new { t, ms })
                    .SelectMany(z => z.ms.DefaultIfEmpty(), (z, m) => new { t = z.t, m });
                var ordered =joined;

                if (!string.IsNullOrEmpty(model.Field))
                {
                    string sortField = model.Field.ToLower() switch
                    {
                        "accountstransactionsaccname" => "m.MaAccName",
                        "accountstransactionsaccno" => "t.AccountsTransactionsAccNo",
                        "accountstransactionsdebit" => "t.AccountsTransactionsDebit",
                        "accountstransactionscredit" => "t.AccountsTransactionsCredit",
                        "accountstransactionsallocbalance" => "t.AccountsTransactionsAllocBalance",
                        "accountstransactionsvoucherno" => "t.AccountsTransactionsVoucherNo",
                        "accountstransactionsvouchertype" => "t.AccountsTransactionsVoucherType",
                        "accountstransactionsdescription" => "t.AccountsTransactionsDescription",
                        "accountstransactionstransdate" => "t.AccountsTransactionsTransDate",
                        "accountstransactionsparticulars" => "t.AccountsTransactionsParticulars",
                        "refno" => "t.RefNo",
                        "accountstransactionstranssno" => "t.AccountsTransactionsTransSno",
                        "accountstransactionsallocdebit" => "t.AccountsTransactionsAllocDebit",
                        "accountstransactionsalloccredit" => "t.AccountsTransactionsAllocCredit",
                        _ => "t.AccountsTransactionsTransDate"
                    };
                    string sortDirection = model.Dir?.ToLower() == "desc" ? "descending" : "ascending";
                    ordered = joined.OrderBy($"{sortField} {sortDirection}");
                }
                else
                {
                    ordered = joined.OrderBy("t.AccountsTransactionsTransDate ascending");
                }

                var paged = ordered.Skip(model.Skip).Take(model.Take);

                var rawResult = await paged.Select(x => new
                {
                    x.t.AccountsTransactionsAccNo,
                    MaAccName = x.m != null ? x.m.MaAccName : "",
                    x.t.AccountsTransactionsDebit,
                    x.t.AccountsTransactionsCredit,
                    x.t.AccountsTransactionsAllocBalance,
                    x.t.AccountsTransactionsVoucherNo,
                    x.t.AccountsTransactionsVoucherType,
                    x.t.AccountsTransactionsDescription,
                    x.t.AccountsTransactionsTransDate,
                    x.t.AccountsTransactionsParticulars,
                    x.t.RefNo,
                    x.t.AccountsTransactionsTransSno,
                    x.t.AccountsTransactionsAllocDebit,
                    x.t.AccountsTransactionsAllocCredit
                }).ToListAsync();
                gridModel.Details = rawResult.Select(r => new GetAccountTransactionResponse
                {
                    AccountsTransactionsAccNo = r.AccountsTransactionsAccNo,
                    AccountsTransactionsAccName = r.MaAccName,
                    AccountsTransactionsDebit = Convert.ToDecimal(r.AccountsTransactionsDebit),
                    AccountsTransactionsCredit = Convert.ToDecimal(r.AccountsTransactionsCredit),
                    AccountsTransactionsAllocBalance = r.AccountsTransactionsAllocBalance,
                    AccountsTransactionsVoucherNo = r.AccountsTransactionsVoucherNo,
                    AccountsTransactionsVoucherType = r.AccountsTransactionsVoucherType,
                    AccountsTransactionsDescription = r.AccountsTransactionsDescription,
                    AccountsTransactionsTransDate = Convert.ToDateTime(r.AccountsTransactionsTransDate),
                    AccountsTransactionsTransDateString = Convert.ToDateTime(r.AccountsTransactionsTransDate).ToString("MM-dd-yyyy"),
                    AccountsTransactionsParticulars = r.AccountsTransactionsParticulars,
                    RefNo = r.RefNo,
                    AccountsTransactionsTransSno = Convert.ToInt32(r.AccountsTransactionsTransSno),
                    AccountsTransactionsAllocDebit = r.AccountsTransactionsAllocDebit,
                    AccountsTransactionsAllocCredit = r.AccountsTransactionsAllocCredit
                }).ToList();
                gridModel.StationMasterStationName = location != null ? location.StationMasterStationName : "";
                gridModel.StationMasterEmail = location != null ? location.StationMasterEmail : "";
                gridModel.StationMasterFax = location != null ? location.StationMasterFax : "";
                gridModel.StationMasterTele1 = location != null ? location.StationMasterTele1 : "";
                gridModel.StationMasterAddress = location != null ? location.StationMasterAddress : "";
                gridModel.StationMasterCity = location != null ? location.StationMasterCity : "";
                gridModel.StationMasterCode = location != null ? location.StationMasterCode : 0;
                gridModel.StationMasterCountry = location != null ? location.StationMasterCountry : "";
                gridModel.StationMasterLogoPath = location != null ? location.StationMasterLogoPath : "";
                gridModel.StationMasterPostOffice = location != null ? location.StationMasterPostOffice : "";
                gridModel.StationMasterSignPath = location != null ? location.StationMasterSignPath : "";
                gridModel.StationMasterVatNo = location != null ? location.StationMasterVatNo : "";
                gridModel.StationMasterWebSite = location != null ? location.StationMasterWebSite : "";
                gridModel.Total = totalCount.ToString();
                return Response<GetVoucherPrintingResponse>.Success(gridModel, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GetVoucherPrintingResponse>.Fail(new GetVoucherPrintingResponse(), ex.Message);
            }
        }
        public async Task<Response<ReturnPrintResponse>> DownloadMultiAccountPrint(GenericGridViewModel model)
        {
            try
            {
                string fileName = "VoucherPrint";
                ReturnPrintResponse response = new ReturnPrintResponse();
                var dbResult = await GetAccountTransactions(model);
                if (dbResult.Result.Details.Count < 1)
                {
                    return Response<ReturnPrintResponse>.Fail(new ReturnPrintResponse(), "No records");
                }
                string cssPath = Path.Combine(Directory.GetCurrentDirectory(),  "Assets","Styles","Account", "voucherStyles.css");
                string outPutPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Files", @$"{fileName}.pdf");
                await _fileService.CheckFileExist(outPutPath);
                AddPDFResponse pDFResponse = new AddPDFResponse()
                {
                    Html = GetHTMLString(dbResult.Result.Details),
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
                    var pdfContentType=await _fileService.GetContentType(pdf.Result);
                    response.stream = downloadPDF.Result;
                    response.ContentType = pdfContentType.Result;
                    response.Path = pdf.Result;
                    return Response<ReturnPrintResponse>.Success(response, "File Created");
                }
                var file = await _fileService.CreateFileFromExtension(new GetFileFromExtensionResponse()
                {
                    Extension = model.Extension,
                    Path = pdf.Result,
                    FileName=fileName
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
        private string GetHTMLString(List<GetAccountTransactionResponse> records)
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
