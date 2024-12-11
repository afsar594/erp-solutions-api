using AutoMapper;
using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Sales.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Inspire.Erp.Application.Account.Interface;
using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Web.ViewModels;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Application.Account;
using Microsoft.EntityFrameworkCore;
using Spire.Pdf.Exporting.XPS.Schema;
using Microsoft.AspNetCore.Authorization;
using Inspire.Erp.Web.ViewModels.sales;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Inspire.Erp.Web.Controllers
{

    [ApiController]
    [Route("api/JobInvoice")]
    public class JobInvoiceController : ControllerBase
    {
        private IJobInvoice _invoice;
        private IMapper _mapper; private IRepository<LocationMaster> locationrepository; private IChartofAccountsService chartofAccountsService; private IRepository<AccountSettings> _accountsSettingsRepo;
        private IRepository<UnitMaster> unitrepository; private IRepository<JobMaster> jobrepository; private IRepository<CurrencyMaster> currencyrepository;
        private IRepository<CustomerMaster> _customerMasterRepository; private IRepository<SalesManMaster> salesmanrepository; private IRepository<ItemMaster> itemrepository;
        private readonly IRepository<JobInvoice> _jobinvoicerepo; private readonly IRepository<JobInvoiceDetails> _jobdetailsrepo; IRepository<UnitDetails> unitDetailRepository;
        public JobInvoiceController(IJobInvoice invoice, IMapper mapper, IRepository<JobInvoice> jobinvoicerepo, IRepository<UnitMaster> _unitrepository, IRepository<AccountSettings> accountsSettingsRepo, IRepository<JobMaster> _jobrepository, IRepository<ItemMaster> _itemrepository
            , IRepository<CurrencyMaster> _currencyrepository, IRepository<UnitDetails> _unitDetailRepository, IRepository<JobInvoiceDetails> jobdetailsrepo, IRepository<SalesManMaster> _salesmanrepository, IRepository<CustomerMaster> customerMasterRepository, IRepository<LocationMaster> _locationrepository, IChartofAccountsService _chartofAccountsService)
        {
            _invoice = invoice;
            _mapper = mapper; locationrepository = _locationrepository; chartofAccountsService = _chartofAccountsService; _jobdetailsrepo = jobdetailsrepo;
            unitrepository = _unitrepository; jobrepository = _jobrepository; currencyrepository = _currencyrepository; _jobinvoicerepo = jobinvoicerepo; _accountsSettingsRepo = accountsSettingsRepo;
            _customerMasterRepository = customerMasterRepository; salesmanrepository = _salesmanrepository; itemrepository = _itemrepository; unitDetailRepository = _unitDetailRepository;
        }
        [HttpGet]
        [Route("GetJobInvoice")]
        public ApiResponse<List<JobInvoice>> Getjobinvoice()
        {
            ApiResponse<List<JobInvoice>> apiResponse = new ApiResponse<List<JobInvoice>>
            {
                Valid = true,
                Result = _mapper.Map<List<JobInvoice>>(_invoice.Getjobinvoice()),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetJobInvoiceById")]
        public IActionResult GetJobInvoiceById(int id)
        {
            try
            {
                var item = _invoice.GetJobInvoiceById(id);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
        [HttpPost]
        [Route("InsertJobInvoice")]
        public async Task<ApiResponse<JobInvoiceViewModel>> InsertJobInvoice([FromBody] JobInvoiceViewModel JobInvoiceViewModel)
        {

            var param1 = _mapper.Map<JobInvoice>(JobInvoiceViewModel);
            var param2 = _mapper.Map<List<JobInvoiceDetails>>(JobInvoiceViewModel.JobInvoiceDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(JobInvoiceViewModel.AccountsTransactions);
            var xs = await _invoice.InsertJobInvoice(param1, param2, param3);
            JobInvoiceViewModel salesVoucherViewModel = new JobInvoiceViewModel
            {
                JobInvoiceId = xs.JobInvoice.JobInvoiceId,
                JobInvoiceCurrencyId = xs.JobInvoice.JobInvoiceCurrencyId,
                JobInvoiceCustomerId = xs.JobInvoice.JobInvoiceCustomerId,
                JobInvoiceCustomerName = xs.JobInvoice.JobInvoiceCustomerName,
                JobInvoiceDelStatus = xs.JobInvoice.JobInvoiceDelStatus,
                JobInvoiceDiscount = xs.JobInvoice.JobInvoiceDiscount,
                JobInvoiceFcRate = xs.JobInvoice.JobInvoiceFcRate,
                JobInvoiceFsno = xs.JobInvoice.JobInvoiceFsno,
                JobInvoiceLocationId = xs.JobInvoice.JobInvoiceLocationId,
                JobInvoiceGrossAmount = xs.JobInvoice.JobInvoiceGrossAmount,
                JobInvoiceLpoDate = xs.JobInvoice.JobInvoiceLpoDate,
                JobInvoiceNetAmount = xs.JobInvoice.JobInvoiceNetAmount,
                JobInvoiceLpoNo = xs.JobInvoice.JobInvoiceLpoNo,
                JobInvoiceJobId = xs.JobInvoice.JobInvoiceJobId,
                JobInvoiceRemarks = xs.JobInvoice.JobInvoiceRemarks,
                JobInvoiceSalesAccount = xs.JobInvoice.JobInvoiceSalesAccount,
                JobInvoiceSalesManId = xs.JobInvoice.JobInvoiceSalesManId,
                JobInvoiceUserId = xs.JobInvoice.JobInvoiceUserId,
                JobInvoiceVat = xs.JobInvoice.JobInvoiceVat,
                JobInvoiceVoucherDate = xs.JobInvoice.JobInvoiceVoucherDate,
                JobInvoiceVatAmt = xs.JobInvoice.JobInvoiceVatAmt,
                JobInvoiceVoucherNo = xs.JobInvoice.JobInvoiceVoucherNo,
                Invoicetype = xs.JobInvoice.Invoicetype,
            };

            salesVoucherViewModel.JobInvoiceDetails = _mapper.Map<List<JobInvoiceDetailsViewModel>>(xs.JobInvoiceDetails);
            salesVoucherViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);
            ApiResponse<JobInvoiceViewModel> apiResponse = new ApiResponse<JobInvoiceViewModel>
            {
                Valid = true,
                Result = _mapper.Map<JobInvoiceViewModel>(salesVoucherViewModel),
                Message = JobInvoiceMessage.SaveJobInvoice
            };
            return apiResponse;
        }
        [HttpPost]
        [Route("UpdateJobInvoice")]
        public async Task<ApiResponse<JobInvoiceViewModel>> UpdateJobInvoice([FromBody] JobInvoiceViewModel JobInvoiceViewModel)
        {
            var param1 = _mapper.Map<JobInvoice>(JobInvoiceViewModel);
            var param2 = _mapper.Map<List<JobInvoiceDetails>>(JobInvoiceViewModel.JobInvoiceDetails);
            var param3 = _mapper.Map<List<AccountsTransactions>>(JobInvoiceViewModel.AccountsTransactions);
            var xs = await _invoice.UpdateJobInvoice(param1, param2, param3);

            JobInvoiceViewModel salesVoucherViewModel = new JobInvoiceViewModel
            {
                JobInvoiceId = xs.JobInvoice.JobInvoiceId,
                JobInvoiceCurrencyId = xs.JobInvoice.JobInvoiceCurrencyId,
                JobInvoiceCustomerId = xs.JobInvoice.JobInvoiceCustomerId,
                JobInvoiceCustomerName = xs.JobInvoice.JobInvoiceCustomerName,
                JobInvoiceDelStatus = xs.JobInvoice.JobInvoiceDelStatus,
                JobInvoiceDiscount = xs.JobInvoice.JobInvoiceDiscount,
                JobInvoiceFcRate = xs.JobInvoice.JobInvoiceFcRate,
                JobInvoiceFsno = xs.JobInvoice.JobInvoiceFsno,
                JobInvoiceLocationId = xs.JobInvoice.JobInvoiceLocationId,
                JobInvoiceGrossAmount = xs.JobInvoice.JobInvoiceGrossAmount,
                JobInvoiceLpoDate = xs.JobInvoice.JobInvoiceLpoDate,
                JobInvoiceNetAmount = xs.JobInvoice.JobInvoiceNetAmount,
                JobInvoiceLpoNo = xs.JobInvoice.JobInvoiceLpoNo,
                JobInvoiceJobId = xs.JobInvoice.JobInvoiceJobId,
                JobInvoiceRemarks = xs.JobInvoice.JobInvoiceRemarks,
                JobInvoiceSalesAccount = xs.JobInvoice.JobInvoiceSalesAccount,
                JobInvoiceSalesManId = xs.JobInvoice.JobInvoiceSalesManId,
                JobInvoiceUserId = xs.JobInvoice.JobInvoiceUserId,
                JobInvoiceVat = xs.JobInvoice.JobInvoiceVat,
                JobInvoiceVoucherDate = xs.JobInvoice.JobInvoiceVoucherDate,
                JobInvoiceVatAmt = xs.JobInvoice.JobInvoiceVatAmt,
                JobInvoiceVoucherNo = xs.JobInvoice.JobInvoiceVoucherNo,
                Invoicetype = xs.JobInvoice.Invoicetype,
            };

            salesVoucherViewModel.JobInvoiceDetails = _mapper.Map<List<JobInvoiceDetailsViewModel>>(xs.JobInvoiceDetails);
            salesVoucherViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);
            ApiResponse<JobInvoiceViewModel> apiResponse = new ApiResponse<JobInvoiceViewModel>
            {
                Valid = true,
                Result = _mapper.Map<JobInvoiceViewModel>(salesVoucherViewModel),
                Message = JobInvoiceMessage.UpdateJobInvoice
            };
            return apiResponse;

        }
        [HttpGet]
        [Route("DeleteJobInvoice")]
        public IActionResult DeleteJobInvoice(string Id)
        {
            try
            {
                return Ok(_invoice.DeleteJobInvoice(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("GetJobInvoiceVoucherNo/{id}")]
        public ApiResponse<JobInvoiceViewModel> GetJobInvoiceVoucherNo(string id)
        {
            JobInvoiceModel xs = _invoice.GetJobInvoiceVoucherNo(id);

            if (xs != null)
            {

                JobInvoiceViewModel salesVoucherViewModel = new JobInvoiceViewModel
                {
                    JobInvoiceId = xs.JobInvoice.JobInvoiceId,
                    JobInvoiceCurrencyId = xs.JobInvoice.JobInvoiceCurrencyId,
                    JobInvoiceCustomerId = xs.JobInvoice.JobInvoiceCustomerId,
                    JobInvoiceCustomerName = xs.JobInvoice.JobInvoiceCustomerName,
                    JobInvoiceDelStatus = xs.JobInvoice.JobInvoiceDelStatus,
                    JobInvoiceDiscount = xs.JobInvoice.JobInvoiceDiscount,
                    JobInvoiceFcRate = xs.JobInvoice.JobInvoiceFcRate,
                    JobInvoiceFsno = xs.JobInvoice.JobInvoiceFsno,
                    JobInvoiceLocationId = xs.JobInvoice.JobInvoiceLocationId,
                    JobInvoiceGrossAmount = xs.JobInvoice.JobInvoiceGrossAmount,
                    JobInvoiceLpoDate = xs.JobInvoice.JobInvoiceLpoDate,
                    JobInvoiceNetAmount = xs.JobInvoice.JobInvoiceNetAmount,
                    JobInvoiceLpoNo = xs.JobInvoice.JobInvoiceLpoNo,
                    JobInvoiceJobId = xs.JobInvoice.JobInvoiceJobId,
                    JobInvoiceRemarks = xs.JobInvoice.JobInvoiceRemarks,
                    JobInvoiceSalesAccount = xs.JobInvoice.JobInvoiceSalesAccount,
                    JobInvoiceSalesManId = xs.JobInvoice.JobInvoiceSalesManId,
                    JobInvoiceUserId = xs.JobInvoice.JobInvoiceUserId,
                    JobInvoiceVat = xs.JobInvoice.JobInvoiceVat,
                    JobInvoiceVoucherDate = xs.JobInvoice.JobInvoiceVoucherDate,
                    JobInvoiceVatAmt = xs.JobInvoice.JobInvoiceVatAmt,
                    JobInvoiceVoucherNo = xs.JobInvoice.JobInvoiceVoucherNo,
                    Invoicetype = xs.JobInvoice.Invoicetype,
                };
                salesVoucherViewModel.JobInvoiceDetails = _mapper.Map<List<JobInvoiceDetailsViewModel>>(xs.JobInvoiceDetails);
                salesVoucherViewModel.AccountsTransactions = _mapper.Map<List<AccountTransactionViewModel>>(xs.accountsTransactions);
                ApiResponse<JobInvoiceViewModel> apiResponse = new ApiResponse<JobInvoiceViewModel>
                {
                    Valid = true,
                    Result = salesVoucherViewModel,
                    Message = ""
                };
                return apiResponse;
            }
            return null;
        }
        [HttpGet]
        [Route("GenerateVoucherNo")]
        public IActionResult GenerateVoucherNo()
        {
            try
            {
                var item = _invoice.GenerateVoucherNo(null);
                return Ok(item);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpPost]
        [Route("JobInvoiceReport")]
        public ResponseInfo JobInvoiceReport(JobInvoiceRptModel model)
        {
            var objresponse = new ResponseInfo();


            var query = _jobinvoicerepo.GetAsQueryable().AsNoTracking()
                      .Where(a => a.JobInvoiceDelStatus != true
                          && (model.locationid == null || model.locationid == 0 || a.JobInvoiceLocationId == model.locationid)
                          && (model.customerId == null || model.customerId == 0 || a.JobInvoiceCustomerId == model.customerId)
                          && (model.salemanId == null || model.salemanId == 0 || a.JobInvoiceSalesManId == model.salemanId));

            if (model.isDateChecked)
            {
                query = query.Where(a => a.JobInvoiceVoucherDate >= model.fromdate && a.JobInvoiceVoucherDate <= model.todate);
            }

            var list = query.ToList();

            var detail = _jobdetailsrepo.GetAsQueryable().AsNoTracking()
                                        .Where(a => a.JobInvoiceDetailsDelStatus != true);

            var data = (from p in list
                        join q in detail on p.JobInvoiceVoucherNo equals q.JobInvoiceDetailsVoucherNo into gimV1
                        from y in gimV1.DefaultIfEmpty()
                        select new
                        {
                            p.JobInvoiceVoucherNo,
                            p.JobInvoiceVoucherDate,
                            p.JobInvoiceCustomerId,
                            p.JobInvoiceSalesManId,
                            p.JobInvoiceLocationId,
                            y.JobInvoiceDetailsGrossAmount,
                            y.JobInvoiceDetailsDiscount,
                            y.JobInvoiceDetailsNetAmount
                        }).ToList();

            objresponse.ResultSet = data;
            return objresponse;
        }

        [HttpGet]
        [Route("JobInvoiceReportDropdown")]
        public async Task<ResponseInfo> JobInvoiceReportDropdown()
        {
            var objresponse = new ResponseInfo();


            var locationMasterTask = await locationrepository.GetAsQueryable().AsNoTracking()
                .Where(a => !a.LocationMasterLocationDelStatus != true)
                .Select(c => new
                {
                    c.LocationMasterLocationId,
                    LocationMasterLocationName = c.LocationMasterLocationName.Trim(),
                }).OrderBy(x=>x.LocationMasterLocationName).ToListAsync();

            var customerMastersTask = await _customerMasterRepository.GetAsQueryable().AsNoTracking()
                .Where(k => !k.CustomerMasterCustomerDelStatus != true)
                .Select(a => new
                {
                    a.CustomerMasterCustomerNo,
                    CustomerMasterCustomerName = a.CustomerMasterCustomerName.Trim(),
                }).OrderBy(x => x.CustomerMasterCustomerName).ToListAsync();

            var saleManListTask = await salesmanrepository.GetAsQueryable().AsNoTracking()
                .Where(a => !a.SalesManMasterSalesManDelStatus != true)
                .Select(c => new
                {
                    c.SalesManMasterSalesManId,
                    SalesManMasterSalesManName = c.SalesManMasterSalesManName.Trim(),
                }).OrderBy(x=>x.SalesManMasterSalesManName).ToListAsync();

            objresponse.ResultSet = new
            {
                locationMaster = locationMasterTask,
                customerMasters = customerMastersTask,
                SaleManList = saleManListTask
            };
            return objresponse;
        }

        [HttpGet]
        [Route("JobInvoiceLoadDropdown")]
        public async Task<ResponseInfo> JobInvoiceLoadDropdown()
        {
            var objresponse = new ResponseInfo();

            var unitMastersTask = await unitrepository.GetAsQueryable().AsNoTracking()
                .Where(a => a.UnitMasterUnitDelStatus != true)
                .Select(x => new
                {
                    x.UnitMasterUnitId,
                    UnitMasterUnitFullName = x.UnitMasterUnitFullName.Trim(),
                    UnitMasterUnitShortName = x.UnitMasterUnitShortName.Trim(),
                }).OrderBy(x=>x.UnitMasterUnitShortName)
                .ToListAsync();

            var jobMastersTask = await jobrepository.GetAsQueryable().AsNoTracking()
                .Where(a => !a.JobMasterJobDelStatus != true)
                .Select(c => new
                {
                    c.JobMasterJobId,
                    c.JobMasterJobName,
                })
                .ToListAsync();

            var currencyMastersTask = await currencyrepository.GetAsQueryable().AsNoTracking()
                .Where(a => !a.CurrencyMasterCurrencyDelStatus != true)
                .Select(c => new
                {
                    c.CurrencyMasterCurrencyId,
                    c.CurrencyMasterCurrencyName,
                    c.CurrencyMasterCurrencyRate
                })
                .ToListAsync();

            var masterAccountsTablesTask = chartofAccountsService.GetAllAccounts()
                .Where(a => !a.MaDelStatus != true)
                .Select(c => new
                {
                    c.MaAccNo,
                    c.MaAccName,
                    c.MaRelativeNo,
                    c.MaSno
                })
                .ToList();

            var accountsSettings = _accountsSettingsRepo.GetAsQueryable().AsNoTracking().Where(a => a.AccountSettingsAccountDelStatus != true).Select(c => new
            {
                c.AccountSettingsAccountId,
                AccountSettingsAccountDescription = c.AccountSettingsAccountDescription.Trim(),
                c.AccountSettingsAccountTextValue
            }).ToList();
            var locationMasterTask = await locationrepository.GetAsQueryable().AsNoTracking()
                .Where(a => !a.LocationMasterLocationDelStatus != true)
                .Select(c => new
                {
                    c.LocationMasterLocationId,
                    c.LocationMasterLocationName,
                })
                .ToListAsync();

            var customerMastersTask = await _customerMasterRepository.GetAsQueryable().AsNoTracking()
                .Where(k => !k.CustomerMasterCustomerDelStatus != true)
                .Select(a => new
                {
                    a.CustomerMasterCustomerNo,
                    a.CustomerMasterCustomerName,
                    a.CustomerMasterCustomerVatNo,
                    a.CustomerMasterCustomerContactPerson,
                    a.CustomerMasterCustomerAddress,
                    a.CustomerMasterCustomerReffAcNo,
                    a.CustomerMasterCustomerEmail,
                    a.CustomerMasterCustomerTel1,
                    a.CustomerMasterCustomerMobile
                })
                .ToListAsync();

            var saleManListTask = await salesmanrepository.GetAsQueryable().AsNoTracking()
                .Where(a => !a.SalesManMasterSalesManDelStatus != true)
                .Select(c => new
                {
                    c.SalesManMasterSalesManId,
                    c.SalesManMasterSalesManName,
                })
                .ToListAsync();

            var itemMaster = await (from item in itemrepository.GetAsQueryable().AsNoTracking()
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
                                        itemMasterBarcode = unit.UnitDetailsBarcode
                                    }).Distinct().ToListAsync();

            objresponse.ResultSet = new
            {
                itemMaster = itemMaster,
                unitMasters = unitMastersTask,
                jobMasters = jobMastersTask,
                currencyMasters = currencyMastersTask,
                masterAccountsTables = masterAccountsTablesTask,
                locationMaster = locationMasterTask,
                customerMasters = customerMastersTask,
                SaleManList = saleManListTask,
                accountsSettings = accountsSettings
            };

            return objresponse;
        }

        [HttpPost("CallDynamicReport")]
        public async Task<IActionResult> CallDynamicReport()
        {
            var objresponse = new ResponseInfo();

            var requestUrl = "http://103.74.54.207:8104/api/cr/DynamicReportCall";            
            var _httpClient = new HttpClient();
            // Prepare the request body
            var requestBody = new
            {
                reportName = "TestReport1",
                sqlQuery = "SELECT * FROM AccountsTransactions WHERE AccountsTransactions_AccNo='01001014'",
                reportDisplayName = "TestReport1.pdf",
                reportParam = new[]
                {
                    new { name = "test", value = "AS11449" },
                    new { name = "name", value = "Ali Abdullah" },
                    new { name = "address", value = "KARACHI" },
                    new { name = "cell", value = "98685790565" }
                }
            };

            // Convert the body to JSON format
            var jsonBody = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                // Send the POST request
                var response = await _httpClient.PostAsync(requestUrl, content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                    // Return the PDF file as a response
                    return File(pdfBytes, "application/pdf", "TestReport1.pdf");                    
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.ReasonPhrase);
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle any exceptions
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
